using HarmonyLib;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(Hediff))]
    [HarmonyPatch(nameof(Hediff.CapMods), MethodType.Getter)]
    internal class Hediff_CapMods_DisableCapModsForRRHediffsOptionally
    {
        public static void Postfix(Hediff __instance, ref List<PawnCapacityModifier> __result) 
        {
            if (__instance.def.defName == Defs.HediffDefOf.RimRound_Weight.defName) 
            {
                List<PawnCapacityModifier> newList = new List<PawnCapacityModifier>();

                for (int i = 0; i < __result.Count; i++)
                {
                    newList.Add(__result[i].Clone());
                }

                Utilities.HediffUtility.AlterCapacityAccordingToSettings(newList, PawnCapacityDefOf.Manipulation, GlobalSettings.weightHediffManipulationPenaltyMult);
                Utilities.HediffUtility.AlterCapacityAccordingToSettings(newList, PawnCapacityDefOf.Moving, GlobalSettings.weightHediffMovementPenaltyMult);

                __result = newList;
            }
            else if (__instance.def.defName == Defs.HediffDefOf.RimRound_Fullness.defName) 
            {
                List<PawnCapacityModifier> newList = new List<PawnCapacityModifier>();

                for (int i = 0; i < __result.Count; i++)
                {
                    newList.Add(__result[i].Clone());
                }
                Utilities.HediffUtility.AlterCapacityAccordingToSettings(newList, PawnCapacityDefOf.Moving, GlobalSettings.fullnessHediffMovementPenaltyMult);
                Utilities.HediffUtility.AlterCapacityAccordingToSettings(newList, PawnCapacityDefOf.Eating, GlobalSettings.fullnessHediffEatingPenaltyMult);

                __result = newList;
            }
        }
    }
}
