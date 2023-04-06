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
    [HarmonyPatch(typeof(Pawn_MechanitorTracker))]
    [HarmonyPatch(nameof(Pawn_MechanitorTracker.CanControlMechs), MethodType.Getter)]
    public class Pawn_MechanitorTracker_CanControlMechs_AllowMechControlForDownedPawnsInBed
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) 
        {
            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);

            bool hookFound = false;

            MethodInfo pawnDownedMI = typeof(Pawn).GetProperty(nameof(Pawn.Downed), BindingFlags.Public | BindingFlags.Instance).GetGetMethod(true);

            if (pawnDownedMI is null)
                Log.Error($"Pawn downed getter was null in {nameof(Pawn_MechanitorTracker_CanControlMechs_AllowMechControlForDownedPawnsInBed)}");


            MethodInfo replacementMI = typeof(Pawn_MechanitorTracker_CanControlMechs_AllowMechControlForDownedPawnsInBed).GetMethod(nameof(ReplacementMethod), BindingFlags.NonPublic | BindingFlags.Static);

            if (pawnDownedMI is null)
                Log.Error($"Pawn downed getter was null in {nameof(Pawn_MechanitorTracker_CanControlMechs_AllowMechControlForDownedPawnsInBed)}");

            for (int i = 0; i < codeInstructions.Count; i++)
            {
                if (!codeInstructions[i].Calls(pawnDownedMI))
                    continue;

                codeInstructions[i] = new CodeInstruction(OpCodes.Call, replacementMI);

                hookFound = true;

                break;
            }
            

            if (!hookFound)
                Log.Error($"Failed to find insertion point for {nameof(Pawn_MechanitorTracker_CanControlMechs_AllowMechControlForDownedPawnsInBed)}");

            return codeInstructions;
        }


        /// <returns><see langword="true"/> if they should be unable to control the mech</returns>
        private static bool ReplacementMethod(Pawn pawn) 
        {
            return (pawn.Downed && !pawn.InBed()) || !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Consciousness);
        }
    }
}
