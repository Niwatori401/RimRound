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


                foreach (Toil toil in (IEnumerable<Toil>)PrepareToIngestToils.Invoke(__instance, new object[] { chew }))
                {
                    toilsToReturn.Add(toil);
                }

                toilsToReturn.Add(chew);

                toilsToReturn.Add(Toils_Ingest.FinalizeIngest(__instance.pawn, TargetIndex.A));
                toilsToReturn.Add(Toils_Jump.JumpIf(chew, () => __instance.job.GetTarget(TargetIndex.A).Thing is Corpse && __instance.pawn.needs.food.CurLevelPercentage < 0.9f));

                __result = toilsToReturn;

                return false;
            }
            return true;
        }

        /*
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) 
        {
            List<CodeInstruction> code = new List<CodeInstruction>(instructions);

            int insertionIndex = -1;



            return code;
        }
        */
    }
}
