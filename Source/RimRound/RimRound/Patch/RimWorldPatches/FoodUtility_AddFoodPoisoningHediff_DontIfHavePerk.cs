using HarmonyLib;
using Verse;
using RimWorld;
using RimRound.Comps;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(FoodUtility))]
    [HarmonyPatch(nameof(FoodUtility.AddFoodPoisoningHediff))]
    public class FoodUtility_AddFoodPoisoningHediff_DontIfHavePerk 
    {
        public static bool Prefix(Pawn pawn) 
        {
            if (pawn is null || !(pawn.TryGetComp<FullnessAndDietStats_ThingComp>() is FullnessAndDietStats_ThingComp comp))
                return true;

            if ((comp.perkLevels?.PerkToLevels?["RR_TitaniumStomach_Title"] ?? 0) > 0)
                return false;

            return true;
        }
    }
}
