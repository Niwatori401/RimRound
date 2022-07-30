using HarmonyLib;
using RimRound.FeedingTube.Comps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.FeedingTube.Patches
{
    [HarmonyPatch(typeof(Designator_Install))]
    [HarmonyPatch(nameof(Designator_Install.SelectedUpdate))]
    public class Designator_Install_SelectedUpdate_UpdateLastDrawnFrameForFoodPipes
    {
        public static void Postfix(Designator_Install __instance)
        {
            ThingDef thingDef = (ThingDef)__instance.PlacingDef;

            if (thingDef is null || thingDef.comps is null)
                return;

            IEnumerable<CompProperties> validCompProps =
                from x in thingDef.comps
                where x.compClass == typeof(FoodTransmitter_ThingComp)
                select x;

            if (validCompProps.Count() > 0)
            {
                SectionLayer_ThingsFoodGrid.DrawFoodGridOverlayThisFrame();
            }
        }
    }
}
