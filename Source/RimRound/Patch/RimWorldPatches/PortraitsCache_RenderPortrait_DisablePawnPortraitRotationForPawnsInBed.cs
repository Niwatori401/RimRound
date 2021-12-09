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
    [HarmonyPatch(typeof(PortraitsCache))]
    [HarmonyPatch("RenderPortrait")]
    public class PortraitsCache_RenderPortrait_DisablePawnPortraitRotationForPawnsInBed
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) 
        {
            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);
            List<CodeInstruction> newInstructions = new List<CodeInstruction>();

            int startJndex = -1;
            int endJndex = -1;

            MethodInfo pawnDeadMI = typeof(Pawn).GetProperty(nameof(Pawn.Dead)).GetGetMethod();
            MethodInfo pawnDownedMI = typeof(Pawn).GetProperty(nameof(Pawn.Downed)).GetGetMethod();
            MethodInfo shouldBeSidewaysMI = typeof(PortraitsCache_RenderPortrait_DisablePawnPortraitRotationForPawnsInBed)
                .GetMethod(nameof(ShouldBeSideways), BindingFlags.NonPublic | BindingFlags.Static);


            if (pawnDeadMI is null)
                Log.Error("pawnDeadMI was null in PortraitsCache_RenderPortrait patch!");
            if (pawnDownedMI is null)
                Log.Error("pawnDownedMI was null in PortraitsCache_RenderPortrait patch!");
            if (shouldBeSidewaysMI is null)
                Log.Error("shouldBeSidewaysMI was null in PortraitsCache_RenderPortrait patch!");



            for (int i = 0; i < codeInstructions.Count; ++i) 
            {
                if (codeInstructions[i].Calls(pawnDeadMI)) 
                {
                    startJndex = i;
                }

                if (codeInstructions[i].Calls(pawnDownedMI)) 
                {
                    endJndex = i;
                }

                if (endJndex != -1 && startJndex != -1)
                    break;
            }

            newInstructions.Add(new CodeInstruction(OpCodes.Call, shouldBeSidewaysMI));


            if (startJndex != -1 && endJndex != -1)
            {
                codeInstructions.RemoveRange(startJndex, (endJndex - startJndex) + 1);
                codeInstructions.InsertRange(startJndex, newInstructions);
            }

            return codeInstructions;
        }

        static bool ShouldBeSideways(Pawn pawn) 
        {
            if (pawn.Dead || (pawn.Downed && !pawn.InBed()))
                return true;

            return false;
        }
    }
}
