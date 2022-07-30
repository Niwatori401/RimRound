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
    class StatPart_MassAdjustedByWeight : StatPart
    {
        public override string ExplanationPart(StatRequest req)
        {
            return null;
        }

        public override void TransformValue(StatRequest req, ref float val)
        {
            if (req.Thing is Pawn pawn)
            {
                if (pawn.RaceProps.Humanlike)
                {
                    float? weight = pawn.Weight();
                    val = weight != null ? (float)weight : val;
                }
            }
        }
    }
}
