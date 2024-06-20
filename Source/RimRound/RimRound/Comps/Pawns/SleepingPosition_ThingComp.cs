using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

using RimRound.UI;
using RimRound.Utilities;

namespace RimRound.Comps
{
    public class SleepingPosition_ThingComp : ThingComp
    {
        public SleepingPosition_ThingComp() : base()
        {
            sleepPositionGizmo = new SleepingPositionGizmo(this);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (!this.parent.AsPawn().IsColonist && !this.parent.AsPawn().IsPrisonerOfColony && !Prefs.DevMode)
                yield break;

            if (GlobalSettings.showSleepPostureManagementGizmo)
                yield return sleepPositionGizmo;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.sleepIndex, "sleepingPos", 0, false);
        }

        public int sleepIndex;
        readonly SleepingPositionGizmo sleepPositionGizmo;
    }
}
