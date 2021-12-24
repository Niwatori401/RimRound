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
    [HarmonyPatch(nameof(Caravan.RemovePawn))]
    public class Caravan_RemovePawn_ChangeDietMode
    {
        public static bool Prefix(Pawn __0)
        {
            CaravanPatchUtility.RestoreDietMode(__0);

            return true;
        }
    }
}
