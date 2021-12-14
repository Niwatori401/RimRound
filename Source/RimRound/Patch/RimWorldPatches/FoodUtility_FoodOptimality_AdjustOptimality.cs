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
    [HarmonyPatch(typeof(FoodUtility))]
    [HarmonyPatch(nameof(FoodUtility.FoodOptimality))]
    public class FoodUtility_FoodOptimality_AdjustOptimality
    {
        public static void Postfix(ref float __result, Pawn __0, Thing __1, ThingDef __2) 
        {
            if (__0 is null)
            {
                Log.Error("Pawn was null in FoodUtility_FoodOptimality_AdjustOptimality");
            }    

            FullnessAndDietStats_ThingComp comp1 = __0.TryGetComp<FullnessAndDietStats_ThingComp>();

            if (comp1 is null)
                return;

            //If it would kill them
            if (__1.GetStatValue(StatDefOf.Nutrition) * FullnessAndDietStats_ThingComp.defaultFullnessToNutritionRatio >= comp1.RemainingFullnessUntil(comp1.HardLimit))
                __result -= 9999999f;

        }
    }
}
