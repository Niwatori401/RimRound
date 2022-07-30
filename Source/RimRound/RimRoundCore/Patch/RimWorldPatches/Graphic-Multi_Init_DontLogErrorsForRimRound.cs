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
        
        public static bool Prefix(GraphicRequest __0) 
        {
            var path1 = ContentFinder<Texture2D>.Get(__0.path + "_north", false);
            var path2 = ContentFinder<Texture2D>.Get(__0.path + "_west", false);
            var path3 = ContentFinder<Texture2D>.Get(__0.path + "_east", false);
            var path4 = ContentFinder<Texture2D>.Get(__0.path + "_south", false);

            //if (path1 is null && path2 is null && path3 is null && path4 is null)
                //return false;

            return true;
        }
        

        static internal void Dugma(string string1) 
        {
            Log.Message("Oh, comma, I'm laffan");
            return;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) 
        {
            List<CodeInstruction> codeInstructions = instructions.ToList();

            MethodInfo logErrorMethodInfo = typeof(Log).GetMethod(nameof(Log.Error), new Type[] { typeof(string) });

            for (int jndex = 0; jndex < codeInstructions.Count; ++jndex) 
            {
                if (codeInstructions[jndex].Calls(logErrorMethodInfo)) 
                {
                    codeInstructions[jndex].operand = typeof(Graphic_Multi_Init_DontLogErrorsForRimRound).GetMethod(nameof(Dugma), BindingFlags.NonPublic | BindingFlags.Static);
                }
            }


            return codeInstructions;
        }

    }
}
