using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(Pawn_HealthTracker))]
    [HarmonyPatch("MakeDowned")]
    internal class Pawn_HealthTracker_MakeDowned_DropHoverChair
    {
            public static void Postfix(Pawn ___pawn)
            {
                if (Utilities.MobilityChairUtility.GetMobilityScooter(___pawn) is Apparel ap)
                {
                    ___pawn.apparel.TryDrop(ap);
                }
            }
    }
}
