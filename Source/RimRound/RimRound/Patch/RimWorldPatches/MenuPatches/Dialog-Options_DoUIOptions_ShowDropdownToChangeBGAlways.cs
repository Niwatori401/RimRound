using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
	[HarmonyPatch(typeof(Dialog_Options))]
	[HarmonyPatch("DoUIOptions")]
    class Dialog_Options_DoUIOptions_ShowDropdownToChangeBGAlways
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) 
        {
			List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);
			List<CodeInstruction> newInstructions = new List<CodeInstruction>();
            bool foundInsertionPoint = false;

            for (int i = 0; i < codeInstructions.Count; ++i) 
			{
				if (codeInstructions[i].opcode == OpCodes.Ldstr && (string)codeInstructions[i].operand == "SetBackgroundImage") 
				{
                    foundInsertionPoint = true;

                    newInstructions.Add(new CodeInstruction(OpCodes.Pop)); // possibly false, possibly true
                    newInstructions.Add(new CodeInstruction(OpCodes.Ldc_I4_1)); // definitely true
                    codeInstructions.InsertRange(i - 2, newInstructions);

                    break;
				}
			}

            if (!foundInsertionPoint)
                Log.Error($"Failed to find insertion point in {nameof(Dialog_Options_DoUIOptions_ShowDropdownToChangeBGAlways)}.");

            return codeInstructions.AsEnumerable();
		}
    }
}
