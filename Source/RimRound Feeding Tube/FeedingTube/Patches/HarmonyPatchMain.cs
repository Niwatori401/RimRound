using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.FeedingTube.Patches
{

	[StaticConstructorOnStartup]
	static class HarmonyPatchMain
	{
		static HarmonyPatchMain()
		{
			var harmony = new Harmony("RRHarmony_FeedingTube");
			harmony.PatchAll();

			int patchedMethodsCount = 0;
			int postfixesCount = 0;
			int prefixesCount = 0;
			int transpilersCount = 0;

			foreach (var x in harmony.GetPatchedMethods())
			{
				var y = Harmony.GetPatchInfo(x);
				IEnumerable<HarmonyLib.Patch> postfixes = from z in y.Postfixes where z.owner == harmony.Id select z;
				IEnumerable<HarmonyLib.Patch> prefixes = from z in y.Prefixes where z.owner == harmony.Id select z;
				IEnumerable<HarmonyLib.Patch> transpilers = from z in y.Transpilers where z.owner == harmony.Id select z;

				patchedMethodsCount++;
				postfixesCount += postfixes.Count();
				prefixesCount += prefixes.Count();
				transpilersCount += transpilers.Count();
			}
			Log.Message($"RimRound Feeding Tube successfully added {prefixesCount} prefixes, {postfixesCount} postfixes, and {transpilersCount} transpilers to {patchedMethodsCount} methods with Harmony!");
		}
	}

}
