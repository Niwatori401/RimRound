using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Defs
{
    public class ClothingModExtension : DefModExtension
    {
        public float weightGainMultiplierMultBonus;
        public float digestionSpeedMultBonus;
        public float stomachElasticityMultBonus;
        public float fullnessGainedMultiplierBonus;
        public float flatMoveBonus;
        public float flatManipulationBonus;
        public float flatEatingSpeedBonus;

        //All below should be in range [0, 1]. 0 means full mitigation, 1 means none.
        public float movementPenaltyMitigationMultBonus_Weight;
        public float movementPenaltyMitigationMultBonus_Fullness;
        public float manipulationPenaltyMitigationMultBonus_Weight;
        public float painMitigationMultBonus_Fullness;
        public float eatingSpeedReductionMitigationMultBonus_Fullness;
    }
}
