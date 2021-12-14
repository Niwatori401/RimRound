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
    [HarmonyPatch(typeof(GenGuest))]
    [HarmonyPatch(nameof(GenGuest.EmancipateSlave))]
    public class GenGuest_EmancipateSlave_UpdateCategoricalExemptions
    {
        public static void Postfix()
        {
            BodyTypeUtility.AssignBodyTypeCategoricalExemptions(true);
        }

    }
}
