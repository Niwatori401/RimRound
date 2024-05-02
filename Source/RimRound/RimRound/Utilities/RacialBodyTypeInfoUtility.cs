using RimRound.Comps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using System.IO;

namespace RimRound.Utilities
{
    [StaticConstructorOnStartup]
    public static class RacialBodyTypeInfoUtility
    {

        static RacialBodyTypeInfoUtility()
        {
            string[] raceEntries = "RR_RaceData".Translate().RawText.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (String line in raceEntries)
            {
                string[] lineData = line.Split(',');
                
                var dictionaryPresetForRace = new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(typeof(RacialBodyTypeInfoUtility).GetField(lineData[1], System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)?.GetValue(null) as Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>);

                RacialBodyTypeInfoUtility.raceToProperDictDictionary.Add(lineData[0], dictionaryPresetForRace);
            }


            string[] bodyTextureInfo = "RR_TextureData".Translate().RawText.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (String line in bodyTextureInfo)
            {
                string[] lineData = line.Split(',');

                RacialBodyTypeInfoUtility.specialRacialTextureSuffixes.Add(lineData[0], lineData[1]);
            }
        }


        // Regex for valid textures
        public static Dictionary<String, Regex> validTextureSuffixes = new Dictionary<String, Regex>()
        {
            {"BW", new Regex(@".*F_[0-9]{3}_.*|.*F_[0-9]{3}a_.*", RegexOptions.Compiled)},
        };

        public static Dictionary<String, String> specialRacialTextureSuffixes = new Dictionary<String, String>();

        public static Dictionary<BodyTypeDef, BodyTypeInfo> GetRacialDictionary(Pawn pawn)
        {
            PawnBodyType_ThingComp pbtComp;

            if (thingIdToPawnCompCache.ContainsKey(pawn.ThingID))
            {

                pbtComp = thingIdToPawnCompCache[pawn.ThingID];
            }
            else
            {
                pbtComp = pawn.TryGetComp<PawnBodyType_ThingComp>();
                thingIdToPawnCompCache.Add(pawn.ThingID, pbtComp);
            }

            if (pbtComp is null)
                return null;

            if (pbtComp.CustomBodyTypeDict != null)
            {
                return pbtComp.CustomBodyTypeDict[pbtComp.BodyArchetype];
            }

            else if (pawn.def is AlienRace.ThingDef_AlienRace race && 
                    raceToProperDictDictionary.ContainsKey(race.defName) && 
                    raceToProperDictDictionary[race.defName].ContainsKey(pawn.gender) && 
                    pbtComp != null && 
                    raceToProperDictDictionary[race.defName][pawn.gender].ContainsKey(pbtComp.BodyArchetype))
            {
                return raceToProperDictDictionary[race.defName][pawn.gender][pbtComp.BodyArchetype];
            }
            return null;
        }

        public static void InvalidateCaches()
        {
            thingIdToPawnCompCache.Clear();
        }

        static Dictionary<string, PawnBodyType_ThingComp> thingIdToPawnCompCache = new Dictionary<string, PawnBodyType_ThingComp>();

