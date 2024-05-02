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
    /// <summary>
    /// This patch allows pawns that are downed but still capable of talking to recieve social interactions. 
    /// </summary>
    [HarmonyPatch(typeof(InteractionUtility))]
    [HarmonyPatch(nameof(InteractionUtility.CanReceiveRandomInteraction))]
    public class InteractionUtility_CanReceiveRandomInteraction_AllowImmobilePawnsToRecieveInteractions
    {
        public static void Postfix(ref bool __result, Pawn p) 
        {
            if (__result == true)
                return;

            if (p.health.capacities.CapableOf(PawnCapacityDefOf.Talking)) 
                __result =  InteractionUtility.CanReceiveInteraction(p, null) && p.RaceProps.Humanlike && !p.InAggroMentalState;

            return;
        }
    }
}
