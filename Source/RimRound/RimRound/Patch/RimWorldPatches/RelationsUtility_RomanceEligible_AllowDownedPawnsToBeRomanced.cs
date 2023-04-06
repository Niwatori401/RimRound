using HarmonyLib;
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
    [HarmonyPatch(typeof(RelationsUtility))]
    [HarmonyPatch(nameof(RelationsUtility.RomanceEligible))]
    public class RelationsUtility_RomanceEligible_AllowDownedPawnsToBeRomanced
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) 
        {
            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);

            bool foundHook = false;

            MethodInfo getDownedMI = typeof(Pawn).GetProperty(nameof(Pawn.Downed)).GetGetMethod(true);
            MethodInfo replacementCheckMI =
                typeof(RelationsUtility_RomanceEligible_AllowDownedPawnsToBeRomanced)
                .GetMethod(nameof(RelationsUtility_RomanceEligible_AllowDownedPawnsToBeRomanced.ReplacementMethod), BindingFlags.Static | BindingFlags.NonPublic);

            if (getDownedMI is null)
                Log.Error("Failed to get MethodInfo for Downed()");

            if (replacementCheckMI is null)
                Log.Error("Failed to get MethodInfo for Downed()");

            for (int i = 0; i < codeInstructions.Count; i++)
            {
                if (!codeInstructions[i].Calls(getDownedMI))
                    continue;

                foundHook = true;
                
                codeInstructions[i] = new CodeInstruction(OpCodes.Call, replacementCheckMI);
                codeInstructions.Insert(i, new CodeInstruction(OpCodes.Ldarg_1));
                break;
            }       

            if (!foundHook)
                Log.Error($"Failed to find insertion point in {nameof(RelationsUtility_RomanceEligible_AllowDownedPawnsToBeRomanced)}");

            return codeInstructions;
        }

        //Return true if should not be elligible
        private static bool ReplacementMethod(Pawn pawn, bool isInitiator)
        {
            if (isInitiator)
                return pawn.Downed;
            else
                return pawn.Downed && !pawn.InBed();
        }
    }
}
