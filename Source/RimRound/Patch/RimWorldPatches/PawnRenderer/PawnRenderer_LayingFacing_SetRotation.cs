using HarmonyLib;
using RimRound.Comps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(PawnRenderer))]
    [HarmonyPatch("LayingFacing")]
    public class PawnRenderer_LayingFacing_SetRotation
    {
        public static bool Prefix(PawnRenderer __instance, ref Rot4 __result, Pawn ___pawn) 
        {
            if (___pawn.TryGetComp<SleepingPosition_ThingComp>() is SleepingPosition_ThingComp comp)
            {
                if (PawnIsDownedAndNotInBed(___pawn))
                    return true;

                if (___pawn.InBed())
                    return GetCustomLayingRotation(ref __result, comp);
            }

            return true;
        }

        private static bool GetCustomLayingRotation(ref Rot4 __result, SleepingPosition_ThingComp comp)
        {
            switch (comp.sleepIndex)
            {
                case 0:
                    __result = Rot4.East;
                    return false;
                case 1:
                    __result = Rot4.North;
                    return false;
                case 2:
                    __result = Rot4.West;
                    return false;
                case 3:
                    __result = Rot4.South;
                    return false;
                default:
                    return true;
            }
        }

        private static bool PawnIsDownedAndNotInBed(Pawn pawn)
        {
            return pawn.Downed && !pawn.InBed();
        }
    }
}
