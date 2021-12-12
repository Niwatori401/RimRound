using HarmonyLib;
using RimRound.Utilities;
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
        public static bool Prefix(Pawn __0, ref Vector3 __3, ref float __4) 
        {
            //3 moves the portrait around
            //1 is the resolution?
            //4 is the zoom of everything
            //if (Values.portraitOffsetByBodyType.ContainsKey(__0.story.bodyType))
                __3.z += Values.debugFloat;

            //if (Values.portraitZoomByBodyType.ContainsKey(__0.story.bodyType)) 
            //{
                __4 *= Comps.Debug_ThingComp.paramFloat1;
           // }

            Log.Message($"__3 {__3.ToString("F3")}, __4 = {__4}");

            

            return true;
        }
    }
}
