using HarmonyLib;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(Toils_LayDown))]
    [HarmonyPatch(nameof(Toils_LayDown.LayDown))]
    internal class Toils_LayDown_LayDown_AddRefreshToClothingWhenLayDownToil
    {
        public static void Postfix(ref Toil __result)
        {
            Action vanillaAction = __result.initAction;

            Toil toil = __result;
            Action newAction = () =>
            {
                if (toil?.actor?.Drawer?.renderer?.renderTree == null)
                    return;

                if (!MobilityChairUtility.IsWearingAMobilityScooter(toil.actor))
                    return;

                toil.actor.Drawer.renderer.renderTree.SetDirty();
            };

            __result.initAction = new Action(() => { vanillaAction(); newAction(); });
        }
    }
}
