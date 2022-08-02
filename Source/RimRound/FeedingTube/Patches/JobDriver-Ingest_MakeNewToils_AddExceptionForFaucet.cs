using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using RimWorld;
using Verse.AI;
using System.Reflection;

namespace RimRound.FeedingTube.Patches
{
    [HarmonyPatch(typeof(JobDriver_Ingest))]
    [HarmonyPatch("MakeNewToils")]
    public class JobDriver_Ingest_MakeNewToils_AddExceptionForFaucet
    {
        static MethodInfo ChewDurationMultiplier = typeof(JobDriver_Ingest).GetProperty("ChewDurationMultiplier", BindingFlags.Instance | BindingFlags.NonPublic).GetGetMethod(true);
        static MethodInfo IngestibleSourceGetter = typeof(JobDriver_Ingest).GetProperty("IngestibleSource", BindingFlags.Instance | BindingFlags.NonPublic).GetGetMethod(true);
        static MethodInfo PrepareToIngestToils = typeof(JobDriver_Ingest).GetMethod("PrepareToIngestToils", BindingFlags.Instance | BindingFlags.NonPublic);


        public static bool Prefix(JobDriver_Ingest __instance, ref IEnumerable<Toil> __result) 
        {
            if ((__instance.job.GetTarget(TargetIndex.A).Thing) is Building_FoodFaucet || (__instance.job.GetTarget(TargetIndex.A).Thing).def == Defs.ThingDefOf.RR_FeedingTubeFluid) 
            {
                List<Toil> toilsToReturn = new List<Toil>();
                Thing ingestibleSource = (Thing)(IngestibleSourceGetter.Invoke(__instance, null));
                float chewDurationMultiplier = (float)ChewDurationMultiplier.Invoke(__instance, null);

                chewDurationMultiplier = 0.1f;

                Toil chew = Toils_Ingest.ChewIngestible(
                    __instance.pawn,
                    chewDurationMultiplier, 
                    TargetIndex.A, 
                    TargetIndex.B).FailOn(
                        (Toil x) => !ingestibleSource.Spawned && 
                        (__instance.pawn.carryTracker == null ||
                        __instance.pawn.carryTracker.CarriedThing != ingestibleSource)).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);

                for (int i = 0; i < 1; i++)
                {
                    foreach (Toil toil in (IEnumerable<Toil>)PrepareToIngestToils.Invoke(__instance, new object[] { chew }))
                    {
                        toilsToReturn.Add(toil);
                        Thing test5 = __instance.job.GetTarget(TargetIndex.A).Thing;
                    }

                    toilsToReturn.Add(chew);
                    Thing test4 = __instance.job.GetTarget(TargetIndex.A).Thing;
                    toilsToReturn.Add(Toils_Ingest.FinalizeIngest(__instance.pawn, TargetIndex.A));
                    Thing test3 = __instance.job.GetTarget(TargetIndex.A).Thing;
                    toilsToReturn.Add(Toils_Jump.JumpIf(chew, () => __instance.job.GetTarget(TargetIndex.A).Thing is Corpse && __instance.pawn.needs.food.CurLevelPercentage < 0.9f));
                    Thing test2 = __instance.job.GetTarget(TargetIndex.A).Thing;
                }

                __result = toilsToReturn;

                return false;
            }
            return true;
        }
    }
}
