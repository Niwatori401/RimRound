using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(PortraitsCache))]
    [HarmonyPatch(nameof(PortraitsCache.Get))]
    public class PortraitsCache_Get_MakePortraitsScaleForBodySize
    {
        public static bool Prefix(Pawn __0, ref Vector2 __1, ref Vector3 __3, ref float __4, bool __5 = true) 
        {
            //3 moves the portrait around
            //1 is the resolution?
            //4 is the zoom of everything

            switch (RimRound.Comps.Debug_ThingComp.paramNumber)
            {
                case 0:
                    break;
                case 1:
                    __1.Scale(new Vector2(Comps.Debug_ThingComp.paramFloat, Comps.Debug_ThingComp.paramFloat));
                    break;
                case 2:
                    break;
                case 3:
                    __3.z += Comps.Debug_ThingComp.paramFloat;
                    break;
                case 4:
                    __4 *= Comps.Debug_ThingComp.paramFloat;
                    break;
                case 5:
                    break;
               default:
                    break;
            }

            return true;
        }
    }
}
