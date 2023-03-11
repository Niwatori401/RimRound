using HarmonyLib;
using RimRound.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
	[HarmonyPatch(typeof(HediffSet))]
	[HarmonyPatch(nameof(HediffSet.RestFallFactor), MethodType.Getter)]
	internal class HediffSet_GetRestFallFactor_AlterAccordingToSettings
    {
		public static void Postfix(ref float __result, HediffSet __instance) 
		{
			if (GlobalSettings.weightHediffRestRateMult.threshold == 1)
				return;

			Hediff weightHediff = __instance.hediffs.Find(x => x.def.defName == Defs.HediffDefOf.RimRound_Weight.defName);
			if (weightHediff != null)
			{
				float additivePortionOfFallFactor = 0;
				for (int j = 0; j < __instance.hediffs.Count; j++)
				{
					HediffStage curStage2 = __instance.hediffs[j].CurStage;
					if (curStage2 != null)
					{
						additivePortionOfFallFactor += curStage2.restFallFactorOffset;
					}
				}

				__result -= additivePortionOfFallFactor;

				float normalRestFallRateFactor = weightHediff.CurStage.restFallFactor;

				__result /= normalRestFallRateFactor;
				__result *=  1 + ((normalRestFallRateFactor - 1) * GlobalSettings.weightHediffRestRateMult.threshold);

				__result += additivePortionOfFallFactor;
			}

			return;
		}

	}
}
