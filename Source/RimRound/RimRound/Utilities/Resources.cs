using AlienRace;
using RimWorld;
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
        static Resources() 
        {
            //BodyTypeUtility.RefreshBodyTypeGraphicLocations();
        }
        public static readonly SoundDef gizmoClick = SoundDefOf.Click;

        public static readonly Texture2D FILLER_TEXTURE = ContentFinder<Texture2D>.Get("FillerTexture", true);

        public static readonly Texture2D TEN_PP_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/10_PP", true);
        public static readonly Texture2D ADD_SEVERITY_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/ADD_SEVERITY", true);
        public static readonly Texture2D BLOB_BED_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/BLOB_BED", true);
        public static readonly Texture2D CHANGE_CARDINALITY_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/CHANGE_CARDINALITY", true);
        public static readonly Texture2D CHANGE_DIRECTION_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/CHANGE_DIRECTION", true);
        public static readonly Texture2D CHANGE_MAGNITUDE_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/CHANGE_MAGNITUDE", true);
        public static readonly Texture2D CHANGE_OFFSET_ITEM_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/CHANGE_OFFSET_ITEM", true);
        public static readonly Texture2D CHANGE_XY_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/CHANGE_XY", true);
        public static readonly Texture2D DEBUGFLOAT_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/DEBUGFLOAT", true);
        public static readonly Texture2D DECREASE_WEIGHT_OPINION_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/DECREASE_WEIGHT_OPINION", true);
        public static readonly Texture2D INCREASE_WEIGHT_OPINION_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/INCREASE_WEIGHT_OPINION", true);
        public static readonly Texture2D DUMP_BODY_DATA_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/DUMP_BODY_DATA", true);
        public static readonly Texture2D EMPTY_STOMACH_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/EMPTY_STOMACH", true);
        public static readonly Texture2D EXTRA_BODY_PART_OFFSET_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/EXTRA_BODY_PART_OFFSET", true);
        public static readonly Texture2D FILL_STOMACH_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/FILL_STOMACH", true);
        public static readonly Texture2D FORCE_CURRENT_BODY_TYPE_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/FORCE_CURRENT_BODY_TYPE", true);
        public static readonly Texture2D HEAD_OFFSET_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/HEAD_OFFSET", true);
        public static readonly Texture2D HIDE_COVERS_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/HIDE_COVERS", true);
        public static readonly Texture2D MAKE_GENERATOR_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/MAKE_GENERATOR", true);
        public static readonly Texture2D RECREATION_SPOT_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/RECREATION_SPOT", true);
        public static readonly Texture2D REDUCE_SEVERITY_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/REDUCE_SEVERITY", true);
        public static readonly Texture2D SLEEPING_POSITION_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/SLEEPING_POSITION", true);
        public static readonly Texture2D SPRITE_SIZE_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/SPRITE_SIZE", true);
        public static readonly Texture2D SWITCH_PAWN_BODY_TYPE_APPLE_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/SWITCH_PAWN_BODY_TYPE_APPLE", true);
        public static readonly Texture2D SWITCH_PAWN_BODY_TYPE_STANDARD_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/SWITCH_PAWN_BODY_TYPE_STANDARD", true);
        public static readonly Texture2D WIPE_PERKS_ICON = ContentFinder<Texture2D>.Get("UI/DebugIcons/WIPE_PERKS", true);



        public static readonly Texture2D weightProgressBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.9f, 0.85f, 0.2f));
        public static readonly Texture2D weightProgressBarTex2 = SolidColorMaterials.NewSolidColorTexture(new Color(1.0f, 1.0f, 1.0f));

        public static readonly Texture2D exampleIcon = ContentFinder<Texture2D>.Get("UI/PerkIcons/exampleIcon", true);

    }
}
