using AlienRace;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Sound;

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
                    icon = (cachedPBTComp is null ? Widgets.GetIconFor(RimWorld.ThingDefOf.Campfire) : (cachedPBTComp.BodyArchetype == BodyArchetype.standard ? Resources.SWITCH_PAWN_BODY_TYPE_APPLE_ICON : Resources.SWITCH_PAWN_BODY_TYPE_STANDARD_ICON)),
                    action = delegate ()
                        {
                            Resources.gizmoClick.PlayOneShotOnCamera(null);
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
                    icon = Resources.FORCE_CURRENT_BODY_TYPE_ICON,
                    action = delegate ()
                    {
                        Resources.gizmoClick.PlayOneShotOnCamera(null);
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

                    defaultLabel = "Add 10 Perk Points",
                    icon = Resources.TEN_PP_ICON,
                    action = delegate ()
                    {
                        Resources.gizmoClick.PlayOneShotOnCamera(null);
                        FullnessAndDietStats_ThingComp comp = parent.AsPawn().TryGetComp<FullnessAndDietStats_ThingComp>();

                        if (comp is null)
                        {
                            return;
                        }
                        comp.perkLevels.availablePoints += 10;
                    }
                };

                yield return new Command_Action
                {
                    defaultLabel = "Wipe perks",
                    icon = Resources.WIPE_PERKS_ICON,
                    action = delegate ()
                    {
                        Resources.gizmoClick.PlayOneShotOnCamera(null);
                        FullnessAndDietStats_ThingComp comp = parent.AsPawn().TryGetComp<FullnessAndDietStats_ThingComp>();

                        if (comp is null)
                        {
                            return;
                        }

                        for (int i = 0; i < comp.perkLevels.PerkToLevels.Count; i++)
                        {
                            comp.perkLevels.PerkToLevels[comp.perkLevels.PerkToLevels.ElementAt(i).Key] = 0;
                        }
                       
                        // wipe perklevels for saving
                    }
                };


                yield return new Command_Action
                {
                    defaultLabel = "Change magnitude",
                    icon = Resources.CHANGE_MAGNITUDE_ICON,
                    action = delegate ()
                    {
                        Resources.gizmoClick.PlayOneShotOnCamera(null);
                        offsetAmountsIndex = (offsetAmountsIndex + 1) % offsetAmounts.Length;
                        Log.Message($"Magnitude is now {offsetAmounts[offsetAmountsIndex]}");
                    }
                };

                yield return new Command_Action
                {
                    defaultLabel = "Change direction",
                    icon = Resources.CHANGE_DIRECTION_ICON,
                    action = delegate ()
                    {
                        Resources.gizmoClick.PlayOneShotOnCamera(null);
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
                        defaultLabel = "Print pawn details",
                        icon = Resources.exampleIcon,
                        action = delegate ()
                        {
                            Resources.gizmoClick.PlayOneShotOnCamera(null);
                            FullnessAndDietStats_ThingComp comp = parent.AsPawn().TryGetComp<FullnessAndDietStats_ThingComp>();
                            Pawn pawn = comp.parent.AsPawn();

                            if (comp is null || pawn is null)
                            {
                                return;
                            }


                            Log.Message("=================================================================");
                            Log.Message("=============-------------Pawn-Info--------------- ==============");
                            Log.Message("=================================================================");

                            Log.Message($"Name: {pawn.Name}");
                            Log.Message($"Age: {pawn.ageTracker.AgeBiologicalYears}");
                            Log.Message($"Weight: {pawn.Weight()}");
                            Log.Message($"Gender: {pawn.gender}");
                            Log.Message($"Digestion Rate (1000x): {comp.DigestionRate * 1000}");
                            Log.Message($"Fullness: {comp.CurrentFullness}");
                            Log.Message($"Age: {pawn.ageTracker.AgeBiologicalYears}");
                            Log.Message($"Faction: {pawn.Faction}");


                            Log.Message("=============--------------Apparel-----------------==============");
                            foreach (Apparel a in pawn.apparel.WornApparel)
                                Log.Message($"Wearing: {a.def.defName}");
                            Log.Message("=============------------END-Apparel---------------==============");

                            if (pawn.def is AlienRace.ThingDef_AlienRace def)
                                Log.Message($"Alien Race: {def.defName}");
                            else
                                Log.Message($"NO RACE RECORDED");

                            Log.Message("=============--------------Hediffs-----------------==============");
                            List<Hediff> hediffs = new List<Hediff>();
                            pawn.health.hediffSet.GetHediffs(ref hediffs);
                            foreach (Hediff a in hediffs)
                                Log.Message($"Hediff: {a.def.defName} S: {a.Severity} ({a.SeverityLabel})");
                            Log.Message("=============------------END-Hediffs---------------==============");

                            Log.Message("=============---------------Perks------------------==============");
                            foreach (var p in comp.perkLevels.PerkToLevels)
                                if (p.Value != 0)
                                    Log.Message($"Perk: Level:{p.Value} -  {p.Key}");
                            Log.Message("=============-------------END-Perks----------------==============");


                            Log.Message("=================================================================");
                            Log.Message("=================================================================");
                            Log.Message("=================================================================");
                        }
                    };
                    yield return new Command_Action
                    {
                        defaultLabel = "Change cardinality",
                        icon = Resources.CHANGE_CARDINALITY_ICON,
                        action = delegate ()
                        {
                            Resources.gizmoClick.PlayOneShotOnCamera(null);
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
                        icon = Resources.CHANGE_XY_ICON,
                        action = delegate ()
                        {
                            Resources.gizmoClick.PlayOneShotOnCamera(null);
                            xy = !xy;
                        }
                    };


                    yield return new Command_Action
                    {
                        defaultLabel = $"{(positivity == 1 ? "Add" : "Subtract")} ({offsetAmounts[offsetAmountsIndex]} to DEBUGFLOAT 1)",
                        icon = Resources.DEBUGFLOAT_ICON,
                        action = delegate ()
                        {
                            Resources.gizmoClick.PlayOneShotOnCamera(null);
                            Values.debugPos += positivity * (offsetAmounts[offsetAmountsIndex]);
                            Log.Message($"Debug Float 1 is now: {Values.debugPos}");
                            BodyTypeUtility.UpdatePawnSprite(parent.AsPawn(), false, false, true, false);
                        }
                    };
                    yield return new Command_Action
                    {
                        defaultLabel = $"{(positivity == 1 ? "Add" : "Subtract")} ({offsetAmounts[offsetAmountsIndex]} to DEBUGFLOAT 2)",
                        icon = Resources.DEBUGFLOAT_ICON,
                        action = delegate ()
                        {
                            Resources.gizmoClick.PlayOneShotOnCamera(null);
                            Values.debugPos2 += positivity * (offsetAmounts[offsetAmountsIndex]);
                            Log.Message($"Debug Float 2 is now: {Values.debugPos2}");
                            BodyTypeUtility.UpdatePawnSprite(parent.AsPawn(), false, false, true, false);
                        }
                    };
                    yield return new Command_Action
                    {
                        defaultLabel = $"{(positivity == 1 ? "Add" : "Subtract")} ({offsetAmounts[offsetAmountsIndex]} to DEBUGFLOAT 3)",
                        icon = Resources.DEBUGFLOAT_ICON,
                        action = delegate ()
                        {
                            Resources.gizmoClick.PlayOneShotOnCamera(null);
                            Values.debugPos3 += positivity * (offsetAmounts[offsetAmountsIndex]);
                            Log.Message($"Debug Float 3 is now: {Values.debugPos3}");
                            BodyTypeUtility.UpdatePawnSprite(parent.AsPawn(), false, false, true, false);
                        }
                    };
                    yield return new Command_Action
                    {
                        defaultLabel = $"{(positivity == 1 ? "Add" : "Subtract")} ({offsetAmounts[offsetAmountsIndex]} to DEBUGFLOAT 4)",
                        icon = Resources.DEBUGFLOAT_ICON,
                        action = delegate ()
                        {
                            Resources.gizmoClick.PlayOneShotOnCamera(null);
                            Values.debugPos4 += positivity * (offsetAmounts[offsetAmountsIndex]);
                            Log.Message($"Debug Float 4 is now: {Values.debugPos4}");
                            BodyTypeUtility.UpdatePawnSprite(parent.AsPawn(), false, false, true, false);
                        }
                    };

                }
               

                yield return new Command_Action
                {
                    defaultLabel = $"{(positivity == 1 ? "Add" : "Subtract")} Severity ({offsetAmounts[offsetAmountsIndex]} kgs)",
                    icon = (positivity == 1 ? Resources.ADD_SEVERITY_ICON : Resources.REDUCE_SEVERITY_ICON),
                    action = delegate ()
                    {
                        Resources.gizmoClick.PlayOneShotOnCamera(null);
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
                        icon = Resources.CHANGE_OFFSET_ITEM_ICON,
                        action = delegate ()
                        {
                            Resources.gizmoClick.PlayOneShotOnCamera(null);
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
                        icon = Resources.EXTRA_BODY_PART_OFFSET_ICON,
                        action = delegate ()
                        {
                            Resources.gizmoClick.PlayOneShotOnCamera(null);
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
                        icon = Resources.HEAD_OFFSET_ICON,
                        action = delegate ()
                        {
                            Resources.gizmoClick.PlayOneShotOnCamera(null);
                            UnityEngine.Vector2 oldOffset = ((Pawn)parent).story.bodyType.headOffset;
                            ((Pawn)parent).story.bodyType.headOffset = new UnityEngine.Vector2(oldOffset.x + (xy ? 0 : positivity * offsetAmounts[offsetAmountsIndex]), oldOffset.y + (xy ? positivity * offsetAmounts[offsetAmountsIndex] : 0));
                            Log.Message($"{((Pawn)parent).story.bodyType}: {((Pawn)parent).story.bodyType.headOffset.ToString("F3")}");
                        }
                    };

                    yield return new Command_Action
                    {
                        defaultLabel = $"Change Sprite size for {((Pawn)parent).story.bodyType}",
                        icon = Resources.SPRITE_SIZE_ICON,
                        action = delegate ()
                        {
                            Resources.gizmoClick.PlayOneShotOnCamera(null);
                            if (((Pawn)parent).def is ThingDef_AlienRace alienProps)
                            {
                                currentBodySizeMeshIndex = (currentBodySizeMeshIndex + 1) % Values.validBodyMeshSizes.Length;

                                PawnBodyType_ThingComp PBTcomp = ((Pawn)parent).TryGetComp<PawnBodyType_ThingComp>();
                                PBTcomp.usingCustomBodyMeshSize = true;
                                //alienProps.alienRace.generalSettings.alienPartGenerator.bodyAddons.First().offsets.north.bodyTypes.First().
                                
                                UnityEngine.Vector2 drawSize = alienProps.alienRace.generalSettings.alienPartGenerator.customDrawSize;
                                alienProps.alienRace.generalSettings.alienPartGenerator.customDrawSize = new UnityEngine.Vector2(Values.validBodyMeshSizes[currentBodySizeMeshIndex], Values.validBodyMeshSizes[currentBodySizeMeshIndex]);
                                Log.Message($"{((Pawn)parent).story.bodyType}: {alienProps.alienRace.generalSettings.alienPartGenerator.customDrawSize.ToString("F3")}");
                                BodyTypeUtility.UpdatePawnSprite(((Pawn)parent), PBTcomp.PersonallyExempt, PBTcomp.CategoricallyExempt, true, false);
                            }

                        }
                    };

                    yield return new Command_Action
                    {
                        defaultLabel = $"Dump body data",
                        icon = Resources.DUMP_BODY_DATA_ICON,
                        action = delegate ()
                        {
                            Resources.gizmoClick.PlayOneShotOnCamera(null);
                            if (((Pawn)parent).def is ThingDef_AlienRace alienProps)
                            {
                                Log.Message("-------------------------------------------------");
                                Log.Message("--------------Pawn Body Data---------------------");
                                Log.Message("-------------------------------------------------");

                                Pawn pawn = ((Pawn)parent);



                                Log.Message($"{pawn.Name.ToStringShort}'s body stats:");
                                Log.Message($"Bodytype: {pawn.story.bodyType}");
                                Log.Message($"Draw size: {alienProps.alienRace.generalSettings.alienPartGenerator.customDrawSize.ToString("F3")}");
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
            100f,
            1000f,
            10000f
        };

        int offsetAmountsIndex = 0;

        int positivity = 1;

        bool xy;
    }
}
