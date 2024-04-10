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
    class JobDriver_ChatToReduceReluctance : JobDriver
    {
        protected Pawn Talkee
        {
            get
            {
                return (Pawn)this.job.targetA.Thing;
            }
        }

        protected override string ReportStringProcessed(string str)
        {
            //Add more based on JobDriver_ChatWithPrisoner
            return base.ReportStringProcessed(str);
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, errorOnFailed);
        }

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedOrNull(TargetIndex.A);
			this.FailOnMentalState(TargetIndex.A);
			this.FailOnNotAwake(TargetIndex.A);
			this.FailOn(() => !this.Talkee.IsPrisonerOfColony || !this.Talkee.guest.PrisonerIsSecure);
			yield return Toils_Interpersonal.GotoPrisoner(this.pawn, this.Talkee, this.Talkee.guest.ExclusiveInteractionMode);
			yield return Toils_Interpersonal.WaitToBeAbleToInteract(this.pawn);
			yield return Toils_Interpersonal.GotoInteractablePosition(TargetIndex.A);
			yield return Toils_Interpersonal.ConvinceRecruitee(this.pawn, this.Talkee, InteractionDefOf.BuildRapport);
			yield return Toils_Interpersonal.GotoPrisoner(this.pawn, this.Talkee, this.Talkee.guest.ExclusiveInteractionMode);
			yield return Toils_Interpersonal.GotoInteractablePosition(TargetIndex.A);
			yield return Toils_Interpersonal.ConvinceRecruitee(this.pawn, this.Talkee, Defs.InteractionDefOf.RR_ChatToReduceReluctance);
			yield return Toils_Interpersonal.GotoPrisoner(this.pawn, this.Talkee, this.Talkee.guest.ExclusiveInteractionMode);
			yield return Toils_Interpersonal.GotoInteractablePosition(TargetIndex.A);
			yield return Toils_Interpersonal.ConvinceRecruitee(this.pawn, this.Talkee, Defs.InteractionDefOf.RR_ChatToReduceReluctance);
			yield return Toils_Interpersonal.GotoPrisoner(this.pawn, this.Talkee, this.Talkee.guest.ExclusiveInteractionMode);
			yield return Toils_Interpersonal.GotoInteractablePosition(TargetIndex.A);
			yield return Toils_Interpersonal.ConvinceRecruitee(this.pawn, this.Talkee, Defs.InteractionDefOf.RR_ChatToReduceReluctance);
			yield return Toils_Interpersonal.GotoPrisoner(this.pawn, this.Talkee, this.Talkee.guest.ExclusiveInteractionMode);
			yield return Toils_Interpersonal.GotoInteractablePosition(TargetIndex.A);
			yield return Toils_Interpersonal.ConvinceRecruitee(this.pawn, this.Talkee, Defs.InteractionDefOf.RR_ChatToReduceReluctance);
			yield return Toils_Interpersonal.GotoPrisoner(this.pawn, this.Talkee, this.Talkee.guest.ExclusiveInteractionMode);
			yield return Toils_Interpersonal.GotoInteractablePosition(TargetIndex.A).FailOn(() => !this.Talkee.guest.ScheduledForInteraction);
			yield return Toils_Interpersonal.SetLastInteractTime(TargetIndex.A);
			yield break;
		}

	}
}
