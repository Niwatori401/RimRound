using AlienRace;
using AlienRace.ExtendedGraphics;
using RimRound.Comps;
using RimWorld;
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

        public static string GetProperBodyGraphicPathFromPawn(Pawn pawn) 
        {
            return ConvertBodyPathStringsIfNecessary("Things/Pawn/Humanlike/Bodies/Naked_" + pawn.story.bodyType.defName);
        }

        public static string ConvertBodyPathStringsIfNecessary(string originalBodyPath)
        {
            int lastSlash = originalBodyPath.LastIndexOf('/');

            string basePath = originalBodyPath.Substring(0, lastSlash + 1);
            string bodyTypeName = originalBodyPath.Substring(lastSlash + 1);

            if (!IsCustomBody(bodyTypeName))
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

            return IsCustomBody(p.story.bodyType);
        }

        public static bool IsCustomBody(BodyTypeDef bodyTypeDef) 
        {
            return IsCustomBody(bodyTypeDef.defName);
        }

        public static bool IsCustomBody(string bodyTypeDefString)
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

        public static BodyTypeDef GetBodyTypeBasedOnWeightSeverity(Pawn pawn, bool personallyExempt = false, bool categoricallyExempt = false)
        {
            //Dictionary<BodyTypeDef, float> bTD
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
                return bodyTypeDictionary.First().Key;

            if (weightSeverity >= bodyTypeDictionary.Last().Value.maxSeverity * weightRequirementMultiplier)
                return bodyTypeDictionary.Last().Key;


            foreach (var x in bodyTypeDictionary)
            {
                if (weightSeverity <= x.Value.maxSeverity * weightRequirementMultiplier)
                    return x.Key;
            }

            return bodyTypeDictionary.First().Key;
        }

        public static void UpdatePawnSprite(Pawn pawn, bool personallyExempt = false, bool categoricallyExempt = false, bool forceUpdate = false, bool BodyCheck = true)
        {
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

        internal static void RedrawPawn(Pawn pawn)
        {
            PortraitsCache.SetDirty(pawn);
            GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(pawn);
            pawn.Drawer.renderer.graphics.ResolveAllGraphics();
        }

        internal static void UpdatePawnDrawSize(Pawn pawn, bool personallyExempt = false, bool categoricallyExempt = false)
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


                //LifeStageAge lifeStageAge = alienProps.race.lifeStageAges;
                //LifeStageAgeAlien lifeStageAgeAlien = lifeStageAge as LifeStageAgeAlien;
            }
        }

        internal static bool CheckExemptions(Pawn p) 
        {
            if ((GlobalSettings.bodyChangeFemale is false && p.gender is Gender.Female) ||
            (GlobalSettings.bodyChangeMale is false && p.gender is Gender.Male) ||
            (GlobalSettings.bodyChangeFriendlyNPC is false && p.Faction != Faction.OfPlayer && p.Faction.AllyOrNeutralTo(Faction.OfPlayer)) ||
            (GlobalSettings.bodyChangeHostileNPC is false && p.Faction.HostileTo(Faction.OfPlayer) && !p.IsPrisoner) ||
            (GlobalSettings.bodyChangePrisoners is false && p.IsPrisoner) ||
            (GlobalSettings.bodyChangeSlaves is false && p.IsSlaveOfColony)
        )
            {
                return true;
            }
            else
            {
                return false;
            }
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
            foreach (Map m in Find.Maps)
            {
                foreach (Pawn p in m.mapPawns.AllPawns)
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
            foreach (Map m in Find.Maps)
            {
                foreach (Pawn p in m.mapPawns.AllPawns)
                {
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
