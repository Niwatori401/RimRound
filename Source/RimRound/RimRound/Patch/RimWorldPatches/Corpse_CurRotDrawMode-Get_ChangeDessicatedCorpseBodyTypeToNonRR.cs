using HarmonyLib;
using RimRound.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(Corpse))]
    [HarmonyPatch(nameof(Corpse.CurRotDrawMode), MethodType.Getter)]
    public class Corpse_CurRotDrawMode_Get_ChangeDessicatedCorpseBodyTypeToNonRR
    {
        public static void Postfix(RotDrawMode __result, Corpse __instance) 
        {
            if (__result != RotDrawMode.Dessicated)
                return;

            if (!(__instance.InnerPawn?.RaceProps?.Humanlike is bool b && b))
                return;

            __instance.InnerPawn.story.bodyType = RimWorld.BodyTypeDefOf.Thin;
            BodyTypeUtility.UpdatePawnSprite(__instance.InnerPawn, true, true, true, false);
        }
    }
}
