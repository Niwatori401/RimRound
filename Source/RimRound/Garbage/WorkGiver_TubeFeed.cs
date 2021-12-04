using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using Verse;
using Verse.AI;

namespace RimRound
{
    class WorkGiver_TubeFeed : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest 
        {
            get 
            {
                return ThingRequest.ForGroup(ThingRequestGroup.FoodDispenser);
            }
        }

        public override Danger MaxPathDanger(Pawn pawn)
        {
            return base.MaxPathDanger(pawn);
        }

        public override PathEndMode PathEndMode 
        {
            get 
            {
                return PathEndMode.InteractionCell;
            }
        }

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn) 
        {
            return pawn.Map.listerThings.ThingsOfDef(ThingDefOf.NutrientPasteDispenser);
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!t.Spawned || t.IsForbidden(pawn))
            {
                return false;
            }
            if (!pawn.CanReserve(t, 1, -1, null, forced))
            {
                return false;
            }

            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Job job = JobMaker.MakeJob(JobDefOf.Ingest, t);
            job.count = finalIngestibleDef.ingestible.maxNumToIngestAtOnce;
            job.ignoreForbidden = this.IgnoreForbid(pawn);
            job.overeat = true;
            return job;
        }
    }
}
