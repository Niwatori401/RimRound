using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using Verse;
using HarmonyLib;


using RimRound.Hediffs;
using RimRound.Utilities;

namespace RimRound.Patch
{
	[HarmonyPatch(typeof(Pawn))]
	[HarmonyPatch("Tick")]
	//Passive loss of weight severity
	public class Pawn_Tick_PassiveWeightLoss
	{
		public static void Postfix(Pawn __instance)
		{
			if (__instance.RaceProps.Humanlike && __instance != null)
			{
				Functions.AddHediffSeverity(Defs.HediffDefOf.RimRound_Weight, __instance, Values.nutritionToSeverity(-1 * __instance.needs.food.FoodFallPerTick));
			}
		}
	}

}
