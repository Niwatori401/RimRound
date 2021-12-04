using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimRound
{
    public class FoodNetTrader_CompProperties : FoodTransmitter_CompProperties
    {
        //Usage or gain per second. Usage is negative, gain is positive
        public float flowDelta = 0;

        public float idleFlowDelta = 0;

        public float buffer = 0;
    }
}
