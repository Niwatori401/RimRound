using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RimRound.FeedingTube.AI
{
    public class JobGiver_EatAtTube : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            return JobMaker.MakeJob(JobDefOf.Ingest);
        }

		private Job IngestJob(Pawn pawn)
		{
			Thing thing = this.BestIngestTarget(pawn);
			if (thing == null)
			{
				return null;
			}
			ThingDef finalIngestibleDef = FoodUtility.GetFinalIngestibleDef(thing, false);
			Job job = JobMaker.MakeJob(JobDefOf.Ingest, thing);
			job.count = finalIngestibleDef.ingestible.maxNumToIngestAtOnce;
			//job.overeat = true;
			return job;
		}

		protected Thing BestIngestTarget(Pawn pawn)
		{
			Thing result;
			ThingDef thingDef;
			#pragma warning disable CS0612 // Type or member is obsolete
            if (FoodUtility.TryFindBestFoodSourceFor(pawn, pawn, false, out result, out thingDef, false, true, true, true, true, false, false, false, false, FoodPreferability.RawTasty))
			#pragma warning restore CS0612 // Type or member is obsolete
            {
				return result;
			}
			return null;
		}
	}
}
