using RimRound.Comps;
using RimRound.FeedingTube.Comps;
using RimRound.FeedingTube.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.FeedingTube
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
            yield return new Command_Action
            {
                disabled = GenConstruct.CanPlaceBlueprintAt(Defs.ThingDefOf.RR_Hopper, Position + new IntVec3(2, 0, 0), Rot4.West, this.Map) ? false : true,
                defaultLabel = "Build Hopper",
                icon = Widgets.GetIconFor(Defs.ThingDefOf.RR_Hopper),
                action = delegate ()
                {
                    GenConstruct.PlaceBlueprintForBuild_NewTemp(Defs.ThingDefOf.RR_Hopper, Position + new IntVec3(2, 0, 0), this.Map, Rot4.West, Faction, null);
                }
            };
        }

        public override void Tick()
        {
            base.Tick();
            ProcessFood();
        }

        private bool CanProcessNow 
        {
            get 
            {
                return trader.IsOn && trader.CanBeOn && trader.TransmitsFoodNow && this.HasEnoughFeedstockInHoppers();
            }
        }

        private bool ShouldProcessFood 
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

        private void ProcessFood() 
        {
            if (FeedingTubeUtility.IsHashIntervalTick((int)processingFrequency) && ShouldProcessFood) 
            {
                //Not used right now, but can be used with CompIngredients.RegisterIngredient();
                List<ThingDef> listOfIngredients = new List<ThingDef>();

                float remainingStorageSpace = trader.FoodNet.StorageCapacity - trader.FoodNet.Stored;

                if (remainingStorageSpace > FeedingTubeUtility.MinRQ && CanProcessNow)
                {
                    Thing foodInHopper = TryGetFoodInHopper();
                    if (foodInHopper is null) 
                        return;


                    float nutritionForOneUnitOfFoodInHopper = foodInHopper.GetStatValue(StatDefOf.Nutrition, true);

                    float ftnRatio = GetNutritionDensityOfFoodInHopper(foodInHopper);
                   

                    int numberOfFoodItemsPerProcess = Mathf.Min(
                        foodInHopper.stackCount, 
                        Mathf.CeilToInt(remainingStorageSpace / (nutritionForOneUnitOfFoodInHopper * ftnRatio)), 
                        Mathf.CeilToInt(maxNutritionToProcessPerTick / nutritionForOneUnitOfFoodInHopper));

                    float amountOfFoodVolumeToAdd = numberOfFoodItemsPerProcess * ftnRatio * nutritionForOneUnitOfFoodInHopper;
                    
                    foodInHopper.SplitOff(numberOfFoodItemsPerProcess);

                    listOfIngredients.Add(foodInHopper.def);

                    trader.FoodNet.UpdateRatio(numberOfFoodItemsPerProcess * nutritionForOneUnitOfFoodInHopper, ftnRatio);

                    trader.FoodNet.Delta(amountOfFoodVolumeToAdd);
                }
            }
        }


        private float GetNutritionDensityOfFoodInHopper(Thing foodInHopper) 
        {
            float ftnRatio = 1;
            
            ThingComp_FoodItems_NutritionDensity NDComp = foodInHopper.TryGetComp<ThingComp_FoodItems_NutritionDensity>();
            if (NDComp != null)
            {
                ftnRatio = NDComp.Props.fullnessToNutritionRatio;
            }

            return ftnRatio;
        }

        private bool HasEnoughFeedstockInHoppers() 
        {
            float totalNutritionInHoppers = 0;

            foreach (var c in AdjCellsCardinalInBounds) 
            {
                Thing hopper = null;
                Thing foodOnHopper = null;

                List<Thing> thingsInCell = c.GetThingList(base.Map);
                foreach (Thing thing in thingsInCell) 
                {
                    if (this.IsAcceptableFeedstock(thing.def)) 
                    {
                        foodOnHopper = thing;
                    }

                    if (thing.def == ThingDefOf.Hopper || thing.def == Defs.ThingDefOf.RR_Hopper) 
                    {
                        hopper = thing;
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

        private List<IntVec3> AdjCellsCardinalInBounds
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

        private bool IsAcceptableFeedstock(ThingDef def) 
        {
            return def.IsNutritionGivingIngestible && 
                def.ingestible.preferability != FoodPreferability.Undefined && 
                (def.ingestible.foodType & FoodTypeFlags.Plant) != FoodTypeFlags.Plant && 
                (def.ingestible.foodType & FoodTypeFlags.Tree) != FoodTypeFlags.Tree;
        }

        private Thing TryGetFoodInHopper() 
        {
            for (int i = 0; i < this.AdjCellsCardinalInBounds.Count; i++)
            {
                Thing foodItem = null;
                Thing hopper = null;
                List<Thing> thingList = this.AdjCellsCardinalInBounds[i].GetThingList(base.Map);
                for (int j = 0; j < thingList.Count; j++)
                {
                    Thing thingInCell = thingList[j];
                    if (this.IsAcceptableFeedstock(thingInCell.def))
                    {
                        foodItem = thingInCell;
                    }
                    if (thingInCell.def == ThingDefOf.Hopper || thingInCell.def == Defs.ThingDefOf.RR_Hopper)
                    {
                        hopper = thingInCell;
                    }
                }
                if (foodItem != null && hopper != null)
                {
                    return foodItem;
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
