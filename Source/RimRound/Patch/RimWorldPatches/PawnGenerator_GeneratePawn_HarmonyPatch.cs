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
                Utilities.HediffUtility.SetHediffSeverity(Defs.HediffDefOf.RimRound_Weight, pawn, Utilities.HediffUtility.KilosToSeverity(GetRandomStartingWeight() * 1000));
                //Utilities.BodyTypeUtility.UpdatePawnSprite(pawn);
            }
        }

        public static void AddWeightOpinion(Pawn pawn)
        {
            if ((pawn?.RaceProps.Humanlike ?? false) && pawn.TryGetComp<ThingComp_PawnAttitude>() is ThingComp_PawnAttitude comp)
            {
                comp.weightOpinion = WeightOpinionUtility.TraitDefToWeightOpinion(WeightOpinionUtility.AssignInitialWeightOpinionTraits(pawn));
            }
        }

        static List<Pair<float, float>> weightDistributionRanges = new List<Pair<float, float>>()
        {
            new Pair<float, float>(0.19301f, 0.040f),
            new Pair<float, float>(0.45035f, 0.045f),
            new Pair<float, float>(0.70770f, 0.055f),
            new Pair<float, float>(0.83637f, 0.075f),
            new Pair<float, float>(0.90071f, 0.090f),
            new Pair<float, float>(0.93931f, 0.105f),
            new Pair<float, float>(0.96505f, 0.130f),
            new Pair<float, float>(0.97791f, 0.160f),
            new Pair<float, float>(0.98435f, 0.195f),
            new Pair<float, float>(0.99078f, 0.240f),
            new Pair<float, float>(0.99464f, 0.270f),
            new Pair<float, float>(0.99721f, 0.320f),
            new Pair<float, float>(0.99850f, 0.390f),
            new Pair<float, float>(0.99914f, 0.470f),
            new Pair<float, float>(0.99953f, 0.575f),
            new Pair<float, float>(0.99979f, 0.700f),
            new Pair<float, float>(0.99992f, 0.840f),
            new Pair<float, float>(0.99998f, 1.005f),
            new Pair<float, float>(0.99999f, 1.200f),
            new Pair<float, float>(1.00000f, 1.450f)
        };

        static float GetRandomStartingWeight() 
        {
            float random = Values.RandomFloat(0, 1);

            for (int i = 0; i < weightDistributionRanges.Count; ++i)
            {
                Pair<float, float> currentPair = weightDistributionRanges[i];
                if (random <= currentPair.First)
                {
                    if (i > 0)
                    {
                        Pair<float, float> previousPair = weightDistributionRanges[i - 1];
                        return Values.RandomFloat(previousPair.Second, currentPair.Second);
                    }
                    else
                    {
                        Pair<float, float> nextPair = weightDistributionRanges[i + 1];
                        return Values.RandomFloat(currentPair.Second, nextPair.Second);
                    }
                }

            }

            return weightDistributionRanges.First().First;


            
        }
    }
}
