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

        static TaggedString GetEnglishStringFromKey(this String key) {
            TaggedString res;
            LanguageDatabase.defaultLanguage.TryGetTextFromKey(key, out res);

            return res;
        }

        static RacialBodyTypeInfoUtility()
        {
            string[] raceEntries = "RR_RaceData".GetEnglishStringFromKey().RawText.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (String line in raceEntries)
            {
                string[] lineData = line.Split(',');
                
                var dictionaryPresetForRace = new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(typeof(RacialBodyTypeInfoUtility).GetField(lineData[1], System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)?.GetValue(null) as Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>);

                RacialBodyTypeInfoUtility.raceToProperDictDictionary.Add(lineData[0], dictionaryPresetForRace);
            }


            string[] bodyTextureInfo = "RR_TextureData".GetEnglishStringFromKey().RawText.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

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
                    Log.Warning("Ran default case in GetBodyTypeWeightRequirementMultiplier!");
                    break;
            }

            return 0;
        }


        private static Dictionary<BodyTypeDef, BodyTypeInfo> RemoveKeysAndReturnClonedDictionary(Dictionary<BodyTypeDef, BodyTypeInfo> dictToClone, List<BodyTypeDef> defsToRemove) 
        {
            Dictionary<BodyTypeDef, BodyTypeInfo> dictToCloneInto = new Dictionary<BodyTypeDef, BodyTypeInfo>(dictToClone);
            foreach (BodyTypeDef btd in defsToRemove) 
            {
                dictToCloneInto.Remove(btd);
            }

            return dictToCloneInto;
        }

        #region 100 Size Sprites

        #region Female

        public static Dictionary<BodyTypeDef, BodyTypeInfo> bambooStandardFemaleSet = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                      new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                     new BodyTypeInfo(0.015f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                   new BodyTypeInfo(0.035f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_005bs_Thick,         new BodyTypeInfo(0.050f, 0.8750f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_006bs_Chonky,        new BodyTypeInfo(0.065f, 1.1250f, 0.90f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_010bs_Chubby,        new BodyTypeInfo(0.090f, 1.2500f, 0.80f, 1.28205f, 0.30f, 0.35f) },
                { RimRound.Defs.BodyTypeDefOf.F_020bs_Corpulent,     new BodyTypeInfo(0.120f, 1.3750f, 0.65f, 0.98205f, 0.40f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_030bs_Fat,           new BodyTypeInfo(0.155f, 1.3750f, 0.50f, 0.88205f, 0.44f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_040bs_Obese,         new BodyTypeInfo(0.200f, 1.3750f, 0.40f, 0.77205f, 0.50f, 0.40f) },
                { RimRound.Defs.BodyTypeDefOf.F_050bs_MorbidlyObese, new BodyTypeInfo(0.280f, 1.3750f, 0.30f, 0.65205f, 0.70f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_060bs_Lardy,         new BodyTypeInfo(0.430f, 2.7500f, 0.20f, 0.58205f, 1.00f, 0.70f) },
                { RimRound.Defs.BodyTypeDefOf.F_070bs_Enormous,      new BodyTypeInfo(0.660f, 2.5000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_080bs_Gigantic,      new BodyTypeInfo(0.965f, 5.5000f, 0.10f, 0.38205f, 1.10f, 0.65f) },
                { RimRound.Defs.BodyTypeDefOf.F_090bs_Titanic,       new BodyTypeInfo(1.410f, 5.2500f, 0.10f, 0.37205f, 1.10f, 0.79f) },
                { RimRound.Defs.BodyTypeDefOf.F_100bs_Gelatinous,    new BodyTypeInfo(1.860f, 7.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_150bs_Gelatinous,    new BodyTypeInfo(2.460f, 8.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_200bs_Gelatinous,    new BodyTypeInfo(2.960f, 9.5000f, 0.05f, 0.20205f, 2.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_250bs_Gelatinous,    new BodyTypeInfo(3.960f, 10.500f, 0.05f, 0.17000f, 2.80f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_300bs_Gelatinous,    new BodyTypeInfo(4.960f, 12.000f, 0.05f, 0.14000f, 3.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_350bs_Gelatinous,    new BodyTypeInfo(6.460f, 13.000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_400bs_Gelatinous,    new BodyTypeInfo(7.960f, 15.000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_450bs_Gelatinous,    new BodyTypeInfo(9.960f, 17.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_500bs_Gelatinous,    new BodyTypeInfo(14.46f, 20.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_900bs_Gelatinous,    new BodyTypeInfo(21.85f  , 25.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_910bs_Gelatinous,    new BodyTypeInfo(42.40f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_920bs_Gelatinous,    new BodyTypeInfo(70.50f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_930bs_Gelatinous,    new BodyTypeInfo(116.6f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_940bs_Gelatinous,    new BodyTypeInfo(164.5f  , 70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_950bs_Gelatinous,    new BodyTypeInfo(286.2f  , 85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_960bs_Gelatinous,    new BodyTypeInfo(411.0f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_970bs_Gelatinous,    new BodyTypeInfo(576.7f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_980bs_Gelatinous,    new BodyTypeInfo(773.2f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_990bs_Gelatinous,    new BodyTypeInfo(999.9f , 175.000f, 0.05f, 0.00700f, 60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_995bs_Gelatinous,    new BodyTypeInfo(10000f , 200.000f, 0.05f, 0.00700f, 60f, 0.00f) },
            };

        public static Dictionary<BodyTypeDef, BodyTypeInfo> bambooAppleFemaleSet = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                      new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                      new BodyTypeInfo(0.015f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                    new BodyTypeInfo(0.035f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_005bs_Thick,          new BodyTypeInfo(0.050f, 0.8750f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_006bs_Chonky,         new BodyTypeInfo(0.065f, 1.1250f, 0.90f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_010ba_Chubby,        new BodyTypeInfo(0.090f, 1.2500f, 0.80f, 1.28205f, 0.30f, 0.35f) },
                { RimRound.Defs.BodyTypeDefOf.F_020ba_Corpulent,     new BodyTypeInfo(0.120f, 1.3750f, 0.65f, 0.98205f, 0.40f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_030ba_Fat,           new BodyTypeInfo(0.155f, 1.3750f, 0.50f, 0.88205f, 0.44f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_040ba_Obese,         new BodyTypeInfo(0.200f, 1.3750f, 0.40f, 0.77205f, 0.50f, 0.40f) },
                { RimRound.Defs.BodyTypeDefOf.F_050ba_MorbidlyObese, new BodyTypeInfo(0.280f, 1.3750f, 0.30f, 0.65205f, 0.70f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_060ba_Lardy,         new BodyTypeInfo(0.430f, 2.7500f, 0.20f, 0.58205f, 1.00f, 0.70f) },
                { RimRound.Defs.BodyTypeDefOf.F_070ba_Enormous,      new BodyTypeInfo(0.660f, 2.5000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_080ba_Gigantic,      new BodyTypeInfo(0.965f, 5.5000f, 0.10f, 0.38205f, 1.10f, 0.65f) },
                { RimRound.Defs.BodyTypeDefOf.F_090ba_Titanic,       new BodyTypeInfo(1.410f, 5.2500f, 0.10f, 0.37205f, 1.10f, 0.79f) },
                { RimRound.Defs.BodyTypeDefOf.F_100ba_Gelatinous,    new BodyTypeInfo(1.860f, 7.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_150ba_Gelatinous,    new BodyTypeInfo(2.460f, 8.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_200ba_Gelatinous,    new BodyTypeInfo(2.960f, 9.5000f, 0.05f, 0.20205f, 2.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_250ba_Gelatinous,    new BodyTypeInfo(3.960f, 10.500f, 0.05f, 0.17000f, 2.80f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_300ba_Gelatinous,    new BodyTypeInfo(4.960f, 12.000f, 0.05f, 0.14000f, 3.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_350ba_Gelatinous,    new BodyTypeInfo(6.460f, 13.000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_400ba_Gelatinous,    new BodyTypeInfo(7.960f, 15.000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_450ba_Gelatinous,    new BodyTypeInfo(9.960f, 17.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_500ba_Gelatinous,    new BodyTypeInfo(14.46f, 20.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_900ba_Gelatinous,    new BodyTypeInfo(21.85f  , 25.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_910ba_Gelatinous,    new BodyTypeInfo(42.40f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_920ba_Gelatinous,    new BodyTypeInfo(70.50f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_930ba_Gelatinous,    new BodyTypeInfo(116.6f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_940ba_Gelatinous,    new BodyTypeInfo(164.5f  , 70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_950ba_Gelatinous,    new BodyTypeInfo(286.2f  , 85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_960ba_Gelatinous,    new BodyTypeInfo(411.0f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_970ba_Gelatinous,    new BodyTypeInfo(576.7f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_980ba_Gelatinous,    new BodyTypeInfo(773.2f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_990ba_Gelatinous,    new BodyTypeInfo(999.9f , 175.000f, 0.05f, 0.00700f, 60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_995ba_Gelatinous,    new BodyTypeInfo(10000f , 200.000f, 0.05f, 0.00700f, 60f, 0.00f) },
            };

        public static Dictionary<BodyTypeDef, BodyTypeInfo> gosukeVanillaPlusFemaleSet = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                      new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                     new BodyTypeInfo(0.015f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                   new BodyTypeInfo(0.035f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_005vp_Thick,         new BodyTypeInfo(0.050f, 0.8750f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_006vp_Chonky,        new BodyTypeInfo(0.065f, 1.1250f, 0.90f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_010vp_Chubby,        new BodyTypeInfo(0.090f, 1.2500f, 0.80f, 1.28205f, 0.30f, 0.35f) },
                { RimRound.Defs.BodyTypeDefOf.F_020vp_Corpulent,     new BodyTypeInfo(0.120f, 1.3750f, 0.65f, 0.98205f, 0.40f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_030vp_Fat,           new BodyTypeInfo(0.155f, 1.3750f, 0.50f, 0.88205f, 0.44f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_040vp_Obese,         new BodyTypeInfo(0.200f, 1.3750f, 0.40f, 0.77205f, 0.50f, 0.40f) },
                { RimRound.Defs.BodyTypeDefOf.F_050vp_MorbidlyObese, new BodyTypeInfo(0.280f, 1.3750f, 0.30f, 0.65205f, 0.70f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_060vp_Lardy,         new BodyTypeInfo(0.430f, 2.7500f, 0.20f, 0.58205f, 1.00f, 0.70f) },
                { RimRound.Defs.BodyTypeDefOf.F_070vp_Enormous,      new BodyTypeInfo(0.660f, 2.5000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_080vp_Gigantic,      new BodyTypeInfo(0.965f, 5.5000f, 0.10f, 0.38205f, 1.10f, 0.65f) },
                { RimRound.Defs.BodyTypeDefOf.F_090vp_Titanic,       new BodyTypeInfo(1.410f, 5.2500f, 0.10f, 0.37205f, 1.10f, 0.79f) },
                { RimRound.Defs.BodyTypeDefOf.F_100vp_Gelatinous,    new BodyTypeInfo(1.860f, 7.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_150vp_Gelatinous,    new BodyTypeInfo(2.460f, 8.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_200vp_Gelatinous,    new BodyTypeInfo(2.960f, 9.5000f, 0.05f, 0.20205f, 2.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_250vp_Gelatinous,    new BodyTypeInfo(3.960f, 10.500f, 0.05f, 0.17000f, 2.80f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_300vp_Gelatinous,    new BodyTypeInfo(4.960f, 12.000f, 0.05f, 0.14000f, 3.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_350vp_Gelatinous,    new BodyTypeInfo(6.460f, 13.000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_400vp_Gelatinous,    new BodyTypeInfo(7.960f, 15.000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_450vp_Gelatinous,    new BodyTypeInfo(9.960f, 17.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_500vp_Gelatinous,    new BodyTypeInfo(14.46f, 20.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_900vp_Gelatinous,    new BodyTypeInfo(21.85f  , 25.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_910vp_Gelatinous,    new BodyTypeInfo(42.40f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_920vp_Gelatinous,    new BodyTypeInfo(70.50f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_930vp_Gelatinous,    new BodyTypeInfo(116.6f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_940vp_Gelatinous,    new BodyTypeInfo(164.5f  , 70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_950vp_Gelatinous,    new BodyTypeInfo(286.2f  , 85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_960vp_Gelatinous,    new BodyTypeInfo(411.0f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_970vp_Gelatinous,    new BodyTypeInfo(576.7f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_980vp_Gelatinous,    new BodyTypeInfo(773.2f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_990vp_Gelatinous,    new BodyTypeInfo(999.9f , 175.000f, 0.05f, 0.00700f, 60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_995vp_Gelatinous,    new BodyTypeInfo(10000f , 200.000f, 0.05f, 0.00700f, 60f, 0.00f) },
            };

        public static Dictionary<BodyTypeDef, BodyTypeInfo> togglePearFemaleSet = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                      new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                     new BodyTypeInfo(0.015f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                   new BodyTypeInfo(0.035f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_005tp_Thick,         new BodyTypeInfo(0.050f, 0.8750f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_006tp_Chonky,        new BodyTypeInfo(0.065f, 1.1250f, 0.90f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_010tp_Chubby,        new BodyTypeInfo(0.090f, 1.2500f, 0.80f, 1.28205f, 0.30f, 0.35f) },
                { RimRound.Defs.BodyTypeDefOf.F_020tp_Corpulent,     new BodyTypeInfo(0.120f, 1.3750f, 0.65f, 0.98205f, 0.40f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_030tp_Fat,           new BodyTypeInfo(0.155f, 1.3750f, 0.50f, 0.88205f, 0.44f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_040tp_Obese,         new BodyTypeInfo(0.200f, 1.3750f, 0.40f, 0.77205f, 0.50f, 0.40f) },
                { RimRound.Defs.BodyTypeDefOf.F_050tp_MorbidlyObese, new BodyTypeInfo(0.280f, 1.3750f, 0.30f, 0.65205f, 0.70f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_060tp_Lardy,         new BodyTypeInfo(0.430f, 2.7500f, 0.20f, 0.58205f, 1.00f, 0.70f) },
                { RimRound.Defs.BodyTypeDefOf.F_070bs_Enormous,      new BodyTypeInfo(0.660f, 2.5000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_080bs_Gigantic,      new BodyTypeInfo(0.965f, 5.5000f, 0.10f, 0.38205f, 1.10f, 0.65f) },
                { RimRound.Defs.BodyTypeDefOf.F_090bs_Titanic,       new BodyTypeInfo(1.410f, 5.2500f, 0.10f, 0.37205f, 1.10f, 0.79f) },
                { RimRound.Defs.BodyTypeDefOf.F_100bs_Gelatinous,    new BodyTypeInfo(1.860f, 7.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_150bs_Gelatinous,    new BodyTypeInfo(2.460f, 8.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_200bs_Gelatinous,    new BodyTypeInfo(2.960f, 9.5000f, 0.05f, 0.20205f, 2.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_250bs_Gelatinous,    new BodyTypeInfo(3.960f, 10.500f, 0.05f, 0.17000f, 2.80f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_300bs_Gelatinous,    new BodyTypeInfo(4.960f, 12.000f, 0.05f, 0.14000f, 3.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_350bs_Gelatinous,    new BodyTypeInfo(6.460f, 13.000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_400bs_Gelatinous,    new BodyTypeInfo(7.960f, 15.000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_450bs_Gelatinous,    new BodyTypeInfo(9.960f, 17.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_500bs_Gelatinous,    new BodyTypeInfo(14.46f, 20.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_900bs_Gelatinous,    new BodyTypeInfo(21.85f  , 25.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_910bs_Gelatinous,    new BodyTypeInfo(42.40f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_920bs_Gelatinous,    new BodyTypeInfo(70.50f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_930bs_Gelatinous,    new BodyTypeInfo(116.6f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_940bs_Gelatinous,    new BodyTypeInfo(164.5f  , 70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_950bs_Gelatinous,    new BodyTypeInfo(286.2f  , 85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_960bs_Gelatinous,    new BodyTypeInfo(411.0f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_970bs_Gelatinous,    new BodyTypeInfo(576.7f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_980bs_Gelatinous,    new BodyTypeInfo(773.2f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_990bs_Gelatinous,    new BodyTypeInfo(999.9f , 175.000f, 0.05f, 0.00700f, 60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_995bs_Gelatinous,    new BodyTypeInfo(10000f , 200.000f, 0.05f, 0.00700f, 60f, 0.00f) },
            };


        #endregion

        #region Male

        public static Dictionary<BodyTypeDef, BodyTypeInfo> artOfFireMaleSet = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                      new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                   new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                     new BodyTypeInfo(0.015f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                     new BodyTypeInfo(0.035f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.M_005af_Thick,         new BodyTypeInfo(0.050f, 0.8750f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.M_006af_Chonky,        new BodyTypeInfo(0.065f, 1.1250f, 0.90f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.M_010af_Chubby,        new BodyTypeInfo(0.090f, 1.2500f, 0.80f, 1.28205f, 0.30f, 0.35f) },
                { RimRound.Defs.BodyTypeDefOf.M_020af_Corpulent,     new BodyTypeInfo(0.120f, 1.3750f, 0.65f, 0.98205f, 0.40f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.M_030af_Fat,           new BodyTypeInfo(0.155f, 1.3750f, 0.50f, 0.88205f, 0.44f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.M_040af_Obese,         new BodyTypeInfo(0.200f, 1.3750f, 0.40f, 0.77205f, 0.50f, 0.40f) },
                { RimRound.Defs.BodyTypeDefOf.M_050af_MorbidlyObese, new BodyTypeInfo(0.280f, 1.3750f, 0.30f, 0.65205f, 0.70f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.M_060af_Lardy,         new BodyTypeInfo(0.430f, 2.7500f, 0.20f, 0.58205f, 1.00f, 0.70f) },
                { RimRound.Defs.BodyTypeDefOf.M_070af_Enormous,      new BodyTypeInfo(0.660f, 2.5000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.M_080af_Gigantic,      new BodyTypeInfo(0.965f, 5.5000f, 0.10f, 0.38205f, 1.10f, 0.65f) },
                { RimRound.Defs.BodyTypeDefOf.M_090af_Titanic,       new BodyTypeInfo(1.410f, 5.2500f, 0.10f, 0.37205f, 1.10f, 0.79f) },
                { RimRound.Defs.BodyTypeDefOf.M_100af_Gelatinous,    new BodyTypeInfo(1.860f, 7.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },


                { RimRound.Defs.BodyTypeDefOf.M_150af_Gelatinous,    new BodyTypeInfo(2.460f, 8.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_200af_Gelatinous,    new BodyTypeInfo(2.960f, 9.5000f, 0.05f, 0.20205f, 2.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_250af_Gelatinous,    new BodyTypeInfo(3.960f, 10.500f, 0.05f, 0.17000f, 2.80f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_300af_Gelatinous,    new BodyTypeInfo(4.960f, 12.000f, 0.05f, 0.14000f, 3.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_350af_Gelatinous,    new BodyTypeInfo(6.460f, 13.000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_400af_Gelatinous,    new BodyTypeInfo(7.960f, 15.000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_450af_Gelatinous,    new BodyTypeInfo(9.960f, 17.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_500af_Gelatinous,    new BodyTypeInfo(14.46f, 20.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_900af_Gelatinous,    new BodyTypeInfo(21.85f, 25.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.M_910af_Gelatinous,    new BodyTypeInfo(42.40f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_920af_Gelatinous,    new BodyTypeInfo(70.50f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_930af_Gelatinous,    new BodyTypeInfo(116.6f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_940af_Gelatinous,    new BodyTypeInfo(164.5f  , 70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_950af_Gelatinous,    new BodyTypeInfo(286.2f  , 85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_960af_Gelatinous,    new BodyTypeInfo(411.0f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_970af_Gelatinous,    new BodyTypeInfo(576.7f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_980af_Gelatinous,    new BodyTypeInfo(773.2f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_990af_Gelatinous,    new BodyTypeInfo(999.9f , 175.000f, 0.05f, 0.00700f, 60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_995af_Gelatinous,    new BodyTypeInfo(10000f , 200.000f, 0.05f, 0.00700f, 60f, 0.00f) },
        };

        public static Dictionary<BodyTypeDef, BodyTypeInfo> gosukeRedrawMaleSet = new Dictionary<BodyTypeDef, BodyTypeInfo>()
        {
            { RimWorld.BodyTypeDefOf.Fat,                      new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Hulk,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Female,                   new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

            { RimWorld.BodyTypeDefOf.Thin,                     new BodyTypeInfo(0.015f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Male,                     new BodyTypeInfo(0.035f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimRound.Defs.BodyTypeDefOf.M_005gr_Thick,         new BodyTypeInfo(0.050f, 0.8750f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimRound.Defs.BodyTypeDefOf.M_006gr_Chonky,        new BodyTypeInfo(0.065f, 1.1250f, 0.90f, 1.28205f, 0.30f, 0.30f) },
            { RimRound.Defs.BodyTypeDefOf.M_010gr_Chubby,        new BodyTypeInfo(0.090f, 1.2500f, 0.80f, 1.28205f, 0.30f, 0.35f) },
            { RimRound.Defs.BodyTypeDefOf.M_020gr_Corpulent,     new BodyTypeInfo(0.120f, 1.3750f, 0.65f, 0.98205f, 0.40f, 0.45f) },
            { RimRound.Defs.BodyTypeDefOf.M_030gr_Fat,           new BodyTypeInfo(0.155f, 1.3750f, 0.50f, 0.88205f, 0.44f, 0.45f) },
            { RimRound.Defs.BodyTypeDefOf.M_040gr_Obese,         new BodyTypeInfo(0.200f, 1.3750f, 0.40f, 0.77205f, 0.50f, 0.40f) },
            { RimRound.Defs.BodyTypeDefOf.M_050gr_MorbidlyObese, new BodyTypeInfo(0.280f, 1.3750f, 0.30f, 0.65205f, 0.70f, 0.50f) },
            { RimRound.Defs.BodyTypeDefOf.M_060gr_Lardy,         new BodyTypeInfo(0.430f, 2.7500f, 0.20f, 0.58205f, 1.00f, 0.70f) },
            { RimRound.Defs.BodyTypeDefOf.M_070gr_Enormous,      new BodyTypeInfo(0.660f, 2.5000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
            { RimRound.Defs.BodyTypeDefOf.M_080gr_Gigantic,      new BodyTypeInfo(0.965f, 5.5000f, 0.10f, 0.38205f, 1.10f, 0.65f) },
            { RimRound.Defs.BodyTypeDefOf.M_090gr_Titanic,       new BodyTypeInfo(1.410f, 5.2500f, 0.10f, 0.37205f, 1.10f, 0.79f) },
            { RimRound.Defs.BodyTypeDefOf.M_100gr_Gelatinous,    new BodyTypeInfo(1.860f, 7.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },


            { RimRound.Defs.BodyTypeDefOf.M_150gr_Gelatinous,    new BodyTypeInfo(2.460f, 8.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_200gr_Gelatinous,    new BodyTypeInfo(2.960f, 9.5000f, 0.05f, 0.20205f, 2.60f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_250gr_Gelatinous,    new BodyTypeInfo(3.960f, 10.500f, 0.05f, 0.17000f, 2.80f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_300gr_Gelatinous,    new BodyTypeInfo(4.960f, 12.000f, 0.05f, 0.14000f, 3.60f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_350gr_Gelatinous,    new BodyTypeInfo(6.460f, 13.000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_400gr_Gelatinous,    new BodyTypeInfo(7.960f, 15.000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_450gr_Gelatinous,    new BodyTypeInfo(9.960f, 17.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_500gr_Gelatinous,    new BodyTypeInfo(14.46f, 20.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_900gr_Gelatinous,    new BodyTypeInfo(21.85f, 25.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

            { RimRound.Defs.BodyTypeDefOf.M_910gr_Gelatinous,    new BodyTypeInfo(42.40f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_920gr_Gelatinous,    new BodyTypeInfo(70.50f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_930gr_Gelatinous,    new BodyTypeInfo(116.6f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_940gr_Gelatinous,    new BodyTypeInfo(164.5f  , 70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_950gr_Gelatinous,    new BodyTypeInfo(286.2f  , 85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_960gr_Gelatinous,    new BodyTypeInfo(411.0f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_970gr_Gelatinous,    new BodyTypeInfo(576.7f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_980gr_Gelatinous,    new BodyTypeInfo(773.2f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_990gr_Gelatinous,    new BodyTypeInfo(999.9f , 175.000f, 0.05f, 0.00700f, 60f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_995gr_Gelatinous,    new BodyTypeInfo(10000f , 200.000f, 0.05f, 0.00700f, 60f, 0.00f) },
        };

        public static Dictionary<BodyTypeDef, BodyTypeInfo> gosukeVanillaPlusMaleSet = new Dictionary<BodyTypeDef, BodyTypeInfo>()
        {
            { RimWorld.BodyTypeDefOf.Fat,                      new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Hulk,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Female,                   new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

            { RimWorld.BodyTypeDefOf.Thin,                     new BodyTypeInfo(0.015f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Male,                     new BodyTypeInfo(0.035f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimRound.Defs.BodyTypeDefOf.M_005vp_Thick,         new BodyTypeInfo(0.050f, 0.8750f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimRound.Defs.BodyTypeDefOf.M_006vp_Chonky,        new BodyTypeInfo(0.065f, 1.1250f, 0.90f, 1.28205f, 0.30f, 0.30f) },
            { RimRound.Defs.BodyTypeDefOf.M_010vp_Chubby,        new BodyTypeInfo(0.090f, 1.2500f, 0.80f, 1.28205f, 0.30f, 0.35f) },
            { RimRound.Defs.BodyTypeDefOf.M_020vp_Corpulent,     new BodyTypeInfo(0.120f, 1.3750f, 0.65f, 0.98205f, 0.40f, 0.45f) },
            { RimRound.Defs.BodyTypeDefOf.M_030vp_Fat,           new BodyTypeInfo(0.155f, 1.3750f, 0.50f, 0.88205f, 0.44f, 0.45f) },
            { RimRound.Defs.BodyTypeDefOf.M_040vp_Obese,         new BodyTypeInfo(0.200f, 1.3750f, 0.40f, 0.77205f, 0.50f, 0.40f) },
            { RimRound.Defs.BodyTypeDefOf.M_050vp_MorbidlyObese, new BodyTypeInfo(0.280f, 1.3750f, 0.30f, 0.65205f, 0.70f, 0.50f) },
            { RimRound.Defs.BodyTypeDefOf.M_060vp_Lardy,         new BodyTypeInfo(0.430f, 2.7500f, 0.20f, 0.58205f, 1.00f, 0.70f) },
            { RimRound.Defs.BodyTypeDefOf.M_070vp_Enormous,      new BodyTypeInfo(0.660f, 2.5000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
            { RimRound.Defs.BodyTypeDefOf.M_080vp_Gigantic,      new BodyTypeInfo(0.965f, 5.5000f, 0.10f, 0.38205f, 1.10f, 0.65f) },
            { RimRound.Defs.BodyTypeDefOf.M_090vp_Titanic,       new BodyTypeInfo(1.410f, 5.2500f, 0.10f, 0.37205f, 1.10f, 0.79f) },
            { RimRound.Defs.BodyTypeDefOf.M_100vp_Gelatinous,    new BodyTypeInfo(1.860f, 7.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },


            { RimRound.Defs.BodyTypeDefOf.M_150vp_Gelatinous,    new BodyTypeInfo(2.460f, 8.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_200vp_Gelatinous,    new BodyTypeInfo(2.960f, 9.5000f, 0.05f, 0.20205f, 2.60f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_250vp_Gelatinous,    new BodyTypeInfo(3.960f, 10.500f, 0.05f, 0.17000f, 2.80f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_300vp_Gelatinous,    new BodyTypeInfo(4.960f, 12.000f, 0.05f, 0.14000f, 3.60f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_350vp_Gelatinous,    new BodyTypeInfo(6.460f, 13.000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_400vp_Gelatinous,    new BodyTypeInfo(7.960f, 15.000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_450vp_Gelatinous,    new BodyTypeInfo(9.960f, 17.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_500vp_Gelatinous,    new BodyTypeInfo(14.46f, 20.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_900vp_Gelatinous,    new BodyTypeInfo(21.85f, 25.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

            { RimRound.Defs.BodyTypeDefOf.M_910vp_Gelatinous,    new BodyTypeInfo(42.40f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_920vp_Gelatinous,    new BodyTypeInfo(70.50f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_930vp_Gelatinous,    new BodyTypeInfo(116.6f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_940vp_Gelatinous,    new BodyTypeInfo(164.5f  , 70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_950vp_Gelatinous,    new BodyTypeInfo(286.2f  , 85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_960vp_Gelatinous,    new BodyTypeInfo(411.0f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_970vp_Gelatinous,    new BodyTypeInfo(576.7f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_980vp_Gelatinous,    new BodyTypeInfo(773.2f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_990vp_Gelatinous,    new BodyTypeInfo(999.9f , 175.000f, 0.05f, 0.00700f, 60f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_995vp_Gelatinous,    new BodyTypeInfo(10000f , 200.000f, 0.05f, 0.00700f, 60f, 0.00f) },
        };

        public static Dictionary<BodyTypeDef, BodyTypeInfo> meatSlopMaleSet = new Dictionary<BodyTypeDef, BodyTypeInfo>()
        {
            { RimWorld.BodyTypeDefOf.Fat,                      new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Hulk,                     new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Female,                   new BodyTypeInfo(-1    , 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

            { RimWorld.BodyTypeDefOf.Thin,                     new BodyTypeInfo(0.015f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Male,                     new BodyTypeInfo(0.035f, 1.0000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimRound.Defs.BodyTypeDefOf.M_005ms_Thick,         new BodyTypeInfo(0.050f, 0.8750f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimRound.Defs.BodyTypeDefOf.M_006ms_Chonky,        new BodyTypeInfo(0.065f, 1.1250f, 0.90f, 1.28205f, 0.30f, 0.30f) },
            { RimRound.Defs.BodyTypeDefOf.M_010ms_Chubby,        new BodyTypeInfo(0.090f, 1.2500f, 0.80f, 1.28205f, 0.30f, 0.35f) },
            { RimRound.Defs.BodyTypeDefOf.M_020ms_Corpulent,     new BodyTypeInfo(0.120f, 1.3750f, 0.65f, 0.98205f, 0.40f, 0.45f) },
            { RimRound.Defs.BodyTypeDefOf.M_030ms_Fat,           new BodyTypeInfo(0.155f, 1.3750f, 0.50f, 0.88205f, 0.44f, 0.45f) },
            { RimRound.Defs.BodyTypeDefOf.M_040ms_Obese,         new BodyTypeInfo(0.200f, 1.3750f, 0.40f, 0.77205f, 0.50f, 0.40f) },
            { RimRound.Defs.BodyTypeDefOf.M_050ms_MorbidlyObese, new BodyTypeInfo(0.280f, 1.3750f, 0.30f, 0.65205f, 0.70f, 0.50f) },
            { RimRound.Defs.BodyTypeDefOf.M_060ms_Lardy,         new BodyTypeInfo(0.430f, 2.7500f, 0.20f, 0.58205f, 1.00f, 0.70f) },
            { RimRound.Defs.BodyTypeDefOf.M_070ms_Enormous,      new BodyTypeInfo(0.660f, 2.5000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
            { RimRound.Defs.BodyTypeDefOf.M_080ms_Gigantic,      new BodyTypeInfo(0.965f, 5.5000f, 0.10f, 0.38205f, 1.10f, 0.65f) },
            { RimRound.Defs.BodyTypeDefOf.M_090ms_Titanic,       new BodyTypeInfo(1.410f, 5.2500f, 0.10f, 0.37205f, 1.10f, 0.79f) },
            { RimRound.Defs.BodyTypeDefOf.M_100ms_Gelatinous,    new BodyTypeInfo(1.860f, 7.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },


            { RimRound.Defs.BodyTypeDefOf.M_150ms_Gelatinous,    new BodyTypeInfo(2.460f, 8.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_200ms_Gelatinous,    new BodyTypeInfo(2.960f, 9.5000f, 0.05f, 0.20205f, 2.60f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_250ms_Gelatinous,    new BodyTypeInfo(3.960f, 10.500f, 0.05f, 0.17000f, 2.80f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_300ms_Gelatinous,    new BodyTypeInfo(4.960f, 12.000f, 0.05f, 0.14000f, 3.60f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_350ms_Gelatinous,    new BodyTypeInfo(6.460f, 13.000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_400ms_Gelatinous,    new BodyTypeInfo(7.960f, 15.000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_450ms_Gelatinous,    new BodyTypeInfo(9.960f, 17.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_500ms_Gelatinous,    new BodyTypeInfo(14.46f, 20.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_900ms_Gelatinous,    new BodyTypeInfo(21.85f, 25.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

            { RimRound.Defs.BodyTypeDefOf.M_910ms_Gelatinous,    new BodyTypeInfo(42.40f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_920ms_Gelatinous,    new BodyTypeInfo(70.50f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_930ms_Gelatinous,    new BodyTypeInfo(116.6f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_940ms_Gelatinous,    new BodyTypeInfo(164.5f  , 70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_950ms_Gelatinous,    new BodyTypeInfo(286.2f  , 85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_960ms_Gelatinous,    new BodyTypeInfo(411.0f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_970ms_Gelatinous,    new BodyTypeInfo(576.7f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_980ms_Gelatinous,    new BodyTypeInfo(773.2f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_990ms_Gelatinous,    new BodyTypeInfo(999.9f , 175.000f, 0.05f, 0.00700f, 60f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_995ms_Gelatinous,    new BodyTypeInfo(10000f , 200.000f, 0.05f, 0.00700f, 60f, 0.00f) },
        };

        #endregion

        #endregion




        #region 090 Size Sprites

        #region Female

        public static Dictionary<BodyTypeDef, BodyTypeInfo> bambooStandardFemaleSet090 = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                             new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                            new BodyTypeInfo(0.015f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                          new BodyTypeInfo(0.035f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_005bs_Thick_090,            new BodyTypeInfo(0.050f, 0.6875f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_006bs_Chonky_090,           new BodyTypeInfo(0.065f, 0.9375f, 0.90f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_010bs_Chubby_090,           new BodyTypeInfo(0.090f, 0.9375f, 0.80f, 1.28205f, 0.30f, 0.35f) },
                { RimRound.Defs.BodyTypeDefOf.F_020bs_Corpulent_090,        new BodyTypeInfo(0.120f, 0.9375f, 0.65f, 0.98205f, 0.40f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_030bs_Fat_090,              new BodyTypeInfo(0.155f, 1.000f, 0.50f, 0.88205f, 0.44f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_040bs_Obese_090,            new BodyTypeInfo(0.200f, 1.125f, 0.40f, 0.77205f, 0.50f, 0.40f) },
                { RimRound.Defs.BodyTypeDefOf.F_050bs_MorbidlyObese_090,    new BodyTypeInfo(0.280f, 1.125f, 0.30f, 0.65205f, 0.70f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_060bs_Lardy_090,            new BodyTypeInfo(0.430f, 2.125f, 0.20f, 0.58205f, 1.00f, 0.70f) },
                { RimRound.Defs.BodyTypeDefOf.F_070bs_Enormous_090,         new BodyTypeInfo(0.660f, 2.000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_080bs_Gigantic_090,         new BodyTypeInfo(0.965f, 3.875f, 0.10f, 0.38205f, 1.10f, 0.65f) },
                { RimRound.Defs.BodyTypeDefOf.F_090bs_Titanic_090,          new BodyTypeInfo(1.410f, 3.875f, 0.10f, 0.37205f, 1.10f, 0.79f) },
                { RimRound.Defs.BodyTypeDefOf.F_100bs_Gelatinous_090,       new BodyTypeInfo(1.860f, 5.000f, 0.05f, 0.22205f, 2.52f, 0.00f) },


                { RimRound.Defs.BodyTypeDefOf.F_150bs_Gelatinous_090,    new BodyTypeInfo(2.460f, 5.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_200bs_Gelatinous_090,    new BodyTypeInfo(2.960f, 6.2500f, 0.05f, 0.20205f, 2.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_250bs_Gelatinous_090,    new BodyTypeInfo(3.960f, 7.0000f, 0.05f, 0.17000f, 2.80f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_300bs_Gelatinous_090,    new BodyTypeInfo(4.960f, 7.7500f, 0.05f, 0.14000f, 3.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_350bs_Gelatinous_090,    new BodyTypeInfo(6.460f, 8.5000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_400bs_Gelatinous_090,    new BodyTypeInfo(7.960f, 9.5000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_450bs_Gelatinous_090,    new BodyTypeInfo(9.960f, 11.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_500bs_Gelatinous_090,    new BodyTypeInfo(14.46f, 13.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_900bs_Gelatinous_090,    new BodyTypeInfo(21.85f, 17.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_910bs_Gelatinous_090,    new BodyTypeInfo(42.40f  , 20.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_920bs_Gelatinous_090,    new BodyTypeInfo(70.50f  , 25.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_930bs_Gelatinous_090,    new BodyTypeInfo(116.6f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_940bs_Gelatinous_090,    new BodyTypeInfo(164.5f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_950bs_Gelatinous_090,    new BodyTypeInfo(286.2f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_960bs_Gelatinous_090,    new BodyTypeInfo(411.0f ,  70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_970bs_Gelatinous_090,    new BodyTypeInfo(576.7f ,  85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_980bs_Gelatinous_090,    new BodyTypeInfo(773.2f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_990bs_Gelatinous_090,    new BodyTypeInfo(999.9f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_995bs_Gelatinous_090,    new BodyTypeInfo(10000f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },

        };

        public static Dictionary<BodyTypeDef, BodyTypeInfo> bambooAppleFemaleSet090 = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                             new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                            new BodyTypeInfo(0.015f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                          new BodyTypeInfo(0.035f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimRound.Defs.BodyTypeDefOf.F_005bs_Thick_090,            new BodyTypeInfo(0.050f, 0.6875f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_006bs_Chonky_090,           new BodyTypeInfo(0.065f, 0.9375f, 0.90f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.F_010ba_Chubby_090,           new BodyTypeInfo(0.090f, 0.9375f, 0.80f, 1.28205f, 0.30f, 0.35f) },
                { RimRound.Defs.BodyTypeDefOf.F_020ba_Corpulent_090,        new BodyTypeInfo(0.120f, 0.9375f, 0.65f, 0.98205f, 0.40f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_030ba_Fat_090,              new BodyTypeInfo(0.155f, 1.000f, 0.50f, 0.88205f, 0.44f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.F_040ba_Obese_090,            new BodyTypeInfo(0.200f, 1.125f, 0.40f, 0.77205f, 0.50f, 0.40f) },
                { RimRound.Defs.BodyTypeDefOf.F_050ba_MorbidlyObese_090,    new BodyTypeInfo(0.280f, 1.125f, 0.30f, 0.65205f, 0.70f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_060ba_Lardy_090,            new BodyTypeInfo(0.430f, 2.125f, 0.20f, 0.58205f, 1.00f, 0.70f) },
                { RimRound.Defs.BodyTypeDefOf.F_070ba_Enormous_090,         new BodyTypeInfo(0.660f, 2.000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.F_080ba_Gigantic_090,         new BodyTypeInfo(0.965f, 3.875f, 0.10f, 0.38205f, 1.10f, 0.65f) },
                { RimRound.Defs.BodyTypeDefOf.F_090ba_Titanic_090,          new BodyTypeInfo(1.410f, 3.875f, 0.10f, 0.37205f, 1.10f, 0.79f) },
                { RimRound.Defs.BodyTypeDefOf.F_100ba_Gelatinous_090,       new BodyTypeInfo(1.860f, 5.000f, 0.05f, 0.22205f, 2.52f, 0.00f) },


                { RimRound.Defs.BodyTypeDefOf.F_150ba_Gelatinous_090,    new BodyTypeInfo(2.460f, 5.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_200ba_Gelatinous_090,    new BodyTypeInfo(2.960f, 6.2500f, 0.05f, 0.20205f, 2.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_250ba_Gelatinous_090,    new BodyTypeInfo(3.960f, 7.0000f, 0.05f, 0.17000f, 2.80f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_300ba_Gelatinous_090,    new BodyTypeInfo(4.960f, 7.7500f, 0.05f, 0.14000f, 3.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_350ba_Gelatinous_090,    new BodyTypeInfo(6.460f, 8.5000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_400ba_Gelatinous_090,    new BodyTypeInfo(7.960f, 9.5000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_450ba_Gelatinous_090,    new BodyTypeInfo(9.960f, 11.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_500ba_Gelatinous_090,    new BodyTypeInfo(14.46f, 13.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_900ba_Gelatinous_090,    new BodyTypeInfo(21.85f, 17.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.F_910ba_Gelatinous_090,    new BodyTypeInfo(42.40f  , 20.000f, 0.05f, 0.09000f, 6.31f, 0.0f) },
                { RimRound.Defs.BodyTypeDefOf.F_920ba_Gelatinous_090,    new BodyTypeInfo(70.50f  , 25.000f, 0.05f, 0.07000f, 7.11f, 0.0f) },
                { RimRound.Defs.BodyTypeDefOf.F_930ba_Gelatinous_090,    new BodyTypeInfo(116.6f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_940ba_Gelatinous_090,    new BodyTypeInfo(164.5f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_950ba_Gelatinous_090,    new BodyTypeInfo(286.2f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_960ba_Gelatinous_090,    new BodyTypeInfo(411.0f ,  70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_970ba_Gelatinous_090,    new BodyTypeInfo(576.7f ,  85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_980ba_Gelatinous_090,    new BodyTypeInfo(773.2f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_990ba_Gelatinous_090,    new BodyTypeInfo(999.9f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.F_995ba_Gelatinous_090,    new BodyTypeInfo(10000f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },

        };

        public static Dictionary<BodyTypeDef, BodyTypeInfo> gosukeVanillaPlusFemaleSet090 = new Dictionary<BodyTypeDef, BodyTypeInfo>()
        {
            { RimWorld.BodyTypeDefOf.Fat,                             new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Hulk,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Male,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

            { RimWorld.BodyTypeDefOf.Thin,                            new BodyTypeInfo(0.015f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Female,                          new BodyTypeInfo(0.035f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimRound.Defs.BodyTypeDefOf.F_005vp_Thick_090,            new BodyTypeInfo(0.050f, 0.6875f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimRound.Defs.BodyTypeDefOf.F_006vp_Chonky_090,           new BodyTypeInfo(0.065f, 0.9375f, 0.90f, 1.28205f, 0.30f, 0.30f) },
            { RimRound.Defs.BodyTypeDefOf.F_010vp_Chubby_090,           new BodyTypeInfo(0.090f, 0.9375f, 0.80f, 1.28205f, 0.30f, 0.35f) },
            { RimRound.Defs.BodyTypeDefOf.F_020vp_Corpulent_090,        new BodyTypeInfo(0.120f, 0.9375f, 0.65f, 0.98205f, 0.40f, 0.45f) },
            { RimRound.Defs.BodyTypeDefOf.F_030vp_Fat_090,              new BodyTypeInfo(0.155f, 1.000f, 0.50f, 0.88205f, 0.44f, 0.45f) },
            { RimRound.Defs.BodyTypeDefOf.F_040vp_Obese_090,            new BodyTypeInfo(0.200f, 1.125f, 0.40f, 0.77205f, 0.50f, 0.40f) },
            { RimRound.Defs.BodyTypeDefOf.F_050vp_MorbidlyObese_090,    new BodyTypeInfo(0.280f, 1.125f, 0.30f, 0.65205f, 0.70f, 0.50f) },
            { RimRound.Defs.BodyTypeDefOf.F_060vp_Lardy_090,            new BodyTypeInfo(0.430f, 2.125f, 0.20f, 0.58205f, 1.00f, 0.70f) },
            { RimRound.Defs.BodyTypeDefOf.F_070vp_Enormous_090,         new BodyTypeInfo(0.660f, 2.000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
            { RimRound.Defs.BodyTypeDefOf.F_080vp_Gigantic_090,         new BodyTypeInfo(0.965f, 3.875f, 0.10f, 0.38205f, 1.10f, 0.65f) },
            { RimRound.Defs.BodyTypeDefOf.F_090vp_Titanic_090,          new BodyTypeInfo(1.410f, 3.875f, 0.10f, 0.37205f, 1.10f, 0.79f) },
            { RimRound.Defs.BodyTypeDefOf.F_100vp_Gelatinous_090,       new BodyTypeInfo(1.860f, 5.000f, 0.05f, 0.22205f, 2.52f, 0.00f) },


            { RimRound.Defs.BodyTypeDefOf.F_150vp_Gelatinous_090,    new BodyTypeInfo(2.460f, 5.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_200vp_Gelatinous_090,    new BodyTypeInfo(2.960f, 6.2500f, 0.05f, 0.20205f, 2.60f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_250vp_Gelatinous_090,    new BodyTypeInfo(3.960f, 7.0000f, 0.05f, 0.17000f, 2.80f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_300vp_Gelatinous_090,    new BodyTypeInfo(4.960f, 7.7500f, 0.05f, 0.14000f, 3.60f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_350vp_Gelatinous_090,    new BodyTypeInfo(6.460f, 8.5000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_400vp_Gelatinous_090,    new BodyTypeInfo(7.960f, 9.5000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_450vp_Gelatinous_090,    new BodyTypeInfo(9.960f, 11.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_500vp_Gelatinous_090,    new BodyTypeInfo(14.46f, 13.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_900vp_Gelatinous_090,    new BodyTypeInfo(21.85f, 17.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

            { RimRound.Defs.BodyTypeDefOf.F_910vp_Gelatinous_090,    new BodyTypeInfo(42.40f  , 20.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_920vp_Gelatinous_090,    new BodyTypeInfo(70.50f  , 25.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_930vp_Gelatinous_090,    new BodyTypeInfo(116.6f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_940vp_Gelatinous_090,    new BodyTypeInfo(164.5f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_950vp_Gelatinous_090,    new BodyTypeInfo(286.2f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_960vp_Gelatinous_090,    new BodyTypeInfo(411.0f ,  70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_970vp_Gelatinous_090,    new BodyTypeInfo(576.7f ,  85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_980vp_Gelatinous_090,    new BodyTypeInfo(773.2f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_990vp_Gelatinous_090,    new BodyTypeInfo(999.9f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_995vp_Gelatinous_090,    new BodyTypeInfo(10000f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },

        };

        public static Dictionary<BodyTypeDef, BodyTypeInfo> togglePearFemaleSet090 = new Dictionary<BodyTypeDef, BodyTypeInfo>()
        {
            { RimWorld.BodyTypeDefOf.Fat,                             new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Hulk,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Male,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

            { RimWorld.BodyTypeDefOf.Thin,                            new BodyTypeInfo(0.015f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Female,                          new BodyTypeInfo(0.035f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimRound.Defs.BodyTypeDefOf.F_005tp_Thick_090,            new BodyTypeInfo(0.050f, 0.6875f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimRound.Defs.BodyTypeDefOf.F_006tp_Chonky_090,           new BodyTypeInfo(0.065f, 0.9375f, 0.90f, 1.28205f, 0.30f, 0.30f) },
            { RimRound.Defs.BodyTypeDefOf.F_010tp_Chubby_090,           new BodyTypeInfo(0.090f, 0.9375f, 0.80f, 1.28205f, 0.30f, 0.35f) },
            { RimRound.Defs.BodyTypeDefOf.F_020tp_Corpulent_090,        new BodyTypeInfo(0.120f, 0.9375f, 0.65f, 0.98205f, 0.40f, 0.45f) },
            { RimRound.Defs.BodyTypeDefOf.F_030tp_Fat_090,              new BodyTypeInfo(0.155f, 1.000f, 0.50f, 0.88205f, 0.44f, 0.45f) },
            { RimRound.Defs.BodyTypeDefOf.F_040tp_Obese_090,            new BodyTypeInfo(0.200f, 1.125f, 0.40f, 0.77205f, 0.50f, 0.40f) },
            { RimRound.Defs.BodyTypeDefOf.F_050tp_MorbidlyObese_090,    new BodyTypeInfo(0.280f, 1.125f, 0.30f, 0.65205f, 0.70f, 0.50f) },
            { RimRound.Defs.BodyTypeDefOf.F_060tp_Lardy_090,            new BodyTypeInfo(0.430f, 2.125f, 0.20f, 0.58205f, 1.00f, 0.70f) },
            { RimRound.Defs.BodyTypeDefOf.F_070bs_Enormous_090,         new BodyTypeInfo(0.660f, 2.000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
            { RimRound.Defs.BodyTypeDefOf.F_080bs_Gigantic_090,         new BodyTypeInfo(0.965f, 3.875f, 0.10f, 0.38205f, 1.10f, 0.65f) },
            { RimRound.Defs.BodyTypeDefOf.F_090bs_Titanic_090,          new BodyTypeInfo(1.410f, 3.875f, 0.10f, 0.37205f, 1.10f, 0.79f) },
            { RimRound.Defs.BodyTypeDefOf.F_100bs_Gelatinous_090,       new BodyTypeInfo(1.860f, 5.000f, 0.05f, 0.22205f, 2.52f, 0.00f) },


            { RimRound.Defs.BodyTypeDefOf.F_150bs_Gelatinous_090,    new BodyTypeInfo(2.460f, 5.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_200bs_Gelatinous_090,    new BodyTypeInfo(2.960f, 6.2500f, 0.05f, 0.20205f, 2.60f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_250bs_Gelatinous_090,    new BodyTypeInfo(3.960f, 7.0000f, 0.05f, 0.17000f, 2.80f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_300bs_Gelatinous_090,    new BodyTypeInfo(4.960f, 7.7500f, 0.05f, 0.14000f, 3.60f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_350bs_Gelatinous_090,    new BodyTypeInfo(6.460f, 8.5000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_400bs_Gelatinous_090,    new BodyTypeInfo(7.960f, 9.5000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_450bs_Gelatinous_090,    new BodyTypeInfo(9.960f, 11.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_500bs_Gelatinous_090,    new BodyTypeInfo(14.46f, 13.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_900bs_Gelatinous_090,    new BodyTypeInfo(21.85f, 17.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

            { RimRound.Defs.BodyTypeDefOf.F_910bs_Gelatinous_090,    new BodyTypeInfo(42.40f  , 20.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_920bs_Gelatinous_090,    new BodyTypeInfo(70.50f  , 25.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_930bs_Gelatinous_090,    new BodyTypeInfo(116.6f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_940bs_Gelatinous_090,    new BodyTypeInfo(164.5f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_950bs_Gelatinous_090,    new BodyTypeInfo(286.2f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_960bs_Gelatinous_090,    new BodyTypeInfo(411.0f ,  70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_970bs_Gelatinous_090,    new BodyTypeInfo(576.7f ,  85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_980bs_Gelatinous_090,    new BodyTypeInfo(773.2f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_990bs_Gelatinous_090,    new BodyTypeInfo(999.9f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.F_995bs_Gelatinous_090,    new BodyTypeInfo(10000f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },

        };


        #endregion

        #region Male

        public static Dictionary<BodyTypeDef, BodyTypeInfo> artOfFireMaleSet090 = new Dictionary<BodyTypeDef, BodyTypeInfo>()
            {
                { RimWorld.BodyTypeDefOf.Fat,                             new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Hulk,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Female,                          new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimWorld.BodyTypeDefOf.Thin,                            new BodyTypeInfo(0.015f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimWorld.BodyTypeDefOf.Male,                            new BodyTypeInfo(0.035f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

                { RimRound.Defs.BodyTypeDefOf.M_005af_Thick_090,            new BodyTypeInfo(0.050f, 0.6875f, 1.00f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.M_006af_Chonky_090,           new BodyTypeInfo(0.065f, 0.9375f, 0.90f, 1.28205f, 0.30f, 0.30f) },
                { RimRound.Defs.BodyTypeDefOf.M_010af_Chubby_090,           new BodyTypeInfo(0.090f, 0.9375f, 0.80f, 1.28205f, 0.30f, 0.35f) },
                { RimRound.Defs.BodyTypeDefOf.M_020af_Corpulent_090,        new BodyTypeInfo(0.120f, 0.9375f, 0.65f, 0.98205f, 0.40f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.M_030af_Fat_090,              new BodyTypeInfo(0.155f, 1.000f, 0.50f, 0.88205f, 0.44f, 0.45f) },
                { RimRound.Defs.BodyTypeDefOf.M_040af_Obese_090,            new BodyTypeInfo(0.200f, 1.125f, 0.40f, 0.77205f, 0.50f, 0.40f) },
                { RimRound.Defs.BodyTypeDefOf.M_050af_MorbidlyObese_090,    new BodyTypeInfo(0.280f, 1.125f, 0.30f, 0.65205f, 0.70f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.M_060af_Lardy_090,            new BodyTypeInfo(0.430f, 2.125f, 0.20f, 0.58205f, 1.00f, 0.70f) },
                { RimRound.Defs.BodyTypeDefOf.M_070af_Enormous_090,         new BodyTypeInfo(0.660f, 2.000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
                { RimRound.Defs.BodyTypeDefOf.M_080af_Gigantic_090,         new BodyTypeInfo(0.965f, 3.875f, 0.10f, 0.38205f, 1.10f, 0.65f) },
                { RimRound.Defs.BodyTypeDefOf.M_090af_Titanic_090,          new BodyTypeInfo(1.410f, 3.875f, 0.10f, 0.37205f, 1.10f, 0.79f) },
                { RimRound.Defs.BodyTypeDefOf.M_100af_Gelatinous_090,       new BodyTypeInfo(1.860f, 5.000f, 0.05f, 0.22205f, 2.52f, 0.00f) },


                { RimRound.Defs.BodyTypeDefOf.M_150af_Gelatinous_090,    new BodyTypeInfo(2.460f, 5.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_200af_Gelatinous_090,    new BodyTypeInfo(2.960f, 6.2500f, 0.05f, 0.20205f, 2.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_250af_Gelatinous_090,    new BodyTypeInfo(3.960f, 7.0000f, 0.05f, 0.17000f, 2.80f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_300af_Gelatinous_090,    new BodyTypeInfo(4.960f, 7.7500f, 0.05f, 0.14000f, 3.60f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_350af_Gelatinous_090,    new BodyTypeInfo(6.460f, 8.5000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_400af_Gelatinous_090,    new BodyTypeInfo(7.960f, 9.5000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_450af_Gelatinous_090,    new BodyTypeInfo(9.960f, 11.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_500af_Gelatinous_090,    new BodyTypeInfo(14.46f, 13.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_900af_Gelatinous_090,    new BodyTypeInfo(21.85f, 17.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

                { RimRound.Defs.BodyTypeDefOf.M_910af_Gelatinous_090,    new BodyTypeInfo(42.40f  , 20.000f, 0.05f, 0.09000f, 6.31f, 0.0f) },
                { RimRound.Defs.BodyTypeDefOf.M_920af_Gelatinous_090,    new BodyTypeInfo(70.50f  , 25.000f, 0.05f, 0.07000f, 7.11f, 0.0f) },
                { RimRound.Defs.BodyTypeDefOf.M_930af_Gelatinous_090,    new BodyTypeInfo(116.6f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_940af_Gelatinous_090,    new BodyTypeInfo(164.5f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_950af_Gelatinous_090,    new BodyTypeInfo(286.2f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_960af_Gelatinous_090,    new BodyTypeInfo(411.0f ,  70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_970af_Gelatinous_090,    new BodyTypeInfo(576.7f ,  85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_980af_Gelatinous_090,    new BodyTypeInfo(773.2f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_990af_Gelatinous_090,    new BodyTypeInfo(999.9f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
                { RimRound.Defs.BodyTypeDefOf.M_995af_Gelatinous_090,    new BodyTypeInfo(10000f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },

        };

        public static Dictionary<BodyTypeDef, BodyTypeInfo> gosukeRedrawMaleSet090 = new Dictionary<BodyTypeDef, BodyTypeInfo>()
        {
            { RimWorld.BodyTypeDefOf.Fat,                             new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Hulk,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Female,                          new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

            { RimWorld.BodyTypeDefOf.Thin,                            new BodyTypeInfo(0.015f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Male,                            new BodyTypeInfo(0.035f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

            { RimRound.Defs.BodyTypeDefOf.M_005gr_Thick_090,            new BodyTypeInfo(0.050f, 0.6875f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimRound.Defs.BodyTypeDefOf.M_006gr_Chonky_090,           new BodyTypeInfo(0.065f, 0.9375f, 0.90f, 1.28205f, 0.30f, 0.30f) },
            { RimRound.Defs.BodyTypeDefOf.M_010gr_Chubby_090,           new BodyTypeInfo(0.090f, 0.9375f, 0.80f, 1.28205f, 0.30f, 0.35f) },
            { RimRound.Defs.BodyTypeDefOf.M_020gr_Corpulent_090,        new BodyTypeInfo(0.120f, 0.9375f, 0.65f, 0.98205f, 0.40f, 0.45f) },
            { RimRound.Defs.BodyTypeDefOf.M_030gr_Fat_090,              new BodyTypeInfo(0.155f, 1.000f, 0.50f, 0.88205f, 0.44f, 0.45f) },
            { RimRound.Defs.BodyTypeDefOf.M_040gr_Obese_090,            new BodyTypeInfo(0.200f, 1.125f, 0.40f, 0.77205f, 0.50f, 0.40f) },
            { RimRound.Defs.BodyTypeDefOf.M_050gr_MorbidlyObese_090,    new BodyTypeInfo(0.280f, 1.125f, 0.30f, 0.65205f, 0.70f, 0.50f) },
            { RimRound.Defs.BodyTypeDefOf.M_060gr_Lardy_090,            new BodyTypeInfo(0.430f, 2.125f, 0.20f, 0.58205f, 1.00f, 0.70f) },
            { RimRound.Defs.BodyTypeDefOf.M_070gr_Enormous_090,         new BodyTypeInfo(0.660f, 2.000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
            { RimRound.Defs.BodyTypeDefOf.M_080gr_Gigantic_090,         new BodyTypeInfo(0.965f, 3.875f, 0.10f, 0.38205f, 1.10f, 0.65f) },
            { RimRound.Defs.BodyTypeDefOf.M_090gr_Titanic_090,          new BodyTypeInfo(1.410f, 3.875f, 0.10f, 0.37205f, 1.10f, 0.79f) },
            { RimRound.Defs.BodyTypeDefOf.M_100gr_Gelatinous_090,       new BodyTypeInfo(1.860f, 5.000f, 0.05f, 0.22205f, 2.52f, 0.00f) },


            { RimRound.Defs.BodyTypeDefOf.M_150gr_Gelatinous_090,    new BodyTypeInfo(2.460f, 5.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_200gr_Gelatinous_090,    new BodyTypeInfo(2.960f, 6.2500f, 0.05f, 0.20205f, 2.60f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_250gr_Gelatinous_090,    new BodyTypeInfo(3.960f, 7.0000f, 0.05f, 0.17000f, 2.80f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_300gr_Gelatinous_090,    new BodyTypeInfo(4.960f, 7.7500f, 0.05f, 0.14000f, 3.60f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_350gr_Gelatinous_090,    new BodyTypeInfo(6.460f, 8.5000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_400gr_Gelatinous_090,    new BodyTypeInfo(7.960f, 9.5000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_450gr_Gelatinous_090,    new BodyTypeInfo(9.960f, 11.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_500gr_Gelatinous_090,    new BodyTypeInfo(14.46f, 13.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_900gr_Gelatinous_090,    new BodyTypeInfo(21.85f, 17.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

            { RimRound.Defs.BodyTypeDefOf.M_910gr_Gelatinous_090,    new BodyTypeInfo(42.40f  , 20.000f, 0.05f, 0.09000f, 6.31f, 0.0f) },
            { RimRound.Defs.BodyTypeDefOf.M_920gr_Gelatinous_090,    new BodyTypeInfo(70.50f  , 25.000f, 0.05f, 0.07000f, 7.11f, 0.0f) },
            { RimRound.Defs.BodyTypeDefOf.M_930gr_Gelatinous_090,    new BodyTypeInfo(116.6f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_940gr_Gelatinous_090,    new BodyTypeInfo(164.5f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_950gr_Gelatinous_090,    new BodyTypeInfo(286.2f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_960gr_Gelatinous_090,    new BodyTypeInfo(411.0f ,  70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_970gr_Gelatinous_090,    new BodyTypeInfo(576.7f ,  85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_980gr_Gelatinous_090,    new BodyTypeInfo(773.2f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_990gr_Gelatinous_090,    new BodyTypeInfo(999.9f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_995gr_Gelatinous_090,    new BodyTypeInfo(10000f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },

        };

        public static Dictionary<BodyTypeDef, BodyTypeInfo> gosukeVanillaPlusMaleSet090 = new Dictionary<BodyTypeDef, BodyTypeInfo>()
        {
            { RimWorld.BodyTypeDefOf.Fat,                             new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Hulk,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Female,                          new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

            { RimWorld.BodyTypeDefOf.Thin,                            new BodyTypeInfo(0.015f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Male,                            new BodyTypeInfo(0.035f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

            { RimRound.Defs.BodyTypeDefOf.M_005vp_Thick_090,            new BodyTypeInfo(0.050f, 0.6875f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimRound.Defs.BodyTypeDefOf.M_006vp_Chonky_090,           new BodyTypeInfo(0.065f, 0.9375f, 0.90f, 1.28205f, 0.30f, 0.30f) },
            { RimRound.Defs.BodyTypeDefOf.M_010vp_Chubby_090,           new BodyTypeInfo(0.090f, 0.9375f, 0.80f, 1.28205f, 0.30f, 0.35f) },
            { RimRound.Defs.BodyTypeDefOf.M_020vp_Corpulent_090,        new BodyTypeInfo(0.120f, 0.9375f, 0.65f, 0.98205f, 0.40f, 0.45f) },
            { RimRound.Defs.BodyTypeDefOf.M_030vp_Fat_090,              new BodyTypeInfo(0.155f, 1.000f, 0.50f, 0.88205f, 0.44f, 0.45f) },
            { RimRound.Defs.BodyTypeDefOf.M_040vp_Obese_090,            new BodyTypeInfo(0.200f, 1.125f, 0.40f, 0.77205f, 0.50f, 0.40f) },
            { RimRound.Defs.BodyTypeDefOf.M_050vp_MorbidlyObese_090,    new BodyTypeInfo(0.280f, 1.125f, 0.30f, 0.65205f, 0.70f, 0.50f) },
            { RimRound.Defs.BodyTypeDefOf.M_060vp_Lardy_090,            new BodyTypeInfo(0.430f, 2.125f, 0.20f, 0.58205f, 1.00f, 0.70f) },
            { RimRound.Defs.BodyTypeDefOf.M_070vp_Enormous_090,         new BodyTypeInfo(0.660f, 2.000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
            { RimRound.Defs.BodyTypeDefOf.M_080vp_Gigantic_090,         new BodyTypeInfo(0.965f, 3.875f, 0.10f, 0.38205f, 1.10f, 0.65f) },
            { RimRound.Defs.BodyTypeDefOf.M_090vp_Titanic_090,          new BodyTypeInfo(1.410f, 3.875f, 0.10f, 0.37205f, 1.10f, 0.79f) },
            { RimRound.Defs.BodyTypeDefOf.M_100vp_Gelatinous_090,       new BodyTypeInfo(1.860f, 5.000f, 0.05f, 0.22205f, 2.52f, 0.00f) },


            { RimRound.Defs.BodyTypeDefOf.M_150vp_Gelatinous_090,    new BodyTypeInfo(2.460f, 5.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_200vp_Gelatinous_090,    new BodyTypeInfo(2.960f, 6.2500f, 0.05f, 0.20205f, 2.60f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_250vp_Gelatinous_090,    new BodyTypeInfo(3.960f, 7.0000f, 0.05f, 0.17000f, 2.80f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_300vp_Gelatinous_090,    new BodyTypeInfo(4.960f, 7.7500f, 0.05f, 0.14000f, 3.60f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_350vp_Gelatinous_090,    new BodyTypeInfo(6.460f, 8.5000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_400vp_Gelatinous_090,    new BodyTypeInfo(7.960f, 9.5000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_450vp_Gelatinous_090,    new BodyTypeInfo(9.960f, 11.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_500vp_Gelatinous_090,    new BodyTypeInfo(14.46f, 13.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_900vp_Gelatinous_090,    new BodyTypeInfo(21.85f, 17.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

            { RimRound.Defs.BodyTypeDefOf.M_910vp_Gelatinous_090,    new BodyTypeInfo(42.40f  , 20.000f, 0.05f, 0.09000f, 6.31f, 0.0f) },
            { RimRound.Defs.BodyTypeDefOf.M_920vp_Gelatinous_090,    new BodyTypeInfo(70.50f  , 25.000f, 0.05f, 0.07000f, 7.11f, 0.0f) },
            { RimRound.Defs.BodyTypeDefOf.M_930vp_Gelatinous_090,    new BodyTypeInfo(116.6f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_940vp_Gelatinous_090,    new BodyTypeInfo(164.5f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_950vp_Gelatinous_090,    new BodyTypeInfo(286.2f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_960vp_Gelatinous_090,    new BodyTypeInfo(411.0f ,  70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_970vp_Gelatinous_090,    new BodyTypeInfo(576.7f ,  85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_980vp_Gelatinous_090,    new BodyTypeInfo(773.2f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_990vp_Gelatinous_090,    new BodyTypeInfo(999.9f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_995vp_Gelatinous_090,    new BodyTypeInfo(10000f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },

        };

        public static Dictionary<BodyTypeDef, BodyTypeInfo> meatSlopMaleSet090 = new Dictionary<BodyTypeDef, BodyTypeInfo>()
        {
            { RimWorld.BodyTypeDefOf.Fat,                             new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Hulk,                            new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Female,                          new BodyTypeInfo(-1    , 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

            { RimWorld.BodyTypeDefOf.Thin,                            new BodyTypeInfo(0.015f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimWorld.BodyTypeDefOf.Male,                            new BodyTypeInfo(0.035f, 0.9000f, 1.00f, 1.28205f, 0.30f, 0.30f) },

            { RimRound.Defs.BodyTypeDefOf.M_005ms_Thick_090,            new BodyTypeInfo(0.050f, 0.6875f, 1.00f, 1.28205f, 0.30f, 0.30f) },
            { RimRound.Defs.BodyTypeDefOf.M_006ms_Chonky_090,           new BodyTypeInfo(0.065f, 0.9375f, 0.90f, 1.28205f, 0.30f, 0.30f) },
            { RimRound.Defs.BodyTypeDefOf.M_010ms_Chubby_090,           new BodyTypeInfo(0.090f, 0.9375f, 0.80f, 1.28205f, 0.30f, 0.35f) },
            { RimRound.Defs.BodyTypeDefOf.M_020ms_Corpulent_090,        new BodyTypeInfo(0.120f, 0.9375f, 0.65f, 0.98205f, 0.40f, 0.45f) },
            { RimRound.Defs.BodyTypeDefOf.M_030ms_Fat_090,              new BodyTypeInfo(0.155f, 1.000f, 0.50f, 0.88205f, 0.44f, 0.45f) },
            { RimRound.Defs.BodyTypeDefOf.M_040ms_Obese_090,            new BodyTypeInfo(0.200f, 1.125f, 0.40f, 0.77205f, 0.50f, 0.40f) },
            { RimRound.Defs.BodyTypeDefOf.M_050ms_MorbidlyObese_090,    new BodyTypeInfo(0.280f, 1.125f, 0.30f, 0.65205f, 0.70f, 0.50f) },
            { RimRound.Defs.BodyTypeDefOf.M_060ms_Lardy_090,            new BodyTypeInfo(0.430f, 2.125f, 0.20f, 0.58205f, 1.00f, 0.70f) },
            { RimRound.Defs.BodyTypeDefOf.M_070ms_Enormous_090,         new BodyTypeInfo(0.660f, 2.000f, 0.10f, 0.48205f, 0.93f, 0.50f) },
            { RimRound.Defs.BodyTypeDefOf.M_080ms_Gigantic_090,         new BodyTypeInfo(0.965f, 3.875f, 0.10f, 0.38205f, 1.10f, 0.65f) },
            { RimRound.Defs.BodyTypeDefOf.M_090ms_Titanic_090,          new BodyTypeInfo(1.410f, 3.875f, 0.10f, 0.37205f, 1.10f, 0.79f) },
            { RimRound.Defs.BodyTypeDefOf.M_100ms_Gelatinous_090,       new BodyTypeInfo(1.860f, 5.000f, 0.05f, 0.22205f, 2.52f, 0.00f) },


            { RimRound.Defs.BodyTypeDefOf.M_150ms_Gelatinous_090,    new BodyTypeInfo(2.460f, 5.5000f, 0.05f, 0.22205f, 2.52f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_200ms_Gelatinous_090,    new BodyTypeInfo(2.960f, 6.2500f, 0.05f, 0.20205f, 2.60f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_250ms_Gelatinous_090,    new BodyTypeInfo(3.960f, 7.0000f, 0.05f, 0.17000f, 2.80f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_300ms_Gelatinous_090,    new BodyTypeInfo(4.960f, 7.7500f, 0.05f, 0.14000f, 3.60f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_350ms_Gelatinous_090,    new BodyTypeInfo(6.460f, 8.5000f, 0.05f, 0.13000f, 3.90f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_400ms_Gelatinous_090,    new BodyTypeInfo(7.960f, 9.5000f, 0.05f, 0.11000f, 4.70f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_450ms_Gelatinous_090,    new BodyTypeInfo(9.960f, 11.000f, 0.05f, 0.10000f, 5.40f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_500ms_Gelatinous_090,    new BodyTypeInfo(14.46f, 13.000f, 0.05f, 0.09000f, 6.31f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_900ms_Gelatinous_090,    new BodyTypeInfo(21.85f, 17.000f, 0.05f, 0.07000f, 7.11f, 0.00f) },

            { RimRound.Defs.BodyTypeDefOf.M_910ms_Gelatinous_090,    new BodyTypeInfo(42.40f  , 20.000f, 0.05f, 0.09000f, 6.31f, 0.0f) },
            { RimRound.Defs.BodyTypeDefOf.M_920ms_Gelatinous_090,    new BodyTypeInfo(70.50f  , 25.000f, 0.05f, 0.07000f, 7.11f, 0.0f) },
            { RimRound.Defs.BodyTypeDefOf.M_930ms_Gelatinous_090,    new BodyTypeInfo(116.6f  , 30.000f, 0.05f, 0.06000f, 8.5f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_940ms_Gelatinous_090,    new BodyTypeInfo(164.5f  , 40.000f, 0.05f, 0.05100f, 12f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_950ms_Gelatinous_090,    new BodyTypeInfo(286.2f  , 55.000f, 0.05f, 0.03400f, 19f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_960ms_Gelatinous_090,    new BodyTypeInfo(411.0f ,  70.000f, 0.05f, 0.02800f, 19f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_970ms_Gelatinous_090,    new BodyTypeInfo(576.7f ,  85.000f, 0.05f, 0.02200f, 29f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_980ms_Gelatinous_090,    new BodyTypeInfo(773.2f , 105.000f, 0.05f, 0.01800f, 29f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_990ms_Gelatinous_090,    new BodyTypeInfo(999.9f , 125.000f, 0.05f, 0.01500f, 41f, 0.00f) },
            { RimRound.Defs.BodyTypeDefOf.M_995ms_Gelatinous_090,    new BodyTypeInfo(10000f , 150.000f, 0.05f, 0.01200f, 41f, 0.00f) },

        };


        #endregion

        #endregion

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

        static Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>> maloFemaleBodytypes = new Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>
        {
            { BodyArchetype.standard, RemoveKeysAndReturnClonedDictionary(bambooAppleFemaleSet, new List<BodyTypeDef>() { BodyTypeDefOf.Thin}) },
            { BodyArchetype.apple,    appleFemaleSetNoThin },
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

        static Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>> maloSet = new Dictionary<Gender, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>()
        {
            { Gender.Female, maloFemaleBodytypes },
            { Gender.Male,   maloFemaleBodytypes },
            { Gender.None,   maloFemaleBodytypes },
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
            RimRound.Defs.BodyTypeDefOf.M_005af_Thick,
            RimRound.Defs.BodyTypeDefOf.M_006af_Chonky,
            RimRound.Defs.BodyTypeDefOf.M_010af_Chubby,
            RimRound.Defs.BodyTypeDefOf.M_020af_Corpulent,
            RimRound.Defs.BodyTypeDefOf.M_030af_Fat,
            RimRound.Defs.BodyTypeDefOf.M_040af_Obese,
            RimRound.Defs.BodyTypeDefOf.M_050af_MorbidlyObese,
            RimRound.Defs.BodyTypeDefOf.M_060af_Lardy,
            RimRound.Defs.BodyTypeDefOf.M_070af_Enormous,
            RimRound.Defs.BodyTypeDefOf.M_080af_Gigantic,
            RimRound.Defs.BodyTypeDefOf.M_090af_Titanic,
            RimRound.Defs.BodyTypeDefOf.M_100af_Gelatinous,

            RimRound.Defs.BodyTypeDefOf.M_150af_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_200af_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_250af_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_300af_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_350af_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_400af_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_450af_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_500af_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_900af_Gelatinous,

            RimRound.Defs.BodyTypeDefOf.M_910af_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_920af_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_930af_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_940af_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_950af_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_960af_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_970af_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_980af_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_990af_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_995af_Gelatinous,



            RimRound.Defs.BodyTypeDefOf.M_005vp_Thick,
            RimRound.Defs.BodyTypeDefOf.M_006vp_Chonky,
            RimRound.Defs.BodyTypeDefOf.M_010vp_Chubby,
            RimRound.Defs.BodyTypeDefOf.M_020vp_Corpulent,
            RimRound.Defs.BodyTypeDefOf.M_030vp_Fat,
            RimRound.Defs.BodyTypeDefOf.M_040vp_Obese,
            RimRound.Defs.BodyTypeDefOf.M_050vp_MorbidlyObese,
            RimRound.Defs.BodyTypeDefOf.M_060vp_Lardy,
            RimRound.Defs.BodyTypeDefOf.M_070vp_Enormous,
            RimRound.Defs.BodyTypeDefOf.M_080vp_Gigantic,
            RimRound.Defs.BodyTypeDefOf.M_090vp_Titanic,
            RimRound.Defs.BodyTypeDefOf.M_100vp_Gelatinous,

            RimRound.Defs.BodyTypeDefOf.M_150vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_200vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_250vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_300vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_350vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_400vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_450vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_500vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_900vp_Gelatinous,

            RimRound.Defs.BodyTypeDefOf.M_910vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_920vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_930vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_940vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_950vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_960vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_970vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_980vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_990vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_995vp_Gelatinous,




            RimRound.Defs.BodyTypeDefOf.M_005gr_Thick,
            RimRound.Defs.BodyTypeDefOf.M_006gr_Chonky,
            RimRound.Defs.BodyTypeDefOf.M_010gr_Chubby,
            RimRound.Defs.BodyTypeDefOf.M_020gr_Corpulent,
            RimRound.Defs.BodyTypeDefOf.M_030gr_Fat,
            RimRound.Defs.BodyTypeDefOf.M_040gr_Obese,
            RimRound.Defs.BodyTypeDefOf.M_050gr_MorbidlyObese,
            RimRound.Defs.BodyTypeDefOf.M_060gr_Lardy,
            RimRound.Defs.BodyTypeDefOf.M_070gr_Enormous,
            RimRound.Defs.BodyTypeDefOf.M_080gr_Gigantic,
            RimRound.Defs.BodyTypeDefOf.M_090gr_Titanic,
            RimRound.Defs.BodyTypeDefOf.M_100gr_Gelatinous,

            RimRound.Defs.BodyTypeDefOf.M_150gr_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_200gr_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_250gr_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_300gr_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_350gr_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_400gr_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_450gr_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_500gr_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_900gr_Gelatinous,

            RimRound.Defs.BodyTypeDefOf.M_910gr_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_920gr_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_930gr_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_940gr_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_950gr_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_960gr_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_970gr_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_980gr_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_990gr_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_995gr_Gelatinous,



            RimRound.Defs.BodyTypeDefOf.M_005ms_Thick,
            RimRound.Defs.BodyTypeDefOf.M_006ms_Chonky,
            RimRound.Defs.BodyTypeDefOf.M_010ms_Chubby,
            RimRound.Defs.BodyTypeDefOf.M_020ms_Corpulent,
            RimRound.Defs.BodyTypeDefOf.M_030ms_Fat,
            RimRound.Defs.BodyTypeDefOf.M_040ms_Obese,
            RimRound.Defs.BodyTypeDefOf.M_050ms_MorbidlyObese,
            RimRound.Defs.BodyTypeDefOf.M_060ms_Lardy,
            RimRound.Defs.BodyTypeDefOf.M_070ms_Enormous,
            RimRound.Defs.BodyTypeDefOf.M_080ms_Gigantic,
            RimRound.Defs.BodyTypeDefOf.M_090ms_Titanic,
            RimRound.Defs.BodyTypeDefOf.M_100ms_Gelatinous,

            RimRound.Defs.BodyTypeDefOf.M_150ms_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_200ms_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_250ms_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_300ms_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_350ms_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_400ms_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_450ms_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_500ms_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_900ms_Gelatinous,

            RimRound.Defs.BodyTypeDefOf.M_910ms_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_920ms_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_930ms_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_940ms_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_950ms_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_960ms_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_970ms_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_980ms_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_990ms_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.M_995ms_Gelatinous,


            RimRound.Defs.BodyTypeDefOf.F_005bs_Thick,
            RimRound.Defs.BodyTypeDefOf.F_006bs_Chonky,
            RimRound.Defs.BodyTypeDefOf.F_010bs_Chubby,
            RimRound.Defs.BodyTypeDefOf.F_020bs_Corpulent,
            RimRound.Defs.BodyTypeDefOf.F_030bs_Fat,
            RimRound.Defs.BodyTypeDefOf.F_040bs_Obese,
            RimRound.Defs.BodyTypeDefOf.F_050bs_MorbidlyObese,
            RimRound.Defs.BodyTypeDefOf.F_060bs_Lardy,
            RimRound.Defs.BodyTypeDefOf.F_070bs_Enormous,
            RimRound.Defs.BodyTypeDefOf.F_080bs_Gigantic,
            RimRound.Defs.BodyTypeDefOf.F_090bs_Titanic,
            RimRound.Defs.BodyTypeDefOf.F_100bs_Gelatinous, // Gel I

            RimRound.Defs.BodyTypeDefOf.F_150bs_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_200bs_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_250bs_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_300bs_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_350bs_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_400bs_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_450bs_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_500bs_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_900bs_Gelatinous, // Gel X

            RimRound.Defs.BodyTypeDefOf.F_910bs_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_920bs_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_930bs_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_940bs_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_950bs_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_960bs_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_970bs_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_980bs_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_990bs_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_995bs_Gelatinous, // Gel XX


            RimRound.Defs.BodyTypeDefOf.F_005ba_Thick,
            RimRound.Defs.BodyTypeDefOf.F_006ba_Chonky,
            RimRound.Defs.BodyTypeDefOf.F_010ba_Chubby,
            RimRound.Defs.BodyTypeDefOf.F_020ba_Corpulent,
            RimRound.Defs.BodyTypeDefOf.F_030ba_Fat,
            RimRound.Defs.BodyTypeDefOf.F_040ba_Obese,
            RimRound.Defs.BodyTypeDefOf.F_050ba_MorbidlyObese,
            RimRound.Defs.BodyTypeDefOf.F_060ba_Lardy,
            RimRound.Defs.BodyTypeDefOf.F_070ba_Enormous,
            RimRound.Defs.BodyTypeDefOf.F_080ba_Gigantic,
            RimRound.Defs.BodyTypeDefOf.F_090ba_Titanic,
            RimRound.Defs.BodyTypeDefOf.F_100ba_Gelatinous, // Gel I

            RimRound.Defs.BodyTypeDefOf.F_150ba_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_200ba_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_250ba_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_300ba_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_350ba_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_400ba_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_450ba_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_500ba_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_900ba_Gelatinous, // Gel X

            RimRound.Defs.BodyTypeDefOf.F_910ba_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_920ba_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_930ba_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_940ba_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_950ba_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_960ba_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_970ba_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_980ba_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_990ba_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_995ba_Gelatinous, // Gel XX




            RimRound.Defs.BodyTypeDefOf.F_005vp_Thick,
            RimRound.Defs.BodyTypeDefOf.F_006vp_Chonky,
            RimRound.Defs.BodyTypeDefOf.F_010vp_Chubby,
            RimRound.Defs.BodyTypeDefOf.F_020vp_Corpulent,
            RimRound.Defs.BodyTypeDefOf.F_030vp_Fat,
            RimRound.Defs.BodyTypeDefOf.F_040vp_Obese,
            RimRound.Defs.BodyTypeDefOf.F_050vp_MorbidlyObese,
            RimRound.Defs.BodyTypeDefOf.F_060vp_Lardy,
            RimRound.Defs.BodyTypeDefOf.F_070vp_Enormous,
            RimRound.Defs.BodyTypeDefOf.F_080vp_Gigantic,
            RimRound.Defs.BodyTypeDefOf.F_090vp_Titanic,
            RimRound.Defs.BodyTypeDefOf.F_100vp_Gelatinous, // Gel I

            RimRound.Defs.BodyTypeDefOf.F_150vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_200vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_250vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_300vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_350vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_400vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_450vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_500vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_900vp_Gelatinous, // Gel X

            RimRound.Defs.BodyTypeDefOf.F_910vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_920vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_930vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_940vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_950vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_960vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_970vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_980vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_990vp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_995vp_Gelatinous, // Gel XX


            RimRound.Defs.BodyTypeDefOf.F_005tp_Thick,
            RimRound.Defs.BodyTypeDefOf.F_006tp_Chonky,
            RimRound.Defs.BodyTypeDefOf.F_010tp_Chubby,
            RimRound.Defs.BodyTypeDefOf.F_020tp_Corpulent,
            RimRound.Defs.BodyTypeDefOf.F_030tp_Fat,
            RimRound.Defs.BodyTypeDefOf.F_040tp_Obese,
            RimRound.Defs.BodyTypeDefOf.F_050tp_MorbidlyObese,
            RimRound.Defs.BodyTypeDefOf.F_060tp_Lardy,
            RimRound.Defs.BodyTypeDefOf.F_070tp_Enormous,
            RimRound.Defs.BodyTypeDefOf.F_080tp_Gigantic,
            RimRound.Defs.BodyTypeDefOf.F_090tp_Titanic,
            RimRound.Defs.BodyTypeDefOf.F_100tp_Gelatinous, // Gel I

            RimRound.Defs.BodyTypeDefOf.F_150tp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_200tp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_250tp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_300tp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_350tp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_400tp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_450tp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_500tp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_900tp_Gelatinous, // Gel X

            RimRound.Defs.BodyTypeDefOf.F_910tp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_920tp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_930tp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_940tp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_950tp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_960tp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_970tp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_980tp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_990tp_Gelatinous,
            RimRound.Defs.BodyTypeDefOf.F_995tp_Gelatinous, // Gel XX

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
