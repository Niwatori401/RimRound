using HarmonyLib;
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
    //[HarmonyPatch(typeof(AlienRace.HarmonyPatches))]
    //[HarmonyPatch(nameof(AlienRace.HarmonyPatches.GetPawnMesh))]
    public class HarmonyPatches_GetPawnMesh_GetMeshForPortraitsToo
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) 
        {
            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);


            for (int i = 0; i < codeInstructions.Count; ++i) 
            {
                if (codeInstructions[i].Calls(flagSetMI)) 
                {
                    codeInstructions[i].operand = replacementMI;
                }
            }

            return codeInstructions;
        }

        private static bool PushFalseOnStack(PawnRenderFlags flag, PawnRenderFlags flag2) 
        {
            return false;
        }

        static MethodInfo flagSetMI = typeof(PawnRenderFlagsExtension).GetMethod(nameof(PawnRenderFlagsExtension.FlagSet), BindingFlags.Public | BindingFlags.Static);
        static MethodInfo replacementMI = typeof(HarmonyPatches_GetPawnMesh_GetMeshForPortraitsToo).GetMethod(nameof(PushFalseOnStack), BindingFlags.NonPublic | BindingFlags.Static);

    }
}
