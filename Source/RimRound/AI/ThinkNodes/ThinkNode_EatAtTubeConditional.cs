using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RimRound.AI
{
    public class ThinkNode_EatAtTubeConditional : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            return false;
        }
    }
}
