using HarmonyLib;
using RimRound.Comps;
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
    [HarmonyPatch(nameof(HediffSet.GetHungerRateFactor))]
    internal class HediffSet_GetHungerRateFactor_AlterAccordingToSettings
    {
		public static void Postfix(ref float __result, HediffSet __instance, HediffDef ignore) 
		{
			if (GlobalSettings.weightHediffHungerRateMult.threshold == 1)
				return;


			Hediff weightHediff = __instance.hediffs.Find(x => x.def.defName == Defs.HediffDefOf.RimRound_Weight.defName);
			if (weightHediff != null && (ignore == null || ignore.defName != weightHediff.def.defName))
			{
				float additiveValue = 0;

				for (int j = 0; j < __instance.hediffs.Count; j++)
				{
					if (__instance.hediffs[j].def != ignore)
					{
						HediffStage curStage = __instance.hediffs[j].CurStage;
						if (curStage != null)
						{
							additiveValue += curStage.hungerRateFactorOffset;
						}
					}
				}

				float normalHungerRateFactor = weightHediff.CurStage.hungerRateFactor;

				__result -= additiveValue;
				__result /= normalHungerRateFactor;
				__result *= 1 + ((normalHungerRateFactor - 1) * GlobalSettings.weightHediffHungerRateMult.threshold);
				__result += additiveValue;
			}

			AlterHungerByAmpleAppetiteLevel(ref __result, __instance);

        }

		private static void AlterHungerByAmpleAppetiteLevel(ref float val, HediffSet h)
		{
			var fndComp = h.pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
			if (fndComp == null)
				return;

            float ampleAppetiteBonusMult = fndComp.perkLevels.PerkToLevels?["RR_Ample_Appetite_Title"] * 0.5f + 1 ?? 1;

			val *= ampleAppetiteBonusMult;
        }
    }
}
