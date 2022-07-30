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
    public class Graphic_LinkedFood : Graphic_Linked
    {
        public Graphic_LinkedFood(Graphic subGraphic) : base(subGraphic) 
        {
            
        }

        public override bool ShouldLinkWith(IntVec3 c, Thing parent)
        {
            return c.InBounds(parent.Map) && FoodTransmitter_NetManager.For(parent.Map).FoodNetAt(c) != null;
        }


        public override void Print(SectionLayer layer, Thing thing, float extraRotation)
        {
            base.Print(layer, thing, extraRotation);

            IEnumerable<IntVec3> cellsWithWires =
                from adjCell
                in GenAdj.CellsAdjacentCardinal(thing)
                where
                (from y
                in adjCell.GetThingList(thing.Map)
                 where thing.TryGetComp<FoodTransmitter_ThingComp>() != null
                 select y).FirstOrDefault() != null
                select adjCell;

            foreach (var c in cellsWithWires) 
            {
                Material material = this.LinkedDrawMatFrom(thing, c);
                Printer_Plane.PrintPlane(layer, c.ToVector3ShiftedWithAltitude(thing.def.Altitude), Vector2.one, material);
            }
        }
    }
}
