using RimRound.FeedingTube.Comps;
using RimRound.FeedingTube.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.FeedingTube
{
    public class FoodNet
    {
        //Returns a positive number. Production per tick.
        public float Production 
        {
            get 
            {
                return Math.Abs((from x
                       in this.foodNetTrader
                       where x.ChangePerTick > 0
                       select x.ChangePerTick).Sum());
            }
        }

        //Returns a positive number. Usage per tick
        public float Consumption 
        {
            get 
            {
                return Math.Abs((from x
                       in this.foodNetTrader
                       where x.ChangePerTick < 0
                       select x.ChangePerTick).Sum());
            }
        }

        public float Stored 
        {
            get 
            {
                IEnumerable <float> fullstoragesvalues =
                                    (from x
                                    in this.foodNetStorages
                                     where x.Stored > 0
                                     select x.Stored);

                return fullstoragesvalues is null ? 0 : fullstoragesvalues.Sum();
            } 
        }

        public float StorageCapacity
        {
            get 
            {
                return (from x
                        in this.foodNetStorages
                        where x.Capacity > 0
                        select x.Capacity).Sum();
            }
        }

        public FoodNet(IEnumerable<FoodTransmitter_ThingComp> connectors, Map map)
        {
            this.netGrid = new BoolGrid(map);

            
            foreach (FoodTransmitter_ThingComp x in connectors) 
            {
                AddNode(x);
            }
        }

        public string NetInfo 
        {
            get 
            {
                return "RR_FT_NetInfo".Translate(Stored.ToString("F1"), StorageCapacity.ToString("F1"));
            }
        }


        public void AddNode(FoodTransmitter_ThingComp x) 
        {
            x.FoodNet = this;

            foodTransmitters.Add(x);

            if (x is FoodNetStorage_ThingComp a)
                foodNetStorages.Add(a);

            if (x is FoodNetTrader_ThingComp b)
                foodNetTrader.Add(b);

            foreach (IntVec3 y in x.parent.OccupiedRect().Cells)
            {
                netGrid.Set(y, true);
            }
        }

        public void Tick() 
        {
            int numberOfTicksPassedSinceLastTick = Find.TickManager.TicksGame - lastNetTick;


            if (numberOfTicksPassedSinceLastTick == 0)
                return;

            foreach (var x in foodNetTrader)
            {
                if (!x.CanBeOn)
                    x.IsOn = false;
            }


            //Negative means making food
            float netUsage = Consumption - Production;
            netUsage *= numberOfTicksPassedSinceLastTick;

            if (netUsage == 0)
                return;

            //Using food
            if (netUsage > 0)
            {
                if (Drain(Math.Abs(netUsage)) > 0)
                {
                    //Ran out of food, turn off things. 
                    foodNetTrader.RandomElement().IsOn = false;
                }
                else 
                {
                    foreach (var x in foodNetTrader) 
                    {
                        if (x.CanBeOn)
                            x.IsOn = true;
                    }
                }
            }
            else //Making excess food
            {
                if (Fill(Math.Abs(netUsage)) > 0) 
                {
                    //give player message to build more batteries?
                }
            }

            lastNetTick = Find.TickManager.TicksGame;
        }


        public void Delta(float change) 
        {
            if (change == 0)
                return;

            if (change > 0)
            {
                Fill(change);
                return;
            }    
                

            Drain(change);
        }


        //returns the amount that wasn't successfully drained.
        public float Drain(float amountToDrain) 
        {
            IEnumerable<FoodNetStorage_ThingComp> fullStores =
                from x in this.foodNetStorages
                where x.Stored > FeedingTubeUtility.MinRQ
                select x;

            float averageDraw = amountToDrain / fullStores.Count();

            while (amountToDrain > FeedingTubeUtility.MinRQ && fullStores.Count() > 0)
            {
                foreach (var store in fullStores) 
                {
                    amountToDrain -= store.Subtract(averageDraw);
                }

                if (fullStores.Count() <= 0) 
                {
                    break;
                }

                averageDraw = amountToDrain / fullStores.Count();
            }

            return amountToDrain;
        }

        //returns amount that was not filled into anything
        public float Fill(float amountToFill)
        {
            IEnumerable<FoodNetStorage_ThingComp> emptyStores =
                from x in this.foodNetStorages
                 where x.Stored < x.Capacity
                select x;

            IEnumerable<float> remainingEmptyness =
                from y in emptyStores
                select y.Remaining;

            int emptyStoresCount = emptyStores.Count();

            if (emptyStoresCount == 0)
                return amountToFill;

            float averageFill = amountToFill / emptyStoresCount;

            while (remainingEmptyness.Sum() > FeedingTubeUtility.MinRQ && amountToFill > FeedingTubeUtility.MinRQ)
            {
                foreach (var store in emptyStores) 
                {
                    amountToFill -= store.Add(averageFill);
                }


                averageFill = amountToFill / emptyStores.Count();
            }

            return amountToFill;
        }


        public void DestroyNet() 
        {
            foreach (FoodTransmitter_ThingComp comp in this.foodTransmitters) 
            {
                comp.FoodNet = null;
            }
        }

        private int lastNetTick = 0;

        public List<FoodNetStorage_ThingComp> foodNetStorages = new List<FoodNetStorage_ThingComp>();
        public List<FoodNetTrader_ThingComp> foodNetTrader = new List<FoodNetTrader_ThingComp>();
        public List<FoodTransmitter_ThingComp> foodTransmitters = new List<FoodTransmitter_ThingComp>();

        public BoolGrid netGrid;

        public float fullnessToNutritionRatio = 1;

        public void UpdateRatio(float incomingNutrition, float incomingRatio = 1)
        {
            //Weighted average of current values and incoming values  
            fullnessToNutritionRatio =
                (Stored + incomingRatio * incomingNutrition) /
                ((Stored / fullnessToNutritionRatio) + incomingNutrition);
        }
    }
}