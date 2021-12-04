using RimRound.Comps;
using RimRound.Enums;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.AI
{
    public class ThoughtWorker_WeightOpinion_Like : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (!p.RaceProps.Humanlike)
            {
                return false;
            }

            if ((p.TryGetComp<ThingComp_PawnAttitude>()?.weightOpinion ?? WeightOpinion.None) != WeightOpinion.Like)
                return false;
            else
                this.def = Defs.ThoughtDefOf.RR_WeightOpinion_Like;

            if (ThoughtUtility.ThoughtNullified(p, def))
                return false;

            float weightSeverity = Functions.GetHediffOfDefFrom(Defs.HediffDefOf.RimRound_Weight, p)?.Severity ?? 0;

            if (weightSeverity == 0)
                return false;

            int index = 0;

            foreach (var x in Values.severityToThoughtIndex) 
            {
                if (weightSeverity <= x.Key) 
                {
                    index = x.Value;
                    break;
                }
            }

            if (weightSeverity >= Values.severityToThoughtIndex.Last().Key)
                index = Values.severityToThoughtIndex.Last().Value;

            return ThoughtState.ActiveAtStage(index);
        }
    }
}
