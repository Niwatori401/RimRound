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
    class GUC_FloatMenuUtility_AddMountingOptions_AddExceptionForMobilityChair
    {
		static MethodInfo getAnimalMI = typeof(RaceProperties).GetProperty("Animal").GetGetMethod(true);

		static MethodInfo mobilityScooterPredicate = 
			typeof(GUC_FloatMenuUtility_AddMountingOptions_AddExceptionForMobilityChair)
			.GetMethod(nameof(GUC_FloatMenuUtility_AddMountingOptions_AddExceptionForMobilityChair.IsMobilityScooter), 
			BindingFlags.Static | BindingFlags.NonPublic);

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) 
        {
			List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);
			List<CodeInstruction> newInstructions = new List<CodeInstruction>();

			int startJndex = -1;

			for (int i = 0; i < codeInstructions.Count; ++i) 
			{
				if (codeInstructions[i].Calls(getAnimalMI)) 
				{
					startJndex = i + 3;
				}
			}

			newInstructions.Add(new CodeInstruction(OpCodes.Ldarg_0));
			newInstructions.Add(new CodeInstruction(OpCodes.Call, mobilityScooterPredicate));
			newInstructions.Add(new CodeInstruction(OpCodes.Or));

			if (startJndex != -1)
			{
				codeInstructions.InsertRange(startJndex, newInstructions);
			}

			return codeInstructions;
        }

		static bool IsMobilityScooter(Pawn pawn) 
		{
			return pawn.def.defName == "RR_HoverChair"; 
		}

		public static PatchCollection GetPatchCollection()
		{
			return new PatchCollection
			{
				transpiler = typeof(GUC_FloatMenuUtility_AddMountingOptions_AddExceptionForMobilityChair).GetMethod(
					nameof(GUC_FloatMenuUtility_AddMountingOptions_AddExceptionForMobilityChair.Transpiler), ModCompatibilityUtility.majorFlags)
			};
		}
	}
}
