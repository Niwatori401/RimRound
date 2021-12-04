using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Sound;

namespace RimRound
{
    public class Building_FoodFaucet : Building
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            powerComp = base.GetComp<CompPowerTrader>();
            foodNetTrader = base.GetComp<FoodNetTrader_ThingComp>();
        }

        public Thing TryDispenseFood() 
        {
            if (foodNetTrader.CanBeOn)
            {
                foodNetTrader.IsOn = true;

                if (foodNetTrader.FoodNet.Stored < litersPerDispense)
                    return null;

                this.def.building.soundDispense.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
                Thing thing = ThingMaker.MakeThing(DispensableDef, null);
                foodNetTrader.FoodNet.Drain(litersPerDispense);
                return thing;
            }
            else 
            {
                foodNetTrader.IsOn = false;
                return null;
            }
        }

        public ThingDef DispensableDef 
        {
            get 
            {
                return Defs.ThingDefOf.RR_FeedingTubeFluid;
            }
        }

        public float litersPerDispense = 0.1f;

        //public int ticksPerDispense = 60;

        public CompPowerTrader powerComp;

        public FoodNetTrader_ThingComp foodNetTrader;
    }
}
