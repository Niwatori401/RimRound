using HarmonyLib;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    /// <summary>
    /// This patch hijacks the wildman check to also prevent pawns from getting mood debuffs for sleeping outside or on the ground if they are sleeping on  a blob bed
    /// </summary>
    [HarmonyPatch(typeof(Toils_LayDown))]
    [HarmonyPatch("ApplyBedThoughts")]
    public class Toils_LayDown_ApplyBedThoughts_AlterThoughtsForBlobBeds
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) 
        {
            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);
            bool hookFound = false;

            MethodInfo isWildManMI = typeof(WildManUtility).GetMethod(nameof(WildManUtility.IsWildMan), BindingFlags.Static | BindingFlags.Public);

            if (isWildManMI is null)
                Log.Error($"Failed to find wildManMI in {nameof(Toils_LayDown_ApplyBedThoughts_AlterThoughtsForBlobBeds)}");

            MethodInfo replacementMethodMI = typeof(Toils_LayDown_ApplyBedThoughts_AlterThoughtsForBlobBeds).GetMethod(nameof(Toils_LayDown_ApplyBedThoughts_AlterThoughtsForBlobBeds.ReplacementMethod), BindingFlags.Static | BindingFlags.NonPublic);

            if (replacementMethodMI is null)
                Log.Error($"Failed to find replacementMethodMI in {nameof(Toils_LayDown_ApplyBedThoughts_AlterThoughtsForBlobBeds)}");


            for (int i = 0; i < codeInstructions.Count; i++)
            {
                if (!codeInstructions[i].Calls(isWildManMI))
                    continue;

                hookFound = true;

                codeInstructions[i] = new CodeInstruction(OpCodes.Call, replacementMethodMI);
                codeInstructions.Insert(i, new CodeInstruction(OpCodes.Ldarg_1)); // Building_Bed

                break;
            }

            if (!hookFound)
                Log.Error($"Failed to find insertion point for {nameof(Toils_LayDown_ApplyBedThoughts_AlterThoughtsForBlobBeds)}");

            return codeInstructions;
        }

        private static bool ReplacementMethod(Pawn pawn, Building_Bed bed) 
        {
            return !pawn.IsWildMan() || bed.IsBlobBed();
        }


    }
}
