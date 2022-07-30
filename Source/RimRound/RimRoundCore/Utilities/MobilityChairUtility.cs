using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RimRound.Utilities
{
    class MobilityChairUtility
    {
        public static bool IsWearingAMobilityScooter(Pawn p)
        {
            bool? isWearingMobilityScooter = (GetMobilityScooter(p) != null);

            if (isWearingMobilityScooter.GetValueOrDefault())
                return true;
            else
                return false;
        }

        public static PawnPosture GetAppropriatePostureGivenWearingChair(Pawn p)
        {
            if (p.Dead || p.Downed)
                return PawnPosture.LayingOnGroundFaceUp;


            if (IsInABedForScooterPurposes(p))
                return PawnPosture.LayingInBed;

            return PawnPosture.Standing;
        }
        public static Apparel GetMobilityScooter(Pawn p)
        {
            Apparel scooter = p?.apparel?.WornApparel?.Find((Apparel x) => x.def.defName == "RR_HoverChair" || x.def.defName == "RR_HoverChairArm");
            return scooter;
        }

        static MethodInfo CurToilMI = typeof(JobDriver).GetProperty("CurToil", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
        public static bool IsInABedForScooterPurposes(Pawn p) 
        {
            if (!(p.jobs.curDriver is JobDriver_LayDown jd))
                return false;
                
            if (!jd.asleep)
                return false;


            Building_Bed building_Bed = null;
            List<Thing> thingList = p.Position.GetThingList(p.Map);
            for (int i = 0; i < thingList.Count; i++)
            {
                building_Bed = (thingList[i] as Building_Bed);
                if (building_Bed != null)
                {
                    break;
                }
            }
            if (building_Bed == null)
            {
                return false;
            }
            for (int j = 0; j < building_Bed.SleepingSlotsCount; j++)
            {
                Func<int, Building_Bed, Pawn> GetCurOccupantLambda = (int slotIndex, Building_Bed b) =>
                {
                    if (!b.Spawned)
                    {
                        return null;
                    }
                    IntVec3 sleepingSlotPos = b.GetSleepingSlotPos(slotIndex);
                    List<Thing> list = b.Map.thingGrid.ThingsListAt(sleepingSlotPos);
                    for (int i = 0; i < list.Count; i++)
                    {
                        Pawn pawn = list[i] as Pawn;
                        if (pawn != null && pawn.CurJob != null)
                        {
                            return pawn;
                        }
                    }
                    return null;
                };

                if (GetCurOccupantLambda(j, building_Bed) == p)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
