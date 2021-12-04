using HarmonyLib;
using RimRound.Comps;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.Patch.RimWorldPatches
{
    [HarmonyPatch(typeof(FloatMenuMakerMap))]
    [HarmonyPatch("AddHumanlikeOrders")]
    public class FloatMenuMakerMap_AddHumanlikeOrders_AlterTextForFoodAboveHardLimit
    {
        public static void Postfix(Vector3 __0, Pawn __1, List<FloatMenuOption> __2) 
        {
            IntVec3 c = IntVec3.FromVector3(__0);
            List<Thing> things = new List<Thing>();
            FloatMenuOption garbageFMO = new FloatMenuOption("", null);

            foreach (Thing t in c.GetThingList(__1.Map)) 
            {
                foreach (FloatMenuOption fmo in __2)
                {
                    garbageFMO.Label = "ConsumeThing".Translate(t.LabelShort, t);
                    if (fmo.Label.Contains(garbageFMO.Label))
                    {
                        if (__1.TryGetComp<FullnessAndDietStats_ThingComp>() is FullnessAndDietStats_ThingComp FnDStatsComp && 
                            t.def.ingestible.CachedNutrition * Values.defaultFullnessToNutritionRatio >= FnDStatsComp.RemainingFullnessUntil(FnDStatsComp.HardLimit)) 
                        {
                            if (!FnDStatsComp.fullnessbar.peaceForeverHeld) 
                            {
                                fmo.Label = "FloatMenuCantConsumeTooBig".Translate(t.LabelShort, t);
                                fmo.action = null;
                            }
                        }
                    }
                }
            }
            

        }
    }
}
