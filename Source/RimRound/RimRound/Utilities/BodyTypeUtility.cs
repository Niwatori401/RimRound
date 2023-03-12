using AlienRace;
using AlienRace.ExtendedGraphics;
using RimRound.Comps;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.Utilities
{
    public static class BodyTypeUtility
    {
        public static void RefreshBodyTypeGraphicLocations() 
        {
            using (List<ThingDef_AlienRace>.Enumerator enumerator = DefDatabase<ThingDef_AlienRace>.AllDefsListForReading.GetEnumerator())
            {
                IGraphicsLoader graphicsLoader = new DefaultGraphicsLoader();
                while (enumerator.MoveNext())
                {
                    ThingDef_AlienRace ar = enumerator.Current;
                    AlienPartGenerator.ExtendedGraphicTop body = ar.alienRace.graphicPaths.body;

                    for (int i = 0; i < body.bodytypeGraphics.Count; i++)
                    {
                        body.bodytypeGraphics[i].path = BodyTypeUtility.ConvertBodyPathStringsIfNecessary(body.bodytypeGraphics[i].path);
                    }
                    graphicsLoader.LoadAllGraphics(ar.alienRace.generalSettings.alienPartGenerator.alienProps.defName, new AlienPartGenerator.ExtendedGraphicTop[]
                    {
                        ar.alienRace.graphicPaths.head,
                        ar.alienRace.graphicPaths.body,
                        ar.alienRace.graphicPaths.skeleton,
                        ar.alienRace.graphicPaths.skull,
                        ar.alienRace.graphicPaths.stump,
                        ar.alienRace.graphicPaths.bodyMasks,
                        ar.alienRace.graphicPaths.headMasks
                    });
                }
            }
        }

        public static bool PawnIsOverWeightThreshold(Pawn pawn, BodyTypeDef bodyType)
        {
            float maxValueForBodyType = 0;
            if (RacialBodyTypeInfoUtility.defaultFemaleSet.ContainsKey(bodyType))
                maxValueForBodyType = RacialBodyTypeInfoUtility.defaultFemaleSet[bodyType].maxSeverity;
            else
                Log.Error("Only use default female bodytypedefs in PawnIsOverWeightThreshold!");

            if (Utilities.HediffUtility.WeightHediff(pawn).Severity > maxValueForBodyType * RacialBodyTypeInfoUtility.GetBodyTypeWeightRequirementMultiplier(pawn))
            {
                return true;
            }

            return false;
        }


        public static string GetProperBodyGraphicPathFromPawn(Pawn pawn) 
        {
            string basePath = "Things/Pawn/Humanlike/Bodies/";

            if (pawn.def is AlienRace.ThingDef_AlienRace alienRace && 
                alienRace.alienRace.graphicPaths.body.path is String alienBodyPath && 
                alienBodyPath != basePath)
            {
                if (!IsRRBody(pawn.story.bodyType))
                    return alienBodyPath + "Naked_" + pawn.story.bodyType.defName;
            }

            return ConvertBodyPathStringsIfNecessary(basePath +"Naked_" + pawn.story.bodyType.defName);
        }

        public static string ConvertBodyPathStringsIfNecessary(string originalBodyPath)
        {
            int lastSlash = originalBodyPath.LastIndexOf('/');

            string basePath = originalBodyPath.Substring(0, lastSlash + 1);
            string bodyTypeName = originalBodyPath.Substring(lastSlash + 1);

            if (!IsRRBody(bodyTypeName))
            {
                return originalBodyPath;
            }

            if (bodyTypeName.Contains("Naked_"))
                bodyTypeName = bodyTypeName.Substring(bodyTypeName.IndexOf("Naked_") + 6);

            if (bodyTypeName == "F_060_LardyAlt")
                bodyTypeName = "F_060_Lardy";


            bodyTypeName = RacialBodyTypeInfoUtility.GetEquivalentBodyTypeDef(DefDatabase<BodyTypeDef>.GetNamed(bodyTypeName)).ToString();
            bodyTypeName = ConvertBodyTypeDefDefnameAccordingToSettings(bodyTypeName);

            return basePath + "Naked_" + bodyTypeName;
        }


        public static bool HasCustomBody(Pawn p)
        {
            if (p?.story?.bodyType is null)
                return false;

            return IsRRBody(p.story.bodyType);
        }

        public static bool IsRRBody(BodyTypeDef bodyTypeDef) 
        {
            return IsRRBody(bodyTypeDef.defName);
        }

        public static bool IsRRBody(string bodyTypeDefString)
        {
            string pattern = @"[fmFM]{1}_+[0-9]{3,}?a*_+[A-Za-z]+";

            Regex regex = new Regex(pattern);

            if (regex.IsMatch(bodyTypeDefString))
                return true;

            return false;
        }


        public static string ConvertBodyTypeDefDefnameAccordingToSettings(string bodytypeCleaned)
        {
            if (GlobalSettings.onlyUseStandardBodyType && Regex.IsMatch(bodytypeCleaned, "[0-9]{3}a"))
            {
                bodytypeCleaned = Regex.Replace(bodytypeCleaned, "a_", "_");
            }


            if (bodytypeCleaned == Defs.BodyTypeDefOf.F_060_Lardy.defName && !GlobalSettings.useOldLardySprite)
            {
                bodytypeCleaned += "Alt";
            }

            if (Regex.IsMatch(bodytypeCleaned, "Gelatinous"))
            {
                bodytypeCleaned = Regex.Replace(bodytypeCleaned, "[0-9]{3}", "100");
            }
            return bodytypeCleaned;
        }

        static Dictionary<Pawn, Corpse> cachedCorpseContainingPawnResults = new Dictionary<Pawn, Corpse>();

        public static void InvalidateCorpseCache() 
        {
            cachedCorpseContainingPawnResults.Clear();        
        }

        /// <summary>
        /// Validates result from cache.
        /// </summary>
        /// <param name="pawn">Pawn returned from cache</param>
        /// <param name="corpse">Corpse returned from cache</param>
        /// <returns>true of cache needs wiped, false otherwise.</returns>
        static bool CorpseCacheIsStale(Pawn pawn, Corpse corpse) 
        {
            if (corpse.InnerPawn.ThingID != pawn.ThingID)
                return true;

            return false;
        }

        public static Corpse GetCorpseContainingPawn(Pawn pawn) 
        {
            if (!pawn.Dead)
            {
                return null;
            }

            Corpse corpse = null;
            if (cachedCorpseContainingPawnResults.TryGetValue(pawn, out corpse))
            {
                return corpse;
            }
            else 
            {
                List<Corpse> allCorpses = new List<Corpse>();
                ThingOwnerUtility.GetAllThingsRecursively<Corpse>(Find.CurrentMap, ThingRequest.ForGroup(ThingRequestGroup.Corpse), allCorpses, true, null, true);

                corpse = (allCorpses.Where(
                    delegate (Corpse c)
                    {
                        if (!(c.InnerPawn?.RaceProps?.Humanlike is bool b && b))
                        return false;

                        return c.InnerPawn.ThingID == pawn.ThingID;
                    }
                    ))?.FirstOrDefault();

                cachedCorpseContainingPawnResults.Add(pawn, corpse);
            }

            if (corpse is null)
            {
                Log.Warning($"Corpse was null in {nameof(BodyTypeUtility.GetCorpseContainingPawn)}.");
                return null;
            }

            if (CorpseCacheIsStale(pawn, corpse))
            {
                InvalidateCorpseCache();
                return GetCorpseContainingPawn(pawn);
            }
            else
                return corpse;
                
        }

        public static BodyTypeDef GetBodyTypeBasedOnWeightSeverity(Pawn pawn, bool personallyExempt = false, bool categoricallyExempt = false)
        {
            BodyTypeDef result = null;

            bool dessicated = GetCorpseContainingPawn(pawn) is Corpse c && c.IsDessicated();

            if (pawn.Dead && dessicated)
                return RimWorld.BodyTypeDefOf.Thin;

            if (personallyExempt || categoricallyExempt)
            {
                return pawn.TryGetComp<FullnessAndDietStats_ThingComp>().DefaultBodyType ??
                    RimWorld.BodyTypeDefOf.Thin;
            }


            Dictionary<BodyTypeDef, BodyTypeInfo> bodyTypeDictionary = RacialBodyTypeInfoUtility.GetRacialDictionary(pawn);


            if (bodyTypeDictionary is null)
                return pawn.story.bodyType;

            float weightSeverity = Utilities.HediffUtility.GetHediffOfDefFrom(Defs.HediffDefOf.RimRound_Weight, pawn)?.Severity ?? -1;
            if (weightSeverity == -1)
                return pawn.story.bodyType;

            float weightRequirementMultiplier = RacialBodyTypeInfoUtility.GetBodyTypeWeightRequirementMultiplier(pawn);

            //Edge cases
            if (weightSeverity == 0)
                result = bodyTypeDictionary.First().Key;

            if (weightSeverity >= bodyTypeDictionary.Last().Value.maxSeverity * weightRequirementMultiplier)
                result = bodyTypeDictionary.Last().Key;


            foreach (var x in bodyTypeDictionary)
            {
                if (weightSeverity <= x.Value.maxSeverity * weightRequirementMultiplier)
                {
                    result = x.Key;
                    break;
                }
            }

            if (result is null)
                result = bodyTypeDictionary.First().Key;

            if (!BodyTypeUtility.IsRRBody(result))
                return result;

            int chosenBodyTypeNumber;
            if (!int.TryParse(Regex.Match(result.defName, "[FM]_[0-9]{3}").Value.Substring(2), out chosenBodyTypeNumber)) 
            {
                Log.Error("Failed to get body number from defName.");
                chosenBodyTypeNumber = 0;
            }

            if (chosenBodyTypeNumber < 100 || GlobalSettings.maxVisualSizeGelLevel.threshold == 20)
                return result;


            int maxSizeNumber = RacialBodyTypeInfoUtility.gelatinousLevelToCode[GlobalSettings.maxVisualSizeGelLevel.threshold];

            if (maxSizeNumber < chosenBodyTypeNumber)
            {
                string resultString = result.defName;

                if (Regex.IsMatch(result.defName, "F_[0-9]{3}"))
                    resultString = Regex.Replace(result.defName, "F_[0-9]{3}", $"F_{maxSizeNumber}");
                else if (Regex.IsMatch(result.defName, "M_[0-9]{3}"))
                    resultString = Regex.Replace(result.defName, "M_[0-9]{3}", $"M_{maxSizeNumber}");

                result = DefDatabase<BodyTypeDef>.GetNamed(resultString);
            }


            return result;
        }

        public static void UpdatePawnSprite(Pawn pawn, bool personallyExempt = false, bool categoricallyExempt = false, bool forceUpdate = false, bool BodyCheck = true)
        {
            if (!(pawn?.RaceProps?.Humanlike is bool isHumanlike && isHumanlike))
                return;

            var comp = pawn.TryGetComp<PawnBodyType_ThingComp>();

            if (comp is null)
                return;

            comp.ticksSinceLastBodyChange = 0;

            if (BodyCheck && BodyTypeUtility.GetBodyTypeBasedOnWeightSeverity(pawn, personallyExempt, categoricallyExempt) is BodyTypeDef b && b != pawn.story.bodyType)
            {
                pawn.story.bodyType = b;
                RedrawPawn(pawn);
                return;
            }
            else if (forceUpdate)
            {
                RedrawPawn(pawn);
                return;
            }
        }

        private static bool ValidatePawnShouldBeRedrawn(Pawn pawn)
        {
            List<Pawn> allPawns = null;
            if (Find.CurrentMap is null)
                allPawns = Find.GameInitData.startingAndOptionalPawns;
            else
                allPawns = new List<Pawn>(Find.CurrentMap?.mapPawns?.AllPawns);

            if (allPawns is null)
                return false;

            if (!(allPawns.Where(delegate (Pawn p) { return p.ThingID == pawn.ThingID; }).Any()))
            {
                if (GetCorpseContainingPawn(pawn) is Corpse c)
                    return true;

                return false;
            }

            return true;

        }

        internal static void RedrawPawn(Pawn pawn)
        {
            if (!ValidatePawnShouldBeRedrawn(pawn))
                return;
            
            PortraitsCache.SetDirty(pawn);
            GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(pawn);  
            pawn?.Drawer?.renderer?.graphics?.ResolveAllGraphics();
        }

        public static void UpdatePawnDrawSize(Pawn pawn, bool personallyExempt = false, bool categoricallyExempt = false)
        {

            if (pawn.def is ThingDef_AlienRace alienProps)
            {

                float drawSize;

                BodyTypeInfo? bodyTypeInfo = RacialBodyTypeInfoUtility.GetRacialBodyTypeInfo(pawn);
                if (bodyTypeInfo is null)
                    drawSize = 1;
                else
                    drawSize = bodyTypeInfo.AsNonNullable().meshSize;

                var alienComp = pawn.TryGetComp<AlienPartGenerator.AlienComp>();
                if (alienComp is null)
                {
                    Debug.LogWarning("AlienComp was null in update pawn drawsize!");
                }

                alienComp.customDrawSize =  new Vector2(drawSize, drawSize);

            }
        }


        /// <summary>
        /// Checks a pawn against settings in the RR tab for if they should use RimRound custom bodies.
        /// </summary>
        /// <param name="p">The pawn to check for eligibility.</param>
        /// <returns>true if they should be exempt, false otherwise.</returns>
        public static ExemptionReason CheckExemptions(Pawn p) 
        {
            if (GlobalSettings.bodyChangeFemale is false && p.gender is Gender.Female)
                return new ExemptionReason("RR_ExemptionReason_FemaleDisabled".Translate());
            else if (GlobalSettings.bodyChangeMale is false && p.gender is Gender.Male)
                return new ExemptionReason("RR_ExemptionReason_MaleDisabled".Translate());
            else if (GlobalSettings.bodyChangeFriendlyNPC is false && p.Faction != Faction.OfPlayer && p.Faction.AllyOrNeutralTo(Faction.OfPlayer))
                return new ExemptionReason("RR_ExemptionReason_FriendlyDisabled".Translate());
            else if (GlobalSettings.bodyChangeHostileNPC is false && p.Faction.HostileTo(Faction.OfPlayer) && !p.IsPrisoner)
                return new ExemptionReason("RR_ExemptionReason_HostileDisabled".Translate());
            else if (GlobalSettings.bodyChangePrisoners is false && p.IsPrisoner)
                return new ExemptionReason("RR_ExemptionReason_PrisonerDisabled".Translate());
            else if (GlobalSettings.bodyChangeSlaves is false && p.IsSlaveOfColony)
                return new ExemptionReason("RR_ExemptionReason_SlaveDisabled".Translate());
            else if (GlobalSettings.minimumAgeForCustomBody.threshold > (p?.ageTracker?.AgeBiologicalYears ?? 0))
                return new ExemptionReason("RR_ExemptionReason_AgeDisabled".Translate());
            else
                return false;

        }


        internal static void AssignPersonalCategoricalExemptions(PawnBodyType_ThingComp comp) 
        {
            if (comp is null)
                return;

            if (!comp.parent?.AsPawn().RaceProps.Humanlike ?? false)
                return;

            comp.CategoricallyExempt = CheckExemptions(comp.parent.AsPawn());
        }

        internal static void UpdateAllPawnSprites() 
        {
            List<Map> maps = Find.Maps.ToList();
            foreach (Map m in maps)
            {
                List<Pawn> pawnsOnMap = m.mapPawns.AllPawns.ToList();
                foreach (Pawn p in pawnsOnMap)
                {
                    PawnBodyType_ThingComp comp = p.TryGetComp<PawnBodyType_ThingComp>();

                    if (comp is null)
                        continue;

                    UpdatePawnSprite(p, comp.PersonallyExempt, comp.CategoricallyExempt, true, true);
                }
            }
        }


        internal static void AssignBodyTypeCategoricalExemptions(bool updatePawnSprite = false)
        {
            List<Map> maps = Find.Maps.ToList();
            foreach (Map m in maps)
            {
                List<Pawn> pawnsOnMap = m.mapPawns.AllPawns.ToList();
                foreach (Pawn p in pawnsOnMap)
                {
                    if (!m.mapPawns.AllPawns.Contains(p))
                        continue;

                    PawnBodyType_ThingComp comp = p.TryGetComp<PawnBodyType_ThingComp>();

                    if (comp is null)
                        continue;

                    AssignPersonalCategoricalExemptions(comp);

                    if (updatePawnSprite)
                        UpdatePawnSprite(p, comp.PersonallyExempt, comp.CategoricallyExempt, true, true);
                }
            }

            return;
        }

    }
}
