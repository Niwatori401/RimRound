using RimRound.AI;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RimRound.Utilities.HungerDroneUtility;

namespace RimRound.Comps
{
    class CompProperties_CausesGameCondition_HungerDrone : CompProperties_CausesGameCondition
    {
        public CompProperties_CausesGameCondition_HungerDrone()
        {
            this.compClass = typeof(CompCauseGameCondition_HungerDrone);
        }
        public HungerDroneLevel droneLevel;
        internal int droneLevelIncreaseInterval = int.MinValue;
    }
}
