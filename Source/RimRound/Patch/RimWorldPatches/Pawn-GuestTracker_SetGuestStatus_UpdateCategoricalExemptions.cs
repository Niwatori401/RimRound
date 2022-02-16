using HarmonyLib;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(Pawn_GuestTracker))]
    [HarmonyPatch(nameof(Pawn_GuestTracker.SetGuestStatus))]
    public class Pawn_GuestTracker_SetGuestStatus_UpdateCategoricalExemptions
    {
        public static void Postfix()
        {
            BodyTypeUtility.AssignBodyTypeCategoricalExemptions(true);
        }
    }
}
