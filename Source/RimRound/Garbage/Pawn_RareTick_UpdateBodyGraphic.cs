using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using RimWorld;
using Verse;
using RimRound.Utilities;
using RimRound.Defs;
using RimRound.Hediffs;
using UnityEngine;
using AlienRace;
using RimRound.Comps;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("TickRare")]
    public class Pawn_RareTick_UpdateBodyGraphic
    {
        public static void Postfix(Pawn __instance) 
        {
            if (__instance.RaceProps.Humanlike)
            {
                if (__instance.TryGetComp<PawnBodyType_ThingComp>().ticksSinceLastBodyChange > numberOfTicksCooldownPerChange)


                if (Functions.GetBodyTypeBasedOnWeightSeverity(__instance) is BodyTypeDef b && b != __instance.story.bodyType)
                {
                    __instance.story.bodyType = b;
                    Functions.RedrawPawn(__instance);
                }
            }
        }


        const int numberOfTicksCooldownPerChange = 150;



    }
}