        public static BodyTypeInfo? GetRacialBodyTypeInfo(Pawn pawn)
        {
            if (GetRacialDictionary(pawn) is Dictionary<BodyTypeDef, BodyTypeInfo> dictionary)
            {
                if (pawn.Dead && (BodyTypeUtility.GetCorpseContainingPawn(pawn)?.IsDessicated() ?? false))
                {
                    return dictionary[RimWorld.BodyTypeDefOf.Thin];
                }
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
                { RimRound.Defs.BodyTypeDefOf.F_100_Gelatinous,    new BodyTypeInfo(1.860f, 7.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_150_Gelatinous,    new BodyTypeInfo(2.460f, 8.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_200_Gelatinous,    new BodyTypeInfo(2.960f, 9.5000f, 0.05f, 0.20205f, 2.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_250_Gelatinous,    new BodyTypeInfo(3.960f, 10.500f, 0.05f, 0.17000f, 2.80f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_300_Gelatinous,    new BodyTypeInfo(4.960f, 12.000f, 0.05f, 0.14000f, 3.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_350_Gelatinous,    new BodyTypeInfo(6.460f, 13.000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_400_Gelatinous,    new BodyTypeInfo(7.960f, 15.000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_450_Gelatinous,    new BodyTypeInfo(9.960f, 17.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_500_Gelatinous,    new BodyTypeInfo(14.46f, 20.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_900_Gelatinous,    new BodyTypeInfo(21.85f  , 25.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_910_Gelatinous,    new BodyTypeInfo(42.40f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_920_Gelatinous,    new BodyTypeInfo(70.50f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_930_Gelatinous,    new BodyTypeInfo(116.6f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_940_Gelatinous,    new BodyTypeInfo(164.5f  , 70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_950_Gelatinous,    new BodyTypeInfo(286.2f  , 85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_960_Gelatinous,    new BodyTypeInfo(411.0f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_970_Gelatinous,    new BodyTypeInfo(576.7f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_980_Gelatinous,    new BodyTypeInfo(773.2f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_990_Gelatinous,    new BodyTypeInfo(999.9f , 175.000f, 0.05f, 0.00700f, 60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_995_Gelatinous,    new BodyTypeInfo(10000f , 200.000f, 0.05f, 0.00700f, 60f, 0.00f) },
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
                { RimRound.Defs.BodyTypeDefOf.F_100a_Gelatinous,    new BodyTypeInfo(1.860f, 7.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_150a_Gelatinous,    new BodyTypeInfo(2.460f, 8.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_200a_Gelatinous,    new BodyTypeInfo(2.960f, 9.5000f, 0.05f, 0.20205f, 2.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_250a_Gelatinous,    new BodyTypeInfo(3.960f, 10.500f, 0.05f, 0.17000f, 2.80f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_300a_Gelatinous,    new BodyTypeInfo(4.960f, 12.000f, 0.05f, 0.14000f, 3.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_350a_Gelatinous,    new BodyTypeInfo(6.460f, 13.000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_400a_Gelatinous,    new BodyTypeInfo(7.960f, 15.000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_450a_Gelatinous,    new BodyTypeInfo(9.960f, 17.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_500a_Gelatinous,    new BodyTypeInfo(14.46f, 20.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_900a_Gelatinous,    new BodyTypeInfo(21.85f  , 25.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_910a_Gelatinous,    new BodyTypeInfo(42.40f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_920a_Gelatinous,    new BodyTypeInfo(70.50f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_930a_Gelatinous,    new BodyTypeInfo(116.6f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_940a_Gelatinous,    new BodyTypeInfo(164.5f  , 70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_950a_Gelatinous,    new BodyTypeInfo(286.2f  , 85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_960a_Gelatinous,    new BodyTypeInfo(411.0f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_970a_Gelatinous,    new BodyTypeInfo(576.7f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_980a_Gelatinous,    new BodyTypeInfo(773.2f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_990a_Gelatinous,    new BodyTypeInfo(999.9f , 175.000f, 0.05f, 0.00700f, 60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_995a_Gelatinous,    new BodyTypeInfo(10000f , 200.000f, 0.05f, 0.00700f, 60f, 0.00f) },
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
                { RimRound.Defs.BodyTypeDefOf.M_100_Gelatinous,    new BodyTypeInfo(1.860f, 7.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },


                { RimRound.Defs.BodyTypeDefOf.M_150_Gelatinous,    new BodyTypeInfo(2.460f, 8.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_200_Gelatinous,    new BodyTypeInfo(2.960f, 9.5000f, 0.05f, 0.20205f, 2.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_250_Gelatinous,    new BodyTypeInfo(3.960f, 10.500f, 0.05f, 0.17000f, 2.80f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_300_Gelatinous,    new BodyTypeInfo(4.960f, 12.000f, 0.05f, 0.14000f, 3.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_350_Gelatinous,    new BodyTypeInfo(6.460f, 13.000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_400_Gelatinous,    new BodyTypeInfo(7.960f, 15.000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_450_Gelatinous,    new BodyTypeInfo(9.960f, 17.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_500_Gelatinous,    new BodyTypeInfo(14.46f, 20.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_900_Gelatinous,    new BodyTypeInfo(21.85f, 25.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.M_910_Gelatinous,    new BodyTypeInfo(42.40f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_920_Gelatinous,    new BodyTypeInfo(70.50f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_930_Gelatinous,    new BodyTypeInfo(116.6f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_940_Gelatinous,    new BodyTypeInfo(164.5f  , 70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_950_Gelatinous,    new BodyTypeInfo(286.2f  , 85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_960_Gelatinous,    new BodyTypeInfo(411.0f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_970_Gelatinous,    new BodyTypeInfo(576.7f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_980_Gelatinous,    new BodyTypeInfo(773.2f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_990_Gelatinous,    new BodyTypeInfo(999.9f , 175.000f, 0.05f, 0.00700f, 60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_995_Gelatinous,    new BodyTypeInfo(10000f , 200.000f, 0.05f, 0.00700f, 60f, 0.00f) },
        };


        public static Dictionary<BodyTypeDef, BodyTypeInfo> defaultFemaleSetNoFemale = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                      new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                   new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                     new BodyTypeInfo(0.015f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
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
                { RimRound.Defs.BodyTypeDefOf.F_100_Gelatinous,    new BodyTypeInfo(1.860f, 7.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_150_Gelatinous,    new BodyTypeInfo(2.460f, 8.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_200_Gelatinous,    new BodyTypeInfo(2.960f, 9.5000f, 0.05f, 0.20205f, 2.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_250_Gelatinous,    new BodyTypeInfo(3.960f, 10.500f, 0.05f, 0.17000f, 2.80f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_300_Gelatinous,    new BodyTypeInfo(4.960f, 12.000f, 0.05f, 0.14000f, 3.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_350_Gelatinous,    new BodyTypeInfo(6.460f, 13.000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_400_Gelatinous,    new BodyTypeInfo(7.960f, 15.000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_450_Gelatinous,    new BodyTypeInfo(9.960f, 17.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_500_Gelatinous,    new BodyTypeInfo(14.46f, 20.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_900_Gelatinous,    new BodyTypeInfo(21.85f, 25.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_910_Gelatinous,    new BodyTypeInfo(42.40f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_920_Gelatinous,    new BodyTypeInfo(70.50f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_930_Gelatinous,    new BodyTypeInfo(116.6f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_940_Gelatinous,    new BodyTypeInfo(164.5f  , 70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_950_Gelatinous,    new BodyTypeInfo(286.2f  , 85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_960_Gelatinous,    new BodyTypeInfo(411.0f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_970_Gelatinous,    new BodyTypeInfo(576.7f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_980_Gelatinous,    new BodyTypeInfo(773.2f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_990_Gelatinous,    new BodyTypeInfo(999.9f , 175.000f, 0.05f, 0.00700f, 60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_995_Gelatinous,    new BodyTypeInfo(10000f , 200.000f, 0.05f, 0.00700f, 60f, 0.00f) },
            };

        public static Dictionary<BodyTypeDef, BodyTypeInfo> appleFemaleSetNoFemale = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                      new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                   new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                      new BodyTypeInfo(0.015f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
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
                { RimRound.Defs.BodyTypeDefOf.F_100a_Gelatinous,    new BodyTypeInfo(1.860f, 7.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_150a_Gelatinous,    new BodyTypeInfo(2.460f, 8.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_200a_Gelatinous,    new BodyTypeInfo(2.960f, 9.5000f, 0.05f, 0.20205f, 2.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_250a_Gelatinous,    new BodyTypeInfo(3.960f, 10.500f, 0.05f, 0.17000f, 2.80f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_300a_Gelatinous,    new BodyTypeInfo(4.960f, 12.000f, 0.05f, 0.14000f, 3.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_350a_Gelatinous,    new BodyTypeInfo(6.460f, 13.000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_400a_Gelatinous,    new BodyTypeInfo(7.960f, 15.000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_450a_Gelatinous,    new BodyTypeInfo(9.960f, 17.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_500a_Gelatinous,    new BodyTypeInfo(14.46f, 20.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_900a_Gelatinous,    new BodyTypeInfo(21.85f, 25.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },


                { RimRound.Defs.BodyTypeDefOf.F_910a_Gelatinous,    new BodyTypeInfo(42.40f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_920a_Gelatinous,    new BodyTypeInfo(70.50f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_930a_Gelatinous,    new BodyTypeInfo(116.6f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_940a_Gelatinous,    new BodyTypeInfo(164.5f  , 70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_950a_Gelatinous,    new BodyTypeInfo(286.2f  , 85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_960a_Gelatinous,    new BodyTypeInfo(411.0f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_970a_Gelatinous,    new BodyTypeInfo(576.7f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_980a_Gelatinous,    new BodyTypeInfo(773.2f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_990a_Gelatinous,    new BodyTypeInfo(999.9f , 175.000f, 0.05f, 0.00700f, 60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_995a_Gelatinous,    new BodyTypeInfo(10000f , 200.000f, 0.05f, 0.00700f, 60f, 0.00f) },
            };



        public static Dictionary<BodyTypeDef, BodyTypeInfo> set090Female = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                             new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                            new BodyTypeInfo(0.015f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                          new BodyTypeInfo(0.035f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
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
                { RimRound.Defs.BodyTypeDefOf.F_100_Gelatinous_090,       new BodyTypeInfo(1.860f, 5.000f, 0.05f, 0.22205f, 2.52f, 0.00f) },


                { RimRound.Defs.BodyTypeDefOf.F_150_Gelatinous_090,    new BodyTypeInfo(2.460f, 5.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_200_Gelatinous_090,    new BodyTypeInfo(2.960f, 6.2500f, 0.05f, 0.20205f, 2.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_250_Gelatinous_090,    new BodyTypeInfo(3.960f, 7.0000f, 0.05f, 0.17000f, 2.80f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_300_Gelatinous_090,    new BodyTypeInfo(4.960f, 7.7500f, 0.05f, 0.14000f, 3.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_350_Gelatinous_090,    new BodyTypeInfo(6.460f, 8.5000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_400_Gelatinous_090,    new BodyTypeInfo(7.960f, 9.5000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_450_Gelatinous_090,    new BodyTypeInfo(9.960f, 11.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_500_Gelatinous_090,    new BodyTypeInfo(14.46f, 13.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_900_Gelatinous_090,    new BodyTypeInfo(21.85f, 17.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_910_Gelatinous_090,    new BodyTypeInfo(42.40f  , 20.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_920_Gelatinous_090,    new BodyTypeInfo(70.50f  , 25.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_930_Gelatinous_090,    new BodyTypeInfo(116.6f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_940_Gelatinous_090,    new BodyTypeInfo(164.5f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_950_Gelatinous_090,    new BodyTypeInfo(286.2f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_960_Gelatinous_090,    new BodyTypeInfo(411.0f ,  70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_970_Gelatinous_090,    new BodyTypeInfo(576.7f ,  85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_980_Gelatinous_090,    new BodyTypeInfo(773.2f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_990_Gelatinous_090,    new BodyTypeInfo(999.9f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_995_Gelatinous_090,    new BodyTypeInfo(10000f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },

        };

        public static Dictionary<BodyTypeDef, BodyTypeInfo> set090FemaleApple = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                             new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                            new BodyTypeInfo(0.015f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                          new BodyTypeInfo(0.035f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimRound.Defs.BodyTypeDefOf.F_005_Thick_090,            new BodyTypeInfo(0.050f, 0.6875f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_006_Chonky_090,           new BodyTypeInfo(0.065f, 0.9375f, 0.90f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_010a_Chubby_090,           new BodyTypeInfo(0.090f, 0.9375f, 0.80f, 1.28205f, 0.30f, 0.35f) },
                { RimRound.Defs.BodyTypeDefOf.F_020a_Corpulent_090,        new BodyTypeInfo(0.120f, 0.9375f, 0.65f, 0.98205f, 0.40f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_030a_Fat_090,              new BodyTypeInfo(0.155f, 1.000f, 0.50f, 0.88205f, 0.44f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_040a_Obese_090,            new BodyTypeInfo(0.200f, 1.125f, 0.40f, 0.77205f, 0.50f, 0.40f) },
                { RimRound.Defs.BodyTypeDefOf.F_050a_MorbidlyObese_090,    new BodyTypeInfo(0.280f, 1.125f, 0.30f, 0.65205f, 0.70f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_060a_Lardy_090,            new BodyTypeInfo(0.430f, 2.125f, 0.20f, 0.58205f, 1.00f, 0.70f) },
                { RimRound.Defs.BodyTypeDefOf.F_070a_Enormous_090,         new BodyTypeInfo(0.660f, 2.000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_080a_Gigantic_090,         new BodyTypeInfo(0.965f, 3.875f, 0.10f, 0.38205f, 1.10f, 0.65f) },
                { RimRound.Defs.BodyTypeDefOf.F_090a_Titanic_090,          new BodyTypeInfo(1.410f, 3.875f, 0.10f, 0.37205f, 1.10f, 0.79f) },
                { RimRound.Defs.BodyTypeDefOf.F_100a_Gelatinous_090,       new BodyTypeInfo(1.860f, 5.000f, 0.05f, 0.22205f, 2.52f, 0.00f) },


                { RimRound.Defs.BodyTypeDefOf.F_150a_Gelatinous_090,    new BodyTypeInfo(2.460f, 5.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_200a_Gelatinous_090,    new BodyTypeInfo(2.960f, 6.2500f, 0.05f, 0.20205f, 2.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_250a_Gelatinous_090,    new BodyTypeInfo(3.960f, 7.0000f, 0.05f, 0.17000f, 2.80f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_300a_Gelatinous_090,    new BodyTypeInfo(4.960f, 7.7500f, 0.05f, 0.14000f, 3.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_350a_Gelatinous_090,    new BodyTypeInfo(6.460f, 8.5000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_400a_Gelatinous_090,    new BodyTypeInfo(7.960f, 9.5000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_450a_Gelatinous_090,    new BodyTypeInfo(9.960f, 11.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_500a_Gelatinous_090,    new BodyTypeInfo(14.46f, 13.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_900a_Gelatinous_090,    new BodyTypeInfo(21.85f, 17.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_910a_Gelatinous_090,    new BodyTypeInfo(42.40f  , 20.000f, 0.05f, 0.09000f, 6.31f, 0.0f) },
                { RimRound.Defs.BodyTypeDefOf.F_920a_Gelatinous_090,    new BodyTypeInfo(70.50f  , 25.000f, 0.05f, 0.07000f, 7.11f, 0.0f) },
                { RimRound.Defs.BodyTypeDefOf.F_930a_Gelatinous_090,    new BodyTypeInfo(116.6f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_940a_Gelatinous_090,    new BodyTypeInfo(164.5f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_950a_Gelatinous_090,    new BodyTypeInfo(286.2f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_960a_Gelatinous_090,    new BodyTypeInfo(411.0f ,  70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_970a_Gelatinous_090,    new BodyTypeInfo(576.7f ,  85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_980a_Gelatinous_090,    new BodyTypeInfo(773.2f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_990a_Gelatinous_090,    new BodyTypeInfo(999.9f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_995a_Gelatinous_090,    new BodyTypeInfo(10000f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },

        };

        public static Dictionary<BodyTypeDef, BodyTypeInfo> set090Male = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                             new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                          new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                            new BodyTypeInfo(0.015f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                            new BodyTypeInfo(0.035f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimRound.Defs.BodyTypeDefOf.M_005_Thick_090,            new BodyTypeInfo(0.050f, 0.6875f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.M_006_Chonky_090,           new BodyTypeInfo(0.065f, 0.9375f, 0.90f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.M_010_Chubby_090,           new BodyTypeInfo(0.090f, 0.9375f, 0.80f, 1.28205f, 0.30f, 0.35f) },
                { RimRound.Defs.BodyTypeDefOf.M_020_Corpulent_090,        new BodyTypeInfo(0.120f, 0.9375f, 0.65f, 0.98205f, 0.40f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.M_030_Fat_090,              new BodyTypeInfo(0.155f, 1.000f, 0.50f, 0.88205f, 0.44f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.M_040_Obese_090,            new BodyTypeInfo(0.200f, 1.125f, 0.40f, 0.77205f, 0.50f, 0.40f) },
                { RimRound.Defs.BodyTypeDefOf.M_050_MorbidlyObese_090,    new BodyTypeInfo(0.280f, 1.125f, 0.30f, 0.65205f, 0.70f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.M_060_Lardy_090,            new BodyTypeInfo(0.430f, 2.125f, 0.20f, 0.58205f, 1.00f, 0.70f) },
                { RimRound.Defs.BodyTypeDefOf.M_070_Enormous_090,         new BodyTypeInfo(0.660f, 2.000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.M_080_Gigantic_090,         new BodyTypeInfo(0.965f, 3.875f, 0.10f, 0.38205f, 1.10f, 0.65f) },
                { RimRound.Defs.BodyTypeDefOf.M_090_Titanic_090,          new BodyTypeInfo(1.410f, 3.875f, 0.10f, 0.37205f, 1.10f, 0.79f) },
                { RimRound.Defs.BodyTypeDefOf.M_100_Gelatinous_090,       new BodyTypeInfo(1.860f, 5.000f, 0.05f, 0.22205f, 2.52f, 0.00f) },


                { RimRound.Defs.BodyTypeDefOf.M_150_Gelatinous_090,    new BodyTypeInfo(2.460f, 5.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_200_Gelatinous_090,    new BodyTypeInfo(2.960f, 6.2500f, 0.05f, 0.20205f, 2.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_250_Gelatinous_090,    new BodyTypeInfo(3.960f, 7.0000f, 0.05f, 0.17000f, 2.80f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_300_Gelatinous_090,    new BodyTypeInfo(4.960f, 7.7500f, 0.05f, 0.14000f, 3.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_350_Gelatinous_090,    new BodyTypeInfo(6.460f, 8.5000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_400_Gelatinous_090,    new BodyTypeInfo(7.960f, 9.5000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_450_Gelatinous_090,    new BodyTypeInfo(9.960f, 11.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_500_Gelatinous_090,    new BodyTypeInfo(14.46f, 13.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_900_Gelatinous_090,    new BodyTypeInfo(21.85f, 17.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.M_910_Gelatinous_090,    new BodyTypeInfo(42.40f  , 20.000f, 0.05f, 0.09000f, 6.31f, 0.0f) },
                { RimRound.Defs.BodyTypeDefOf.M_920_Gelatinous_090,    new BodyTypeInfo(70.50f  , 25.000f, 0.05f, 0.07000f, 7.11f, 0.0f) },
                { RimRound.Defs.BodyTypeDefOf.M_930_Gelatinous_090,    new BodyTypeInfo(116.6f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_940_Gelatinous_090,    new BodyTypeInfo(164.5f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_950_Gelatinous_090,    new BodyTypeInfo(286.2f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_960_Gelatinous_090,    new BodyTypeInfo(411.0f ,  70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_970_Gelatinous_090,    new BodyTypeInfo(576.7f ,  85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_980_Gelatinous_090,    new BodyTypeInfo(773.2f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_990_Gelatinous_090,    new BodyTypeInfo(999.9f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_995_Gelatinous_090,    new BodyTypeInfo(10000f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },

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

        public static Dictionary<BodyTypeDef, BodyTypeInfo> set090FemaleNoFemaleSprite = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                             new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                          new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

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
                { RimRound.Defs.BodyTypeDefOf.F_100_Gelatinous_090,       new BodyTypeInfo(1.860f, 5.000f, 0.05f, 0.22205f, 2.52f, 0.00f) },


                { RimRound.Defs.BodyTypeDefOf.F_150_Gelatinous_090,    new BodyTypeInfo(2.460f, 5.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_200_Gelatinous_090,    new BodyTypeInfo(2.960f, 6.2500f, 0.05f, 0.20205f, 2.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_250_Gelatinous_090,    new BodyTypeInfo(3.960f, 7.0000f, 0.05f, 0.17000f, 2.80f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_300_Gelatinous_090,    new BodyTypeInfo(4.960f, 7.7500f, 0.05f, 0.14000f, 3.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_350_Gelatinous_090,    new BodyTypeInfo(6.460f, 8.5000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_400_Gelatinous_090,    new BodyTypeInfo(7.960f, 9.5000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_450_Gelatinous_090,    new BodyTypeInfo(9.960f, 11.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_500_Gelatinous_090,    new BodyTypeInfo(14.46f, 13.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_900_Gelatinous_090,    new BodyTypeInfo(21.85f, 17.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_910_Gelatinous_090,    new BodyTypeInfo(42.40f  , 20.000f, 0.05f, 0.09000f, 6.31f, 0.0f) },
                { RimRound.Defs.BodyTypeDefOf.F_920_Gelatinous_090,    new BodyTypeInfo(70.50f  , 25.000f, 0.05f, 0.07000f, 7.11f, 0.0f) },
                { RimRound.Defs.BodyTypeDefOf.F_930_Gelatinous_090,    new BodyTypeInfo(116.6f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_940_Gelatinous_090,    new BodyTypeInfo(164.5f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_950_Gelatinous_090,    new BodyTypeInfo(286.2f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_960_Gelatinous_090,    new BodyTypeInfo(411.0f ,  70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_970_Gelatinous_090,    new BodyTypeInfo(576.7f ,  85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_980_Gelatinous_090,    new BodyTypeInfo(773.2f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_990_Gelatinous_090,    new BodyTypeInfo(999.9f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_995_Gelatinous_090,    new BodyTypeInfo(10000f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },
        };

        public static Dictionary<BodyTypeDef, BodyTypeInfo> set090FemaleAppleNoFemaleSprite = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                             new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                          new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                            new BodyTypeInfo(0.015f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },


                { RimRound.Defs.BodyTypeDefOf.F_005_Thick_090,            new BodyTypeInfo(0.050f, 0.6875f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_006_Chonky_090,           new BodyTypeInfo(0.065f, 0.9375f, 0.90f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_010a_Chubby_090,           new BodyTypeInfo(0.090f, 0.9375f, 0.80f, 1.28205f, 0.30f, 0.35f) },
                { RimRound.Defs.BodyTypeDefOf.F_020a_Corpulent_090,        new BodyTypeInfo(0.120f, 0.9375f, 0.65f, 0.98205f, 0.40f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_030a_Fat_090,              new BodyTypeInfo(0.155f, 1.000f, 0.50f, 0.88205f, 0.44f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_040a_Obese_090,            new BodyTypeInfo(0.200f, 1.125f, 0.40f, 0.77205f, 0.50f, 0.40f) },
                { RimRound.Defs.BodyTypeDefOf.F_050a_MorbidlyObese_090,    new BodyTypeInfo(0.280f, 1.125f, 0.30f, 0.65205f, 0.70f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_060a_Lardy_090,            new BodyTypeInfo(0.430f, 2.125f, 0.20f, 0.58205f, 1.00f, 0.70f) },
                { RimRound.Defs.BodyTypeDefOf.F_070a_Enormous_090,         new BodyTypeInfo(0.660f, 2.000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_080a_Gigantic_090,         new BodyTypeInfo(0.965f, 3.875f, 0.10f, 0.38205f, 1.10f, 0.65f) },
                { RimRound.Defs.BodyTypeDefOf.F_090a_Titanic_090,          new BodyTypeInfo(1.410f, 3.875f, 0.10f, 0.37205f, 1.10f, 0.79f) },
                { RimRound.Defs.BodyTypeDefOf.F_100a_Gelatinous_090,       new BodyTypeInfo(1.860f, 5.000f, 0.05f, 0.22205f, 2.52f, 0.00f) },


                { RimRound.Defs.BodyTypeDefOf.F_150a_Gelatinous_090,    new BodyTypeInfo(2.460f, 5.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_200a_Gelatinous_090,    new BodyTypeInfo(2.960f, 6.2500f, 0.05f, 0.20205f, 2.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_250a_Gelatinous_090,    new BodyTypeInfo(3.960f, 7.0000f, 0.05f, 0.17000f, 2.80f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_300a_Gelatinous_090,    new BodyTypeInfo(4.960f, 7.7500f, 0.05f, 0.14000f, 3.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_350a_Gelatinous_090,    new BodyTypeInfo(6.460f, 8.5000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_400a_Gelatinous_090,    new BodyTypeInfo(7.960f, 9.5000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_450a_Gelatinous_090,    new BodyTypeInfo(9.960f, 11.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_500a_Gelatinous_090,    new BodyTypeInfo(14.46f, 13.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_900a_Gelatinous_090,    new BodyTypeInfo(21.85f, 17.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_910a_Gelatinous_090,    new BodyTypeInfo(42.40f  , 20.000f, 0.05f, 0.09000f, 6.31f, 0.0f) },
                { RimRound.Defs.BodyTypeDefOf.F_920a_Gelatinous_090,    new BodyTypeInfo(70.50f  , 25.000f, 0.05f, 0.07000f, 7.11f, 0.0f) },
                { RimRound.Defs.BodyTypeDefOf.F_930a_Gelatinous_090,    new BodyTypeInfo(116.6f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_940a_Gelatinous_090,    new BodyTypeInfo(164.5f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_950a_Gelatinous_090,    new BodyTypeInfo(286.2f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_960a_Gelatinous_090,    new BodyTypeInfo(411.0f ,  70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_970a_Gelatinous_090,    new BodyTypeInfo(576.7f ,  85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_980a_Gelatinous_090,    new BodyTypeInfo(773.2f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_990a_Gelatinous_090,    new BodyTypeInfo(999.9f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_995a_Gelatinous_090,    new BodyTypeInfo(10000f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },
        };

        public static Dictionary<BodyTypeDef, BodyTypeInfo> set090FemaleNoFemaleStandardThin = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                             new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                          new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                            new BodyTypeInfo(0.015f, 1.000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
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
                { RimRound.Defs.BodyTypeDefOf.F_100_Gelatinous_090,       new BodyTypeInfo(1.860f, 5.000f, 0.05f, 0.22205f, 2.52f, 0.00f) },


                { RimRound.Defs.BodyTypeDefOf.F_150_Gelatinous_090,    new BodyTypeInfo(2.460f, 5.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_200_Gelatinous_090,    new BodyTypeInfo(2.960f, 6.2500f, 0.05f, 0.20205f, 2.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_250_Gelatinous_090,    new BodyTypeInfo(3.960f, 7.0000f, 0.05f, 0.17000f, 2.80f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_300_Gelatinous_090,    new BodyTypeInfo(4.960f, 7.7500f, 0.05f, 0.14000f, 3.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_350_Gelatinous_090,    new BodyTypeInfo(6.460f, 8.5000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_400_Gelatinous_090,    new BodyTypeInfo(7.960f, 9.5000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_450_Gelatinous_090,    new BodyTypeInfo(9.960f, 11.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_500_Gelatinous_090,    new BodyTypeInfo(14.46f, 13.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_900_Gelatinous_090,    new BodyTypeInfo(21.85f, 17.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_910_Gelatinous_090,    new BodyTypeInfo(42.40f  , 20.000f, 0.05f, 0.09000f, 6.31f, 0.0f) },
                { RimRound.Defs.BodyTypeDefOf.F_920_Gelatinous_090,    new BodyTypeInfo(70.50f  , 25.000f, 0.05f, 0.07000f, 7.11f, 0.0f) },
                { RimRound.Defs.BodyTypeDefOf.F_930_Gelatinous_090,    new BodyTypeInfo(116.6f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_940_Gelatinous_090,    new BodyTypeInfo(164.5f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_950_Gelatinous_090,    new BodyTypeInfo(286.2f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_960_Gelatinous_090,    new BodyTypeInfo(411.0f ,  70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_970_Gelatinous_090,    new BodyTypeInfo(576.7f ,  85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_980_Gelatinous_090,    new BodyTypeInfo(773.2f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_990_Gelatinous_090,    new BodyTypeInfo(999.9f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_995_Gelatinous_090,    new BodyTypeInfo(10000f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },
        };

        public static Dictionary<BodyTypeDef, BodyTypeInfo> set090FemaleAppleNoFemaleSpriteStandardThin = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                             new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                          new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                            new BodyTypeInfo(0.015f, 1.000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimRound.Defs.BodyTypeDefOf.F_005_Thick_090,            new BodyTypeInfo(0.050f, 0.6875f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_006_Chonky_090,           new BodyTypeInfo(0.065f, 0.9375f, 0.90f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_010a_Chubby_090,           new BodyTypeInfo(0.090f, 0.9375f, 0.80f, 1.28205f, 0.30f, 0.35f) },
                { RimRound.Defs.BodyTypeDefOf.F_020a_Corpulent_090,        new BodyTypeInfo(0.120f, 0.9375f, 0.65f, 0.98205f, 0.40f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_030a_Fat_090,              new BodyTypeInfo(0.155f, 1.000f, 0.50f, 0.88205f, 0.44f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_040a_Obese_090,            new BodyTypeInfo(0.200f, 1.125f, 0.40f, 0.77205f, 0.50f, 0.40f) },
                { RimRound.Defs.BodyTypeDefOf.F_050a_MorbidlyObese_090,    new BodyTypeInfo(0.280f, 1.125f, 0.30f, 0.65205f, 0.70f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_060a_Lardy_090,            new BodyTypeInfo(0.430f, 2.125f, 0.20f, 0.58205f, 1.00f, 0.70f) },
                { RimRound.Defs.BodyTypeDefOf.F_070a_Enormous_090,         new BodyTypeInfo(0.660f, 2.000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_080a_Gigantic_090,         new BodyTypeInfo(0.965f, 3.875f, 0.10f, 0.38205f, 1.10f, 0.65f) },
                { RimRound.Defs.BodyTypeDefOf.F_090a_Titanic_090,          new BodyTypeInfo(1.410f, 3.875f, 0.10f, 0.37205f, 1.10f, 0.79f) },
                { RimRound.Defs.BodyTypeDefOf.F_100a_Gelatinous_090,       new BodyTypeInfo(1.860f, 5.000f, 0.05f, 0.22205f, 2.52f, 0.00f) },


                { RimRound.Defs.BodyTypeDefOf.F_150a_Gelatinous_090,    new BodyTypeInfo(2.460f, 5.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_200a_Gelatinous_090,    new BodyTypeInfo(2.960f, 6.2500f, 0.05f, 0.20205f, 2.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_250a_Gelatinous_090,    new BodyTypeInfo(3.960f, 7.0000f, 0.05f, 0.17000f, 2.80f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_300a_Gelatinous_090,    new BodyTypeInfo(4.960f, 7.7500f, 0.05f, 0.14000f, 3.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_350a_Gelatinous_090,    new BodyTypeInfo(6.460f, 8.5000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_400a_Gelatinous_090,    new BodyTypeInfo(7.960f, 9.5000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_450a_Gelatinous_090,    new BodyTypeInfo(9.960f, 11.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_500a_Gelatinous_090,    new BodyTypeInfo(14.46f, 13.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_900a_Gelatinous_090,    new BodyTypeInfo(21.85f, 17.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_910a_Gelatinous_090,    new BodyTypeInfo(42.40f  , 20.000f, 0.05f, 0.09000f, 6.31f, 0.0f) },
                { RimRound.Defs.BodyTypeDefOf.F_920a_Gelatinous_090,    new BodyTypeInfo(70.50f  , 25.000f, 0.05f, 0.07000f, 7.11f, 0.0f) },
                { RimRound.Defs.BodyTypeDefOf.F_930a_Gelatinous_090,    new BodyTypeInfo(116.6f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_940a_Gelatinous_090,    new BodyTypeInfo(164.5f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_950a_Gelatinous_090,    new BodyTypeInfo(286.2f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_960a_Gelatinous_090,    new BodyTypeInfo(411.0f ,  70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_970a_Gelatinous_090,    new BodyTypeInfo(576.7f ,  85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_980a_Gelatinous_090,    new BodyTypeInfo(773.2f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_990a_Gelatinous_090,    new BodyTypeInfo(999.9f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_995a_Gelatinous_090,    new BodyTypeInfo(10000f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },
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

        static Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>> fullsizeFemaleBodytypesNoFemaleBodytypes = new Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>
        {
            { BodyArchetype.standard, set090FemaleNoFemaleStandardThin },
            { BodyArchetype.apple,    set090FemaleAppleNoFemaleSpriteStandardThin   },
        };

        static Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>> P090FemaleBodytypes = new Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>
        {
            { BodyArchetype.standard, set090Female      },
            { BodyArchetype.apple,    set090FemaleApple },
        };

        static Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>> P090MaleBodytypes = new Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>
        {
            { BodyArchetype.standard, set090Male },
            { BodyArchetype.apple,    set090Male },
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
            { BodyArchetype.standard, set090FemaleNoFemaleSprite },
            { BodyArchetype.apple,    set090FemaleAppleNoFemaleSprite },
        };

        static Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>> ratkinFemaleBodytypes = new Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>
        {
            { BodyArchetype.standard, set090FemaleNoFemaleSprite },
            { BodyArchetype.apple,    set090FemaleAppleNoFemaleSprite },
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

        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> rabbieSet = new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>()
        {
            { Gender.Female, fullsizeFemaleBodytypesNoFemaleBodytypes },
            { Gender.Male,   fullsizeFemaleBodytypesNoFemaleBodytypes },
            { Gender.None,   fullsizeFemaleBodytypesNoFemaleBodytypes },
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
            { "ArtOfFire1's Set (0.9 size)", P090MaleBodytypes },
            { "ArtOfFire1's Set (0.7 size)", P070MaleBodytypes },
            { "Bamboo's Set (Ratkin only)", ratkinFemaleBodytypes },
            { "Bamboo's Set (Anty only)", antyFemaleBodytypes },
        };


        public static Dictionary<string, Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>> raceToProperDictDictionary = new Dictionary<string, Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>>();

        public static Dictionary<int, int> gelatinousLevelToCode = new Dictionary<int, int>()
        {
            { 1,  100 },
            { 2,  150 },
            { 3,  200 },
            { 4,  250 },
            { 5,  300 },
            { 6,  350 },
            { 7,  400 },
            { 8,  450 },
            { 9,  500 },
            { 10, 900 },
            { 11, 910 },
            { 12, 920 },
            { 13, 930 },
            { 14, 940 },
            { 15, 950 },
            { 16, 960 },
            { 17, 970 },
            { 18, 980 },
            { 19, 990 },
            { 20, 995 },
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

            RimRound.Defs.BodyTypeDefOf.F_150_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_200_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_250_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_300_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_350_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_400_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_450_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_500_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_900_Gelatinous,


            RimRound.Defs.BodyTypeDefOf.F_910_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_920_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_930_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_940_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_950_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_960_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_970_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_980_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_990_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_995_Gelatinous,

            RimRound.Defs.BodyTypeDefOf.F_910a_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_920a_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_930a_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_940a_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_950a_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_960a_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_970a_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_980a_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_990a_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_995a_Gelatinous,

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

            RimRound.Defs.BodyTypeDefOf.F_150a_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_200a_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_250a_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_300a_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_350a_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_400a_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_450a_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_500a_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_900a_Gelatinous,

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
            RimRound.Defs.BodyTypeDefOf.M_100_Gelatinous,

            RimRound.Defs.BodyTypeDefOf.M_150_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_200_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_250_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_300_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_350_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_400_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_450_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_500_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_900_Gelatinous,

            RimRound.Defs.BodyTypeDefOf.M_910_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_920_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_930_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_940_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_950_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_960_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_970_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_980_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_990_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_995_Gelatinous,

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
