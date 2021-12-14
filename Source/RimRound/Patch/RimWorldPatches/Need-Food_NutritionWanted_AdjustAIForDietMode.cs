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
using RimRound.Utilities;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(Need_Food))]
    [HarmonyPatch("NutritionWanted", MethodType.Getter)]
    public class RimRound_NeedFood_NutritionWantedPatch
    {
        public static void Postfix(Need_Food __instance , ref float __result, Pawn ___pawn) 
        {
			if (!GlobalSettings.showPawnDietManagementGizmo)
				return;

			FullnessAndDietStats_ThingComp fullnessComp = ___pawn?.TryGetComp<FullnessAndDietStats_ThingComp>();
			//FullnessAndDietStats_ThingComp fullnessComp = ((Pawn)Traverse.Create(__instance)?.Field("pawn")?.GetValue())?.TryGetComp<FullnessAndDietStats_ThingComp>();
			//WeightGizmo_ThingComp gizmoComp = ((Pawn)Traverse.Create(__instance)?.Field("pawn")?.GetValue())?.TryGetComp<WeightGizmo_ThingComp>();


			if (fullnessComp == null)
				return;
			if (!fullnessComp.parent.AsPawn().RaceProps.Humanlike)
				return;

			float burstingNutritionMult = 2f;

			switch (fullnessComp.DietMode)
			{
				case DietMode.Nutrition:
					__result = fullnessComp.SetAboveHardLimit ? (fullnessComp.GetRanges().Second - fullnessComp.GetRanges().First) * burstingNutritionMult : fullnessComp.GetRanges().Second - fullnessComp.GetRanges().First;
					return;
				case DietMode.Hybrid:
					__result = fullnessComp.SetAboveHardLimit ? fullnessComp.RemainingFullnessUntil(fullnessComp.HardLimit) * burstingNutritionMult : (fullnessComp.GetRanges().Second - fullnessComp.CurrentFullness);// / fullnessComp.CurrentFullnessToNutritionRatio;
					return;
				case DietMode.Fullness:
					__result = fullnessComp.SetAboveHardLimit ? fullnessComp.RemainingFullnessUntil(fullnessComp.HardLimit) * burstingNutritionMult : (fullnessComp.GetRanges().Second - fullnessComp.CurrentFullness);
					return;
				case DietMode.Disabled:
					return;
				default:
					Log.Error($"{fullnessComp.parent.AsPawn().Name.ToStringShort}'s DietMode was invalid in RimRound_NeedFood_NutritionWantedPatch");
					return;

			}
		}
    }
}
