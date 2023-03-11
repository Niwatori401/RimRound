using HarmonyLib;
using RimRound.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.Patch
{
	[HarmonyPatch(typeof(PawnRenderer))]
    [HarmonyPatch("RenderPawnInternal")]
    public class PawnRenderer_RenderPawnInternal_AdjustHeadDrawDepth
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);

            bool foundInsertionPoint = AdjustHeadPosition(codeInstructions);

            if (!foundInsertionPoint)
                Log.Error($"Failed to find insertion point in {nameof(PawnRenderer_RenderPawnInternal_AdjustHeadDrawDepth)}.");

            return codeInstructions;
        }


        private static bool AdjustHeadPosition(List<CodeInstruction> codeInstructions)
        {
            bool foundInsertionPoint = false;

            if (drawMeshNowOrLaterMI is null)
                Log.Error("drawMeshNowOrLaterMI was null in PawnRenderer_RenderPawnInternal_AdjustHeadDrawPosForPortrait patch!");

            if (replacementFunctionMI is null)
                Log.Error("replacementFunctionMI was null in PawnRenderer_RenderPawnInternal_AdjustHeadDrawPosForPortrait patch!");

            for (int i = 0; i < codeInstructions.Count; ++i)
            {
                if (codeInstructions[i].Calls(drawMeshNowOrLaterMI))
                {
                    foundInsertionPoint = true;

                    codeInstructions[i].operand = replacementFunctionMI;
                    codeInstructions.InsertRange(i, new List<CodeInstruction>() { new CodeInstruction(OpCodes.Ldarg_S, 6 ), new CodeInstruction(OpCodes.Ldarg_0)});
                }
            }

            return foundInsertionPoint;
        }

        private static void DrawMeshForHeadBasedOnIfPortrait(Mesh mesh, Vector3 vector3, Quaternion quaternion, Material material, bool drawNow, PawnRenderFlags renderFlags, PawnRenderer instance) 
		{
			if (renderFlags.FlagSet(PawnRenderFlags.Portrait))
			{
				GenDraw.DrawMeshNowOrLater(mesh, vector3, quaternion, material, drawNow);
			}
			else 
			{
                Pawn pawn = pawnRendererPawnFI.GetValue(instance).AsPawn();

                Vector3 extraOffset = new Vector3(0, (GlobalSettings.alternateNorthHeadPositionForRRBodytypes && BodyTypeUtility.HasCustomBody(pawn)) ? -headYOffset : 0 , 0);

                GenDraw.DrawMeshNowOrLater(mesh, vector3 + extraOffset, quaternion, material, drawNow);
			}
		}

        static MethodInfo drawMeshNowOrLaterMI = typeof(GenDraw).GetMethod(
            nameof(GenDraw.DrawMeshNowOrLater),
            BindingFlags.Public | BindingFlags.Static,
            Type.DefaultBinder,
            new Type[] { typeof(Mesh), typeof(Vector3), typeof(Quaternion), typeof(Material), typeof(bool) },
            null);

        static MethodInfo replacementFunctionMI = typeof(PawnRenderer_RenderPawnInternal_AdjustHeadDrawDepth).GetMethod(
            nameof(DrawMeshForHeadBasedOnIfPortrait),
            BindingFlags.NonPublic | BindingFlags.Static);


        static MethodInfo drawHeadHairMI = typeof(PawnRenderer).GetMethod("DrawHeadHair", BindingFlags.NonPublic | BindingFlags.Instance);

        static FieldInfo pawnRendererPawnFI = typeof(PawnRenderer).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);

        const float headYOffset = 0.01f;
    }
}
