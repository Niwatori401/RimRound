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
    public class HistoryAutoRecorderWorker_PawnWeight : HistoryAutoRecorderWorker
    {
        public override float PullRecord()
        {
            if (currentPawn is null)
                return 0;

            return currentPawn.Weight();
        }

        public Pawn currentPawn;
    }
}
