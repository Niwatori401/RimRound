using AlienRace;
using RimRound.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Comps
{
    public class Debug_ThingComp : ThingComp
    {
        static public int paramNumber = 0;
        static public float magnification = 1f;

        PawnBodyType_ThingComp cachedPBTComp = null;


        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            cachedPBTComp = parent.TryGetComp<PawnBodyType_ThingComp>();
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (Prefs.DevMode)
            {
                yield return new Command_Action
                {
                    defaultLabel = $"Switch pawn body type to {(cachedPBTComp is null ? "other type" : (cachedPBTComp.BodyArchetype == BodyArchetype.standard ? BodyArchetype.apple.ToString() : BodyArchetype.standard.ToString()))}",
                    icon = Widgets.GetIconFor(RimWorld.ThingDefOf.Fire),
                    action = delegate ()
                    {
                        if (cachedPBTComp is null)
                        {
                            Log.Error("cachedPBTComp was null in debug action!");
                            return;
                        }

                        cachedPBTComp.BodyArchetype = cachedPBTComp.BodyArchetype == BodyArchetype.standard ? BodyArchetype.apple : BodyArchetype.standard;
                        BodyTypeUtility.UpdatePawnSprite((Pawn)parent, cachedPBTComp.PersonallyExempt, cachedPBTComp.CategoricallyExempt, true, true);

                    }
                };

                yield return new Command_Action
                {
                    defaultLabel = $"Force default body type to current",
                    icon = Widgets.GetIconFor(RimWorld.ThingDefOf.Muffalo),
                    action = delegate ()
                    {
                        FullnessAndDietStats_ThingComp comp = parent.AsPawn().TryGetComp<FullnessAndDietStats_ThingComp>();
                        
                        if (comp is null) 
                        {
                            return;
                        }
                        

                        comp.DefaultBodyType = parent.AsPawn().story.bodyType;
                        comp.defaultBodyTypeForced = true;
                    }
                };



                yield return new Command_Action
                {
                    defaultLabel = "Change magnitude",
                    icon = Widgets.GetIconFor(RimWorld.ThingDefOf.DropPodIncoming),
                    action = delegate ()
                    {
                        offsetAmountsIndex = (offsetAmountsIndex + 1) % offsetAmounts.Length;
                        Log.Message($"Magnitude is now {offsetAmounts[offsetAmountsIndex]}");
                    }
                };

                yield return new Command_Action
                {
                    defaultLabel = "Change direction",
                    icon = Widgets.GetIconFor(RimWorld.ThingDefOf.Ship_Reactor),
                    action = delegate ()
                    {
                        if (positivity == 1)
                            positivity = -1;
                        else
                            positivity = 1;
                    }
                };

                if (GlobalSettings.showSpecialDebugSettings)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "Change cardinality",
                        icon = Widgets.GetIconFor(RimWorld.ThingDefOf.Cow),
                        action = delegate ()
                        {
                            cardinality++;
                            cardinality %= 4;

                            switch (cardinality)
                            {
                                case 0:
                                    Log.Message("Offset direction: North");
                                    break;
                                case 1:
                                    Log.Message("Offset direction: East");
                                    break;
                                case 2:
                                    Log.Message("Offset direction: South");
                                    break;
                                case 3:
                                    Log.Message("Offset direction: West");
                                    break;
                            }
                        }
                    };

                    yield return new Command_Action
                    {
                        defaultLabel = "Change X/Y",
                        icon = Widgets.GetIconFor(RimWorld.ThingDefOf.Door),
                        action = delegate ()
                        {
                            xy = !xy;
                        }
                    };


                    yield return new Command_Action
                    {
                        defaultLabel = $"{(positivity == 1 ? "Add" : "Subtract")} ({offsetAmounts[offsetAmountsIndex]} to DEBUGFLOAT)",
                        icon = Widgets.GetIconFor(RimWorld.ThingDefOf.Campfire),
                        action = delegate ()
                        {
                            Values.debugPos += positivity * (offsetAmounts[offsetAmountsIndex]);
                            Log.Message($"Debug Float is now: {Values.debugPos}");
                            BodyTypeUtility.UpdatePawnSprite(parent.AsPawn(), false, false, true, false);
                        }
                    };
                    yield return new Command_Action
                    {
                        defaultLabel = $"{(positivity == 1 ? "Add" : "Subtract")} ({offsetAmounts[offsetAmountsIndex]} to DEBUGFLOAT2)",
                        icon = Widgets.GetIconFor(RimWorld.ThingDefOf.Campfire),
                        action = delegate ()
                        {
                            Values.debugPos2 += positivity * (offsetAmounts[offsetAmountsIndex]);
                            Log.Message($"Debug Float is now: {Values.debugPos2}");
                            BodyTypeUtility.UpdatePawnSprite(parent.AsPawn(), false, false, true, false);
                        }
                    };

                }
               

                yield return new Command_Action
                {
                    defaultLabel = $"{(positivity == 1 ? "Add" : "Subtract")} Severity ({offsetAmounts[offsetAmountsIndex]} kgs)",
                    action = delegate ()
                    {
                        PawnBodyType_ThingComp comp = ((Pawn)parent).TryGetComp<PawnBodyType_ThingComp>();

                        if (comp is null)
                            return;

                        Utilities.HediffUtility.AddHediffSeverity(Defs.HediffDefOf.RimRound_Weight, (Pawn)parent, positivity * (offsetAmounts[offsetAmountsIndex] / 1000f), false);
                        BodyTypeUtility.UpdatePawnSprite((Pawn)parent, comp.PersonallyExempt, comp.CategoricallyExempt, false, true);
                    }
                };

                if (GlobalSettings.showSpecialDebugSettings)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "Change Offset Item",
                        icon = Widgets.GetIconFor(RimWorld.ThingDefOf.Mote_ThoughtGood),
                        action = delegate ()
                        {
                            if (((Pawn)parent).def is ThingDef_AlienRace alienProps)
                            {
                                List<AlienPartGenerator.BodyAddon> bodyAddons = alienProps.alienRace.generalSettings.alienPartGenerator.bodyAddons;
                                if (!(bodyAddons is null) && bodyAddons.Count > 0)
                                {
                                    currentBodyPartAddonIndex += 1;
                                    currentBodyPartAddonIndex %= bodyAddons.Count;

                                    Log.Message($"New offset item path: {bodyAddons[currentBodyPartAddonIndex].path}");
                                }
                            }
                        }
                    };



                    yield return new Command_Action
                    {
                        defaultLabel = $"Change Extra Body Part Offset {(xy ? "Y" : "X")}{(positivity == 1 ? "+" : "-")} for {(cardinality == 0 ? "north" : (cardinality == 1 ? "east" : (cardinality == 2 ? "south" : "west")))} at {offsetAmounts[offsetAmountsIndex]}",
                        action = delegate ()
                        {
                            if (((Pawn)parent).def is ThingDef_AlienRace alienProps)
                            {
                                List<AlienPartGenerator.BodyAddon> bodyAddons = alienProps.alienRace.generalSettings.alienPartGenerator.bodyAddons;
                                if (!(bodyAddons is null) && bodyAddons.Count > 0)
                                {
                                    switch (cardinality)
                                    {
                                        case 0:
                                            UnityEngine.Vector2 oldOffsetNorth = bodyAddons[currentBodyPartAddonIndex].offsets.north.offset;
                                            bodyAddons[currentBodyPartAddonIndex].offsets.north.offset = new UnityEngine.Vector2(oldOffsetNorth.x + (xy ? 0 : positivity * offsetAmounts[offsetAmountsIndex]), oldOffsetNorth.y + (xy ? positivity * offsetAmounts[offsetAmountsIndex] : 0));
                                            Log.Message($"New offset: {bodyAddons[currentBodyPartAddonIndex].offsets.north.offset.ToString("F2")}");
                                            break;
                                        case 1:
                                            UnityEngine.Vector2 oldOffsetEast = bodyAddons[currentBodyPartAddonIndex].offsets.east.offset;
                                            bodyAddons[currentBodyPartAddonIndex].offsets.east.offset = new UnityEngine.Vector2(oldOffsetEast.x + (xy ? 0 : positivity * offsetAmounts[offsetAmountsIndex]), oldOffsetEast.y + (xy ? positivity * offsetAmounts[offsetAmountsIndex] : 0));
                                            Log.Message($"New offset: {bodyAddons[currentBodyPartAddonIndex].offsets.east.offset.ToString("F2")}");
                                            break;
                                        case 2:
                                            UnityEngine.Vector2 oldOffsetSouth = bodyAddons[currentBodyPartAddonIndex].offsets.south.offset;
                                            bodyAddons[currentBodyPartAddonIndex].offsets.south.offset = new UnityEngine.Vector2(oldOffsetSouth.x + (xy ? 0 : positivity * offsetAmounts[offsetAmountsIndex]), oldOffsetSouth.y + (xy ? positivity * offsetAmounts[offsetAmountsIndex] : 0));
                                            Log.Message($"New offset: {bodyAddons[currentBodyPartAddonIndex].offsets.south.offset.ToString("F2")}");
                                            break;
                                        case 3:
                                            UnityEngine.Vector2 oldOffsetWest = bodyAddons[currentBodyPartAddonIndex].offsets.west.offset;
                                            bodyAddons[currentBodyPartAddonIndex].offsets.west.offset = new UnityEngine.Vector2(oldOffsetWest.x + (xy ? 0 : positivity * offsetAmounts[offsetAmountsIndex]), oldOffsetWest.y + (xy ? positivity * offsetAmounts[offsetAmountsIndex] : 0));
                                            Log.Message($"New offset: {bodyAddons[currentBodyPartAddonIndex].offsets.west.offset.ToString("F2")}");
                                            break;
                                        default:
                                            Log.Error("PAIN ELEMENTAL");
                                            break;

                                    }
                                    PawnBodyType_ThingComp PBTcomp = ((Pawn)parent).TryGetComp<PawnBodyType_ThingComp>();
                                    BodyTypeUtility.UpdatePawnSprite(((Pawn)parent), PBTcomp.PersonallyExempt, PBTcomp.CategoricallyExempt, true, false);
                                }
                            }
                        }
                    };

                    yield return new Command_Action
                    {
                        defaultLabel = $"Change Head Offset {(xy ? "Y" : "X")}{(positivity == 1 ? "+" : "-")} for {((Pawn)parent).story.bodyType} {(cardinality == 0 ? "north" : (cardinality == 1 ? "east" : (cardinality == 2 ? "south" : "west")))} at {offsetAmounts[offsetAmountsIndex]}",
                        action = delegate ()
                        {
                            UnityEngine.Vector2 oldOffset = ((Pawn)parent).story.bodyType.headOffset;
                            ((Pawn)parent).story.bodyType.headOffset = new UnityEngine.Vector2(oldOffset.x + (xy ? 0 : positivity * offsetAmounts[offsetAmountsIndex]), oldOffset.y + (xy ? positivity * offsetAmounts[offsetAmountsIndex] : 0));
                            Log.Message($"{((Pawn)parent).story.bodyType}: {((Pawn)parent).story.bodyType.headOffset.ToString("F3")}");
                        }
                    };

                    yield return new Command_Action
                    {
                        defaultLabel = $"Change Sprite size for {((Pawn)parent).story.bodyType}",
                        action = delegate ()
                        {
                            if (((Pawn)parent).def is ThingDef_AlienRace alienProps)
                            {
                                currentBodySizeMeshIndex = (currentBodySizeMeshIndex + 1) % Values.validBodyMeshSizes.Length;

                                PawnBodyType_ThingComp PBTcomp = ((Pawn)parent).TryGetComp<PawnBodyType_ThingComp>();
                                PBTcomp.usingCustomBodyMeshSize = true;
                                GraphicPaths gp = alienProps.alienRace.graphicPaths.GetCurrentGraphicPath(((Pawn)parent).ageTracker.CurLifeStage);
                                //alienProps.alienRace.generalSettings.alienPartGenerator.bodyAddons.First().offsets.north.bodyTypes.First().

                                UnityEngine.Vector2 drawSize = gp.customDrawSize;
                                gp.customDrawSize = new UnityEngine.Vector2(Values.validBodyMeshSizes[currentBodySizeMeshIndex], Values.validBodyMeshSizes[currentBodySizeMeshIndex]);
                                Log.Message($"{((Pawn)parent).story.bodyType}: {gp.customDrawSize.ToString("F3")}");
                                BodyTypeUtility.UpdatePawnSprite(((Pawn)parent), PBTcomp.PersonallyExempt, PBTcomp.CategoricallyExempt, true, false);
                            }

                        }
                    };

                    yield return new Command_Action
                    {
                        defaultLabel = $"Dump body data",
                        action = delegate ()
                        {
                            if (((Pawn)parent).def is ThingDef_AlienRace alienProps)
                            {
                                Log.Message("-------------------------------------------------");
                                Log.Message("--------------Pawn Body Data---------------------");
                                Log.Message("-------------------------------------------------");

                                Pawn pawn = ((Pawn)parent);



                                Log.Message($"{pawn.Name.ToStringShort}'s body stats:");
                                Log.Message($"Bodytype: {pawn.story.bodyType}");
                                Log.Message($"Draw size: {alienProps.alienRace.graphicPaths.GetCurrentGraphicPath(((Pawn)parent).ageTracker.CurLifeStage).customDrawSize.ToString("F3")}");
                                currentBodySizeMeshIndex = (currentBodySizeMeshIndex + 1) % Values.validBodyMeshSizes.Length;
                                Log.Message($"Head Offset: {pawn.story.bodyType.headOffset.ToString("F3")}");

                                List<AlienPartGenerator.BodyAddon> bodyAddons = alienProps.alienRace.generalSettings.alienPartGenerator.bodyAddons;

                                foreach (var x in bodyAddons)
                                {
                                    Log.Message($"Path: {x.path}");
                                    Log.Message($"Offset south: {x.offsets.south.offset.ToString("F3")}");
                                    Log.Message($"Offset north: {x.offsets.north.offset.ToString("F3")}");
                                    Log.Message($"Offset east: {x.offsets.east.offset.ToString("F3")}");

                                    //Log.Message($"Offset west: {x.offsets.west.offset.ToString("F3")}");
                                }

                                Log.Message("`------------------------------------------------");
                                Log.Message("-`-----------------------------------------------");
                                Log.Message("`------------------------------------------------");
                            }

                        }
                    };
                }
            }
        }



        int currentBodySizeMeshIndex = 0;

        // 0 is north, 1 is east, 2 is south 3 is west
        int cardinality = 0;

        int currentBodyPartAddonIndex = 0;

        float[] offsetAmounts = new float[] 
        {
            0.001f,
            0.01f,
            0.1f,
            1,
            10f,
            100f
        };

        int offsetAmountsIndex = 0;

        int positivity = 1;

        bool xy;
    }
}
