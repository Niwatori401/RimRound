using HarmonyLib;
using RimRound.Utilities;
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
    [HarmonyPatch(typeof(PawnRenderer))]
    [HarmonyPatch("DrawBodyApparel")]
    public class PawnRenderer_DrawBodyApparel_AlterShellClothingIfNoRRSprite
    {
        static FieldInfo pawnFieldInfo = typeof(PawnRenderer).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic);
        static MethodInfo validatorMethod = typeof(PawnRenderer_DrawBodyApparel_AlterShellClothingIfNoRRSprite)
            .GetMethod(nameof(PawnRenderer_DrawBodyApparel_AlterShellClothingIfNoRRSprite.ShouldBeDrawnAsShell), BindingFlags.Static | BindingFlags.NonPublic);
        static FieldInfo shellCoversHeadFieldInfo = typeof(ApparelProperties).GetField(nameof(ApparelProperties.shellCoversHead), BindingFlags.Instance | BindingFlags.Public);

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);
            List<CodeInstruction> newInstructions = new List<CodeInstruction>();

            int startJndex = -1;

            bool attemptedPatch = false;

            if (pawnFieldInfo is null)
            {
                Log.Error($"PawnFieldInfo was null in {nameof(PawnRenderer_DrawBodyApparel_AlterShellClothingIfNoRRSprite.Transpiler)}");
                return codeInstructions;
            }

            if (validatorMethod is null)
            {
                Log.Error($"validatorMethod was null in {nameof(PawnRenderer_DrawBodyApparel_AlterShellClothingIfNoRRSprite.Transpiler)}");
                return codeInstructions;
            }

            if (shellCoversHeadFieldInfo is null)
            {
                Log.Error($"shellCoversHeadFieldInfo was null in {nameof(PawnRenderer_DrawBodyApparel_AlterShellClothingIfNoRRSprite.Transpiler)}");
                return codeInstructions;
            }

            for (int i = 0; i < codeInstructions.Count; ++i)
            {
                if (!(codeInstructions[i].operand is FieldInfo fi) || fi != shellCoversHeadFieldInfo)
                    continue;

                startJndex = i;
                attemptedPatch = true;
                break;
            }


            if (startJndex != -1)
            {
                newInstructions.Add(new CodeInstruction(OpCodes.Ldarg_0)); // Load instance onto stack
                newInstructions.Add(new CodeInstruction(OpCodes.Ldfld, pawnFieldInfo)); // Load pawn field from instance
                newInstructions.Add(new CodeInstruction(OpCodes.Call, validatorMethod));

                codeInstructions.InsertRange(startJndex + 1, newInstructions);
            }

            if (!attemptedPatch)
            {
                Log.Error($"Failed to find suitable patch location in {nameof(PawnRenderer_DrawBodyApparel_AlterShellClothingIfNoRRSprite.Transpiler)}");
            }


            return codeInstructions;
        }


        /// <param name="shellCoversHead">popped from previous call on stack. Used inside of function rather than adding logical opcodes for simplicity.</param>
        /// <returns><see langword="false"/> to skip the typical offset, <see langword="true"/> to give it one.</returns>
        private static bool ShouldBeDrawnAsShell(bool shellCoversHead, Pawn pawn) 
        {
            bool isRRBody = BodyTypeUtility.IsRRBody(pawn.story.bodyType);

            if (!shellCoversHead || isRRBody)
                return false;

            return true;
        }
    }
}
