using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.FeedingTube
{
    [StaticConstructorOnStartup]
    public static class Resources
    {
        public static readonly Graphic_LinkedFoodOverlay FoodOverlay = new Graphic_LinkedFoodOverlay(GraphicDatabase.Get<Graphic_Single>
            ("Things/Building/Production/FoodPipe_Atlas", ShaderDatabase.MetaOverlay));
    }
}
