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
    [HarmonyPatch(typeof(Faction))]
    [HarmonyPatch(nameof(Faction.SetRelationDirect))]
    public class Faction_SetRelationDirect_UpdateCategoricalExemption
    {
        public static void Postfix(Faction __instance, Faction __0) 
        {
            if (__instance == Faction.OfPlayer)
                return;

            if (__0 == Faction.OfPlayer)
                Functions.AssignBodyTypeCategoricalExemptions(true);

            return;
        }
    }
}
