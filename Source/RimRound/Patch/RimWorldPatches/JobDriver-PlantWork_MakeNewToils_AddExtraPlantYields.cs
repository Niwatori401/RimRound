using HarmonyLib;
using RimRound.Defs;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(JobDriver_PlantHarvest))]
    [HarmonyPatch("PlantWorkDoneToil")]
    class JobDriver_PlantWork_MakeNewToils_AddExtraPlantYields : ThingDef
    {
        public static bool Prefix(ref Toil __result) 
        {
            __result = GiveExtraYieldsAndDestroyPlant();

            return false;
        }

        static Toil GiveExtraYieldsAndDestroyPlant() 
        {
            Toil toil = new Toil();
            toil.initAction = delegate ()
            {
                Thing thing = toil.actor.jobs.curJob.GetTarget(TargetIndex.A).Thing;
                ExtraProductPlantYield modExtension = thing.def.GetModExtension<ExtraProductPlantYield>();
                if (modExtension != null) 
                {
                    Pair<ThingDef, int>? pair = modExtension.GetThingDefQuantityPair();
                    if (pair != null) 
                    {
                        Pair<ThingDef, int> pair2 = pair.AsNonNullable();

                        Thing newThing = ThingMaker.MakeThing(pair2.First, null);
                        newThing.stackCount = pair2.Second;

                        GenPlace.TryPlaceThing(newThing, toil.actor.Position, toil.actor.Map, ThingPlaceMode.Near);
                    }

                }


                toil.actor.Map.designationManager.RemoveAllDesignationsOn(thing, false);

            };
            return toil;
        }




    }
}
