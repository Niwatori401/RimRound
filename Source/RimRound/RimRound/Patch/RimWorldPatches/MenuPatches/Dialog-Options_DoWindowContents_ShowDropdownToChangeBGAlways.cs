using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RimRound.Patch
{
	[HarmonyPatch(typeof(Dialog_Options))]
	[HarmonyPatch(nameof(Dialog_Options.DoWindowContents))]
    class Dialog_Options_DoWindowContents_ShowDropdownToChangeBGAlways
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) 
        {
			List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);

			for (int i = 0; i < codeInstructions.Count; ++i) 
			{
				if (codeInstructions[i].opcode == OpCodes.Ldstr && (string)codeInstructions[i].operand == "SetBackgroundImage") 
				{
					codeInstructions[i - 2] = new CodeInstruction(OpCodes.Pop);
					//codeInstructions[i + 7] = new CodeInstruction(OpCodes.Pop);
				}
			}

			return codeInstructions.AsEnumerable();
		}
    }
}
