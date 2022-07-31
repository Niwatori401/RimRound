using RimRound.FeedingTube.Comps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Sound;

namespace RimRound.FeedingTube
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

                if (foodNetTrader.FoodNet.Stored * foodNetTrader.FoodNet.fullnessToNutritionRatio < nutritionPerDispense)
                    return null;

                this.def.building.soundDispense.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
                Thing thing = ThingMaker.MakeThing(DispensableDef, null);
                var comp = thing.TryGetComp<RimRound.Comps.ThingComp_FoodItems_NutritionDensity>();
                
                if (comp != null)
                    comp.Props.fullnessToNutritionRatio = foodNetTrader.FoodNet.fullnessToNutritionRatio;

                foodNetTrader.FoodNet.Drain(nutritionPerDispense * foodNetTrader.FoodNet.fullnessToNutritionRatio);
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

        public float nutritionPerDispense = 0.1f;


        public CompPowerTrader powerComp;

        public FoodNetTrader_ThingComp foodNetTrader;
    }
}
