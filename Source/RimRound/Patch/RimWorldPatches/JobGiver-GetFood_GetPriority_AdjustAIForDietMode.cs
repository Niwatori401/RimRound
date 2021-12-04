using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using RimWorld;
using Verse;
using HarmonyLib;

using RimRound.UI;
using RimRound.Comps;
using RimRound.Enums;
using RimRound.Utilities;

namespace RimRound.Patch
{
	[HarmonyPatch(typeof(JobGiver_GetFood))]
	[HarmonyPatch("GetPriority")]
	//Makes the weight gizmo work. In chanrge of altering when the pawn seeks food.
	public class RimRound_JobGiver_GetFoodPatch
	{
		public static void Postfix(Pawn __0, ref float __result)
		{
			if (__0.RaceProps.Humanlike)
			{
				FullnessAndDietStats_ThingComp fullnessComp = __0?.TryGetComp<FullnessAndDietStats_ThingComp>();
				//WeightGizmo_ThingComp gizmoComp = __0?.TryGetComp<WeightGizmo_ThingComp>();

				if (fullnessComp == null)
					return;

				switch (fullnessComp.DietMode) 
				{
					case DietMode.Nutrition:
						if (__0.needs.food.CurLevel + fullnessComp.CurrentFullness / fullnessComp.CurrentFullnessToNutritionRatio <= fullnessComp.GetRanges().First)
							__result = 9.5f;
						else
							__result = 0;
						return;
					case DietMode.Hybrid:
						if (__0.needs.food.CurLevel + fullnessComp.CurrentFullness / fullnessComp.CurrentFullnessToNutritionRatio <= fullnessComp.GetRanges().First)
							__result = 9.5f;
						else
							__result = 0;
						return;
					case DietMode.Fullness:
						if (fullnessComp.CurrentFullness <= fullnessComp.GetRanges().First)
							__result = 9.5f;
						else
							__result = 0;
						return;
					case DietMode.Disabled:
						return;
					default:
						Log.Error($"{__0.Name.ToStringShort}'s DietMode was invalid in RimRound_JobGiver_GetFoodPatch");
						return;
				
				}
			}
		}
	}

}
