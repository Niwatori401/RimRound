using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using Verse;
using UnityEngine;
using RimRound.Enums;

namespace RimRound.Utilities
{
    public static class Values
    {
        public static float debugFloat = 0;

        public const float defaultSoftLimit = 0.9f;
        public static Vector2 softLimitVariation = new Vector2(-0.10f, 0.50f);
        public const float defaultHardLimitAdditionalPercentage = 0.3f;
        public const float defaultPersonalStomachElasticity = 1f;
        //How much (in liters) the stomach grows when over the softlimit per tick.
        public const float baseStomachElasticity = 0.00001f;
        public const float baseDigestionRate = 3.0f;
        public const float defaultFullnessToNutritionRatio = 1f; //i.e. 0.5 Fullness for 1 nutrition is 0.5f

        //Note: you must add "_{bodyType.defName}" to this to get the shirt graphic or it will not work.
        public static string defaultClothingSetGraphicPath = "Things/Pawn/Humanlike/Apparel/ShirtBasic/ShirtBasic";

        public static float severityPerKilo = 0.001f;
        public static float nutritionPerKilo = 1.0f;

        public static Dictionary<BodyTypeDef, float> bodyTypeWiggleSpeed = new Dictionary<BodyTypeDef, float>()
        {
            {RimWorld.BodyTypeDefOf.Fat, 1.0f},
            {RimWorld.BodyTypeDefOf.Female, 1.0f},
            {RimWorld.BodyTypeDefOf.Hulk, 1.0f},
            {RimWorld.BodyTypeDefOf.Male, 1.0f},
            {RimWorld.BodyTypeDefOf.Thin, 1.0f},

            //{RimRound.Defs.BodyTypeDefOf.F_004_BaseThin, 1f },
            //{RimRound.Defs.BodyTypeDefOf.F_007_BaseFemale, 1f },
            {RimRound.Defs.BodyTypeDefOf.M_005_Thick,                1.0f },
            {RimRound.Defs.BodyTypeDefOf.F_005_Thick,                1.0f },
            {RimRound.Defs.BodyTypeDefOf.F_005_Thick_Ratkin,         1.0f },
            {RimRound.Defs.BodyTypeDefOf.F_006_Chonky,               0.9f },
            {RimRound.Defs.BodyTypeDefOf.F_006_Chonky_Ratkin,        0.9f },
            {RimRound.Defs.BodyTypeDefOf.F_010_Chubby,               0.8f },
            {RimRound.Defs.BodyTypeDefOf.F_010_Chubby_Ratkin,        0.8f },
            {RimRound.Defs.BodyTypeDefOf.F_020_Corpulent,           0.65f },
            {RimRound.Defs.BodyTypeDefOf.F_020_Corpulent_Ratkin,    0.65f },
            {RimRound.Defs.BodyTypeDefOf.F_030_Fat,                 0.50f },
            {RimRound.Defs.BodyTypeDefOf.F_030_Fat_Ratkin,          0.50f },
            {RimRound.Defs.BodyTypeDefOf.F_040_Obese,               0.40f },
            {RimRound.Defs.BodyTypeDefOf.F_040_Obese_Ratkin,        0.40f },
            {RimRound.Defs.BodyTypeDefOf.F_050_MorbidlyObese,       0.30f },
            {RimRound.Defs.BodyTypeDefOf.F_050_MorbidlyObese_Ratkin,0.30f },
            {RimRound.Defs.BodyTypeDefOf.F_060_Lardy,               0.20f },
            {RimRound.Defs.BodyTypeDefOf.F_060_Lardy_Ratkin,        0.20f },
            {RimRound.Defs.BodyTypeDefOf.F_070_Enormous,            0.10f },
            {RimRound.Defs.BodyTypeDefOf.F_070_Enormous_Ratkin,     0.10f },
            {RimRound.Defs.BodyTypeDefOf.F_080_Gigantic,            0.10f },
            {RimRound.Defs.BodyTypeDefOf.F_080_Gigantic_Ratkin,     0.10f },
            {RimRound.Defs.BodyTypeDefOf.F_090_Titanic,             0.10f },
            {RimRound.Defs.BodyTypeDefOf.F_090_Titanic_Ratkin,     0.10f },
            {RimRound.Defs.BodyTypeDefOf.F_100_Gelatinous,          0.05f },
            {RimRound.Defs.BodyTypeDefOf.F_100_Gelatinous_Ratkin,   0.05f }
        };

