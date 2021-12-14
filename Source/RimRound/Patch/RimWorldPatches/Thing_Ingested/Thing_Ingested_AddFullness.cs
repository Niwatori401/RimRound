using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using Verse;
using HarmonyLib;
using AlienRace;

using RimRound.Hediffs;
using RimRound.Utilities;
using RimRound.UI;
using RimRound.Comps;
using System.Reflection;



namespace RimRound.Patch
{
	public class Thing_Ingested_AddFullness
	{
		public static void Postfix(Thing __instance, Pawn __0, ref float __result, FullnessAndDietStats_ThingComp comp) //,
		{
			if (__0.RaceProps.Humanlike && __result > 0)
			{
				if (comp.DietMode != DietMode.Disabled)
				{
					if (comp == null)
						return;

					ThingComp_FoodItems_NutritionDensity nDThingComp = __instance.TryGetComp<ThingComp_FoodItems_NutritionDensity>();

					comp.UpdateRatio(__result, nDThingComp is null ? FullnessAndDietStats_ThingComp.defaultFullnessToNutritionRatio : nDThingComp.Props.fullnessToNutritionRatio);

					comp.CurrentFullness += __result * comp.CurrentFullnessToNutritionRatio * GlobalSettings.fullnessMultiplier.threshold;



					__result = 0;
				}
			}
		}
	}
}
