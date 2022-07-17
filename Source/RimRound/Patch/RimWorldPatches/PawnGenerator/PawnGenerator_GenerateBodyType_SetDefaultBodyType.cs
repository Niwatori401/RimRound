using HarmonyLib;
using RimRound.Comps;
using RimRound.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(PawnGenerator))]
    [HarmonyPatch("GenerateBodyType")]
    public class PawnGenerator_GenerateBodyType_SetDefaultBodyType
    {
        public static void Postfix(Pawn pawn)
        {
            var comp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
            if (comp != null)
                comp.DefaultBodyType = pawn.story.bodyType;
        }
    }
}
