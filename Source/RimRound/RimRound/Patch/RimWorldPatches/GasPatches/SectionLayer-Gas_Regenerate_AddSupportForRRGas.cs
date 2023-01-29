using HarmonyLib;
using RimRound.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    /// <summary>
    /// Overrides the base behavior of SectionLayer_Gas.Regenerate() to add meshes for RR gas. The order is set up so that vanilla gasses override RimRound ones.
    /// </summary>
    [HarmonyPatch(typeof(SectionLayer_Gas))]
    [HarmonyPatch(nameof(SectionLayer_Gas.Regenerate))]
    public class SectionLayer_Gas_Regenerate_AddSupportForRRGas
    {
        public static bool Prefix(SectionLayer_Gas __instance)
        {
            Section section = (Section)typeof(SectionLayer).GetField("section", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            Map map = (Map)typeof(SectionLayer).GetProperty("Map", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(__instance, null);
            MethodInfo addCellMI = typeof(SectionLayer_Gas).GetMethod("AddCell", BindingFlags.NonPublic | BindingFlags.Instance);


            MapComp_RRGasGrid mapComp = map.GetComponent<MapComp_RRGasGrid>();
            if (section is null || mapComp is null || addCellMI is null || map is null)
                return true;


            foreach (LayerSubMesh layerSubMesh in __instance.subMeshes)
                layerSubMesh.Clear(MeshParts.All);


            LayerSubMesh subMesh = __instance.GetSubMesh(__instance.Mat);
            float altitude = AltitudeLayer.Gas.AltitudeFor();
            int num = section.botLeft.x;
            foreach (IntVec3 intVec in section.CellRect)
            {
                if (map.gasGrid.AnyGasAt(intVec))
                {
                    int count = subMesh.verts.Count;
                    addCellMI.Invoke(__instance, new Object[] { intVec, num, count, subMesh, altitude });
                }
                else if (mapComp.AnyGasAt(intVec)) 
                {
                    int count = subMesh.verts.Count;
                    addCellMI.Invoke(__instance, new Object[] { intVec, num, count, subMesh, altitude });
                }
                num++;
            }
            if (subMesh.verts.Count > 0)
            {
                subMesh.FinalizeMesh(MeshParts.All);
            }


            return false;
        }
    }
}
