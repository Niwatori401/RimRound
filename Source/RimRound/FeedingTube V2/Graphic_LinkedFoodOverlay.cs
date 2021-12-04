using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound
{
    public class Graphic_LinkedFoodOverlay : Graphic_LinkedTransmitterOverlay
    {
		public Graphic_LinkedFoodOverlay(Graphic subGraphic) : base(subGraphic)
		{
		}

		public override bool ShouldLinkWith(IntVec3 c, Thing parent)
		{
			return c.InBounds(parent.Map) && FoodTransmitter_NetManager.For(parent.Map).FoodNetAt(c) != null;
		}
    }
}
