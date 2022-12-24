using HarmonyLib;
using RimRound.Comps;
using RimRound.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch.RimWorldPatches
{
    [HarmonyPatch(typeof(GenTemperature))]
    [HarmonyPatch(nameof(GenTemperature.PushHeat), new Type[] { typeof(Thing), typeof(float) })]
    public class GenTemperature_PushHeat_AdjustForPerks
    {
        public static bool Prefix(Thing __0, ref float __1) 
        {
            if (__0 is Pawn p)
            {
                var comp = p.TryGetComp<FullnessAndDietStats_ThingComp>();
                if (comp is null)
                    return true;

                int fatFurnaceLevels = comp.perkLevels.PerkToLevels?["RR_Fat_Furnace_Title"] ?? 0;
                
                float gel1Severity = 1.410f * RacialBodyTypeInfoUtility.GetBodyTypeWeightRequirementMultiplier(p.AsPawn());
                if (p.AsPawn().WeightHediff().Severity < gel1Severity)
                {
                    return true;
                }
                
                if (fatFurnaceLevels == 1)
                {
                    __1 = 3.5f * p.BodySize * 4.1666665f;
                }
                else if (fatFurnaceLevels == 2)
                {
                    __1 = 10f * p.BodySize * 4.1666665f;
                } 
                else 
                {
                    // don't change energy output
                }
            }

            return true;
        }
    }
}
