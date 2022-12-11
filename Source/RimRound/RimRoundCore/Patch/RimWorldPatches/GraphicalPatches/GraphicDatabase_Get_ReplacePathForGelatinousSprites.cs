using HarmonyLib;
using RimRound.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    /*
    [HarmonyPatch(typeof(GraphicDatabase))]
    [HarmonyPatch("Get", new Type[] {typeof(GraphicRequest)})]
    public class GraphicDatabase_Get_ReplacePathForGelatinousSprites
    {
        public static bool Prefix(ref GraphicRequest req) 
        {
            return FixGraphicRequestPath.AlterBodyPath(ref req);
        }
    }
    */
    //[HarmonyPatch]
    public class GraphicDatabase_GetInner_ReplaceForGelatinousSprites 
    {
        public static MethodInfo TargetMethod() 
        {
            return typeof(GraphicDatabase).GetMethod("GetInner", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(new Type[] { typeof(Graphic_Multi) });
        }

        
        public static bool Prefix(ref GraphicRequest req)
        {
            return FixGraphicRequestPath.AlterBodyPath(ref req);
        }
    }

    public static class FixGraphicRequestPath
    {
        public static bool AlterBodyPath(ref GraphicRequest req)
        {
            if (req.path is null)
                return true;

            Log.Message($"Ran on {req.path}");

            if (BodyTypeUtility.IsCustomBody(req.path))
            {
                Log.Message($"))) Ran on {req.path}");

                req.path = BodyTypeUtility.ConvertBodyPathStringsIfNecessary(req.path);
                Log.Message($"Which became {req.path}");
            }

            return true;
        }
    }
}
