using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.FeedingTube
{
    public class Building_AdvancedAutoFeeder : Building_AutoFeeder
    {
        public override Vector3 GetAutoFeederLocationVector()
        {
            Vector3 autoFeederHoseAttachPoint = this.TrueCenter();

            autoFeederHoseAttachPoint.y = AltitudeLayer.MetaOverlays.AltitudeFor();

            autoFeederHoseAttachPoint.x += 0.05f;
            autoFeederHoseAttachPoint.z += 0.43f;

            return autoFeederHoseAttachPoint;
        }
    }
}
