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
    public class NutrientDistillery_PlaceWorker : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            base.DrawGhost(def, center, rot, ghostCol, thing);

            IntVec3 input = center + IntVec3.West + IntVec3.South;
            IntVec3 output = center + 2 * IntVec3.East + IntVec3.South;
            GenDraw.DrawFieldEdges(new List<IntVec3>
            {
                input
            }, GenTemperature.ColorSpotCold, null);
            GenDraw.DrawFieldEdges(new List<IntVec3>
            {
                output
            }, GenTemperature.ColorSpotHot, null);
        }
    }
}

