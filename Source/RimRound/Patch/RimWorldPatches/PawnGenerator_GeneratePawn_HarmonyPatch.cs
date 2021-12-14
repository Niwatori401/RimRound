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
    [HarmonyPatch("GeneratePawn", typeof(PawnGenerationRequest))]
    public class PawnGenerator_GeneratePawn_HarmonyPatch
    {
        public static void Postfix(Pawn __result) 
        {
            AddHediffsAndSetBodyType(__result);
            AddWeightOpinion(__result);
        }

        public static void AddHediffsAndSetBodyType(Pawn pawn)
        {
            if ((pawn?.RaceProps.Humanlike ?? false) && pawn.TryGetComp<FullnessAndDietStats_ThingComp>() is FullnessAndDietStats_ThingComp comp)
            {
                comp.defaultBodyType = pawn.story.bodyType;
                Utilities.HediffUtility.AddHediffOfDefTo(Defs.HediffDefOf.RimRound_Weight, pawn);
                Utilities.HediffUtility.AddHediffOfDefTo(Defs.HediffDefOf.RimRound_Fullness, pawn);
            }
        }

        public static void AddWeightOpinion(Pawn pawn)
        {
            if ((pawn?.RaceProps.Humanlike ?? false) && pawn.TryGetComp<ThingComp_PawnAttitude>() is ThingComp_PawnAttitude comp && comp.weightOpinion != WeightOpinion.None)
            {
                comp.weightOpinion = WeightOpinionUtility.TraitDefToWeightOpinion(WeightOpinionUtility.AssignInitialWeightOpinionTraits(pawn));
            }
        }
    }
}
