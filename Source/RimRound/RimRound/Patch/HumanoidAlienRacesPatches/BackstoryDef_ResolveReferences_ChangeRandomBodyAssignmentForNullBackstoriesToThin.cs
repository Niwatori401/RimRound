using HarmonyLib;
using RimWorld;
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
    [HarmonyPatch(typeof(AlienRace.AlienBackstoryDef))]
    [HarmonyPatch(nameof(AlienRace.AlienBackstoryDef.ResolveReferences))]
    internal class BackstoryDef_ResolveReferences_ChangeRandomBodyAssignmentForNullBackstoriesToThin
    {
		static MethodInfo getRandomMI = typeof(DefDatabase<BodyTypeDef>).GetMethod("GetRandom", BindingFlags.Static | BindingFlags.Public);
		static MethodInfo getNamedMI = typeof(DefDatabase<BodyTypeDef>).GetMethods().Where(m => m.Name == "GetNamed").First();
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);
			List<CodeInstruction> newInstructions = new List<CodeInstruction>();

            bool foundInsertionPoint = false;

            for (int i = 0; i < codeInstructions.Count; i++)
            {
				if (codeInstructions[i].Calls(getRandomMI))
				{
                    foundInsertionPoint = true;

                    codeInstructions[i].operand = getNamedMI;

					newInstructions.Add(new CodeInstruction(OpCodes.Ldstr, "Thin"));
					newInstructions.Add(new CodeInstruction(OpCodes.Ldc_I4_1));

					codeInstructions.InsertRange(i, newInstructions);

					break;
				}
            }

            if (!foundInsertionPoint)
                Log.Error($"Failed to find insertion point in {nameof(BackstoryDef_ResolveReferences_ChangeRandomBodyAssignmentForNullBackstoriesToThin)}.");
            
            return codeInstructions;
		}
	}
}
