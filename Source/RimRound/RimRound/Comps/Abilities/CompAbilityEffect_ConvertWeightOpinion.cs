using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimRound.Comps
{
    public class CompAbilityEffect_ConvertWeightOpinion : CompAbilityEffect
    {
        public new CompProperties_ConvertWeightOpinion Props
        {
            get
            {
                return (CompProperties_ConvertWeightOpinion)this.props;
            }
        }
        public override bool HideTargetPawnTooltip
        {
            get => true;
        }

        public override bool ShouldHideGizmo => base.ShouldHideGizmo;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Log.Message("Ran LocalTarget");
            Pawn initiator = this.parent.pawn;
            Pawn targetPawn = target.Pawn;

            float certaintyChange = GetWeightOpinionFloatChange(initiator, targetPawn) * this.Props.convertPowerFactor;
            float thresholdForMajorChange = 20;
            //float certainty = targetPawn.ideo.Certainty;

            //if (AttemptConversion(initiator, targetPawn, certaintyChange))
            //{
            string message = certaintyChange > 0 ? 
                (certaintyChange > thresholdForMajorChange ? this.Props.majorIncrease : this.Props.minorIncrease) : 
                (Mathf.Abs(certaintyChange) > thresholdForMajorChange ? this.Props.majorDecrease : this.Props.minorDecrease);

            Messages.Message(message.Formatted(initiator.Named("INITIATOR"), targetPawn.Named("RECIPIENT"), initiator.Ideo.name.Named("IDEO")), new LookTargets(new Pawn[]
            {
                initiator,
                targetPawn
            }), MessageTypeDefOf.PositiveEvent, false);

                //initiator.needs.mood.thoughts.memories.TryGainMemory(this.Props.failedThoughtInitiator, targetPawn, null);
                //targetPawn.needs.mood.thoughts.memories.TryGainMemory(this.Props.failedThoughtRecipient, initiator, null);
                
            //}
            if (this.Props.sound != null)
            {
                this.Props.sound.PlayOneShot(new TargetInfo(target.Cell, this.parent.pawn.Map, false));
            }
        }

        float GetWeightOpinionFloatChange(Pawn initiator, Pawn recipient)
        {
            throw new NotImplementedException();
        }


        /// <returns>true if pawn opinion is changed successfully, false otherwise.</returns>
        bool AttemptConversion(Pawn initiator, Pawn recipient, float amountToChangeBy) 
        {
            throw new NotImplementedException();
        }

        public override void Apply(GlobalTargetInfo target)
        {
            base.Apply(target);
            Log.Message("Ran GlobalTarget");
        }

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            return base.CanApplyOn(target, dest);
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            return base.Valid(target, throwMessages);
        }

        public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
        {
            return base.ExtraLabelMouseAttachment(target);
        }
    }
}
