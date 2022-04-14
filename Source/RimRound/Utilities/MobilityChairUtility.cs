using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Utilities
{
    class MobilityChairUtility
    {
        public static bool IsWearingAMobilityScooter(Pawn p)
        {
            Pawn_ApparelTracker apparelTracker = p.apparel;

            bool? isWearingMobilityScooter = apparelTracker?.WornApparel.Any((Apparel x) => x.def.defName == "RR_HoverChair" || x.def.defName == "RR_HoverChairArm");

            if (isWearingMobilityScooter.GetValueOrDefault())
                return true;
            else
                return false;
        }

        public static PawnPosture GetAppropriatePostureGivenWearingChair(Pawn p)
        {
            if (p.Dead || p.Downed)
                return PawnPosture.LayingOnGroundFaceUp;

            return PawnPosture.Standing;
        }
    }
}
