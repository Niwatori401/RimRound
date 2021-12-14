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

            AdjustHeadPositionInPortrait(codeInstructions);

            AdjustHairPositionInPortrait(codeInstructions);

            return codeInstructions;
        }

        private static void AdjustHairPositionInPortrait(List<CodeInstruction> codeInstructions)
        {
            for (int i = 0; i < codeInstructions.Count; ++i)
            {
                if (codeInstructions[i].Calls(drawHeadHairMI))
                {
                    codeInstructions[i].operand = drawHeadHairAdjustedMI;
                    codeInstructions[i].opcode = OpCodes.Call;
                }
            }
        }

        private static void AdjustHeadPositionInPortrait(List<CodeInstruction> codeInstructions)
        {


            if (drawMeshNowOrLaterMI is null)
                Log.Error("drawMeshNowOrLaterMI was null in PawnRenderer_RenderPawnInternal_AdjustHeadDrawPosForPortrait patch!");

            if (replacementFunctionMI is null)
                Log.Error("replacementFunctionMI was null in PawnRenderer_RenderPawnInternal_AdjustHeadDrawPosForPortrait patch!");

            for (int i = 0; i < codeInstructions.Count; ++i)
            {
                if (codeInstructions[i].Calls(drawMeshNowOrLaterMI))
                {
                    codeInstructions[i].operand = replacementFunctionMI;
                    codeInstructions.InsertRange(i, new List<CodeInstruction>() { new CodeInstruction(OpCodes.Ldarg_S, 6 ), new CodeInstruction(OpCodes.Ldarg_0)});
                }
            }
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

                Vector3 extraOffset = new Vector3(0, (BodyTypeUtility.HasCustomBody(pawn) && GlobalSettings.alternateNorthHeadPositionForRRBodytypes) ? -headYOffset : 0 , 0);

                GenDraw.DrawMeshNowOrLater(mesh, vector3 + extraOffset, quaternion, material, drawNow);
			}
		}

        private static void DrawHeadHairAdjusted(PawnRenderer instance, Vector3 rootloc, Vector3 position, float angle, Rot4 bodyfacing, Rot4 headfacing, RotDrawMode rotDrawMode, PawnRenderFlags renderFlags) 
        {
            if (renderFlags.FlagSet(PawnRenderFlags.Portrait))
            {
                drawHeadHairMI.Invoke(instance, new object[] { rootloc, position, angle, bodyfacing, headfacing, rotDrawMode, renderFlags });
            }
            else
            {
                drawHeadHairMI.Invoke(instance, new object[] { rootloc, position, angle, bodyfacing, headfacing, rotDrawMode, renderFlags });
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

        static MethodInfo drawHeadHairAdjustedMI = typeof(PawnRenderer_RenderPawnInternal_AdjustHeadDrawDepth).GetMethod(nameof(DrawHeadHairAdjusted), BindingFlags.Static | BindingFlags.NonPublic);

        static FieldInfo pawnRendererPawnFI = typeof(PawnRenderer).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);

        const float headYOffset = 0.01f;
    }
}
