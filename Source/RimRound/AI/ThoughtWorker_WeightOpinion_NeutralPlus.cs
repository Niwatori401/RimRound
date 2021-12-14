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
    class ThoughtWorker_WeightOpinion_NeutralPlus : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (!WeightOpinionUtility.ShouldHaveThisKindOfThought(this, p, WeightOpinion.NeutralPlus))
                return false;

            int index = WeightOpinionUtility.GetThoughtIndex(p);

            return ThoughtState.ActiveAtStage(index);
        }
    }
}
