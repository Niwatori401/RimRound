using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using AlienRace;
using HarmonyLib;
using RimRound.Comps;
using RimRound.Utilities;
using UnityEngine;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(AlienRace.AlienRenderTreePatches))]
    [HarmonyPatch(nameof(AlienRace.AlienRenderTreePatches.TrySetupGraphIfNeededPrefix))]
    public class HarmonyPatches_TrySetupGraphIfNeededPrefix_UpdatePawnBodySize
    {
        public static bool Prefix(PawnRenderTree __0)
        {
            if (__0?.pawn is null || !__0.pawn.RaceProps.Humanlike)
                return true;

            PawnBodyType_ThingComp PBTComp = __0.pawn.TryGetComp<PawnBodyType_ThingComp>();

            if (PBTComp != null)
                if (!PBTComp.usingCustomBodyMeshSize)
                    BodyTypeUtility.UpdatePawnDrawSize(__0.pawn, PBTComp.PersonallyExempt, PBTComp.CategoricallyExempt);

            return true;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);

            bool patchSuccess = ChangeDrawSize(codeInstructions);

            if (!patchSuccess)
            {
                throw new Exception("Failed to transpile TrySetupGraphIfNeededPrefix.");
            }

            return codeInstructions;
        }

        

        private static bool ChangeDrawSize(List<CodeInstruction> codeInstructions)
        {
            FieldInfo customDrawSizeLifeStageAlienFI = typeof(LifeStageAgeAlien).GetField(nameof(LifeStageAgeAlien.customDrawSize));
            FieldInfo customDrawSizeAlienCompFI = typeof(AlienPartGenerator.AlienComp).GetField(nameof(AlienPartGenerator.AlienComp.customDrawSize));

            if (customDrawSizeAlienCompFI is null || customDrawSizeLifeStageAlienFI is null)
                return false;

            bool success1 = false;
            bool success2 = false;

            for (int i = 0; i < codeInstructions.Count; i++)
            {
                if (success2 && !success1 && codeInstructions[i].operand is FieldInfo fi && fi == customDrawSizeLifeStageAlienFI)
                {
                    //Portrait draw size
                    codeInstructions.Remove(codeInstructions[i+7]); // Remove call to LifeStageAgeAlien.customDrawSize
                    codeInstructions.Remove(codeInstructions[i+7]);

                    codeInstructions.Insert(i + 7, new CodeInstruction(OpCodes.Call, ReplacementMethodInfo)); // Replace with call to new function and duplicate argument needed for later that also needs consumed by the function
                    codeInstructions.Insert(i + 7, new CodeInstruction(OpCodes.Dup));


                    //Regular drawsize
                    codeInstructions.Remove(codeInstructions[i - 1]);
                    codeInstructions.Remove(codeInstructions[i - 1]);

                    codeInstructions.Insert(i - 1, new CodeInstruction(OpCodes.Call, ReplacementMethodInfo));
                    codeInstructions.Insert(i - 1, new CodeInstruction(OpCodes.Dup));

                    success1 = true;
                }

                else if (!success2 && codeInstructions[i].opcode == OpCodes.Stfld && codeInstructions[i].operand is FieldInfo fi2 && fi2 == customDrawSizeAlienCompFI)
                {
                    codeInstructions.Insert(i + 24, new CodeInstruction(OpCodes.Call, ReplacementMethodInfo));
                    codeInstructions.Insert(i + 24, new CodeInstruction(OpCodes.Ldloc_2)); // AlienComp
                    codeInstructions.Insert(i + 24, new CodeInstruction(OpCodes.Pop)); // AlienLifeStage

                    codeInstructions.Insert(i, new CodeInstruction(OpCodes.Call, ReplacementMethodInfo));
                    codeInstructions.Insert(i, new CodeInstruction(OpCodes.Ldloc_2)); // AlienComp
                    codeInstructions.Insert(i, new CodeInstruction(OpCodes.Pop)); // AlienLifeStage

                    success2 = true;
                }

                if (success1 && success2)
                    break;
            }

            return success1 && success2;
        }

        static readonly MethodInfo ReplacementMethodInfo =
            typeof(HarmonyPatches_TrySetupGraphIfNeededPrefix_UpdatePawnBodySize)
            .GetMethod(nameof(HarmonyPatches_TrySetupGraphIfNeededPrefix_UpdatePawnBodySize.GetDrawSize), BindingFlags.NonPublic | BindingFlags.Static);

        static Vector2 GetDrawSize(AlienPartGenerator.AlienComp alienComp)
        {
            Pawn pawn = alienComp.parent.AsPawn();
            float drawSize;

            BodyTypeInfo? bodyTypeInfo = RacialBodyTypeInfoUtility.GetRacialBodyTypeInfo(pawn);
            if (bodyTypeInfo is null)
                drawSize = 1;
            else
                drawSize = bodyTypeInfo.AsNonNullable().meshSize;


            return new Vector2(drawSize, drawSize);
        }

    }
}
