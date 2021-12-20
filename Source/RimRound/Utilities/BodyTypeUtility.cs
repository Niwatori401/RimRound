using AlienRace;
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
    public static class BodyTypeUtility
    {
        public static bool HasCustomBody(Pawn p)
        {
            if (p?.story?.bodyType is null)
                return false;

            foreach (BodyTypeDef bodyTypeDef in RacialBodyTypeInfoUtility.standardBodyTypeDefs)
            {
                if (RacialBodyTypeInfoUtility.GetEquivalentBodyTypeDef(p.story.bodyType).defName == bodyTypeDef.defName)
                {
                    return true;
                }
            }
            return false;
        }

        public static BodyTypeDef GetBodyTypeBasedOnWeightSeverity(Pawn pawn, bool personallyExempt = false, bool categoricallyExempt = false)
        {
            //Dictionary<BodyTypeDef, float> bTD
            if (personallyExempt || categoricallyExempt)
            {
                return pawn.TryGetComp<FullnessAndDietStats_ThingComp>().defaultBodyType ??
                    RimWorld.BodyTypeDefOf.Thin;
            }

            Dictionary<BodyTypeDef, BodyTypeInfo> bodyTypeDictionary = RacialBodyTypeInfoUtility.GetRacialDictionary(pawn);
            if (bodyTypeDictionary is null)
                return pawn.story.bodyType;

            float weightSeverity = Utilities.HediffUtility.GetHediffOfDefFrom(Defs.HediffDefOf.RimRound_Weight, pawn)?.Severity ?? -1;
            if (weightSeverity == -1)
                return pawn.story.bodyType;


            //Edge cases
            if (weightSeverity == 0)
                return bodyTypeDictionary.First().Key;

            if (weightSeverity >= bodyTypeDictionary.Last().Value.maxSeverity)
                return bodyTypeDictionary.Last().Key;


            foreach (var x in bodyTypeDictionary)
            {
                if (weightSeverity <= x.Value.maxSeverity)
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

        private static void RedrawPawn(Pawn pawn)
        {
            PortraitsCache.SetDirty(pawn);
            GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(pawn);
            pawn.Drawer.renderer.graphics.ResolveAllGraphics();
        }

        internal static void UpdatePawnDrawSize(Pawn pawn, bool personallyExempt = false, bool categoricallyExempt = false)
        {

            if (pawn.def is ThingDef_AlienRace alienProps)
            {
                GraphicPaths gp = alienProps.alienRace.graphicPaths.GetCurrentGraphicPath(pawn.ageTracker.CurLifeStage);

                float drawSize;

                BodyTypeInfo? bodyTypeInfo = RacialBodyTypeInfoUtility.GetRacialBodyTypeInfo(pawn);
                if (bodyTypeInfo is null)
                    drawSize = 1;
                else
                    drawSize = bodyTypeInfo.AsNonNullable().meshSize;


                gp.customDrawSize = new Vector2(drawSize, drawSize);
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

        internal static void AssignBodyTypeCategoricalExemptions(bool updatePawnSprite = false)
        {
            foreach (Map m in Find.Maps)
            {
                foreach (Pawn p in m.mapPawns.AllPawns)
                {
                    if (p is null || (!p?.RaceProps.Humanlike ?? false))
                        continue;

                    PawnBodyType_ThingComp comp = p.TryGetComp<PawnBodyType_ThingComp>();

                    if (comp is null)
                        continue;

                    comp.CategoricallyExempt = CheckExemptions(p);

                    if (updatePawnSprite)
                        UpdatePawnSprite(p, comp.PersonallyExempt, comp.CategoricallyExempt, true, true);
                }
            }

            return;
        }

    }
}
