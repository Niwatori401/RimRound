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

            foreach (var x in WeightOpinionUtility.severityToThoughtIndex)
            {
                if (weightSeverity <= x.Key)
                {
                    index = x.Value;
                    break;
                }
            }

            if (weightSeverity >= WeightOpinionUtility.severityToThoughtIndex.Last().Key)
                index = WeightOpinionUtility.severityToThoughtIndex.Last().Value;
            return index;
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

        public static Dictionary<float, int> severityToThoughtIndex = new Dictionary<float, int>()
        {
            { 0.000f,  0  },
            { 0.010f,  1  },
            { 0.020f,  2  },
            { 0.035f,  3  },
            { 0.050f,  4  },
            { 0.070f,  5  },
            { 0.095f,  6  },
            { 0.120f,  7  },
            { 0.150f,  8  },
            { 0.190f,  9  },
            { 0.235f,  10 },
            { 0.295f,  11 },
            { 0.360f,  12 },
            { 0.440f,  13 },
            { 0.530f,  14 },
            { 0.660f,  15 },
            { 0.800f,  16 },
            { 0.965f,  17 },
            { 1.160f,  18 },
            { 1.410f,  19 },
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
        Fanatical
    }

}
