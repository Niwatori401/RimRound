using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RimRound.FeedingTube.Patches
{
    [HarmonyPatch(typeof(JobDriver_Ingest))]
    [HarmonyPatch("PrepareToIngestToils")]
    public class JobDriver_Ingest_PrepareToIngestToils_AddFaucetSupport
    {
        public static bool Prefix(JobDriver_Ingest __instance, ref IEnumerable<Toil> __result) 
        {
            if (__instance.job.GetTarget(TargetIndex.A).Thing is Building_FoodFaucet)
            {
                List<Toil> tempToilList = new List<Toil>();

                tempToilList.Add(Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell).FailOnDespawnedNullOrForbidden(TargetIndex.A));
                tempToilList.Add(GrabFoodFromFaucet(TargetIndex.A, __instance.pawn, __instance));

                __result = tempToilList;
                return false;
            }
            return true;
        }

        public static Toil GrabFoodFromFaucet(TargetIndex ind, Pawn eater, JobDriver_Ingest instance) 
        {
            Toil toil = new Toil();

            toil.initAction = delegate ()
            {
                Pawn actor = toil.actor;
                toil.actor.rotationTracker.FaceTarget(instance.job.GetTarget(ind));
                Thing thing = ((Building_FoodFaucet)actor.jobs.curJob.GetTarget(ind).Thing).TryDispenseFood();
                if (thing == null)
                {
                    actor.jobs.curDriver.EndJobWith(JobCondition.Incompletable);
                    return;
                }
                actor.carryTracker.TryStartCarry(thing);
                actor.CurJob.SetTarget(ind, actor.carryTracker.CarriedThing);
            };
            
            toil.FailOnCannotTouch(ind, PathEndMode.Touch);
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            //Change this value.
            toil.defaultDuration = 10;//Building_NutrientPasteDispenser.CollectDuration;
            return toil;
        }
    }
}
