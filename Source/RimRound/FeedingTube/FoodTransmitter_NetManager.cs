using RimRound.FeedingTube.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.FeedingTube
{
    public class FoodTransmitter_NetManager : MapComponent
    {
        public FoodTransmitter_NetManager(Map map) : base(map) 
        { 
        }

        public static FoodTransmitter_NetManager For(Map map) 
        {
            return map.GetComponent<FoodTransmitter_NetManager>();
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            this.Init();

        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            foreach (var x in FoodNets) 
            {
                x.Tick();
            }
        }

        public void Notify_ConnectorAdded(FoodTransmitter_ThingComp comp) 
        {
            if (this.FoodNets.Any((FoodNet f) => f.foodTransmitters.Contains(comp)) || comp.FoodNet != null)
            {
                return;
            }

            List<FoodTransmitter_ThingComp> neighbors = new List<FoodTransmitter_ThingComp>();

            foreach (var x in GenAdj.CellsAdjacentCardinal(comp.parent)) 
            {
                foreach (var y in x.GetThingList(this.map)) 
                {
                    if (y.TryGetComp<FoodTransmitter_ThingComp>() is FoodTransmitter_ThingComp tempcomp) 
                    {
                        neighbors.Add(tempcomp);
                    }
                }
            }

            IEnumerable<FoodNet> foodNetQuery =
                from net in
                    (from neighbor
                    in neighbors
                     select neighbor.FoodNet).Distinct()
                where net != null
                select net;

            if (foodNetQuery.Count() == 1) 
            {
                foodNetQuery.First().AddNode(comp);
                return;
            }

            ResetNetManager(comp);
        }

        public void Notify_ConnectorRemoved()
        {
            ResetNetManager();
        }

        public void DeleteFoodNet(FoodNet oldNet) 
        {
            FoodNets.Remove(oldNet);
            oldNet.DestroyNet();
        }

        public FoodNet FoodNetAt(IntVec3 loc) 
        {
            foreach (FoodNet foodNet in this.FoodNets)
            {
                if (foodNet.netGrid[loc])
                {
                    return foodNet;
                }
            }
            return null;
        }


        public void Init(FoodTransmitter_ThingComp comp = null) 
        {
            IEnumerable<FoodTransmitter_ThingComp> FT_TCComps =
                from buildingWithComp
                in 
                (from building
                in this.map.listerBuildings.allBuildingsColonist
                where building.TryGetComp<FoodTransmitter_ThingComp>() is FoodTransmitter_ThingComp
                select building)
                select buildingWithComp.TryGetComp<FoodTransmitter_ThingComp>()
                into q
                where q != null
                select q;

            List<FoodTransmitter_ThingComp> allFT_TCComps = FT_TCComps.ToList();
            
            if (comp != null && !allFT_TCComps.Contains(comp))
                allFT_TCComps.Add(comp);

            FoodNets = MakeFoodNets(allFT_TCComps);
        }


        public void ResetNetManager(FoodTransmitter_ThingComp rootNode = null) 
        {
            foreach (FoodNet f in FoodNets) 
            {
                f.DestroyNet();
            }
            FoodNets.Clear();
            Init(rootNode);
        }

        public List<FoodNet> MakeFoodNets(List<FoodTransmitter_ThingComp> compsToProcess) 
        {
            List<List<FoodTransmitter_ThingComp>> foodNetsComps = new List<List<FoodTransmitter_ThingComp>>();

            while (compsToProcess.Count > 0)
            {
                if (compsToProcess.NullOrEmpty())
                {
                    Log.Error("Tried to make food net from null or empty comps!");
                    return null;
                }

                Queue<FoodTransmitter_ThingComp> queue = new Queue<FoodTransmitter_ThingComp>();
                List<FoodTransmitter_ThingComp> neighbors = new List<FoodTransmitter_ThingComp>();

                neighbors.Add(compsToProcess[0]);
                queue.Enqueue(compsToProcess[0]);
                compsToProcess.RemoveAt(0);


                while (queue.Count > 0)
                {
                    FoodTransmitter_ThingComp baseComp = queue.Dequeue();

                    // Accounts for things like valves being off and things which recieve connections but do not seek neighbor connections themselves
                    if (!baseComp.TransmitsFoodNow || baseComp.Props.isOneWay) 
                    {
                        compsToProcess.Remove(baseComp);
                        continue;
                    }
                       

                    if (baseComp.FoodNet != null)
                        Log.Error("Tried to add foodnet to a comp with an existing foodnet!");

                    //Get collection of Things in adjacent cells
                    IEnumerable<IEnumerable<Thing>> adjacentThingsCollections =
                        from c in GenAdj.CellsAdjacentCardinal(baseComp.parent)
                        select c.GetThingList(this.map).Distinct();

                    List<FoodTransmitter_ThingComp> listOfNeighboringComps = new List<FoodTransmitter_ThingComp>();

                    //Get all the FoodTransmitter_ThingComp for neighboring cells and add to list
                    foreach (var adjacentThings in adjacentThingsCollections)
                    {
                        foreach (var thing in adjacentThings)
                        {
                            if (thing.TryGetComp<FoodTransmitter_ThingComp>() is FoodTransmitter_ThingComp comp1 && 
                            comp1.AcceptConnectionFrom(baseComp.parent.Position))
                                listOfNeighboringComps.Add(comp1);
                        }
                    }

                    foreach (FoodTransmitter_ThingComp FT_TC in listOfNeighboringComps)
                    {
                        if (compsToProcess.Contains(FT_TC))
                        {
                            neighbors.Add(FT_TC);
                            queue.Enqueue(FT_TC);
                            compsToProcess.Remove(FT_TC);
                        }
                    }
                }

                foodNetsComps.Add(neighbors);
            }

            List<FoodNet> listOfFoodNets = new List<FoodNet>();

            foreach (var k in foodNetsComps) 
            {
                listOfFoodNets.Add(new FoodNet(k, this.map));
            }

            return listOfFoodNets;
        }


        public List<FoodNet> FoodNets = new List<FoodNet>();
    }
}
