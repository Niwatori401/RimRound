using HarmonyLib;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.Patch
{

    [HarmonyPatch(typeof(Graphic_Multi))]
    [HarmonyPatch(nameof(Graphic_Multi.Init))]
    public class Graphic_Multi_Init_DontLogErrorsForRimRound
    {
        /// <summary>
        /// Filler function to consume the string off of the stack in place of the error logger.
        /// </summary>
        static internal void Dugma(string string1) 
        {
            return;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) 
        {
            List<CodeInstruction> codeInstructions = instructions.ToList();

            MethodInfo logErrorMethodInfo = typeof(Log).GetMethod(nameof(Log.Error), new Type[] { typeof(string) });

            bool foundInsertionPoint = false;

            for (int jndex = 0; jndex < codeInstructions.Count; ++jndex) 
            {
                if (codeInstructions[jndex].Calls(logErrorMethodInfo)) 
                {
                    foundInsertionPoint = true;

                    codeInstructions[jndex].operand = typeof(Graphic_Multi_Init_DontLogErrorsForRimRound).GetMethod(nameof(Dugma), BindingFlags.NonPublic | BindingFlags.Static);
                }
            }

            if (!foundInsertionPoint)
                Log.Error($"Failed to find insertion point in {nameof(Graphic_Multi_Init_DontLogErrorsForRimRound)}.");

            return codeInstructions;
        }

    }
}
