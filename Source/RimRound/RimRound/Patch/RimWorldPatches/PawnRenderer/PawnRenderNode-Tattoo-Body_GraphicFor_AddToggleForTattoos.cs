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
    [HarmonyPatch(typeof(PawnRenderNode_Tattoo_Body))]
    [HarmonyPatch(nameof(PawnRenderNode_Tattoo_Body.GraphicFor))]
    public class PawnRenderNode_Tattoo_Body_GraphicFor_AddToggleForTattoos
    {
        public static void Postfix(ref Graphic __result, Pawn pawn) {
            if (!GlobalSettings.showBodyTatoosForCustomSprites && BodyTypeUtility.HasCustomBody(pawn)) {
                __result = null;
            }
        }
    }
}
