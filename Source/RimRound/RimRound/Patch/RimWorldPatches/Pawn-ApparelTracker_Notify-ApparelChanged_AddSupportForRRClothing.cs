using HarmonyLib;
using RimRound.Comps;
using RimRound.Defs;
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
    [HarmonyPatch(typeof(Pawn_ApparelTracker))]
    [HarmonyPatch(nameof(Pawn_ApparelTracker.Notify_ApparelChanged))]
    public class Pawn_ApparelTracker_Notify_ApparelChanged_AddSupportForRRClothing
    {
        public static void Postfix(Pawn_ApparelTracker __instance) 
        {
            Pawn pawn = __instance.pawn;

            if (pawn is null || !pawn.RaceProps.Humanlike)
                return;

            var comp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
            if (comp is null)
                return;

            comp.clothingBonuses = new ClothingStatsBonuses();

            foreach (var def in clothingItemsToCheck)
            {
                if (!IsWearingClothingOfDef(pawn, def))
                    continue;

                ClothingModExtension clothingStats = def.GetModExtension<ClothingModExtension>();
                if (clothingStats is null)
                {
                    Log.Error("Clothing item was missing appropriate mod extension!");
                    return;
                }


                comp.clothingBonuses.weightGainMultiplierMultBonus += clothingStats.weightGainMultiplierMultBonus;
                comp.clothingBonuses.digestionSpeedMultBonus += clothingStats.digestionSpeedMultBonus;
                comp.clothingBonuses.stomachElasticityMultBonus += clothingStats.stomachElasticityMultBonus;
                comp.clothingBonuses.fullnessGainedMultiplierBonus += clothingStats.fullnessGainedMultiplierBonus;
                comp.clothingBonuses.flatMoveBonus += clothingStats.flatMoveBonus;
                comp.clothingBonuses.flatManipulationBonus += clothingStats.flatManipulationBonus;
                comp.clothingBonuses.flatEatingSpeedBonus += clothingStats.flatEatingSpeedBonus;
                comp.clothingBonuses.movementPenaltyMitigationMultBonus_Weight += clothingStats.movementPenaltyMitigationMultBonus_Weight;
                comp.clothingBonuses.movementPenaltyMitigationMultBonus_Fullness += clothingStats.movementPenaltyMitigationMultBonus_Fullness;
                comp.clothingBonuses.manipulationPenaltyMitigationMultBonus_Weight += clothingStats.manipulationPenaltyMitigationMultBonus_Weight;
                comp.clothingBonuses.painMitigationMultBonus_Fullness += clothingStats.painMitigationMultBonus_Fullness;
                comp.clothingBonuses.eatingSpeedReductionMitigationMultBonus_Fullness += clothingStats.eatingSpeedReductionMitigationMultBonus_Fullness;
            }

            ClampClothingBonuses(ref comp.clothingBonuses);
        }

        static bool IsWearingClothingOfDef(Pawn pawn, ThingDef clothingDef) 
        {
            if (pawn.apparel.WornApparel.Any(apparel => { return apparel.def.defName == clothingDef.defName; }))
                return true;

            return false;
        }

        static void ClampClothingBonuses(ref ClothingStatsBonuses clothingBonuses) 
        {
            clothingBonuses.movementPenaltyMitigationMultBonus_Weight = Mathf.Clamp01(clothingBonuses.movementPenaltyMitigationMultBonus_Weight);
            clothingBonuses.movementPenaltyMitigationMultBonus_Fullness = Mathf.Clamp01(clothingBonuses.movementPenaltyMitigationMultBonus_Fullness);
            clothingBonuses.manipulationPenaltyMitigationMultBonus_Weight = Mathf.Clamp01(clothingBonuses.manipulationPenaltyMitigationMultBonus_Weight);
            clothingBonuses.painMitigationMultBonus_Fullness = Mathf.Clamp01(clothingBonuses.painMitigationMultBonus_Fullness);
            clothingBonuses.eatingSpeedReductionMitigationMultBonus_Fullness = Mathf.Clamp01(clothingBonuses.eatingSpeedReductionMitigationMultBonus_Fullness);
        }

        static List<ThingDef> clothingItemsToCheck = new List<ThingDef>() 
        {
            Defs.ThingDefOf.RimRound_NovaTechApparel_Hyperbelt,
            Defs.ThingDefOf.RimRound_NovaTechApparel_Jumpsuit,
            Defs.ThingDefOf.RimRound_NovaTechApparel_ProtoSuit,
            Defs.ThingDefOf.RimRound_NovaTechApparel_Gloves,
            Defs.ThingDefOf.RimRound_NovaTechApparel_Collar,
            Defs.ThingDefOf.RimRound_NovaTechApparel_Belt,
        };
    }
}
