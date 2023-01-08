using RimRound.Comps;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static RimRound.UI.MainTabWindow_RRGlobalSettings;

namespace RimRound.UI
{
    public class CustomBodySelectDropdown
    {
        public static void DrawResponseButton(Rect rect, Pawn pawn, bool paintable)
        {
            Widgets.Dropdown<Pawn, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(
                rect,
                pawn,
                IconColor,
                new Func<Pawn, Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>(DrawResponseButton_GetResponse),
                new Func<Pawn, IEnumerable<Widgets.DropdownMenuElement<Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>>>(BodyTypeSetDropdownMenuGenerator),
                null,
                Utilities.Resources.FILLER_TEXTURE,
                null,
                null,
                null,
                paintable);

            if (Mouse.IsOver(rect))
            {
                //TooltipHandler.TipRegion(rect, "DietMode_Tip".Translate() + "\n\n" + "DietMode_CurrentMode".Translate() + ": " + pawn.TryGetComp<FullnessAndDietStats_ThingComp>().DietMode.GetLabel());
            }
        }

        private static Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>> DrawResponseButton_GetResponse(Pawn pawn)
        {
            return pawn.TryGetComp<PawnBodyType_ThingComp>().CustomBodyTypeDict;
        }


        public static IEnumerable<Widgets.DropdownMenuElement<Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>> BodyTypeSetDropdownMenuGenerator(Pawn pawn)
        {
            using (var enumerator = RacialBodyTypeInfoUtility.genderedSets.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var currentEntry = enumerator.Current;

                    string label = currentEntry.Key;
                    var dicitonaryPayload = currentEntry.Value;

                    yield return new Widgets.DropdownMenuElement<Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>>>
                    {
                        option = new FloatMenuOption(label, delegate ()
                        {
                            pawn.TryGetComp<PawnBodyType_ThingComp>().CustomBodyTypeDict = dicitonaryPayload;

                            BodyTypeUtility.UpdateAllPawnSprites();
                        }),
                        payload = dicitonaryPayload
                    };
                }
            }
        }

        private static readonly Color IconColor = new Color(0.84f, 0.84f, 0.84f);
    }
}
