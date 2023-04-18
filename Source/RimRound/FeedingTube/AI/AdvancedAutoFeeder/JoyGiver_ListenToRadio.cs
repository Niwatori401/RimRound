using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimRound.FeedingTube.AI
{
    public class JoyGiver_ListenToRadio : JoyGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            return null;
        }

        public override Job TryGiveJobWhileInBed(Pawn pawn)
        {
            Job result;
            try
            {
                advancedAutoFeederCandidates.AddRange(pawn?.Map?.listerThings?.ThingsMatching(ThingRequest.ForDef(Defs.ThingDefOf.RR_AdvancedAutoFeeder))?.Where(delegate (Thing thing)
                {
                    if (!(thing is Building_AdvancedAutoFeeder autoFeeder))
                        return false;

                    if (autoFeeder.CurrentPawn.Name == pawn.Name)
                        return true;
                    else
                        return false;
                }));

                if (advancedAutoFeederCandidates.Count() >= 1)
                    result = JobMaker.MakeJob(this.def.jobDef, advancedAutoFeederCandidates[0], pawn.Position, pawn.Position.GetEdifice(pawn.Map));
                else
                    result = null;
                
            }
            finally
            {
                advancedAutoFeederCandidates.Clear();
            }
            return result;
        }



        private List<Thing> advancedAutoFeederCandidates = new List<Thing>();
    }
}
