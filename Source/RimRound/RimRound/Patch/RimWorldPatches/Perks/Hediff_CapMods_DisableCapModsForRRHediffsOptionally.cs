﻿using HarmonyLib;
using RimRound.Comps;
using RimRound.Hediffs;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(Hediff))]
    [HarmonyPatch(nameof(Hediff.CapMods), MethodType.Getter)]
    internal class Hediff_CapMods_DisableCapModsForRRHediffsOptionally
    {
        public static void Postfix(Hediff __instance, ref List<PawnCapacityModifier> __result) 
        {
            if (__instance is null || __result is null)
                return;

            Pawn pawn = __instance.pawn;

            if (pawn is null)
                return;

            var comp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();

            if (comp is null)
                return;


            if (__instance.def.defName == Defs.HediffDefOf.RimRound_Weight.defName) 
            {
                List<PawnCapacityModifier> newList = new List<PawnCapacityModifier>();

                for (int i = 0; i < __result.Count; i++)
                {
                    newList.Add(__result[i].Clone());
                }

                Utilities.HediffUtility.AlterCapacityAccordingToSettings(newList, PawnCapacityDefOf.Manipulation, GlobalSettings.weightHediffManipulationPenaltyMult);
                Utilities.HediffUtility.AlterCapacityAccordingToSettings(newList, PawnCapacityDefOf.Moving, GlobalSettings.weightHediffMovementPenaltyMult);
                ReduceMovementPenaltyByPerkLevels(newList, __instance, comp, comp.clothingBonuses.movementPenaltyMitigationMultBonus_Weight);
                ReduceManipulationPenaltyByPerkLevels(newList, __instance, comp, comp.clothingBonuses.manipulationPenaltyMitigationMultBonus_Weight);
                __result = newList;
            }
            else if (__instance.def.defName == Defs.HediffDefOf.RimRound_Fullness.defName) 
            {
                List<PawnCapacityModifier> newList = new List<PawnCapacityModifier>();

                for (int i = 0; i < __result.Count; i++)
                {
                    newList.Add(__result[i].Clone());
                }

                Hediff_Fullness fullnessHediff = __instance as Hediff_Fullness;

                Utilities.HediffUtility.AlterCapacityAccordingToSettings(newList, PawnCapacityDefOf.Moving, GlobalSettings.fullnessHediffMovementPenaltyMult, fullnessHediff.PersonalFullnessMovementMult);
                Utilities.HediffUtility.AlterCapacityAccordingToSettings(newList, PawnCapacityDefOf.Eating, GlobalSettings.fullnessHediffEatingPenaltyMult, fullnessHediff.PersonalFullnessEatingSpeedMult);
                ReduceMovementPenaltyByPerkLevels(newList, fullnessHediff, comp, comp.clothingBonuses.movementPenaltyMitigationMultBonus_Fullness);
                ReduceEatingPenaltyByPerkLevels(newList, fullnessHediff, comp, comp.clothingBonuses.eatingSpeedReductionMitigationMultBonus_Fullness);
                __result = newList;
            }
        }


        static void ReduceMovementPenaltyByPerkLevels(List<PawnCapacityModifier> newList, Hediff hediff, FullnessAndDietStats_ThingComp comp, float offsetMultiplier) 
        {
            int pcmIndex = newList.FindIndex(x => x.capacity == PawnCapacityDefOf.Moving);
            if (pcmIndex != -1)
            {
                newList[pcmIndex].offset = Mathf.Min(0,
                    newList[pcmIndex].offset * (1 - offsetMultiplier) + 
                    (comp.perkLevels?.PerkToLevels?["RR_Comfortable_Corpulence_Title"] * 0.03f * Mathf.Abs(newList[pcmIndex].offset) ?? 0) + 
                    (comp.perkLevels?.PerkToLevels?["RR_HeavyRevian_Title"] * 0.6f ?? 0));
                
            }

            return;
        }

        static void ReduceEatingPenaltyByPerkLevels(List<PawnCapacityModifier> newList, Hediff hediff, FullnessAndDietStats_ThingComp comp, float offsetMultiplier)
        {
            int pcmIndex = newList.FindIndex(x => x.capacity == PawnCapacityDefOf.Eating);
            if (pcmIndex != -1)
            {
                newList[pcmIndex].offset = Mathf.Min(0, newList[pcmIndex].offset * (1 - offsetMultiplier));
            }

            return;
        }

        static void ReduceManipulationPenaltyByPerkLevels(List<PawnCapacityModifier> newList, Hediff hediff, FullnessAndDietStats_ThingComp comp, float offsetMultiplier)
        {
            int pcmIndex = newList.FindIndex(x => x.capacity == PawnCapacityDefOf.Manipulation);
            if (pcmIndex != -1)
            {
                newList[pcmIndex].offset = Mathf.Min(0, newList[pcmIndex].offset * (1 - offsetMultiplier));
            }

            return;
        }

    }
}