        public static Dictionary<BodyTypeDef, float> bodyTypeDrawSizes = new Dictionary<BodyTypeDef, float>()
        {
            {RimWorld.BodyTypeDefOf.Fat, 1.0f},
            {RimWorld.BodyTypeDefOf.Female, 1.0f},
            {RimWorld.BodyTypeDefOf.Hulk, 1.0f},
            {RimWorld.BodyTypeDefOf.Male, 1.0f},
            {RimWorld.BodyTypeDefOf.Thin, 1.0f},

            //{RimRound.Defs.BodyTypeDefOf.F_004_BaseThin,      4.6f },
            //{RimRound.Defs.BodyTypeDefOf.F_007_BaseFemale,    4.6f },
            {RimRound.Defs.BodyTypeDefOf.M_005_Thick,        0.75f },


            {RimRound.Defs.BodyTypeDefOf.F_005_Thick,         0.8750f },
            {RimRound.Defs.BodyTypeDefOf.F_006_Chonky,        1.1250f },
            {RimRound.Defs.BodyTypeDefOf.F_010_Chubby,        1.2500f },
            {RimRound.Defs.BodyTypeDefOf.F_020_Corpulent,     1.3750f },
            {RimRound.Defs.BodyTypeDefOf.F_030_Fat,           1.3750f },
            {RimRound.Defs.BodyTypeDefOf.F_040_Obese,         1.3750f },
            {RimRound.Defs.BodyTypeDefOf.F_050_MorbidlyObese, 1.3750f },
            {RimRound.Defs.BodyTypeDefOf.F_060_Lardy,         2.7500f },
            {RimRound.Defs.BodyTypeDefOf.F_070_Enormous,      2.5000f },
            {RimRound.Defs.BodyTypeDefOf.F_080_Gigantic,      5.5000f },
            {RimRound.Defs.BodyTypeDefOf.F_090_Titanic,       5.2500f },
            {RimRound.Defs.BodyTypeDefOf.F_100_Gelatinous,    7.5000f },

            //Ratkin
            
            //{RimRound.Defs.BodyTypeDefOf.RatkinBase,                 1.0f },
            {RimRound.Defs.BodyTypeDefOf.F_005_Thick_Ratkin,         0.5f },
            {RimRound.Defs.BodyTypeDefOf.F_006_Chonky_Ratkin,        1.0f },
            {RimRound.Defs.BodyTypeDefOf.F_010_Chubby_Ratkin,        1.0f },
            {RimRound.Defs.BodyTypeDefOf.F_020_Corpulent_Ratkin,     1.0f },
            {RimRound.Defs.BodyTypeDefOf.F_030_Fat_Ratkin,           1.0f },
            {RimRound.Defs.BodyTypeDefOf.F_040_Obese_Ratkin,         1.0f },
            {RimRound.Defs.BodyTypeDefOf.F_050_MorbidlyObese_Ratkin, 1.0f },
            {RimRound.Defs.BodyTypeDefOf.F_060_Lardy_Ratkin,         1.0f },
            {RimRound.Defs.BodyTypeDefOf.F_070_Enormous_Ratkin,      1.0f },
            {RimRound.Defs.BodyTypeDefOf.F_080_Gigantic_Ratkin,      1.0f },
            {RimRound.Defs.BodyTypeDefOf.F_090_Titanic_Ratkin,       1.0f },
            {RimRound.Defs.BodyTypeDefOf.F_100_Gelatinous_Ratkin,    1.0f }
        };

