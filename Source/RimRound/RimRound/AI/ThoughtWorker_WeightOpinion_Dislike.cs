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
    public class ThoughtWorker_WeightOpinion_Dislike : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            return GetActiveThought(p);
        }

        private ThoughtState GetActiveThought(Pawn p)
        {
            if (!WeightOpinionUtility.ShouldHaveThisKindOfThought(this, p, WeightOpinion.Dislike))
                return false;

            int index = WeightOpinionUtility.GetThoughtIndex(p);

            return ThoughtState.ActiveAtStage(index);
        }


    }
}
