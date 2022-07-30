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
    [HarmonyPatch(typeof(JobDriver_LayDown))]
    [HarmonyPatch(nameof(JobDriver_LayDown.LayDownToil))]
    class JobDriver_LayDown_LayDownToil_UpdatePawnPortrait
    {
        public static void Postfix(JobDriver_LayDown __instance) 
        {
            if (Scribe.mode != LoadSaveMode.PostLoadInit)
                BodyTypeUtility.UpdatePawnSprite(__instance.pawn, false, false, true, false);
        }
    }
}
