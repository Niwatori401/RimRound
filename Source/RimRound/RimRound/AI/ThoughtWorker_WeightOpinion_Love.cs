using RimRound.Comps;
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
    public class ThoughtWorker_WeightOpinion_Love : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (!WeightOpinionUtility.ShouldHaveThisKindOfThought(this, p, WeightOpinion.Love))
                return false;

            int index = WeightOpinionUtility.GetThoughtIndex(p);

            return ThoughtState.ActiveAtStage(index);
        }
    }
}
