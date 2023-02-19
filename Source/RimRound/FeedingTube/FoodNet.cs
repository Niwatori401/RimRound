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
        static int currentID;
        //Returns a positive number. Production per tick.

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
            this.id = currentID++;
            
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
            foreach (var x in foodNetTrader)
            {
                if (!x.CanBeOn)
                    x.IsOn = false;
            }
        }


        /// <returns>A positive number representing the amount that wasn't successfully drained.</returns>
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

        //returns 
        /// <returns>A positive number representing the amount that was not filled into anything.</returns>
        public float Fill(float amountToFill, float ftnRatio)
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
                    amountToFill -= store.Add(averageFill, ftnRatio);
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

        public int id;

        public List<FoodNetStorage_ThingComp> foodNetStorages = new List<FoodNetStorage_ThingComp>();
        public List<FoodNetTrader_ThingComp> foodNetTrader = new List<FoodNetTrader_ThingComp>();
        public List<FoodTransmitter_ThingComp> foodTransmitters = new List<FoodTransmitter_ThingComp>();

        public BoolGrid netGrid;

        public float FullnessToNutritionRatio 
        {
            get 
            {
                IEnumerable<FoodNetStorage_ThingComp> fullstorages =
                    (from attachedStorages
                    in this.foodNetStorages
                    where attachedStorages.Stored > 0
                    select attachedStorages);

                float totalVolume = 0;
                float totalDensityVolumeProduct = 0;

                foreach (var storage in fullstorages)
                {
                    totalVolume += storage.Stored;
                    totalDensityVolumeProduct += storage.Stored * storage.FullnessToNutritionRatio;
                }

                if (totalVolume == 0)
                    return 0;
                else
                    return totalDensityVolumeProduct / totalVolume;
            }
        }

        public float NutritionToFullnessRatio
        {
            get 
            {
                if (FullnessToNutritionRatio == 0)
                    return 0;
                else
                    return 1 / FullnessToNutritionRatio;
            }
        }
    }
}