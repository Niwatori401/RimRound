using AlienRace;
using HarmonyLib;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(HarmonyPatches))]
    [HarmonyPatch(nameof(HarmonyPatches.CheckBodyType))]
    internal class HarmonyPatches_CheckBodyType_ChangeRRToThin
    {
        public static void Postfix(ref BodyTypeDef __result) 
        {
            if (BodyTypeUtility.IsRRBody(__result))
                __result = RimWorld.BodyTypeDefOf.Thin;

            return;
        }
    }
}
