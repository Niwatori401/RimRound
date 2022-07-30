using RimRound.FeedingTube.Comps;
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
    public class SectionLayer_ThingsFoodGrid : SectionLayer_Things
    {
        public SectionLayer_ThingsFoodGrid(Section section) : base(section)
        {
            this.requireAddToMapMesh = false;
            this.relevantChangeTypes = MapMeshFlag.Things;
        }

        public override void DrawLayer()
        {
            if (ShouldDrawFoodGrid)
            {
                base.DrawLayer();
            }
        }

        protected override void TakePrintFrom(Thing t)
        {
            if (t.Faction != null && t.Faction != Faction.OfPlayer)
            {
                return;
            }
            
            if (t is Building building && building.TryGetComp<FoodTransmitter_ThingComp>() is FoodTransmitter_ThingComp)
            {
                FeedingTube.Resources.FoodOverlay.Print(this, t, 0.0f);
            }
        }


        //From OverlayDrawHandler

        public static void DrawFoodGridOverlayThisFrame()
        {
            lastFoodGridDrawFrame = Time.frameCount;
        }


        public static void DrawZonesThisFrame()
        {
            lastZoneDrawFrame = Time.frameCount;
        }

        public static bool ShouldDrawFoodGrid
        {
            get
            {
                return lastFoodGridDrawFrame + 1 >= Time.frameCount;
            }
        }

        private static int lastFoodGridDrawFrame;

        private static int lastZoneDrawFrame;

    }
}
