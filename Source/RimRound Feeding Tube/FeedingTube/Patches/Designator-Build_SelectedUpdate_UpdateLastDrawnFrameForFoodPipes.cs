using HarmonyLib;
using RimWorld;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimRound.FeedingTube.Comps;

namespace RimRound.FeedingTube.Patches
{
	[HarmonyPatch(typeof(Designator_Build))]
	[HarmonyPatch(nameof(Designator_Build.SelectedUpdate))]
    public class Designator_Build_SelectedUpdate_UpdateLastDrawnFrameForFoodPipes
    {
		public static void Postfix(ThingDef ___entDef)
		{
            if (___entDef is null || ___entDef.comps is null)
                return;

            IEnumerable<CompProperties> validCompProps =
                from x in ___entDef.comps
                where x.compClass == typeof(FoodTransmitter_ThingComp)
                select x;

            if (validCompProps.Count() > 0)
            {
                SectionLayer_ThingsFoodGrid.DrawFoodGridOverlayThisFrame();
            }
        }
	}
}
