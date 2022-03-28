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
    [HarmonyPatch(typeof(MechClusterGenerator))]
    [HarmonyPatch(nameof(MechClusterGenerator.MechKindSuitableForCluster))]
    class MechClusterGenerator_MechKindSuitableForCluster_ExcludeHoverchair
    {
        static void Postfix(ref bool __result, PawnKindDef def) 
        {
            if (def.defName == "RR_HoverChairPawnKind") 
            {
                __result = false;
            }

            return;
        }
    }
}
