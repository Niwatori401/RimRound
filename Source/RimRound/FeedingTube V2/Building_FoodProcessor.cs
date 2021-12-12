using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound
{
    public class Building_FoodProcessor : Building
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.trader = base.GetComp<FoodNetTrader_ThingComp>();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            //return base.GetGizmos();

            yield return new Command_Action
            {
                defaultLabel = "Build Hopper",
                icon = Widgets.GetIconFor(RimWorld.ThingDefOf.Hopper),
                action = delegate ()
                {
                    GenConstruct.PlaceBlueprintForBuild_NewTemp(RimWorld.ThingDefOf.Hopper, Position + new IntVec3(-1, 0, 2), this.Map, Rot4.East, Faction, null);
                }
            };
        }

        public override void Tick()
        {
            base.Tick();
            ProcessFood();
        }

        public bool CanProcessNow 
        {
            get 
            {
                return trader.IsOn && trader.CanBeOn && trader.TransmitsFoodNow && this.HasEnoughFeedstockInHoppers();
            }
        }

        public bool ShouldProcessFood 
        {
            get 
            {
                if (FoodTransmitter_NetManager.For(this.Map)?.FoodNetAt(this.Position) is FoodNet f && 
                    f.Stored < f.StorageCapacity && 
                    CanProcessNow) 
                {
                    return true;
                }

                return false;
            }
        }

        public void ProcessFood() 
        {
            if (Functions.IsHashIntervalTick((int)processingFrequency) && ShouldProcessFood) 
            {
                //Not use right now, but can be used with CompIngredients.RegisterIngredient();
                List<ThingDef> listOfIngredients = new List<ThingDef>();

                float remainingStorageSpace = trader.FoodNet.StorageCapacity - trader.FoodNet.Stored;
                //Convert nutrition to liters here.


                if (remainingStorageSpace > Values.MinRQ && CanProcessNow)
                {
                    Thing t = TryGetFoodInHopper();
                    if (t is null) 
                    {
                        return;
                    }

                    int numberOfItemNeeded = Mathf.Min(
                        t.stackCount, 
                        Mathf.CeilToInt(remainingStorageSpace / t.GetStatValue(StatDefOf.Nutrition, true)), 
                        Mathf.CeilToInt(maxNutritionToProcessPerTick / t.GetStatValue(StatDefOf.Nutrition, true)));

                    remainingStorageSpace -= numberOfItemNeeded * t.GetStatValue(StatDefOf.Nutrition, true);
                    t.SplitOff(numberOfItemNeeded);

                    listOfIngredients.Add(t.def);
                }

                trader.FoodNet.Delta((trader.FoodNet.StorageCapacity - trader.FoodNet.Stored) - remainingStorageSpace);
            }
        }

        public bool HasEnoughFeedstockInHoppers() 
        {
            float totalNutritionInHoppers = 0;

            foreach (var c in AdjCellsCardinalInBounds) 
            {
                Thing hopper = null;
                Thing foodOnHopper = null;

                List<Thing> thingList = c.GetThingList(base.Map);
                foreach (var t in thingList) 
                {
                    if (this.IsAcceptableFeedstock(t.def)) 
                    {
                        foodOnHopper = t;
                    }

                    if (t.def == ThingDefOf.Hopper) 
                    {
                        hopper = t;
                    }
                }

                if (foodOnHopper != null && hopper != null) 
                {
                    totalNutritionInHoppers += (float)foodOnHopper.stackCount * foodOnHopper.GetStatValue(StatDefOf.Nutrition, true);
                }

                if (totalNutritionInHoppers >= nutritionCostPerUse) 
                {
                    return true;
                }
            }

            return false;
        }

        public List<IntVec3> AdjCellsCardinalInBounds
        {
            get
            {
                if (cachedAdjCells == null)
                {
                    cachedAdjCells = (from c in GenAdj.CellsAdjacentCardinal(this)
                     where c.InBounds(base.Map)
                     select c).ToList<IntVec3>();
                }

                return cachedAdjCells;      
            }
        }

        public bool IsAcceptableFeedstock(ThingDef def) 
        {
            return def.IsNutritionGivingIngestible && 
                def.ingestible.preferability != FoodPreferability.Undefined && 
                (def.ingestible.foodType & FoodTypeFlags.Plant) != FoodTypeFlags.Plant && 
                (def.ingestible.foodType & FoodTypeFlags.Tree) != FoodTypeFlags.Tree;
        }

        public Thing TryGetFoodInHopper() 
        {
            for (int i = 0; i < this.AdjCellsCardinalInBounds.Count; i++)
            {
                Thing thing = null;
                Thing thing2 = null;
                List<Thing> thingList = this.AdjCellsCardinalInBounds[i].GetThingList(base.Map);
                for (int j = 0; j < thingList.Count; j++)
                {
                    Thing thing3 = thingList[j];
                    if (this.IsAcceptableFeedstock(thing3.def))
                    {
                        thing = thing3;
                    }
                    if (thing3.def == ThingDefOf.Hopper)
                    {
                        thing2 = thing3;
                    }
                }
                if (thing != null && thing2 != null)
                {
                    return thing;
                }
            }
            return null;
        }


        FoodNetTrader_ThingComp trader;

        //amount of ticks between processing
        float processingFrequency = 60;

        float nutritionCostPerUse = 0;

        float maxNutritionToProcessPerTick = 0.1f;

        List<IntVec3> cachedAdjCells;
    }
}
