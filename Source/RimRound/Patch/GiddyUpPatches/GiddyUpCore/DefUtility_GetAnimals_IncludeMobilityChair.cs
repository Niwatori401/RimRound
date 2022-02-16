using HarmonyLib;
using RimRound.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    public class DefUtility_GetAnimals_IncludeMobilityChair
    {
        static void Postfix(List<ThingDef> __result) 
        {
            __result.Add(DefDatabase<ThingDef>.AllDefsListForReading.Find(x => x.defName == "RR_HoverChair"));
        }

        public static PatchCollection GetPatchCollection()
        {
            return new PatchCollection
            {
                postfix = typeof(DefUtility_GetAnimals_IncludeMobilityChair).GetMethod(
                    nameof(DefUtility_GetAnimals_IncludeMobilityChair.Postfix), ModCompatibilityUtility.majorFlags)
            };
        }
    }
}
