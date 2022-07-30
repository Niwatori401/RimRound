using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.FeedingTube.AI
{
    internal class Verb_FeedFromFeeder : Verb
    {
        protected override bool TryCastShot()
        {
            Log.Message("Verbed it up!");
            return true;
        }
    }
}
