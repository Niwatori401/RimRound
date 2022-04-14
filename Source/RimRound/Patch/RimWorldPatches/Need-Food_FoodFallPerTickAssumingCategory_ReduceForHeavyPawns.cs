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
    [HarmonyPatch(typeof(Need_Food))]
    [HarmonyPatch(nameof(Need_Food.FoodFallPerTickAssumingCategory))]
    public class Need_Food_FoodFallPerTickAssumingCategory_ReduceForHeavyPawns
    {
        public static void Postfix(HungerCategory __0, Pawn ___pawn, ref float __result) 
        {
            if (!___pawn.RaceProps.Humanlike)
                return;
            __result *= BaseFoodFallMultiplierByWeightAndHungerCategory(___pawn, __0);
            __result *= AlterNutritionFallRateForHungerDrain(___pawn);
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
                    return 1 / (0.3f * Utilities.HediffUtility.KilosToSeverity(p.Weight()) + 1);
                case HungerCategory.UrgentlyHungry:
                    return 1 / (Utilities.HediffUtility.KilosToSeverity(p.Weight()) + 1);
                case HungerCategory.Starving:
                    return 0f;
                default:
                    return 0f;
            }
        }
    }
}
