using RimRound.Comps;
using RimRound.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRoundExtraWeapons.DamageWorker
{
    public class DamageWorker_Enbiggen : DamageWorker_AddInjury
    {

        ME_DamageWorker_Enbiggen Props => def.GetModExtension<ME_DamageWorker_Enbiggen>();
        protected override void ApplySpecialEffectsToPart(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageResult result)
        {

            if (!(pawn.RaceProps.Humanlike && pawn.WeightHediff() is Hediff))
            {
                base.ApplySpecialEffectsToPart(pawn, totalDamage, dinfo, result);
                return;
            }

            var comp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();

            if (comp is null)
            {
                base.ApplySpecialEffectsToPart(pawn, totalDamage, dinfo, result);
                return;
            }

            float kilosToAdd = totalDamage * Props.kilosPerDamage;

            int numberOfApplications = (int)Math.Floor(kilosToAdd / Props.kilosPerApplication);

            int currentTick = Find.TickManager.TicksGame;

            for (int i = 0; i < numberOfApplications; ++i)
            {
                comp.activeWeightGainRequests.Enqueue(new WeightGainRequest(Props.kilosPerApplication, currentTick + i * Props.ticksBetweenApplication, Props.effectDuration, Props.triggerMessages));
            }

            float kilosLeft = kilosToAdd % Props.kilosPerApplication;

            if (kilosLeft > 0)
            {
                int finalApplicationTick = currentTick + numberOfApplications * Props.ticksBetweenApplication;

                comp.activeWeightGainRequests.Enqueue(new WeightGainRequest(kilosLeft, finalApplicationTick, Props.effectDuration, Props.triggerMessages));
            }

            //Log.Message("totalDamage: " + totalDamage
            //    + "\n" + "previousDamage: " + dinfo.Amount
            //    + "\n" + "kilosPerDamage: " + Props.kilosPerDamage
            //    + "\n" + "kilosToAdd: " + kilosToAdd
            //    + "\n" + "kilosPerApplication: " + Props.kilosPerApplication
            //    + "\n" + "numberOfApplications: " + numberOfApplications
            //    + "\n" + "kilosLeft: " + kilosLeft
            //    + "\n" + "instantPermanentInjury: " + dinfo.InstantPermanentInjury
            //    + "\n" + "ignoredArmor: " + dinfo.IgnoreArmor);
        }
    }
}