        //The float value represents the max severity value for that body type.
        public static Dictionary<BodyTypeDef, float> defaultBodyTypeSeverityPair = new Dictionary<BodyTypeDef, float>()
        {
            {RimWorld.BodyTypeDefOf.Fat, -1f},
            //{RimWorld.BodyTypeDefOf.Female, -1f},
            {RimWorld.BodyTypeDefOf.Hulk, -1f},
            {RimWorld.BodyTypeDefOf.Male, -1f},
            //{RimWorld.BodyTypeDefOf.Thin, -1f},

            {RimWorld.BodyTypeDefOf.Thin,                     0.020f },
            {RimWorld.BodyTypeDefOf.Female,                   0.035f },
            {RimRound.Defs.BodyTypeDefOf.F_005_Thick,         0.040f },
            {RimRound.Defs.BodyTypeDefOf.F_006_Chonky,        0.050f },
            {RimRound.Defs.BodyTypeDefOf.F_010_Chubby,        0.095f },
            {RimRound.Defs.BodyTypeDefOf.F_020_Corpulent,     0.120f },
            {RimRound.Defs.BodyTypeDefOf.F_030_Fat,           0.150f },
            {RimRound.Defs.BodyTypeDefOf.F_040_Obese,         0.190f },
            {RimRound.Defs.BodyTypeDefOf.F_050_MorbidlyObese, 0.295f },
            {RimRound.Defs.BodyTypeDefOf.F_060_Lardy,         0.440f },
            {RimRound.Defs.BodyTypeDefOf.F_070_Enormous,      0.660f },
            {RimRound.Defs.BodyTypeDefOf.F_080_Gigantic,      0.965f },
            {RimRound.Defs.BodyTypeDefOf.F_090_Titanic,       1.410f },
            {RimRound.Defs.BodyTypeDefOf.F_100_Gelatinous,    100f }
        };
        
        public static Dictionary<BodyTypeDef, float> ratkinBodyTypeSeverityPair = new Dictionary<BodyTypeDef, float>()
        {
            {RimWorld.BodyTypeDefOf.Thin, 0.020f},

            //{RimRound.Defs.BodyTypeDefOf.RatkinBase,                 0.020f },
            //{RimRound.Defs.BodyTypeDefOf.F_007_BaseFemale_Ratkin,    0.030f },
            {RimRound.Defs.BodyTypeDefOf.F_005_Thick_Ratkin,         0.040f },
            {RimRound.Defs.BodyTypeDefOf.F_006_Chonky_Ratkin,        0.050f },
            {RimRound.Defs.BodyTypeDefOf.F_010_Chubby_Ratkin,        0.095f },
            {RimRound.Defs.BodyTypeDefOf.F_020_Corpulent_Ratkin,     0.120f },
            {RimRound.Defs.BodyTypeDefOf.F_030_Fat_Ratkin,           0.150f },
            {RimRound.Defs.BodyTypeDefOf.F_040_Obese_Ratkin,         0.190f },
            {RimRound.Defs.BodyTypeDefOf.F_050_MorbidlyObese_Ratkin, 0.295f },
            {RimRound.Defs.BodyTypeDefOf.F_060_Lardy_Ratkin,         0.440f },
            {RimRound.Defs.BodyTypeDefOf.F_070_Enormous_Ratkin,      0.660f },
            {RimRound.Defs.BodyTypeDefOf.F_080_Gigantic_Ratkin,      0.965f },
            {RimRound.Defs.BodyTypeDefOf.F_090_Titanic_Ratkin,       1.410f },
            {RimRound.Defs.BodyTypeDefOf.F_100_Gelatinous_Ratkin,    100f }
        };

        public static Dictionary<string, Dictionary<BodyTypeDef, float>> raceToBodytypeDictionary = new Dictionary<string, Dictionary<BodyTypeDef, float>>() 
        {
            { "Human",           defaultBodyTypeSeverityPair },
            { "Ratkin",          ratkinBodyTypeSeverityPair  },
            { "Alien_Drow_Otto", defaultBodyTypeSeverityPair }
        };
        


