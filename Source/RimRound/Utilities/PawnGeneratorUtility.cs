using RimRound.Comps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Utilities
{
    public static class PawnGeneratorUtility
    {
        public static void AddHediffsToPawn(Pawn pawn)
        {
            if (pawn?.RaceProps.Humanlike ?? false)
            {
                Utilities.HediffUtility.AddHediffOfDefTo(Defs.HediffDefOf.RimRound_Weight, pawn);
                Utilities.HediffUtility.AddHediffOfDefTo(Defs.HediffDefOf.RimRound_Fullness, pawn);

                var raceDict = RacialBodyTypeInfoUtility.GetRacialDictionary(pawn);
                float weightMultiplier = 1;
                if (raceDict != null)
                {
                    string finalBodyTypeDefName = raceDict?.Last().Key?.defName;
                    if (finalBodyTypeDefName != null)
                        weightMultiplier = RacialBodyTypeInfoUtility.GetBodyTypeWeightRequirementMultiplierByDefName(finalBodyTypeDefName);
                }



                Utilities.HediffUtility.SetHediffSeverity(
                    Defs.HediffDefOf.RimRound_Weight,
                    pawn,
                    Utilities.HediffUtility.KilosToSeverityWithBaseWeight(
                        GetRandomStartingWeight(GetWeightPercentileByFaction(pawn)) * 1000 * weightMultiplier)
                    );

            }
        }

        public static void SetDefaultBodyType(Pawn pawn)
        {
            if ((pawn?.RaceProps.Humanlike ?? false) && pawn.TryGetComp<FullnessAndDietStats_ThingComp>() is FullnessAndDietStats_ThingComp comp)
            {
                comp.defaultBodyType = pawn?.story?.adulthood is null ? BodyTypeDefOf.Thin : pawn.story.adulthood.BodyTypeFor(pawn.gender);
            }
        }

        public static void AddWeightOpinion(Pawn pawn)
        {
            if ((pawn?.RaceProps.Humanlike ?? false) && pawn.TryGetComp<ThingComp_PawnAttitude>() is ThingComp_PawnAttitude comp)
            {
                comp.weightOpinion = WeightOpinionUtility.TraitDefToWeightOpinion(WeightOpinionUtility.AssignInitialWeightOpinionTraits(pawn));
            }
        }


        //Percentiles/Weight
        static List<Pair<float, float>> playerFactionWeightDistribution = new List<Pair<float, float>>()
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

        static List<Pair<float, float>> hostileFactionWeightDistribution = new List<Pair<float, float>>()
        {
            new Pair<float, float>(0.19737f, 0.040f),
            new Pair<float, float>(0.46053f, 0.045f),
            new Pair<float, float>(0.72368f, 0.055f),
            new Pair<float, float>(0.85526f, 0.075f),
            new Pair<float, float>(0.92105f, 0.090f),
            new Pair<float, float>(0.96053f, 0.105f),
            new Pair<float, float>(0.98684f, 0.130f),
            new Pair<float, float>(1.00000f, 0.160f),
            new Pair<float, float>(1.00000f, 0.195f),
            new Pair<float, float>(1.00000f, 0.240f),
            new Pair<float, float>(1.00000f, 0.270f),
            new Pair<float, float>(1.00000f, 0.320f),
            new Pair<float, float>(1.00000f, 0.390f),
            new Pair<float, float>(1.00000f, 0.470f),
            new Pair<float, float>(1.00000f, 0.575f),
            new Pair<float, float>(1.00000f, 0.700f),
            new Pair<float, float>(1.00000f, 0.840f),
            new Pair<float, float>(1.00000f, 1.005f),
            new Pair<float, float>(1.00000f, 1.200f),
            new Pair<float, float>(1.00000f, 1.450f)
        };

        static List<Pair<float, float>> friendlyFactionWeightPercentiles = new List<Pair<float, float>>()
        {
            new Pair<float, float>(0.20979f, 0.040f),
            new Pair<float, float>(0.48951f, 0.045f),
            new Pair<float, float>(0.76923f, 0.055f),
            new Pair<float, float>(0.90909f, 0.075f),
            new Pair<float, float>(0.97902f, 0.090f),
            new Pair<float, float>(1.00000f, 0.105f),
            new Pair<float, float>(1.00000f, 0.130f),
            new Pair<float, float>(1.00000f, 0.160f),
            new Pair<float, float>(1.00000f, 0.195f),
            new Pair<float, float>(1.00000f, 0.240f),
            new Pair<float, float>(1.00000f, 0.270f),
            new Pair<float, float>(1.00000f, 0.320f),
            new Pair<float, float>(1.00000f, 0.390f),
            new Pair<float, float>(1.00000f, 0.470f),
            new Pair<float, float>(1.00000f, 0.575f),
            new Pair<float, float>(1.00000f, 0.700f),
            new Pair<float, float>(1.00000f, 0.840f),
            new Pair<float, float>(1.00000f, 1.005f),
            new Pair<float, float>(1.00000f, 1.200f),
            new Pair<float, float>(1.00000f, 1.450f)
        };

        static float GetRandomStartingWeight(List<Pair<float, float>> weightPercentiles)
        {
            float random = Values.RandomFloat(0, 1);

            for (int i = 0; i < weightPercentiles.Count; ++i)
            {
                Pair<float, float> currentPair = weightPercentiles[i];
                if (random <= currentPair.First)
                {
                    if (i > 0)
                    {
                        Pair<float, float> previousPair = weightPercentiles[i - 1];
                        return Values.RandomFloat(previousPair.Second, currentPair.Second);
                    }
                    else
                    {
                        Pair<float, float> nextPair = weightPercentiles[i + 1];
                        return Values.RandomFloat(currentPair.Second, nextPair.Second);
                    }
                }

            }

            return weightPercentiles.First().First;



        }


        static List<Pair<float, float>> GetWeightPercentileByFaction(Pawn p)
        {
            if (p?.Faction is null)
            {
                Log.Error($"Pawn or faction was null in weight percentile assignment!");
                return playerFactionWeightDistribution;
            }

            if (p.Faction.IsPlayer)
            {
                return playerFactionWeightDistribution;
            }
            else if (p.Faction.AllyOrNeutralTo(Find.FactionManager.OfPlayer))
            {
                return friendlyFactionWeightPercentiles;
            }
            else if (p.Faction.HostileTo(Find.FactionManager.OfPlayer))
            {
                return hostileFactionWeightDistribution;
            }
            else
            {
                Log.Warning($"Failed to get faction specific weight percentiles for {p.Name} of Faction {p.Faction}!");
                return playerFactionWeightDistribution;
            }

        }
    }
}
