using HarmonyLib;
using RimRound.Comps;
using RimRound.Enums;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(FeedPatientUtility))]
    [HarmonyPatch(nameof(FeedPatientUtility.IsHungry))]
    public static class FeedPatientUtility_IsHungry_CheckForFullness
    {
		public static void Postfix(Pawn __0, ref bool __result)
		{
			if (__0.TryGetComp<FullnessAndDietStats_ThingComp>() is FullnessAndDietStats_ThingComp fullnessComp)
			{
				switch (fullnessComp.DietMode)
				{
					case DietMode.Nutrition:
						if (__0.needs.food.CurLevel + fullnessComp.CurrentFullness / fullnessComp.CurrentFullnessToNutritionRatio <= fullnessComp.GetRanges().First)
							__result = true;
						else
							__result = false;
						return;
					case DietMode.Hybrid:
						if (__0.needs.food.CurLevel + fullnessComp.CurrentFullness / fullnessComp.CurrentFullnessToNutritionRatio <= fullnessComp.GetRanges().First)
							__result = true;
						else
							__result = false;
						return;
					case DietMode.Fullness:
						if (fullnessComp.CurrentFullness <= fullnessComp.GetRanges().First)
							__result = true;
						else
							__result = false;
						return;
					case DietMode.Disabled:
						return;
					default:
						Log.Error($"{__0.Name.ToStringShort}'s DietMode was invalid in FeedPatientUtility_IsHungry_CheckForFullness");
						return;
				}
			}
		}
    }
}
