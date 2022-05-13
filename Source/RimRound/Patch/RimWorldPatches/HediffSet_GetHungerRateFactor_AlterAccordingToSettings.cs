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
						HediffStage curStage2 = __instance.hediffs[j].CurStage;
						if (curStage2 != null)
						{
							additiveValue += curStage2.hungerRateFactorOffset;
						}
					}
				}

				float normalHungerRateFactor = weightHediff.CurStage.hungerRateFactor;

				__result -= additiveValue;
				__result /= normalHungerRateFactor;
				__result *= 1 + ((normalHungerRateFactor - 1) * GlobalSettings.weightHediffHungerRateMult.threshold);
				__result += additiveValue;
			}

		}

		//public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		//{
		//	List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);

		//	int getCurStageIndex = -1;
		//	int multIndex = -1;

  //          for (int jndex = 0; jndex < codeInstructions.Count; jndex++)
  //          {
		//		if (codeInstructions[jndex].Calls(getCurStageMI)) 
		//		{
		//			getCurStageIndex = jndex;
		//			multIndex = getCurStageIndex + 5;
		//			break;
		//		}
  //          }


		//	if (multIndex > 0)
		//	{
		//		codeInstructions.Insert(multIndex, new CodeInstruction(OpCodes.Call, getAppropriateHungerMult));
		//		codeInstructions.Insert(getCurStageIndex, new CodeInstruction(OpCodes.Dup));
		//	}
		//	else 
		//	{
		//		Log.Error("Failed to find index for path in HediffSet_GetHungerRateFactor_AlterAccordingToSettings");
		//	}



		//	return codeInstructions;
		//}

		//static float GetAppropriateHungerRateMult(Hediff hediff, float originalValue) 
		//{
		//	if (hediff.def.defName == Defs.HediffDefOf.RimRound_Weight.defName)
		//		return (originalValue * GlobalSettings.weightHediffHungerRateMult.threshold);

		//	return 1f;
		//}


		//static MethodInfo getAppropriateHungerMult = typeof(HediffSet_GetHungerRateFactor_AlterAccordingToSettings)
		//	.GetMethod(nameof(HediffSet_GetHungerRateFactor_AlterAccordingToSettings.GetAppropriateHungerRateMult), BindingFlags.Static | BindingFlags.NonPublic);
		//static MethodInfo getCurStageMI = typeof(Hediff).GetProperty(nameof(Hediff.CurStage)).GetGetMethod(true);

	}
}
