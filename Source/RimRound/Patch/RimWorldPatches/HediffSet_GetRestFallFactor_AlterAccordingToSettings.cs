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

		//public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		//{
		//	List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);
		//	List<CodeInstruction> newInstructions = new List<CodeInstruction>();

		//	int getCurStageIndex = -1;
		//	int multIndex = -1;

		//	for (int jndex = 0; jndex < codeInstructions.Count; jndex++)
		//	{
		//		if (codeInstructions[jndex].Calls(getCurStageMI))
		//		{
		//			getCurStageIndex = jndex;
		//			multIndex = getCurStageIndex + 5;
		//			break;
		//		}
		//	}


		//	if (multIndex > 0)
		//	{
		//		codeInstructions.Insert(multIndex, new CodeInstruction(OpCodes.Call, getAppropriateRestFallMult));
		//		codeInstructions.Insert(getCurStageIndex, new CodeInstruction(OpCodes.Dup));
		//	}
		//	else
		//	{
		//		Log.Error("Failed to find index for path in HediffSet_GetHungerRateFactor_AlterAccordingToSettings");
		//	}



		//	return codeInstructions;
		//}

		//static float GetAppropriateRestFallRateMult(Hediff hediff, float originalValue)
		//{
		//	if (hediff.def.defName == Defs.HediffDefOf.RimRound_Weight.defName)
		//		return originalValue * GlobalSettings.weightHediffRestRateMult.threshold;

		//	return 1;
		//}


		//static MethodInfo getAppropriateRestFallMult = typeof(HediffSet_GetRestFallFactor_AlterAccordingToSettings)
		//	.GetMethod(nameof(HediffSet_GetRestFallFactor_AlterAccordingToSettings.GetAppropriateRestFallRateMult), BindingFlags.Static | BindingFlags.NonPublic);
		//static MethodInfo getCurStageMI = typeof(Hediff).GetProperty(nameof(Hediff.CurStage)).GetGetMethod(true);

	}
}
