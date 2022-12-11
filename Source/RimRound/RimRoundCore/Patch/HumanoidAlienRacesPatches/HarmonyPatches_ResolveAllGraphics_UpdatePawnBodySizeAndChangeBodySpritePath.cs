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
    [HarmonyPatch(typeof(AlienRace.HarmonyPatches))]
    [HarmonyPatch(nameof(AlienRace.HarmonyPatches.ResolveAllGraphicsPrefix))]
    public class HarmonyPatches_ResolveAllGraphics_UpdatePawnBodySizeAndChangeBodySpritePath
    {
        public static bool Prefix(PawnGraphicSet __0) 
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

            for (int i = 0; i < codeInstructions.Count; i++)
            {
                if (codeInstructions[i].opcode == OpCodes.Callvirt && codeInstructions[i-1].opcode == OpCodes.Ldnull && codeInstructions[i+1].opcode == OpCodes.Stloc_S)
                {
                    int indexOfStoreStr = i + 1;
                    
                    codeInstructions.Insert(indexOfStoreStr, new CodeInstruction(OpCodes.Call, ChangeSpritePathMethodInfo));
                    codeInstructions.Insert(indexOfStoreStr, new CodeInstruction(OpCodes.Ldarg_0));
                    break;
                }
            }

            ChangeDrawSize(codeInstructions);

            return codeInstructions;
        }



        private static string ChangeSpritePathIfNecessary(string path, PawnGraphicSet pawnGraphicSet) 
        {
            Log.Message($"I GOT CALLED ON {path} and PAWN {pawnGraphicSet.pawn.Name.ToStringShort}");
            string newBodyPath = BodyTypeUtility.GetProperBodyGraphicPathFromPawn(pawnGraphicSet.pawn);
            Log.Message($"NEW BODY PATH {newBodyPath}");
            return newBodyPath;
        }

        static readonly MethodInfo ChangeSpritePathMethodInfo =
            typeof(HarmonyPatches_ResolveAllGraphics_UpdatePawnBodySizeAndChangeBodySpritePath)
            .GetMethod(nameof(HarmonyPatches_ResolveAllGraphics_UpdatePawnBodySizeAndChangeBodySpritePath.ChangeSpritePathIfNecessary), BindingFlags.NonPublic | BindingFlags.Static);


        private static void ChangeDrawSize(List<CodeInstruction> codeInstructions)
        {
            MethodInfo GetCurLifeStageMethodInfo = typeof(Pawn_AgeTracker).GetProperty("CurLifeStageRace", BindingFlags.Instance | BindingFlags.Public).GetGetMethod(true);

            for (int i = 0; i < codeInstructions.Count; i++)
            {
                if (codeInstructions[i].Calls(GetCurLifeStageMethodInfo))
                {
                    codeInstructions.Remove(codeInstructions[i+4]);
                    codeInstructions.Remove(codeInstructions[i+4]);


                    codeInstructions.Insert(i + 4, new CodeInstruction(OpCodes.Call, ReplacementMethodInfo));
                    codeInstructions.Insert(i + 4, new CodeInstruction(OpCodes.Dup));
                    break;
                }
            }
        }

        static readonly MethodInfo ReplacementMethodInfo = 
            typeof(HarmonyPatches_ResolveAllGraphics_UpdatePawnBodySizeAndChangeBodySpritePath)
            .GetMethod(nameof(HarmonyPatches_ResolveAllGraphics_UpdatePawnBodySizeAndChangeBodySpritePath.GetDrawSize), BindingFlags.NonPublic | BindingFlags.Static);

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
