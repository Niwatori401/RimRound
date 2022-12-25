using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Defs
{
    class ME_ExtraProductPlantYield : DefModExtension
    {
        //Format entries as number + defName. Ie 3WoodLog
        string extraThing = "";

        public Pair<ThingDef, int>? GetThingDefQuantityPair() 
        {
            string input = extraThing;

            string basePattern = @"(\b[0-9]+)\w+";
            string numberPartPattern = @"(\b[0-9]+)";
            string thingDefPattern = @"[a-zA-Z]\w+";
            
            if (Regex.IsMatch(input, basePattern)) 
            {
                int quantity = int.Parse(Regex.Match(input, numberPartPattern).Value ?? "");
                ThingDef thingDef = ThingDef.Named(Regex.Match(input, thingDefPattern).Value ?? "");

                if (quantity == 0 || thingDef is null) 
                {
                    Log.Error("Could not extract ThingDef quantity pair!");
                    return null;
                }

                return new Pair<ThingDef, int>(thingDef, quantity);
            }

            Log.Error("Input did not match base pattern for ThingDefQuantityPair!");

            return null;
        }
    }
}
