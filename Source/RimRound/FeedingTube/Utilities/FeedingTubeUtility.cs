using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.FeedingTube.Utilities
{
    internal class FeedingTubeUtility
    {
        internal static bool IsHashIntervalTick(int interval)
        {
            return Find.TickManager.TicksGame % interval == 0;
        }

        internal const float MinRQ = 0.000001f;
    }
}
