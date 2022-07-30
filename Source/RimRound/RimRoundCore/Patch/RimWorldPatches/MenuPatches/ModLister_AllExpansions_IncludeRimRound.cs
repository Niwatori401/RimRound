using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(ModLister))]
    [HarmonyPatch(nameof(ModLister.AllExpansions), MethodType.Getter)]
    public class ModLister_AllExpansions_IncludeRimRound
    {
        static FieldInfo AllExpansionsCachedFI = typeof(ModLister).GetField("AllExpansionsCached");

        public static void Postfix(ref List<ExpansionDef> __result) 
        {
            if (!__result.Contains(Defs.ExpansionDefOf.RimRound_Expansion_Royalty)) 
            {
                List<ExpansionDef> rimRoundExpansions = new List<ExpansionDef>();
                foreach (var x in DefDatabase<ExpansionDef>.AllDefsListForReading)
                {
                    if (x.linkedMod == "niwatori401.rimround")
                    {
                        rimRoundExpansions.Add(x);
                    }
                }


                if (rimRoundExpansions.Count > 0)
                {
                    __result.InsertRange(__result.Count, rimRoundExpansions);
                }
            }

            return;
        }
    }
}
