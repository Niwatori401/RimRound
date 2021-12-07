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
        public static bool Prefix(Pawn __0, Vector2 __1, Rot4 __2, Vector3 __3, float __4, bool __5 = true) 
        {
            if (true) 
            {
                Log.Message("");
            }

            return true;
        }
    }
}
