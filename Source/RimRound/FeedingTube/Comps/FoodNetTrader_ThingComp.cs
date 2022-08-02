using RimRound.FeedingTube.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.FeedingTube.Comps
{
    public class FoodNetTrader_ThingComp : FoodTransmitter_ThingComp
    {
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            flickComp = this.parent.TryGetComp<CompFlickable>();
            breakdownComp = this.parent.TryGetComp<CompBreakdownable>();
            compPowerTrader = this.parent.TryGetComp<CompPowerTrader>();
        }

        public new FoodNetTrader_CompProperties Props 
        {
            get 
            {
                return this.props as FoodNetTrader_CompProperties;
            }
        }

        public float ChangePerSecond 
        {
            get 
            {
                if (isOn)
                    return Props.flowDelta;
                else
                    return Props.idleFlowDelta;
            }
        }

        public float ChangePerTick
        {
            get 
            {
                if (isOn)
                    return Props.flowDelta / 60f;
                else
                    return Props.idleFlowDelta / 60f;
            }
        }

        public bool IsOn 
        {
            get 
            {
                return this.isOn;
            }
            set 
            {
                this.isOn = value;
            } 
        }

        public bool CanBeOn 
        {
            get 
            {
                return (flickComp != null ? 
                        flickComp.SwitchIsOn : true) &&
                       (breakdownComp != null ? 
                       !breakdownComp.BrokenDown : true) &&
                       (compPowerTrader != null ?
                        compPowerTrader.PowerOn : true);
            }
        }

        public override void CompTick()
        {
            if (FeedingTubeUtility.IsHashIntervalTick(tickInterval) && CanBeOn)
            {
                IsOn = true;
            }
        }


        public override void ReceiveCompSignal(string signal)
        {
            base.ReceiveCompSignal(signal);
            if (signal == "Breakdown" || signal == "FlickedOff")
                IsOn = false;
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(base.CompInspectStringExtra());
            sb.AppendLine("RR_FT_TraderUsing".Translate(ChangePerSecond.ToString("F1").Named("CONSUMPTIONRATE")));

            return sb.ToString().Trim();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref isOn, "FoodOn", false);
        }

        CompBreakdownable breakdownComp;
        CompPowerTrader compPowerTrader;
        CompFlickable flickComp;

        private bool isOn = true;

        private const int tickInterval = 30;
    }
}