        public static float[] validBodyMeshSizes = new float[] 
        {
            0.5f,
            0.5625f,
            0.625f,
            0.6875f,
            0.75f,
            0.8125f,
            0.875f,
            0.9375f,
            1.0f,
            1.125f,
            1.25f,
            1.375f,
            1.5f,
            1.625f,
            1.75f,
            1.875f,
            2.0f,
            2.125f,
            2.25f,
            2.375f,
            2.5f,
            2.625f,
            2.75f,
            2.875f,
            3.0f,
            3.125f,
            3.25f,
            3.375f,
            3.5f,
            3.625f,
            3.75f,
            3.875f,
            4.0f,
            4.25f,
            4.50f,
            4.75f,
            5.00f,
            5.25f,
            5.50f,
            5.75f,
            6.00f,
            6.25f,
            6.50f,
            6.75f,
            7.00f,
            7.25f,
            7.50f,
            7.75f,
            8.00f
        };


        //In ltg order
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

        public static List<BodyTypeDef> rimRoundBodyTypeDefs = new List<BodyTypeDef>() 
        {
            //RimRound.Defs.BodyTypeDefOf.F_004_BaseThin,
            RimRound.Defs.BodyTypeDefOf.M_005_Thick,
            RimRound.Defs.BodyTypeDefOf.F_005_Thick,
            RimRound.Defs.BodyTypeDefOf.F_006_Chonky,
            //RimRound.Defs.BodyTypeDefOf.F_007_BaseFemale,
            RimRound.Defs.BodyTypeDefOf.F_010_Chubby,
            RimRound.Defs.BodyTypeDefOf.F_020_Corpulent,
            RimRound.Defs.BodyTypeDefOf.F_030_Fat,
            RimRound.Defs.BodyTypeDefOf.F_040_Obese,
            RimRound.Defs.BodyTypeDefOf.F_050_MorbidlyObese,
            RimRound.Defs.BodyTypeDefOf.F_060_Lardy,
            RimRound.Defs.BodyTypeDefOf.F_070_Enormous,
            RimRound.Defs.BodyTypeDefOf.F_080_Gigantic,
            RimRound.Defs.BodyTypeDefOf.F_090_Titanic,
            RimRound.Defs.BodyTypeDefOf.F_100_Gelatinous,

            //Ratkin
            RimRound.Defs.BodyTypeDefOf.F_005_Thick_Ratkin,
            RimRound.Defs.BodyTypeDefOf.F_006_Chonky_Ratkin,
            RimRound.Defs.BodyTypeDefOf.F_010_Chubby_Ratkin,
            RimRound.Defs.BodyTypeDefOf.F_020_Corpulent_Ratkin,
            RimRound.Defs.BodyTypeDefOf.F_030_Fat_Ratkin,
            RimRound.Defs.BodyTypeDefOf.F_040_Obese_Ratkin,
            RimRound.Defs.BodyTypeDefOf.F_050_MorbidlyObese_Ratkin,
            RimRound.Defs.BodyTypeDefOf.F_060_Lardy_Ratkin,
            RimRound.Defs.BodyTypeDefOf.F_070_Enormous_Ratkin,
            RimRound.Defs.BodyTypeDefOf.F_080_Gigantic_Ratkin,
            RimRound.Defs.BodyTypeDefOf.F_090_Titanic_Ratkin,
            RimRound.Defs.BodyTypeDefOf.F_100_Gelatinous_Ratkin
        };

        public static int RandomInt(int lower, int upper) 
        {
            return random.Next(lower, upper + 1);
        }

        public static float RandomFloat(float lower, float upper) 
        {
            return (float)random.NextDouble() * (upper - lower) + lower;
        }

        public const float MinRQ = 0.000001f;

        public static System.Random random = new System.Random();
    }
}


