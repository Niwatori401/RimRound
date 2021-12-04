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
        public static bool Prefix(PawnRenderer __instance, ref Rot4 __result) 
        {
            if (((Pawn)Traverse.Create(__instance).Field("pawn").GetValue()).TryGetComp<SleepingPosition_ThingComp>() is SleepingPosition_ThingComp comp)
            {
                if (((Pawn)comp.parent).Downed && !((Pawn)comp.parent).InBed())
                {
                    __result = Rot4.South;
                    return false;
                }

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

            return true;
        }
    }
}
