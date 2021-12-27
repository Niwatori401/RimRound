using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Utilities
{
    public static class RacialBodyTypeInfoUtility
    {
        public static Dictionary<BodyTypeDef, BodyTypeInfo> GetRacialDictionary(Pawn pawn) 
        {
            if (pawn.def is AlienRace.ThingDef_AlienRace race)
            {
                if (raceToProperDictDictionary.ContainsKey(race.defName)) 
                {
                    return raceToProperDictDictionary[race.defName];
                }
            }
            return null;
        }

        public static BodyTypeInfo? GetRacialBodyTypeInfo(Pawn pawn)
        {
            if (GetRacialDictionary(pawn) is Dictionary<BodyTypeDef, BodyTypeInfo> dictionary) 
            {
                if (dictionary.ContainsKey(pawn.story.bodyType))
                {
                    return dictionary[pawn.story.bodyType];
                }
            }
            return null;
        }

        public static BodyTypeDef GetEquivalentBodyTypeDef(BodyTypeDef raceSpecificDef)
        {
            if (standardBodyTypeDefs.Contains(raceSpecificDef))
                return raceSpecificDef;

            int endPos = raceSpecificDef.defName.LastIndexOf('_');

            if (endPos == -1)
                return raceSpecificDef;

            string cleanedDefName = raceSpecificDef.defName.Substring(0, endPos);

            foreach (BodyTypeDef b in standardBodyTypeDefs)
            {
                if (b.defName == cleanedDefName)
                    return b;
            }

            Log.Error("Could not get equivalent BodyTypeDef! Make sure the body type is well formatted and has an equivalent entry in standardBodyTypeDefs");

            return raceSpecificDef;
        }

        static Dictionary<BodyTypeDef, BodyTypeInfo> defaultSet = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                      new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                     new BodyTypeInfo(0.015f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                   new BodyTypeInfo(0.035f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_005_Thick,         new BodyTypeInfo(0.050f, 0.8750f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_006_Chonky,        new BodyTypeInfo(0.065f, 1.1250f, 0.90f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_010_Chubby,        new BodyTypeInfo(0.090f, 1.2500f, 0.80f, 1.28205f, 0.30f, 0.35f) },
                { RimRound.Defs.BodyTypeDefOf.F_020_Corpulent,     new BodyTypeInfo(0.120f, 1.3750f, 0.65f, 0.98205f, 0.40f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_030_Fat,           new BodyTypeInfo(0.155f, 1.3750f, 0.50f, 0.88205f, 0.44f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_040_Obese,         new BodyTypeInfo(0.200f, 1.3750f, 0.40f, 0.77205f, 0.50f, 0.40f) },
                { RimRound.Defs.BodyTypeDefOf.F_050_MorbidlyObese, new BodyTypeInfo(0.280f, 1.3750f, 0.30f, 0.65205f, 0.70f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_060_Lardy,         new BodyTypeInfo(0.430f, 2.7500f, 0.20f, 0.58205f, 1.00f, 0.70f) },
                { RimRound.Defs.BodyTypeDefOf.F_070_Enormous,      new BodyTypeInfo(0.660f, 2.5000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_080_Gigantic,      new BodyTypeInfo(0.965f, 5.5000f, 0.10f, 0.38205f, 1.10f, 0.65f) },
                { RimRound.Defs.BodyTypeDefOf.F_090_Titanic,       new BodyTypeInfo(1.410f, 5.2500f, 0.10f, 0.37205f, 1.10f, 0.79f) },
                { RimRound.Defs.BodyTypeDefOf.F_100_Gelatinous,    new BodyTypeInfo(100f  , 7.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) }
            };

        static Dictionary<BodyTypeDef, BodyTypeInfo> ratkinSet = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                             new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                            new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                            new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                          new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                            new BodyTypeInfo(0.015f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_005_Thick_Ratkin,         new BodyTypeInfo(0.050f, 0.6875f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_006_Chonky_Ratkin,        new BodyTypeInfo(0.065f, 0.9375f, 0.90f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_010_Chubby_Ratkin,        new BodyTypeInfo(0.090f, 0.9375f, 0.80f, 1.28205f, 0.30f, 0.35f) },
                { RimRound.Defs.BodyTypeDefOf.F_020_Corpulent_Ratkin,     new BodyTypeInfo(0.120f, 0.9375f, 0.65f, 0.98205f, 0.40f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_030_Fat_Ratkin,           new BodyTypeInfo(0.155f, 1.000f, 0.50f, 0.88205f, 0.44f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_040_Obese_Ratkin,         new BodyTypeInfo(0.200f, 1.125f, 0.40f, 0.77205f, 0.50f, 0.40f) },
                { RimRound.Defs.BodyTypeDefOf.F_050_MorbidlyObese_Ratkin, new BodyTypeInfo(0.280f, 1.125f, 0.30f, 0.65205f, 0.70f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_060_Lardy_Ratkin,         new BodyTypeInfo(0.430f, 2.125f, 0.20f, 0.58205f, 1.00f, 0.70f) },
                { RimRound.Defs.BodyTypeDefOf.F_070_Enormous_Ratkin,      new BodyTypeInfo(0.660f, 2.0f, 0.10f, 0.48205f, 0.93f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_080_Gigantic_Ratkin,      new BodyTypeInfo(0.965f, 3.875f, 0.10f, 0.38205f, 1.10f, 0.65f) },
                { RimRound.Defs.BodyTypeDefOf.F_090_Titanic_Ratkin,       new BodyTypeInfo(1.410f, 3.875f, 0.10f, 0.37205f, 1.10f, 0.79f) },
                { RimRound.Defs.BodyTypeDefOf.F_100_Gelatinous_Ratkin,    new BodyTypeInfo(100f  , 5.000f, 0.05f, 0.22205f, 2.52f, 0.00f) }
            };

        static Dictionary<string, Dictionary<BodyTypeDef, BodyTypeInfo>> raceToProperDictDictionary = new Dictionary<string, Dictionary<BodyTypeDef, BodyTypeInfo>>()
        {
            { "Alien_Drow_Otto",    defaultSet }, //100%
            { "Anthro",             defaultSet }, //100%
            { "ESCP_AltmerRace",    defaultSet },
            { "ESCP_AshlanderRace", defaultSet },
            { "ESCP_DunmerRace",    defaultSet },
            { "Human",              defaultSet }, //100%
            { "Ratkin",             ratkinSet  }  //Partial

        };

        public static List<BodyTypeDef> standardBodyTypeDefs = new List<BodyTypeDef>() 
        {
            RimRound.Defs.BodyTypeDefOf.F_005_Thick,
            RimRound.Defs.BodyTypeDefOf.F_006_Chonky,
            RimRound.Defs.BodyTypeDefOf.F_010_Chubby,
            RimRound.Defs.BodyTypeDefOf.F_020_Corpulent,
            RimRound.Defs.BodyTypeDefOf.F_030_Fat,
            RimRound.Defs.BodyTypeDefOf.F_040_Obese,
            RimRound.Defs.BodyTypeDefOf.F_050_MorbidlyObese,
            RimRound.Defs.BodyTypeDefOf.F_060_Lardy,
            RimRound.Defs.BodyTypeDefOf.F_070_Enormous,
            RimRound.Defs.BodyTypeDefOf.F_080_Gigantic,
            RimRound.Defs.BodyTypeDefOf.F_090_Titanic,
            RimRound.Defs.BodyTypeDefOf.F_100_Gelatinous
        };


    }


    public struct BodyTypeInfo 
    {
        public BodyTypeInfo(float maxSeverity, float meshSize, float wiggleSpeed, float portraitZoom, float portraitOffsetZoomMethod, float portraitOffsetPanMethod) 
        {
            this.maxSeverity = maxSeverity;
            this.meshSize = meshSize;
            this.wiggleSpeed = wiggleSpeed;
            this.portraitZoom = portraitZoom;
            this.portraitOffsetZoomMethod = portraitOffsetZoomMethod;
            this.portraitOffsetPanMethod = portraitOffsetPanMethod;
        }

        public readonly float maxSeverity;
        public readonly float meshSize;
        public readonly float wiggleSpeed;
        public readonly float portraitZoom;
        public readonly float portraitOffsetZoomMethod;
        public readonly float portraitOffsetPanMethod;
    }
}
