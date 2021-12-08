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
    [HarmonyPatch(typeof(ModsConfig))]
    [HarmonyPatch(nameof(ModsConfig.LastInstalledExpansion), MethodType.Getter)]
    public static class ModsConfig_LastInstalledExpansion_DefaultToRimRoundBG
    {
        public static void Postfix(ref ExpansionDef __result) 
        {
            __result = RimRound.Defs.ExpansionDefOf.RimRound_Expansion_Royalty;
            return; 
        }
    }
}
