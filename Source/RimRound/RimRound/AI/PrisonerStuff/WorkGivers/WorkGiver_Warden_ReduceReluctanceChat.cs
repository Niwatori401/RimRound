using RimRound.Comps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RimRound.AI
{
    public class WorkGiver_Warden_ReduceReluctanceChat : WorkGiver_Warden
    {
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
			if (!base.ShouldTakeCareOfPrisoner(pawn, t, false))
			{
				return null;
			}
			Pawn prisoner = (Pawn)t;
			PrisonerInteractionModeDef interactionMode = prisoner.guest.ExclusiveInteractionMode;
			if ((interactionMode != Defs.PrisonerInteractionModeDefOf.RR_Fatten) || !prisoner.guest.ScheduledForInteraction || !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking) || (prisoner.Downed && !prisoner.InBed()) || !pawn.CanReserve(t, 1, -1, null, false) || !prisoner.Awake())
			{
				return null;
			}
			var paComp = prisoner.TryGetComp<ThingComp_PawnAttitude>();
			if (paComp is null || paComp.GainingResistance <= 0f)
			{
				return null;
			}

			return JobMaker.MakeJob(Defs.JobDefOf.RR_PrisonerAttemptReduceReluctance, t);
		}
    }
}
