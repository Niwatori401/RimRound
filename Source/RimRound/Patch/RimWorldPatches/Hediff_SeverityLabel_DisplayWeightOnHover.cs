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
	[HarmonyPatch(typeof(Hediff))]
	[HarmonyPatch("SeverityLabel", MethodType.Getter)]
	//Alters the tooltip on the weight hediff to display the weight when you hover
	public class Hediff_SeverityLabel_DisplayWeightOnHover
	{
		public static void Postfix(ref string __result, Hediff __instance)
		{
			if (__instance.def.defName == "RimRound_Weight")
			{
				__result = $"{Utilities.HediffUtility.SeverityToKilosWithBaseWeight(__instance.Severity):F1}Kgs";
			}
		}
	}
}

