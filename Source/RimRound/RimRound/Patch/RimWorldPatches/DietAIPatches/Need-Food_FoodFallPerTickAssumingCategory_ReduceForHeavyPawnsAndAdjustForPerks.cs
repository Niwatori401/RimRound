using HarmonyLib;
using RimRound.Comps;
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
    [HarmonyPatch(typeof(Need_Food))]
    [HarmonyPatch(nameof(Need_Food.FoodFallPerTickAssumingCategory))]
    public class Need_Food_FoodFallPerTickAssumingCategory_ReduceForHeavyPawnsAndAdjustForPerks
    {
        public static void Postfix(HungerCategory __0, Pawn ___pawn, ref float __result) 
        {
            if (!___pawn.RaceProps.Humanlike)
                return;
            __result *= GetHungerRateMultByPerkLevels(___pawn);
            __result *= BaseFoodFallMultiplierByWeightAndHungerCategory(___pawn, __0);
            __result *= AlterNutritionFallRateForHungerDrain(___pawn);
        }

        private static float GetHungerRateMultByPerkLevels(Pawn pawn)
        {
            var fndComp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
            if (fndComp == null)
                return 1;

            float ampleAppetiteBonusMult = fndComp.perkLevels.PerkToLevels?["RR_Ample_Appetite_Title"] * 0.1f ?? 0;
            float gluttonyIncarnateBonusMult = fndComp.perkLevels.PerkToLevels?["RR_Voracious_Title"] * 0.25f ?? 0; 
            float voraciousBonusMult = fndComp.perkLevels.PerkToLevels?["RR_GluttonyIncarnate_Title"] * 1.0f ?? 0;

            float multiplier = ampleAppetiteBonusMult + gluttonyIncarnateBonusMult + voraciousBonusMult + 1;

            return multiplier;
        }

        static float AlterNutritionFallRateForHungerDrain(Pawn p) 
        {
            return HungerDroneUtility.GetCurrentNutritionFallMultiplierFromDrone(p);
        }

        static float BaseFoodFallMultiplierByWeightAndHungerCategory(Pawn p, HungerCategory hungerCategory) 
        {
            switch (hungerCategory)
            {
                case HungerCategory.Fed:
                    return 1;
                case HungerCategory.Hungry:
                    return 1 / (0.3f * Utilities.HediffUtility.KilosToSeverityWithBaseWeight(p.Weight()) + 1);
                case HungerCategory.UrgentlyHungry:
                    return 1 / (Utilities.HediffUtility.KilosToSeverityWithBaseWeight(p.Weight()) + 1);
                case HungerCategory.Starving:
                    return 0f;
                default:
                    return 0f;
            }
        }
    }
}
