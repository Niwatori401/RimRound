using RimRound.Comps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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
                    if (raceToProperDictDictionary[race.defName].ContainsKey(pawn.gender))
                    {
                        PawnBodyType_ThingComp pbtComp = pawn.TryGetComp<PawnBodyType_ThingComp>();

                        if (pbtComp != null && raceToProperDictDictionary[race.defName][pawn.gender].ContainsKey(pbtComp.BodyArchetype))
                        {
                            return raceToProperDictDictionary[race.defName][pawn.gender][pbtComp.BodyArchetype];
                        }
                    }
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


        public static float GetBodyTypeWeightRequirementMultiplier(Pawn p)
        {
            if (p is null || !p.RaceProps.Humanlike)
                return 1;

            if (!GlobalSettings.varyMinWeightForBodyTypeByBodySize)
                return 1;

            return GetBodyTypeWeightRequirementMultiplierByDefName(p.story.bodyType.defName);
        }

        public static float GetBodyTypeWeightRequirementMultiplierByDefName(string defName)
        {
            foreach (var bodyTypeDef in RacialBodyTypeInfoUtility.standardBodyTypeDefs)
                if (bodyTypeDef.defName == defName)
                    return 1;


            int endPos = defName.LastIndexOf('_');

            if (endPos == -1)
                return 1;

            string cleanedDefName = defName.Substring(endPos + 1);

            //Log.Message($"cleaned bodytype! {cleanedDefName}");

            switch (cleanedDefName)
            {
                case "090":
                    return 0.6f;
                case "070":
                    return 0.35f;
                case "Ratkin":
                    return 0.6f;
                case "Anty":
                    return 0.6f;
                default:
                    Log.Warning("Ran defualt case in GetBodyTypeWeightRequirementMultiplier!");
                    break;
            }

            return 0;
        }


        public static Dictionary<BodyTypeDef, BodyTypeInfo> defaultFemaleSet = new Dictionary<BodyTypeDef, BodyTypeInfo>()
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

        public static Dictionary<BodyTypeDef, BodyTypeInfo> appleFemaleSet = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                      new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                      new BodyTypeInfo(0.015f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                    new BodyTypeInfo(0.035f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_005_Thick,          new BodyTypeInfo(0.050f, 0.8750f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_006_Chonky,         new BodyTypeInfo(0.065f, 1.1250f, 0.90f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_010a_Chubby,        new BodyTypeInfo(0.090f, 1.2500f, 0.80f, 1.28205f, 0.30f, 0.35f) },
                { RimRound.Defs.BodyTypeDefOf.F_020a_Corpulent,     new BodyTypeInfo(0.120f, 1.3750f, 0.65f, 0.98205f, 0.40f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_030a_Fat,           new BodyTypeInfo(0.155f, 1.3750f, 0.50f, 0.88205f, 0.44f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_040a_Obese,         new BodyTypeInfo(0.200f, 1.3750f, 0.40f, 0.77205f, 0.50f, 0.40f) },
                { RimRound.Defs.BodyTypeDefOf.F_050a_MorbidlyObese, new BodyTypeInfo(0.280f, 1.3750f, 0.30f, 0.65205f, 0.70f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_060a_Lardy,         new BodyTypeInfo(0.430f, 2.7500f, 0.20f, 0.58205f, 1.00f, 0.70f) },
                { RimRound.Defs.BodyTypeDefOf.F_070a_Enormous,      new BodyTypeInfo(0.660f, 2.5000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_080a_Gigantic,      new BodyTypeInfo(0.965f, 5.5000f, 0.10f, 0.38205f, 1.10f, 0.65f) },
                { RimRound.Defs.BodyTypeDefOf.F_090a_Titanic,       new BodyTypeInfo(1.410f, 5.2500f, 0.10f, 0.37205f, 1.10f, 0.79f) },
                { RimRound.Defs.BodyTypeDefOf.F_100a_Gelatinous,     new BodyTypeInfo(100f  , 7.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) }
            };

        public static Dictionary<BodyTypeDef, BodyTypeInfo> defaultMaleSet = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                      new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                   new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                     new BodyTypeInfo(0.015f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                     new BodyTypeInfo(0.035f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.M_005_Thick,         new BodyTypeInfo(0.050f, 0.8750f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.M_006_Chonky,        new BodyTypeInfo(0.065f, 1.1250f, 0.90f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.M_010_Chubby,        new BodyTypeInfo(0.090f, 1.2500f, 0.80f, 1.28205f, 0.30f, 0.35f) },
                { RimRound.Defs.BodyTypeDefOf.M_020_Corpulent,     new BodyTypeInfo(0.120f, 1.3750f, 0.65f, 0.98205f, 0.40f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.M_030_Fat,           new BodyTypeInfo(0.155f, 1.3750f, 0.50f, 0.88205f, 0.44f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.M_040_Obese,         new BodyTypeInfo(0.200f, 1.3750f, 0.40f, 0.77205f, 0.50f, 0.40f) },
                { RimRound.Defs.BodyTypeDefOf.M_050_MorbidlyObese, new BodyTypeInfo(0.280f, 1.3750f, 0.30f, 0.65205f, 0.70f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.M_060_Lardy,         new BodyTypeInfo(0.430f, 2.7500f, 0.20f, 0.58205f, 1.00f, 0.70f) },
                { RimRound.Defs.BodyTypeDefOf.M_070_Enormous,      new BodyTypeInfo(0.660f, 2.5000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.M_080_Gigantic,      new BodyTypeInfo(0.965f, 5.5000f, 0.10f, 0.38205f, 1.10f, 0.65f) },
                { RimRound.Defs.BodyTypeDefOf.M_090_Titanic,       new BodyTypeInfo(1.410f, 5.2500f, 0.10f, 0.37205f, 1.10f, 0.79f) },
                { RimRound.Defs.BodyTypeDefOf.M_100_Gelatinous,    new BodyTypeInfo(100f  , 7.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) }
            };


        public static Dictionary<BodyTypeDef, BodyTypeInfo> set090Female = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                             new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                            new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                            new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                          new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                            new BodyTypeInfo(0.015f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_005_Thick_090,            new BodyTypeInfo(0.050f, 0.6875f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_006_Chonky_090,           new BodyTypeInfo(0.065f, 0.9375f, 0.90f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_010_Chubby_090,           new BodyTypeInfo(0.090f, 0.9375f, 0.80f, 1.28205f, 0.30f, 0.35f) },
                { RimRound.Defs.BodyTypeDefOf.F_020_Corpulent_090,        new BodyTypeInfo(0.120f, 0.9375f, 0.65f, 0.98205f, 0.40f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_030_Fat_090,              new BodyTypeInfo(0.155f, 1.000f, 0.50f, 0.88205f, 0.44f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_040_Obese_090,            new BodyTypeInfo(0.200f, 1.125f, 0.40f, 0.77205f, 0.50f, 0.40f) },
                { RimRound.Defs.BodyTypeDefOf.F_050_MorbidlyObese_090,    new BodyTypeInfo(0.280f, 1.125f, 0.30f, 0.65205f, 0.70f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_060_Lardy_090,            new BodyTypeInfo(0.430f, 2.125f, 0.20f, 0.58205f, 1.00f, 0.70f) },
                { RimRound.Defs.BodyTypeDefOf.F_070_Enormous_090,         new BodyTypeInfo(0.660f, 2.000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_080_Gigantic_090,         new BodyTypeInfo(0.965f, 3.875f, 0.10f, 0.38205f, 1.10f, 0.65f) },
                { RimRound.Defs.BodyTypeDefOf.F_090_Titanic_090,          new BodyTypeInfo(1.410f, 3.875f, 0.10f, 0.37205f, 1.10f, 0.79f) },
                { RimRound.Defs.BodyTypeDefOf.F_100_Gelatinous_090,       new BodyTypeInfo(100f  , 5.000f, 0.05f, 0.22205f, 2.52f, 0.00f) }
            };

        public static Dictionary<BodyTypeDef, BodyTypeInfo> set070Female = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                             new BodyTypeInfo(-1    , 0.7000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                            new BodyTypeInfo(-1    , 0.7000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                            new BodyTypeInfo(-1    , 0.7000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                            new BodyTypeInfo(0.015f, 0.7000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                          new BodyTypeInfo(0.035f, 0.7000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimRound.Defs.BodyTypeDefOf.F_005_Thick_070,                new BodyTypeInfo(0.050f, 0.5625f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_006_Chonky_070,               new BodyTypeInfo(0.065f, 0.7500f, 0.90f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_010_Chubby_070,               new BodyTypeInfo(0.090f, 0.7500f, 0.80f, 1.28205f, 0.30f, 0.35f) },
                { RimRound.Defs.BodyTypeDefOf.F_020_Corpulent_070,            new BodyTypeInfo(0.120f, 0.7500f, 0.65f, 0.98205f, 0.40f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_030_Fat_070,                  new BodyTypeInfo(0.155f, 0.7500f, 0.50f, 0.88205f, 0.44f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_040_Obese_070,                new BodyTypeInfo(0.200f, 0.7500f, 0.40f, 0.77205f, 0.50f, 0.40f) },
                { RimRound.Defs.BodyTypeDefOf.F_050_MorbidlyObese_070,        new BodyTypeInfo(0.280f, 0.7500f, 0.30f, 0.65205f, 0.70f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_060_Lardy_070,                new BodyTypeInfo(0.430f, 1.6250f, 0.20f, 0.58205f, 1.00f, 0.70f) },
                { RimRound.Defs.BodyTypeDefOf.F_070_Enormous_070,             new BodyTypeInfo(0.660f, 1.5000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_080_Gigantic_070,             new BodyTypeInfo(0.965f, 2.8750f, 0.10f, 0.38205f, 1.10f, 0.65f) },
                { RimRound.Defs.BodyTypeDefOf.F_090_Titanic_070,              new BodyTypeInfo(1.410f, 2.8750f, 0.10f, 0.37205f, 1.10f, 0.79f) },
                { RimRound.Defs.BodyTypeDefOf.F_100_Gelatinous_070,           new BodyTypeInfo(100f  , 3.6250f, 0.05f, 0.22205f, 2.52f, 0.00f) }
            };

        public static Dictionary<BodyTypeDef, BodyTypeInfo> set070Male = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                             new BodyTypeInfo(-1    , 0.7000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                            new BodyTypeInfo(-1    , 0.7000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                          new BodyTypeInfo(-1    , 0.7000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                            new BodyTypeInfo(0.015f, 0.7000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                            new BodyTypeInfo(0.035f, 0.7000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.M_005_Thick_070,                new BodyTypeInfo(0.050f, 0.6250f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.M_006_Chonky_070,               new BodyTypeInfo(0.065f, 0.7500f, 0.90f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.M_010_Chubby_070,               new BodyTypeInfo(0.090f, 0.7500f, 0.80f, 1.28205f, 0.30f, 0.35f) },
                { RimRound.Defs.BodyTypeDefOf.M_020_Corpulent_070,            new BodyTypeInfo(0.120f, 0.7500f, 0.65f, 0.98205f, 0.40f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.M_030_Fat_070,                  new BodyTypeInfo(0.155f, 0.7500f, 0.50f, 0.88205f, 0.44f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.M_040_Obese_070,                new BodyTypeInfo(0.200f, 0.7500f, 0.40f, 0.77205f, 0.50f, 0.40f) },
                { RimRound.Defs.BodyTypeDefOf.M_050_MorbidlyObese_070,        new BodyTypeInfo(0.280f, 0.7500f, 0.30f, 0.65205f, 0.70f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.M_060_Lardy_070,                new BodyTypeInfo(0.430f, 1.6250f, 0.20f, 0.58205f, 1.00f, 0.70f) },
                { RimRound.Defs.BodyTypeDefOf.M_070_Enormous_070,             new BodyTypeInfo(0.660f, 1.5000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.M_080_Gigantic_070,             new BodyTypeInfo(0.965f, 2.8750f, 0.10f, 0.38205f, 1.10f, 0.65f) },
                { RimRound.Defs.BodyTypeDefOf.M_090_Titanic_070,              new BodyTypeInfo(1.410f, 2.8750f, 0.10f, 0.37205f, 1.10f, 0.79f) },
                { RimRound.Defs.BodyTypeDefOf.M_100_Gelatinous_070,           new BodyTypeInfo(100f  , 3.6250f, 0.05f, 0.22205f, 2.52f, 0.00f) }
            };

        [MayRequire("Roo.AntyRaceMod")]
        public static Dictionary<BodyTypeDef, BodyTypeInfo> antyFemaleSet = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                      new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                   new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },


                { RimWorld.BodyTypeDefOf.Thin,                          new BodyTypeInfo(0.015f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_005_Thick_Anty,         new BodyTypeInfo(0.050f, 0.6875f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_006_Chonky_Anty,        new BodyTypeInfo(0.065f, 0.9375f, 0.90f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_010_Chubby_Anty,        new BodyTypeInfo(0.090f, 0.9375f, 0.80f, 1.28205f, 0.30f, 0.35f) },
                { RimRound.Defs.BodyTypeDefOf.F_020_Corpulent_Anty,     new BodyTypeInfo(0.120f, 0.9375f, 0.65f, 0.98205f, 0.40f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_030_Fat_Anty,           new BodyTypeInfo(0.155f, 1.000f, 0.50f, 0.88205f, 0.44f, 0.45f)  },
                { RimRound.Defs.BodyTypeDefOf.F_040_Obese_Anty,         new BodyTypeInfo(0.200f, 1.125f, 0.40f, 0.77205f, 0.50f, 0.40f)  },
                { RimRound.Defs.BodyTypeDefOf.F_050_MorbidlyObese_Anty, new BodyTypeInfo(0.280f, 1.125f, 0.30f, 0.65205f, 0.70f, 0.50f)  },
                { RimRound.Defs.BodyTypeDefOf.F_060_Lardy_Anty,         new BodyTypeInfo(0.430f, 2.125f, 0.20f, 0.58205f, 1.00f, 0.70f)  },
                { RimRound.Defs.BodyTypeDefOf.F_070_Enormous_Anty,      new BodyTypeInfo(0.660f, 2.000f, 0.10f, 0.48205f, 0.93f, 0.50f)  },
                { RimRound.Defs.BodyTypeDefOf.F_080_Gigantic_Anty,      new BodyTypeInfo(0.965f, 3.875f, 0.10f, 0.38205f, 1.10f, 0.65f)  },
                { RimRound.Defs.BodyTypeDefOf.F_090_Titanic_Anty,       new BodyTypeInfo(1.410f, 3.875f, 0.10f, 0.37205f, 1.10f, 0.79f)  },
                { RimRound.Defs.BodyTypeDefOf.F_100_Gelatinous_Anty,    new BodyTypeInfo(100f  , 5.000f, 0.05f, 0.22205f, 2.52f, 0.00f)  }
            };

        [MayRequire("Solaris.RatkinRaceMod")]
        public static Dictionary<BodyTypeDef, BodyTypeInfo> ratkinFemaleSet = new Dictionary<BodyTypeDef, BodyTypeInfo>()
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
                { RimRound.Defs.BodyTypeDefOf.F_070_Enormous_Ratkin,      new BodyTypeInfo(0.660f, 2.000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_080_Gigantic_Ratkin,      new BodyTypeInfo(0.965f, 3.875f, 0.10f, 0.38205f, 1.10f, 0.65f) },
                { RimRound.Defs.BodyTypeDefOf.F_090_Titanic_Ratkin,       new BodyTypeInfo(1.410f, 3.875f, 0.10f, 0.37205f, 1.10f, 0.79f) },
                { RimRound.Defs.BodyTypeDefOf.F_100_Gelatinous_Ratkin,    new BodyTypeInfo(100f  , 5.000f, 0.05f, 0.22205f, 2.52f, 0.00f) }
            };






        static Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>> fullsizeMaleBodytypes = new Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>
        {
            { BodyArchetype.standard, defaultMaleSet },
            { BodyArchetype.apple,    defaultMaleSet },
        };

        static Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>> fullsizeFemaleBodytypes = new Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>
        {
            { BodyArchetype.standard, defaultFemaleSet },
            { BodyArchetype.apple,    appleFemaleSet   },
        };

        static Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>> P090FemaleBodytypes = new Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>
        {
            { BodyArchetype.standard, set090Female },
            { BodyArchetype.apple,    set090Female },
        };

        static Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>> P090MaleBodytypes = new Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>
        {
            { BodyArchetype.standard, set090Female },
            { BodyArchetype.apple,    set090Female },
        };

        static Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>> P070FemaleBodytypes = new Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>
        {
            { BodyArchetype.standard, set070Female },
            { BodyArchetype.apple,    set070Female },
        };

        static Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>> P070MaleBodytypes = new Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>
        {
            { BodyArchetype.standard, set070Male },
            { BodyArchetype.apple,    set070Male },
        };

        static Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>> antyFemaleBodytypes = new Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>
        {
            { BodyArchetype.standard, antyFemaleSet },
            { BodyArchetype.apple,    antyFemaleSet },
        };

        static Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>> ratkinFemaleBodytypes = new Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>
        {
            { BodyArchetype.standard, ratkinFemaleSet },
            { BodyArchetype.apple,    ratkinFemaleSet },
        };






        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> defaultSet = new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>()
        {
            { Gender.Female, fullsizeFemaleBodytypes },
            { Gender.Male,   fullsizeMaleBodytypes   },
            { Gender.None,   fullsizeFemaleBodytypes },
        };

        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> antySet = new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>()
        {
            { Gender.Female, antyFemaleBodytypes },
            { Gender.Male,   antyFemaleBodytypes },
            { Gender.None,   antyFemaleBodytypes },
        };

        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> ratkinSet = new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>()
        {
            { Gender.Female, ratkinFemaleBodytypes },
            { Gender.Male,   ratkinFemaleBodytypes },
            { Gender.None,   ratkinFemaleBodytypes },
        };


        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> set090 = new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>()
        {
            { Gender.Female, P090FemaleBodytypes },
            { Gender.Male,   P090MaleBodytypes   },
            { Gender.None,   P090FemaleBodytypes },
        };

        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> set070 = new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>()
        {
            { Gender.Female, P070FemaleBodytypes },
            { Gender.Male,   P070MaleBodytypes   },
            { Gender.None,   P070FemaleBodytypes },
        };

        //-------------------Gendered Sets-------------
        public static Dictionary<String, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> genderedSets = new Dictionary<String, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>() 
        {
            { "Bamboo's Set (Standard size)", fullsizeFemaleBodytypes },
            { "ArtOfFire1's Set (Standard size)", fullsizeMaleBodytypes },
            { "Bamboo's Set (0.9 size)", P090FemaleBodytypes },
            { "Bamboo's Set (0.7 size)", P070FemaleBodytypes },
            { "ArtOfFire1's Set (0.7 size)", P070MaleBodytypes },
            { "Bamboo's Set (Ratkin only)", ratkinFemaleBodytypes },
            { "Bamboo's Set (Anty only)", antyFemaleBodytypes },
        };




        //--------------------Mutable Dicts-------------
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> Alien_DogboldFoxbold_Set =     new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(set070);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> Alien_Drow_Otto_Set =          new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> Alien_Orassan_Set =            new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(set070);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> Alien_OrassanHumanHybrid_Set = new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(set090);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> Alien_Protogen_Set =           new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> Alien_Ferrex_Set =             new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> Alien_Fennex_Set =             new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> Alien_Frijjid_Set =            new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> Alien_Loompa_Set =             new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(set070);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> anthro_Set =                   new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> Anty_Set =                     new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(antySet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> ATK_Avianmorph_Set =           new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> ATK_Bovinemorph_Set =          new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> ATK_Caninemorph_Set =          new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> ATK_Cervinemorph_Set =         new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> ATK_Dragomorph_Set =           new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> ATK_Felinemorph_Set =          new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> ATK_Gnollmorph_Set =           new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> ATK_Lagomorph_Set =            new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> ATK_Vulpinemorph_Set =         new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> HalfDragon_Set =               new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> Alien_Equium_Set =             new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> EA_SylveonRace_Set =           new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> Human_Set =                    new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> MinotaurRace_Set =             new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> Moonjelly_Race_Set     =       new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> MQT_Miqote_Set =               new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_OR_Abednedo_Set     =      new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_OR_Aqualish_Set     =      new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_OR_Bith_Set     =          new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_OR_Cerean_Set     =        new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_OR_Devaronian_Set     =    new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_OR_Duros_Set     =         new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_OR_Iktotchi_Set     =      new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_OR_Iridonian_Set     =     new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_OR_Ithorian_Set     =      new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_OR_Mirialan_Set     =      new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_OR_Neimoidian_Set     =    new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_OR_Nikto_Set     =         new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_OR_Selkath_Set     =       new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_OR_Sith_Set     =          new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_OR_Togruta_Set     =       new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_OR_Trandoshan_Set     =    new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_OR_Twilek_Set     =        new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_FR_Tiefling_Set     =      new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_FR_WoodElf_Set     =       new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_FR_Tabaxi_Set     =        new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_FR_SunElf_Set     =        new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_FR_Orc_Set     =           new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_FR_MoonElf_Set     =       new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_FR_Kobold_Set     =        new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(set070);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_FR_Killoren_Set     =      new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_FR_Illithid_Set     =      new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_FR_Hobgoblin_Set     =     new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_FR_HalfOrc_Set     =       new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_FR_Halfling_Set     =      new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(set070);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_FR_Goblin_Set     =        new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(set090);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_FR_Gith_Set     =          new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_FR_Firbolg_Set     =       new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_FR_Dwarf_Set     =         new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(set090);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_FR_DarkElf_Set     =       new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(set090);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> O21_FR_Chitine_Set     =       new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> RE_Asari_Set =                 new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> SlitherRace_Set =              new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> IkquanRace_Set     =           new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> Klikmala_Set     =             new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> Stick_Stickman_Set =           new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> TRAHS_DreemurrRace_Set =       new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> TRAHS_FurretRace_Set =         new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> Ratkin_Set =                   new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(ratkinSet);
        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> ReviaRaceAlien_Set =           new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(defaultSet);


        public static Dictionary<string, Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>> raceToProperDictDictionary = new Dictionary<string, Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>>()
        {
            { "AFoxbold",                  Alien_DogboldFoxbold_Set          },
            { "Alien_Dogbold",             Alien_DogboldFoxbold_Set          },
            { "Alien_Drow_Otto",           Alien_Drow_Otto_Set               },

            { "Alien_Equium",              Alien_Equium_Set                  },
            { "Alien_KEquium",             Alien_Equium_Set                  },
            { "Alien_PEquium",             Alien_Equium_Set                  },
            { "Alien_SMaleEquium",         Alien_Equium_Set                  },

            { "Alien_Ferrex",              Alien_Ferrex_Set                  },
            { "Alien_Fennex",              Alien_Fennex_Set                  },
            { "Alien_Frijjid",             Alien_Frijjid_Set                 },
            { "Alien_Loompa",              Alien_Loompa_Set                  },
            { "Alien_Orassan",             Alien_Orassan_Set                 },
            { "Alien_OrassanHumanHybrid",  Alien_OrassanHumanHybrid_Set      },

            { "Alien_Protogen",            Alien_Protogen_Set                },
            { "Alien_ProtogenNME",         Alien_Protogen_Set                },

            { "Anthro",                    anthro_Set                        }, 
            { "Anty",                      Anty_Set                          },

            { "ATK_Avianmorph",            ATK_Avianmorph_Set                },
            { "ATK_Bovinemorph",           ATK_Bovinemorph_Set               },
            { "ATK_Caninemorph",           ATK_Caninemorph_Set               },
            { "ATK_Cervinemorph",          ATK_Cervinemorph_Set              },
            { "ATK_Dragomorph",            ATK_Dragomorph_Set                },
            { "ATK_Felinemorph",           ATK_Felinemorph_Set               },
            { "ATK_Gnollmorph",            ATK_Gnollmorph_Set                },
            { "ATK_Lagomorph",             ATK_Lagomorph_Set                 },
            { "ATK_Vulpinemorph",          ATK_Vulpinemorph_Set              },

            { "EA_SylveonRace",            EA_SylveonRace_Set                },

            { "HalfDragon",                HalfDragon_Set                    },
            { "HalfDragon_colonist",       HalfDragon_Set                    },
            { "Human",                     Human_Set                         },

            { "MinotaurRace",              MinotaurRace_Set                  },

            { "Moonjelly_Race",            Moonjelly_Race_Set                }, 
            { "MQT_Miqote",                MQT_Miqote_Set                    }, 

            { "O21_OR_Abednedo",           O21_OR_Abednedo_Set               }, 
            { "O21_OR_Aqualish",           O21_OR_Aqualish_Set               }, 
            { "O21_OR_Bith",               O21_OR_Bith_Set                   }, 
            { "O21_OR_Cerean",             O21_OR_Cerean_Set                 }, 
            { "O21_OR_Devaronian",         O21_OR_Devaronian_Set             }, 
            { "O21_OR_Duros",              O21_OR_Duros_Set                  }, 
            { "O21_OR_Iktotchi",           O21_OR_Iktotchi_Set               }, 
            { "O21_OR_Iridonian",          O21_OR_Iridonian_Set              }, 
            { "O21_OR_Ithorian",           O21_OR_Ithorian_Set               }, 
            { "O21_OR_Mirialan",           O21_OR_Mirialan_Set               }, 
            { "O21_OR_Neimoidian",         O21_OR_Neimoidian_Set             }, 
            { "O21_OR_Nikto",              O21_OR_Nikto_Set                  }, 
            { "O21_OR_Selkath",            O21_OR_Selkath_Set                }, 
            { "O21_OR_Sith",               O21_OR_Sith_Set                   }, 
            { "O21_OR_Togruta",            O21_OR_Togruta_Set                }, 
            { "O21_OR_Trandoshan",         O21_OR_Trandoshan_Set             }, 
            { "O21_OR_Twilek",             O21_OR_Twilek_Set                 },

            { "O21_FR_Tiefling",           O21_FR_Tiefling_Set               },
            { "O21_FR_WoodElf",            O21_FR_WoodElf_Set                },
            { "O21_FR_Tabaxi",             O21_FR_Tabaxi_Set                 },
            { "O21_FR_SunElf",             O21_FR_SunElf_Set                 },
            { "O21_FR_Orc",                O21_FR_Orc_Set                    },
            { "O21_FR_MoonElf",            O21_FR_MoonElf_Set                },
            { "O21_FR_Kobold",             O21_FR_Kobold_Set                 },
            { "O21_FR_Killoren",           O21_FR_Killoren_Set               },
            { "O21_FR_Illithid",           O21_FR_Illithid_Set               },
            { "O21_FR_Hobgoblin",          O21_FR_Hobgoblin_Set              },
            { "O21_FR_HalfOrc",            O21_FR_HalfOrc_Set                },
            { "O21_FR_Halfling",           O21_FR_Halfling_Set               },
            { "O21_FR_Goblin",             O21_FR_Goblin_Set                 },
            { "O21_FR_Gith",               O21_FR_Gith_Set                   },
            { "O21_FR_Firbolg",            O21_FR_Firbolg_Set                },
            { "O21_FR_Dwarf",              O21_FR_Dwarf_Set                  },
            { "O21_FR_DarkElf",            O21_FR_DarkElf_Set                },
            { "O21_FR_Chitine",            O21_FR_Chitine_Set                },


            { "Ratkin",                    Ratkin_Set                        }, 
            { "Ratkin_Su",                 Ratkin_Set                        }, 
            { "ReviaRaceAlien",            ReviaRaceAlien_Set                },

            { "RE_Asari",                  RE_Asari_Set                      },

            { "Stick_Stickman",            Stick_Stickman_Set                },
            { "SlitherRace",               SlitherRace_Set                   },
            { "IkquanRace",                IkquanRace_Set                    },  
            { "KlickmalaRace",             Klikmala_Set                      },

            { "TRAHS_DreemurrRace",        TRAHS_DreemurrRace_Set            },
            { "TRAHS_FurretRace",          TRAHS_FurretRace_Set              },

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
            RimRound.Defs.BodyTypeDefOf.F_100_Gelatinous,

            RimRound.Defs.BodyTypeDefOf.F_005a_Thick,
            RimRound.Defs.BodyTypeDefOf.F_006a_Chonky,
            RimRound.Defs.BodyTypeDefOf.F_010a_Chubby,
            RimRound.Defs.BodyTypeDefOf.F_020a_Corpulent,
            RimRound.Defs.BodyTypeDefOf.F_030a_Fat,
            RimRound.Defs.BodyTypeDefOf.F_040a_Obese,
            RimRound.Defs.BodyTypeDefOf.F_050a_MorbidlyObese,
            RimRound.Defs.BodyTypeDefOf.F_060a_Lardy,
            RimRound.Defs.BodyTypeDefOf.F_070a_Enormous,
            RimRound.Defs.BodyTypeDefOf.F_080a_Gigantic,
            RimRound.Defs.BodyTypeDefOf.F_090a_Titanic,
            RimRound.Defs.BodyTypeDefOf.F_100a_Gelatinous,

            RimRound.Defs.BodyTypeDefOf.M_005_Thick,
            RimRound.Defs.BodyTypeDefOf.M_006_Chonky,
            RimRound.Defs.BodyTypeDefOf.M_010_Chubby,
            RimRound.Defs.BodyTypeDefOf.M_020_Corpulent,
            RimRound.Defs.BodyTypeDefOf.M_030_Fat,
            RimRound.Defs.BodyTypeDefOf.M_040_Obese,
            RimRound.Defs.BodyTypeDefOf.M_050_MorbidlyObese,
            RimRound.Defs.BodyTypeDefOf.M_060_Lardy,
            RimRound.Defs.BodyTypeDefOf.M_070_Enormous,
            RimRound.Defs.BodyTypeDefOf.M_080_Gigantic,
            RimRound.Defs.BodyTypeDefOf.M_090_Titanic,
            RimRound.Defs.BodyTypeDefOf.M_100_Gelatinous

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
