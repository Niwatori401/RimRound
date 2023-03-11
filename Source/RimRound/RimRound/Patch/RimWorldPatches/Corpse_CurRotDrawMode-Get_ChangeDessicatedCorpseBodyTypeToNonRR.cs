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

            if (checkedCorpseIds.Contains(__instance.ThingID))
                return;
            else
                checkedCorpseIds.Add(__instance.ThingID);
            
            if (!(__instance.InnerPawn?.RaceProps?.Humanlike is bool b && b))
                return;

            BodyTypeUtility.UpdatePawnSprite(__instance.InnerPawn, true, true, true, true);
        }


        static HashSet<string> checkedCorpseIds = new HashSet<string>();
    }
}
