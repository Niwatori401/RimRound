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
    [HarmonyPatch(typeof(GenGuest))]
    [HarmonyPatch(nameof(GenGuest.EnslavePrisoner))]
    public class GenGuest_EnslavePrisoner_UpdateCategoricalExemptions
    {
        public static void Postfix()
        {
            BodyTypeUtility.AssignBodyTypeCategoricalExemptions(true);
        }
    }
}
