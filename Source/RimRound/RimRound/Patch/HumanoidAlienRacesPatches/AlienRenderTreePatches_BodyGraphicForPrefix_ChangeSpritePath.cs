using AlienRace;
using HarmonyLib;
using RimRound.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch.HumanoidAlienRacesPatches
{
    [HarmonyPatch(typeof(AlienRenderTreePatches))]
    [HarmonyPatch(nameof(AlienRenderTreePatches.BodyGraphicForPrefix))]
    public class AlienRenderTreePatches_BodyGraphicForPrefix_ChangeSpritePath
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);

            bool patchSuccess = ChangeBodyDrawPath(codeInstructions);

            if (!patchSuccess)
            {
                throw new Exception("Failed to transpile BodyGraphicForPrefix.");
            }

            return codeInstructions;
        }

        // Replaces GetPath() call.If SetPass(0) error crops up, this patch may be the cause
        private static bool ChangeBodyDrawPath(List<CodeInstruction> codeInstructions)
        {
            for (int i = 0; i < codeInstructions.Count; i++)
            {
                if (codeInstructions[i].opcode == OpCodes.Callvirt && codeInstructions[i-1].opcode == OpCodes.Ldnull && codeInstructions[i+1].opcode == OpCodes.Stloc_S)
                {
                    int indexOfStoreStr = i + 1;

                    codeInstructions.Insert(indexOfStoreStr, new CodeInstruction(OpCodes.Call, ChangeSpritePathMethodInfo));
                    codeInstructions.Insert(indexOfStoreStr, new CodeInstruction(OpCodes.Ldarg_1));
                    return true;
                }
            }
            return false;
        }

        private static string ChangeSpritePathIfNecessary(string path, Pawn pawn)
        {
            return BodyTypeUtility.GetProperBodyGraphicPathFromPawn(pawn);
        }

        static readonly MethodInfo ChangeSpritePathMethodInfo =
            typeof(AlienRenderTreePatches_BodyGraphicForPrefix_ChangeSpritePath)
            .GetMethod(nameof(ChangeSpritePathIfNecessary), BindingFlags.NonPublic | BindingFlags.Static);

    }
}
