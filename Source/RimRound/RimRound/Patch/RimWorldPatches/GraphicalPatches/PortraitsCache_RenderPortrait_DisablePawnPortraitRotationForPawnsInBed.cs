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
    [HarmonyPatch]
    public class PortraitsCache_RenderPortrait_DisablePawnPortraitRotationForPawnsInBed
    {
       
        static MethodBase TargetMethod() 
        {
            Type a =
				(from type
				in typeof(PortraitsCache).GetNestedTypes(BindingFlags.Instance | BindingFlags.NonPublic)
				where type.Name == "PortraitParams"
				select type).First();

            return a.GetMethod("RenderPortrait");
        }
        

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) 
        {
            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);
            List<CodeInstruction> newInstructions = new List<CodeInstruction>();

            MethodInfo getAngleMI = typeof(PortraitsCache_RenderPortrait_DisablePawnPortraitRotationForPawnsInBed)
                .GetMethod(nameof(GetAngle), BindingFlags.NonPublic | BindingFlags.Static);

            if (getAngleMI is null)
                Log.Error("shouldBeSidewaysMI was null in PortraitsCache_RenderPortrait patch!");



            for (int i = 0; i < codeInstructions.Count; ++i) 
            {
                if (codeInstructions[i].opcode == OpCodes.Ldc_R4 && (float)codeInstructions[i].operand == 85) 
                {
                    codeInstructions[i] = new CodeInstruction(OpCodes.Ldarg_1);
                    codeInstructions.Insert(i + 1, new CodeInstruction(OpCodes.Call, getAngleMI));
                    break;
                }
            }

            return codeInstructions;
        }

        static float GetAngle(Pawn pawn) 
        {
            if (pawn.Dead || (pawn.Downed && !pawn.InBed()))
                return 85f;

            return 0f;
        }
    }
}
