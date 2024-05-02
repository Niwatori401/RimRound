using HarmonyLib;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(PawnRenderUtility))]
    [HarmonyPatch(nameof(PawnRenderUtility.RenderAsPack))]
    public class PawnRenderer_RenderAsPack_DontRenderAsPackForCustomBodies
    {
        public static void Postfix(ref bool __result, Apparel __0)
        {
            if (!GlobalSettings.hidePacksForCustomBodies || __result == false || __0 is null)
                return;

            Pawn pawn = __0.Wearer;

            if (pawn is null)
                return;

            if (BodyTypeUtility.IsRRBody(pawn.story.bodyType))
            {
                __result = false;
            }

            return;
        }
    }
}
