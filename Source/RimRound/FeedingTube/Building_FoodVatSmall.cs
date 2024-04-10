using RimRound.FeedingTube.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.FeedingTube
{
    [StaticConstructorOnStartup]
    public class Building_FoodVatSmall : Building
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            comp = base.GetComp<FoodNetStorage_ThingComp>();
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            Vector3 center = this.DrawPos + Vector3.up * 0.1f;
            center.x += BarXOFF;
            center.z += BarYOFF;
            base.DrawAt(drawLoc, flip);
            GenDraw.FillableBarRequest r = default;
            r.center = center;
            r.size = BarSize;
            r.fillPercent = comp.Stored / comp.Capacity;
            r.filledMat = Building_FoodVatSmall.StorageBarFilledMat;
            r.unfilledMat = Building_FoodVatSmall.StorageBarUnfilledMat;
            r.margin = 0.05f;
            Rot4 rotation = base.Rotation;
            //rotation.Rotate(RotationDirection.Clockwise);
            r.rotation = rotation;
            GenDraw.DrawFillableBar(r);
        }

        FoodNetStorage_ThingComp comp;

        public Vector2 BarSize = new Vector2(0.3f, 0.1f);

        public float BarYOFF = -0.19f;
        public float BarXOFF = 0f;

        private static readonly Material StorageBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.99f, 0.96f, 0.90f), false);

        private static readonly Material StorageBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f), false);

    }
}
