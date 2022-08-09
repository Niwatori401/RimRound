using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimRound.Defs;
using RimRound.Utilities;
using RimWorld;
using UnityEngine;

namespace RimRound.Hediffs
{
    public class Hediff_Fullness : Hediff
    {
        float _personalFullnessPainMult = 1f;
        float _personalFullnessMovementMult = 1f;
        float _personalFullnessEatingMult = 1f;

        public float PersonalFullnessPainMult
        {
            get 
            {
                return Mathf.Clamp(_personalFullnessPainMult, 0f, 10f); ;
            }
            set 
            {
                _personalFullnessPainMult = value;
            }
        }

        public float PersonalFullnessMovementMult
        {
            get
            {
                return Mathf.Clamp(_personalFullnessMovementMult, 0f, 10f); ;
            }
            set
            {
                _personalFullnessMovementMult = value;
            }
        }

        public float PersonalFullnessEatingSpeedMult
        {
            get
            {
                return Mathf.Clamp(_personalFullnessEatingMult, 0f, 10f); ;
            }
            set
            {
                _personalFullnessEatingMult = value;
            }
        }


        public override float PainOffset
        {
            get
            {
                return (base.PainOffset * GlobalSettings.fullnessHediffPainMult.threshold * _personalFullnessPainMult);
            }
        }
    }
}
