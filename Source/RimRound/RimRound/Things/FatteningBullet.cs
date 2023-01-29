using RimRound.Comps;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Things
{
    public class FatteningBullet : Bullet
    {
        ME_FatteningBullet Props => def.GetModExtension<ME_FatteningBullet>();

        protected override void Impact(Thing hitThing, bool BlockedByShield = false)
        {
            base.Impact(hitThing);

            if (Values.RandomFloat(0, 1) <= Props.chanceToInflictIllness)
                return;

            if (!(hitThing is Pawn p && p.RaceProps.Humanlike))
                return;

            if (!(p.WeightHediff() is Hediff))
                return;

            var comp = p.TryGetComp<FullnessAndDietStats_ThingComp>();

            if (comp is null)
                return;

            float kilosPerApplication = Props.kilosToAdd / Props.numberOfApplications;

            int currentTick = Find.TickManager.TicksGame;

            for (int i = 0; i < Props.numberOfApplications; ++i) 
            {
                comp.activeWeightGainRequests.Enqueue(new WeightGainRequest(kilosPerApplication, currentTick + i * Props.ticksBetweenApplication, Props.effectDuration, Props.triggerMessages));
            }    
        }
    }
}
