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

            if (__instance.def.defName == Defs.HediffDefOf.RimRound_Weight.defName) 
            {
                List<PawnCapacityModifier> newList = new List<PawnCapacityModifier>();

                for (int i = 0; i < __result.Count; i++)
                {
                    newList.Add(__result[i].Clone());
                }

                Utilities.HediffUtility.AlterCapacityAccordingToSettings(newList, PawnCapacityDefOf.Manipulation, GlobalSettings.weightHediffManipulationPenaltyMult);
                Utilities.HediffUtility.AlterCapacityAccordingToSettings(newList, PawnCapacityDefOf.Moving, GlobalSettings.weightHediffMovementPenaltyMult);
                ReduceMovementPenaltyByPerkLevels(newList, __instance);
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
                
                __result = newList;
            }
        }




        static void ReduceMovementPenaltyByPerkLevels(List<PawnCapacityModifier> newList, Hediff hediff) 
        {
            Pawn pawn = hediff.pawn;

            if (pawn is null)
                return;

            var comp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();

            if (comp is null)
                return;

            int pcmIndex = newList.FindIndex(x => x.capacity == PawnCapacityDefOf.Moving);
            if (pcmIndex != -1)
            {
                newList[pcmIndex].offset = Mathf.Min(0, newList[pcmIndex].offset + (comp.perkLevels?.PerkToLevels?["RR_Comfortable_Corpulence_Title"] * 0.03f ?? 0) + (comp.perkLevels.PerkToLevels?["RR_HeavyRevian_Title"] * 0.6f ?? 0));
                
            }

            return;
        }
    }
}
