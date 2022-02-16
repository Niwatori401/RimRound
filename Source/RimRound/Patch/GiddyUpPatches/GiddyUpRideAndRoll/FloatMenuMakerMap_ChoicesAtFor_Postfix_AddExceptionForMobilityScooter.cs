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
    class FloatMenuMakerMap_ChoicesAtFor_Postfix_AddExceptionForMobilityScooter
    {
		static MethodInfo getAnimalMI = typeof(RaceProperties).GetProperty("Animal").GetGetMethod(true);

		static MethodInfo mobilityScooterPredicate =
			typeof(FloatMenuMakerMap_ChoicesAtFor_Postfix_AddExceptionForMobilityScooter)
			.GetMethod(nameof(FloatMenuMakerMap_ChoicesAtFor_Postfix_AddExceptionForMobilityScooter.IsMobilityScooter),
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
					startJndex = i - 1;
					codeInstructions[startJndex].opcode = OpCodes.Call;
					codeInstructions[startJndex].operand = mobilityScooterPredicate;
					codeInstructions.RemoveAt(startJndex + 1);
				}
			}

			return codeInstructions;
		}

		static bool IsMobilityScooter(Pawn pawn)
		{
			return (pawn.def.defName == "RR_HoverChair") || pawn.RaceProps.Animal;
		}

		public static PatchCollection GetPatchCollection()
		{
			return new PatchCollection
			{
				transpiler = typeof(FloatMenuMakerMap_ChoicesAtFor_Postfix_AddExceptionForMobilityScooter).GetMethod(
					nameof(FloatMenuMakerMap_ChoicesAtFor_Postfix_AddExceptionForMobilityScooter.Transpiler), ModCompatibilityUtility.majorFlags)
			};
		}

	}
}
