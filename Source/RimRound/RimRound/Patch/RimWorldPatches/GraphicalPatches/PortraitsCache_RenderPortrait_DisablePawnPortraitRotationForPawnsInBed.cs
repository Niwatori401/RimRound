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
        

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) 
        {
            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);
            List<CodeInstruction> newInstructions = new List<CodeInstruction>();

            MethodInfo shouldSkipOffsetMI = typeof(PortraitsCache_RenderPortrait_DisablePawnPortraitRotationForPawnsInBed)
                .GetMethod(nameof(ShouldSkipOffsets), BindingFlags.NonPublic | BindingFlags.Static);

            MethodInfo pawnCacheRenderGetMI = typeof(Find).GetProperty(nameof(Find.PawnCacheRenderer), BindingFlags.Static | BindingFlags.Public).GetGetMethod(true);

            if (pawnCacheRenderGetMI is null)
            {
                Log.Error($"pawnCacheRenderGetMI was null in {nameof(PortraitsCache_RenderPortrait_DisablePawnPortraitRotationForPawnsInBed)}");
            }

            bool foundInsertionPoint = false;

            for (int i = 0; i < codeInstructions.Count; ++i) 
            {
                if (codeInstructions[i].opcode == OpCodes.Ldc_R4 && (float)codeInstructions[i].operand == 85) 
                {
                    foundInsertionPoint = true;
                    bool foundLabel = false;
                    Label label = generator.DefineLabel();
                    for (int j = i; j < codeInstructions.Count; ++j)
                    {
                        if (codeInstructions[j].Calls(pawnCacheRenderGetMI))
                        {
                            codeInstructions[j].labels.Add(label);
                            foundLabel = true;
                        }
                    }

                    if (!foundLabel)
                    {
                        Log.Error($"Failed to find label for transpiler in {nameof(PortraitsCache_RenderPortrait_DisablePawnPortraitRotationForPawnsInBed)}");
                    }

                    newInstructions.Add(new CodeInstruction(OpCodes.Ldarg_1)); // Branch out of block conditionally
                    newInstructions.Add(new CodeInstruction(OpCodes.Call, shouldSkipOffsetMI));
                    newInstructions.Add(new CodeInstruction(OpCodes.Brtrue, label));

                    codeInstructions.InsertRange(i, newInstructions);

                    break;
                }
            }

            if (!foundInsertionPoint)
                Log.Error($"Failed to find suitable insert location in patch {nameof(PortraitsCache_RenderPortrait_DisablePawnPortraitRotationForPawnsInBed.Transpiler)}.");

            return codeInstructions;
        }

        // we already know the pawn is downed or dead
        static bool ShouldSkipOffsets(Pawn pawn)
        {
            return pawn.InBed();
        }
    }
}
