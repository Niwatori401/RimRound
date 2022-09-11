using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.Utilities
{
    [StaticConstructorOnStartup]
    public static class Resources
    {
        public static readonly Texture2D weightProgressBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.9f, 0.85f, 0.2f));
        public static readonly Texture2D weightProgressBarTex2 = SolidColorMaterials.NewSolidColorTexture(new Color(1.0f, 1.0f, 1.0f));

        public static readonly Texture2D exampleIcon = ContentFinder<Texture2D>.Get("UI/PerkIcons/exampleIcon", true);

    }
}
