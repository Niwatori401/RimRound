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

namespace RimRound.Patch.RimWorldPatches
{
	[HarmonyPatch(typeof(StatWorker))]
	[HarmonyPatch("GetValue", new Type[] { typeof(StatRequest), typeof(bool) })]
	//Makes mass for pawns dynamic for those with the weight hediff
	public class StatWorker_GetValue_DynamicMassPatch
	{
		public static void Postfix(ref float __result, StatRequest __0, StatWorker __instance, StatDef ___stat)
		{
			//StatDef stat = (StatDef)Traverse.Create(__instance).Field("stat").GetValue();

			if (___stat.LabelForFullStatListCap == "Mass")
			{
				if (__0.Thing is Pawn pawn)
				{
					if (pawn.RaceProps.Humanlike)
					{
						float? weight = pawn.Weight();
						__result = weight != null ? (float)weight : __result;
					}
				}
			}
		}
	}
}
