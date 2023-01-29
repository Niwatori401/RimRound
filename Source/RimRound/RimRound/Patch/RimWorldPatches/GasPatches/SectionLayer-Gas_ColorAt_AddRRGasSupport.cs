using HarmonyLib;
using RimRound.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.Patch
{
    /// <summary>
    /// Adds support for RR gasses so that it is colored correctly.
    /// </summary>
    [HarmonyPatch(typeof(SectionLayer_Gas))]
    [HarmonyPatch(nameof(SectionLayer_Gas.ColorAt))]
    public class SectionLayer_Gas_ColorAt_AddRRGasSupport
    {
        static MethodInfo mapGetterMI = typeof(SectionLayer).GetProperty("Map", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
        public static void Postfix(IntVec3 cell, SectionLayer_Gas __instance, ref Color __result)
        {
            Map map = (Map)mapGetterMI.Invoke(__instance, null);

            if (map is null)
            {
                Log.Error("Map was null in SectionLayer_Gas patch.");
                return;
            }

            if (!(map.GetComponent<MapComp_RRGasGrid>() is MapComp_RRGasGrid comp))
            {
                Log.Message("Comp was null in SectionLayer_Gas");
                return;
            }

            if (comp.AnyGasAt(cell))
                __result = comp.ColorAt(cell);

            return;
        }
    }
}
