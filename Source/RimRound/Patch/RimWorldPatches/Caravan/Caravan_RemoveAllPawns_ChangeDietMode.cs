using HarmonyLib;
using RimRound.Utilities;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(Caravan))]
    [HarmonyPatch(nameof(Caravan.RemoveAllPawns))]
    public class Caravan_RemoveAllPawns_ChangeDietMode
    {
        public static bool Prefix(Caravan __instance) 
        {
            int caravanID = __instance.ID; //CaravanPatchUtility.GetUniqueID(__instance);

            if (CaravanPatchUtility.activeCaravans.ContainsKey(caravanID))
            {
                foreach (Pawn p in CaravanPatchUtility.activeCaravans[caravanID])
                {
                    CaravanPatchUtility.RestoreDietMode(p);
                }
                CaravanPatchUtility.activeCaravans.Remove(caravanID);
            }

            return true;
        }
    }
}
