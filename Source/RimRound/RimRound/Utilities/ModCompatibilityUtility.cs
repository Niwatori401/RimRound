using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Utilities
{
    public struct PatchCollection 
    {
        public MethodInfo prefix;
        public MethodInfo postfix;
        public MethodInfo transpiler;
        public MethodInfo finalizer;
    };

    public struct ModPatchInfo 
    {
        public ModPatchInfo(string modName, string typeName, string methodName, MethodType methodType, List<string> methodParams = null) 
        {
            ModName = modName;
            TypeName = typeName;
            MethodName = methodName;
            MethodType = methodType;
            methodParameters = methodParams;
        }
        public string ModName;
        public string TypeName;
        public string MethodName;
        public MethodType MethodType;
        public List<string> methodParameters;
    };

    [StaticConstructorOnStartup]
    public static class ModCompatibilityUtility 
    {
        static ModCompatibilityUtility() 
        {
            Assembly mscorlib = Assembly.GetAssembly(typeof(int));

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly a in assemblies)
            {
                if (a == mscorlib)
                    continue;

                loadedTypes.AddRange(GetLoadableTypes(a));
            }
        }

        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                Log.Warning($"RimRound threw an exception loading a module from {assembly.FullName}! This is likely NOT due to RimRound. The full exception is as follows:\n{e.ToString()}");
                return e.Types.Where(t => t != null);
            }
        }


        public static BindingFlags majorFlags =
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Static | BindingFlags.Instance |
            BindingFlags.FlattenHierarchy;

        public static List<Type> loadedTypes = new List<Type>();

        public static void TryPatch(Harmony harmonyinstance, ModPatchInfo modPatchInfo, PatchCollection patchCollection)
        {
            if (!CheckModInstalled(modPatchInfo.ModName) ||
                (patchCollection.prefix is null && patchCollection.postfix is null && patchCollection.transpiler is null && patchCollection.finalizer is null))
                return;

            MethodInfo methodToPatchMI;

            switch (modPatchInfo.MethodType)
            {
                case MethodType.Normal:
                    methodToPatchMI = GetMethodInfo(modPatchInfo.ModName, modPatchInfo.TypeName, modPatchInfo.MethodName, modPatchInfo.methodParameters);
                    break;
                case MethodType.Getter:
                    methodToPatchMI = GetPropertyInfo(modPatchInfo.ModName, modPatchInfo.TypeName, modPatchInfo.MethodName)?.GetGetMethod(true);
                    break;
                case MethodType.Setter:
                    methodToPatchMI = GetPropertyInfo(modPatchInfo.ModName, modPatchInfo.TypeName, modPatchInfo.MethodName)?.GetSetMethod(true);
                    break;
                case MethodType.Constructor:
                    throw new NotImplementedException();
                case MethodType.StaticConstructor:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }

            harmonyinstance.Patch(methodToPatchMI, 
                patchCollection.prefix     is null ? null : new HarmonyMethod(patchCollection.prefix), 
                patchCollection.postfix    is null ? null : new HarmonyMethod(patchCollection.postfix),  
                patchCollection.transpiler is null ? null : new HarmonyMethod(patchCollection.transpiler),
                patchCollection.finalizer  is null ? null : new HarmonyMethod(patchCollection.finalizer));

            Log.Message($"[RimRound] successfully patched {modPatchInfo.ModName}'s {modPatchInfo.MethodName}!");
        }

        public static bool CheckModInstalled(string modname) 
        {
            if (ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name == modname)) 
            {
                return true;
            }
            return false;
        }

        public static MethodInfo GetMethodInfo(string modname, string typeName, string methodName, List<string> types = null) 
        {
            if (CheckModInstalled(modname))
            {
                foreach (Type t in ModCompatibilityUtility.loadedTypes) 
                {
                    if (t.Name == typeName || t.FullName == typeName)
                    {
                        MethodInfo methodInfo;
                        if (types is null)
                            methodInfo = t.GetMethod(methodName, majorFlags);
                        else
                        {
                            List<Type> typeParameters = new List<Type>();
                            foreach (string s in types)
                            {
                                foreach (Type type in ModCompatibilityUtility.loadedTypes) 
                                {
                                    if (s == type.Name) 
                                    {
                                        typeParameters.Add(type);
                                    }
                                }
                            }
                            if (typeParameters.Count != types.Count)
                                throw new Exception("Was unable to match types in ModCompatibilityUtility.GetMethodInfo!");
                            
                            
                            Type[] methodParams = typeParameters.ToArray();

                            methodInfo = t.GetMethod(methodName, majorFlags, null, methodParams, null);
                        }
                            

                        if (methodInfo is null)
                        {
                            Log.Error($"Could not get method {methodName} from {t.Name}");
                        }

                        return methodInfo;
                    }
                }
            }

            return null;
        }

        public static Type GetTypeFromMod(string modname, string typeName)
        {
            if (CheckModInstalled(modname))
            {
                foreach (Type t in ModCompatibilityUtility.loadedTypes)
                {
                    if (t.Name == typeName)
                    {
                        return t;
                    }
                }
            }
            return null;
        }

        public static PropertyInfo GetPropertyInfo(string modname, string typeName, string propertyName) 
        {
            if (CheckModInstalled(modname))
            {
                foreach (Type t in loadedTypes)
                {
                    if (t.Name == typeName)
                    {
                        PropertyInfo m = t.GetProperty(propertyName, majorFlags);

                        if (m is null)
                        {
                            Log.Error($"Could not get method {propertyName} from {t.Name}");
                        }

                        return m;
                    }
                }
            }

            return null;
        }

        public static FieldInfo GetFieldInfo(string modname, string typeName, string propertyName)
        {
            if (CheckModInstalled(modname))
            {
                foreach (Type t in loadedTypes)
                {
                    if (t.Name == typeName)
                    {
                        FieldInfo fieldInfo = t.GetField(propertyName, majorFlags);

                        if (fieldInfo is null)
                        {
                            Log.Error($"Could not get field {propertyName} from {t.Name}");
                        }

                        return fieldInfo;
                    }
                }
            }

            return null;
        }

    }
}
