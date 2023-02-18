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
    public static class WeightOpinionUtility
    {
        public static bool HasAnyWeightOpinionTrait(Pawn pawn)
        {
            foreach (var x in traitAndCommonalityPair)
            {
                if (pawn.story.traits.HasTrait(x.Key))
                {
                    return true;
                }
            }
            return false;
        }

        public static TraitDef GetWeightedRandWeightOpinionTrait(Dictionary<TraitDef, float> traitCommonalityPairs)
        {
            float sumOfKeys = 0;

            foreach (var x in traitCommonalityPairs)
            {
                sumOfKeys += x.Value;
            }

            float randomPortion = (float)Values.random.NextDouble() * sumOfKeys;

            foreach (var x in traitCommonalityPairs)
            {
                randomPortion -= x.Value;
                if (randomPortion <= 0)
                {
                    return x.Key;
                }
            }

            return null;
        }

        public static TraitDef AssignInitialWeightOpinionTraits(Pawn p)
        {
            if (HasAnyWeightOpinionTrait(p))
                return null;

            TraitDef t = GetWeightedRandWeightOpinionTrait(traitAndCommonalityPair);

            p.story.traits.GainTrait(new Trait(t, 0, true));

            return t;
        }

        public static WeightOpinion TraitDefToWeightOpinion(TraitDef t)
        {
            if (t is null)
                return WeightOpinion.None;

            if (t == RimRound.Defs.TraitDefOf.RR_WeightOpinion_Hate_Trait)
                return WeightOpinion.Hate;
            else if (t == RimRound.Defs.TraitDefOf.RR_WeightOpinion_Dislike_Trait)
                return WeightOpinion.Dislike;
            else if (t == RimRound.Defs.TraitDefOf.RR_WeightOpinion_NeutralMinus_Trait)
                return WeightOpinion.NeutralMinus;
            else if (t == RimRound.Defs.TraitDefOf.RR_WeightOpinion_Neutral_Trait)
                return WeightOpinion.Neutral;
            else if (t == RimRound.Defs.TraitDefOf.RR_WeightOpinion_NeutralPlus_Trait)
                return WeightOpinion.NeutralPlus;
            else if (t == RimRound.Defs.TraitDefOf.RR_WeightOpinion_Like_Trait)
                return WeightOpinion.Like;
            else if (t == RimRound.Defs.TraitDefOf.RR_WeightOpinion_Love_Trait)
                return WeightOpinion.Love;
            else if (t == RimRound.Defs.TraitDefOf.RR_WeightOpinion_Fanatical_Trait)
                return WeightOpinion.Fanatical;
            else
                return WeightOpinion.None;
        }

        public static TraitDef GetTraitByWeightOpinion(WeightOpinion w)
        {
            return weightOpinionToTraitDef[w];
        }

        public static void RemoveWeightOpinionTraits(Pawn p)
        {
            List<Trait> shitList = new List<Trait>();
            foreach (var x in p.story.traits.allTraits)
            {
                if (x.def.exclusionTags.Contains("RR_Trait_WeightOpinion"))
                    shitList.Add(x);
            }

            foreach (var y in shitList)
                p.story.traits.RemoveTrait(y);
        }

        public static bool ShouldHaveThisKindOfThought(ThoughtWorker thoughtWorker, Pawn p, WeightOpinion opinion)
        {
            if (!GlobalSettings.moodletsForWeightOpinions)
                return false;
            
            if (!p.RaceProps.Humanlike)
            {
                return false;
            }

            if ((p.TryGetComp<ThingComp_PawnAttitude>()?.weightOpinion ?? WeightOpinion.None) != opinion)
                return false;
            else
                thoughtWorker.def = WeightOpinionUtility.weightOpinionToThoughtDef[opinion];

            if (ThoughtUtility.ThoughtNullified(p, thoughtWorker.def))
                return false;

            float weightSeverity = Utilities.HediffUtility.GetHediffOfDefFrom(Defs.HediffDefOf.RimRound_Weight, p)?.Severity ?? -1;

            if (weightSeverity == -1)
                return false;

            return true;
        }

        public static int GetThoughtIndex(Pawn p)
        {
            int index = 0;

            float weightSeverity = Utilities.HediffUtility.GetHediffOfDefFrom(Defs.HediffDefOf.RimRound_Weight, p).Severity;

            float weightRequirementMultiplier = RacialBodyTypeInfoUtility.GetBodyTypeWeightRequirementMultiplier(p);

            foreach (var x in WeightOpinionUtility.severityToThoughtIndex)
            {
                if (weightSeverity <= x.Key * weightRequirementMultiplier)
                {
                    index = x.Value;
                    break;
                }
            }

            if (weightSeverity >= WeightOpinionUtility.severityToThoughtIndex.Last().Key * weightRequirementMultiplier)
                index = WeightOpinionUtility.severityToThoughtIndex.Last().Value;
            return index;
        }

        public static float GetRandomWeightFloatForOpinion(WeightOpinion weightOpinion) 
        {
            for (int i = 0; i < WeightOpinionUtility.reluctanceGap.Count(); ++i)
            {
                if (weightOpinion != WeightOpinionUtility.reluctanceGap[i].First)
                    continue;

                if (i == 0)
                    return Values.RandomFloat(0, WeightOpinionUtility.reluctanceGap[0].Second);

                return Values.RandomFloat(WeightOpinionUtility.reluctanceGap[i - 1].Second, WeightOpinionUtility.reluctanceGap[i].Second);
            }

            return -1;
        }


        public static float GetBonusWeightSeverityForWeightOpinion(WeightOpinion weightOpinion)
        {
            float amountOfWeightToAddSeverity = 0;
            switch (weightOpinion) 
            {
                case WeightOpinion.Hate:
                    amountOfWeightToAddSeverity = HediffUtility.KilosToSeverityWithoutBaseWeight(Values.RandomFloat(-20, -10));
                    break;
                case WeightOpinion.Dislike:
                    amountOfWeightToAddSeverity = HediffUtility.KilosToSeverityWithoutBaseWeight(Values.RandomFloat(-10, 0));
                    break;
                case WeightOpinion.NeutralMinus:
                    break;
                case WeightOpinion.Neutral:
                    amountOfWeightToAddSeverity = HediffUtility.KilosToSeverityWithoutBaseWeight(Values.RandomFloat(0, 10));
                    break;
                case WeightOpinion.NeutralPlus:
                    amountOfWeightToAddSeverity = HediffUtility.KilosToSeverityWithoutBaseWeight(Values.RandomFloat(0, 20));
                    break;
                case WeightOpinion.Like:
                    amountOfWeightToAddSeverity = HediffUtility.KilosToSeverityWithoutBaseWeight(Values.RandomFloat(0, 30));
                    break;
                case WeightOpinion.Love:
                    amountOfWeightToAddSeverity = HediffUtility.KilosToSeverityWithoutBaseWeight(Values.RandomFloat(20, 70));
                    break;
                case WeightOpinion.Fanatical:
                    amountOfWeightToAddSeverity = HediffUtility.KilosToSeverityWithoutBaseWeight(Values.RandomFloat(20, 100));
                    break;
                case WeightOpinion.Extreme:
                    amountOfWeightToAddSeverity = HediffUtility.KilosToSeverityWithoutBaseWeight(Values.RandomFloat(20, 100));
                    break;
                default:
                    Log.Error("Ran default case in GetBonusWeightForWeightOpinion()!");
                    break;
            }

            return amountOfWeightToAddSeverity;

        }


        public static Dictionary<WeightOpinion, TraitDef> weightOpinionToTraitDef = new Dictionary<WeightOpinion, TraitDef>()
        {
            {WeightOpinion.Hate ,         Defs.TraitDefOf.RR_WeightOpinion_Hate_Trait         },
            {WeightOpinion.Dislike ,      Defs.TraitDefOf.RR_WeightOpinion_Dislike_Trait      },
            {WeightOpinion.NeutralMinus , Defs.TraitDefOf.RR_WeightOpinion_NeutralMinus_Trait },
            {WeightOpinion.Neutral ,      Defs.TraitDefOf.RR_WeightOpinion_Neutral_Trait      },
            {WeightOpinion.NeutralPlus ,  Defs.TraitDefOf.RR_WeightOpinion_NeutralPlus_Trait  },
            {WeightOpinion.Like ,         Defs.TraitDefOf.RR_WeightOpinion_Like_Trait         },
            {WeightOpinion.Love ,         Defs.TraitDefOf.RR_WeightOpinion_Love_Trait         },
            {WeightOpinion.Fanatical ,    Defs.TraitDefOf.RR_WeightOpinion_Fanatical_Trait    }
        };

        public static Dictionary<WeightOpinion, ThoughtDef> weightOpinionToThoughtDef = new Dictionary<WeightOpinion, ThoughtDef>()
        {
            { WeightOpinion.None,         null                                            },
            { WeightOpinion.Hate,         Defs.ThoughtDefOf.RR_WeightOpinion_Hate         },
            { WeightOpinion.Dislike,      Defs.ThoughtDefOf.RR_WeightOpinion_Dislike      },
            { WeightOpinion.NeutralMinus, Defs.ThoughtDefOf.RR_WeightOpinion_NeutralMinus },
            { WeightOpinion.Neutral,      Defs.ThoughtDefOf.RR_WeightOpinion_Neutral      },
            { WeightOpinion.NeutralPlus,  Defs.ThoughtDefOf.RR_WeightOpinion_NeutralPlus  },
            { WeightOpinion.Like,         Defs.ThoughtDefOf.RR_WeightOpinion_Like         },
            { WeightOpinion.Love,         Defs.ThoughtDefOf.RR_WeightOpinion_Love         },
            { WeightOpinion.Fanatical,    Defs.ThoughtDefOf.RR_WeightOpinion_Fanatical    }

        };

        public static Dictionary<TraitDef, float> traitAndCommonalityPair = new Dictionary<TraitDef, float>()
        {
            { Defs.TraitDefOf.RR_WeightOpinion_Hate_Trait,         0.08f},
            { Defs.TraitDefOf.RR_WeightOpinion_Dislike_Trait,      0.17f},
            { Defs.TraitDefOf.RR_WeightOpinion_NeutralMinus_Trait, 0.28f},
            { Defs.TraitDefOf.RR_WeightOpinion_Neutral_Trait,      0.17f},
            { Defs.TraitDefOf.RR_WeightOpinion_NeutralPlus_Trait,  0.14f},
            { Defs.TraitDefOf.RR_WeightOpinion_Like_Trait,         0.08f},
            { Defs.TraitDefOf.RR_WeightOpinion_Love_Trait,         0.06f},
            { Defs.TraitDefOf.RR_WeightOpinion_Fanatical_Trait,    0.03f},
        };

        /// <summary>
        /// Key represents max severity for thought.
        /// </summary>
        public static Dictionary<float, int> severityToThoughtIndex = new Dictionary<float, int>()
        {
            { 0.010f,  0  },
            { 0.020f,  1  },
            { 0.035f,  2  },
            { 0.050f,  3  },
            { 0.070f,  4  },
            { 0.095f,  5  },
            { 0.120f,  6  },
            { 0.150f,  7  },
            { 0.190f,  8  },
            { 0.235f,  9  },
            { 0.295f,  10 },
            { 0.360f,  11 },
            { 0.440f,  12 },
            { 0.530f,  13 },
            { 0.660f,  14 },
            { 0.800f,  15 },
            { 0.965f,  16 },
            { 1.160f,  17 },
            { 1.410f,  18 },
            { 1.860f,  19 }, // Gel 1
            { 3.960f,  20 }, // Gel 2-4
            { 9.960f,  21 }, // Gel 5-8
            { 21.85f,  22 }, // Gel 9-10
            { 42.40f,  23 }, // Gel 11
            { 116.6f,  24 }, // Gel 12-13
            { 411.0f,  25 }, // Gel 14-16
            { 999.999f,  26 }, // Gel 17-19
            { 9999999f,  27 }, // Gel 20
        };


        // Second value is the max amount of points for that opinion.
        public static List<Pair<WeightOpinion, float>> reluctanceGap = new List<Pair<WeightOpinion, float>>()
            {
                new Pair<WeightOpinion, float>(WeightOpinion.Hate, 250),
                new Pair<WeightOpinion, float>(WeightOpinion.Dislike, 350),
                new Pair<WeightOpinion, float>(WeightOpinion.NeutralMinus, 400),
                new Pair<WeightOpinion, float>(WeightOpinion.Neutral, 450),
                new Pair<WeightOpinion, float>(WeightOpinion.NeutralPlus, 500),
                new Pair<WeightOpinion, float>(WeightOpinion.Like, 700),
                new Pair<WeightOpinion, float>(WeightOpinion.Love, 1000),
                new Pair<WeightOpinion, float>(WeightOpinion.Fanatical, 1500),
                new Pair<WeightOpinion, float>(WeightOpinion.Extreme, float.MaxValue),
            };

        public static Dictionary<WeightOpinion, float> weightOpinionToGainResistance = new Dictionary<WeightOpinion, float>()
            {
                { WeightOpinion.Hate, 0.2f},
                { WeightOpinion.Dislike, 0.4f},
                { WeightOpinion.NeutralMinus, 0.6f},
                { WeightOpinion.Neutral, 0.7f},
                { WeightOpinion.NeutralPlus, 0.8f},
                { WeightOpinion.Like, 1.0f},
                { WeightOpinion.Love, 1.5f},
                { WeightOpinion.Fanatical, 2.0f},
                { WeightOpinion.Extreme, 3.0f},
            };

        public static Dictionary<WeightOpinion, float> weightOpinionToLossResistance = new Dictionary<WeightOpinion, float>()
            {
                { WeightOpinion.Hate, 1.5f},
                { WeightOpinion.Dislike, 1.0f},
                { WeightOpinion.NeutralMinus, 0.8f},
                { WeightOpinion.Neutral, 0.7f},
                { WeightOpinion.NeutralPlus, 0.6f},
                { WeightOpinion.Like, 0.5f},
                { WeightOpinion.Love, 0.4f},
                { WeightOpinion.Fanatical, 0.4f},
                { WeightOpinion.Extreme, 0.0f},
            };

    }

    public enum WeightOpinion : byte
    {
        None,
        Hate,
        Dislike,
        NeutralMinus,
        Neutral,
        NeutralPlus,
        Like,
        Love,
        Fanatical,
        Extreme
    }

}
