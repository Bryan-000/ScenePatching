namespace ScenePatching;

using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using BepLogger = BepInEx.Logging.Logger;

/// <summary> handles scene patches and stuff :3 </summary>
public static class SceneModding
{
    /// <summary> Dictionary full of all the scene patches to run on scene load. </summary>
    public static Dictionary<string, List<ScenePatchAttribute>> ScenePatches = [];

    /// <summary> BepInEx logger for like logging yknow :P </summary>
    private static ManualLogSource Log;

    /// <summary> same as <see cref="PatchAll(Assembly)"/> but like just this type instead of the entire assembly :3 </summary>
    public static void PatchAll(Type type)
    {
        List<ScenePatchAttribute> patches = [.. type.GetCustomAttributes<ScenePatchAttribute>()];
        foreach (MethodInfo meth in AccessTools.GetDeclaredMethods(type))
        {
            try
            {
                List<ScenePatchAttribute> methodpatches = [.. meth.GetCustomAttributes<ScenePatchAttribute>()];
                if (!methodpatches.Any())
                    continue;

                ScenePatchAttribute patch = ScenePatchAttribute.Merge(methodpatches.Concat(patches));
                patch.patcherMethod ??= meth;

                if (ScenePatches.TryGetValue(patch.TargetSceneName, out List<ScenePatchAttribute> allPatchesOfTarget))
                    allPatchesOfTarget.Add(patch);
                else
                    ScenePatches.Add(patch.TargetSceneName, patch);
            }
            catch
            {
                Log.LogError($"Failed to register patch {type.Name}.{meth.Name}()");
            }
        }
    }

    /// <summary> Searches the provided or current assembly for any <see cref="ScenePatchAttribute"/>'s and uses them to register new scene patches :3 </summary>
    public static void PatchAll(Assembly asm = null)
    {
        asm ??= new StackTrace().GetFrame(1).GetMethod().ReflectedType.Assembly;

        Log.LogMessage($"Scene patching {asm.GetName().Name}...");
        foreach (Type type in AccessTools.GetTypesFromAssembly(asm))
            PatchAll(type);
    }

    /// <summary> Execute's a ScenePatch, providing all the wanted parameters. </summary>
    public static void ExecutePatch(ScenePatchAttribute patch, Scene? scene = null, GameObject targetObj = null)
    {
        targetObj ??= GameObject.FindObject(patch.TargetObjects.FirstOrDefault(), scene);

        List<object> parameters = [];
        foreach (ParameterInfo parameter in patch.patcherMethod.GetParameters())
        {
            // if the parameter's type is of GameObject then pass in the target obj
            if (parameter.ParameterType == typeof(GameObject))
                parameters.Add(targetObj);

            // if the parameter's type is Transform then pass in the transform of the target obj
            else if (parameter.ParameterType == typeof(Transform))
                parameters.Add(targetObj?.transform);

            // if the type of the parameter inherits component then do getcomponent on the target obj and pass that in
            else if (parameter.ParameterType.IsSubclassOf(typeof(Component)))
                parameters.Add(targetObj?.GetComponent(parameter.ParameterType));

            // else just return null because we only support gameobject, transform, and component parameters
            else
                parameters.Add(null);
        }

        // if the person who made the patch is stupid and didnt make it static then try to create and instance 3:<
        object patchClassInstance = patch.patcherMethod.IsStatic ? null
            : Activator.CreateInstance(patch.patcherMethod.DeclaringType);

        patch.patcherMethod.Invoke(patchClassInstance, [.. parameters]);
    }

    /// <summary> Loads the scene handler. </summary>
    internal static void Load()
    {
        Log = BepLogger.CreateLogSource("SceneModding");
        SceneManager.sceneLoaded += (scene, _) =>
        {
            if (ScenePatches.TryGetValue(SceneHelper.CurrentScene, out List<ScenePatchAttribute> scenePatches))
                foreach (ScenePatchAttribute patch in scenePatches) 
                    try
                    {
                        foreach (string targetObj in patch.TargetObjects)
                            ExecutePatch(patch, scene, GameObject.FindObject(targetObj, scene));
                    }
                    catch (Exception ex)
                    {
                        Log.LogError($"Exception while running patch({patch.patcherMethod.DeclaringType.FullName}:: {patch.patcherMethod.Name}): {ex.Message}\n{ex.StackTrace}");
                    }
        };
    }
}