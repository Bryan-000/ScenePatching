namespace ScenePatching;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary> Annotation to define targets of your ScenePatch methods </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class ScenePatchAttribute(string target = null, params string[] targetObjects) : Attribute
{
    /// <summary> The name of the target scene. </summary>
    public string TargetSceneName = target;

    /// <summary> The paths to some object's in the scene to pass into the patch method. </summary>
    public List<string> TargetObjects = [.. targetObjects];

    /// <summary> The method to be ran when we actually load into the target scene. </summary>
    public MethodInfo patcherMethod;

    /// <summary> creates a new scenepatch meow mrrrp miau maow miaow :3 ^&gt;w&lt;^ </summary>
    public ScenePatchAttribute(string[] targetObjects) : this(null, targetObjects) { }

    /// <summary> Merges the scene patches into one :3 </summary>
    public static ScenePatchAttribute Merge(IEnumerable<ScenePatchAttribute> patches)
    {
        ScenePatchAttribute result = new();
        foreach (ScenePatchAttribute toMerge in patches)
        {
            result.TargetSceneName ??= toMerge.TargetSceneName;
            result.patcherMethod ??= toMerge.patcherMethod;
            result.TargetObjects.AddRange(toMerge.TargetObjects);
        }

        // filter out null target objs then if there arent any left then add an empty one so the patch is still executed
        result.TargetObjects = [.. result.TargetObjects.Where(str => !string.IsNullOrEmpty(str))];
        if (result.TargetObjects.Count == 0)
            result.TargetObjects = [null];

        return result;
    }

    /// <summary> Override's <see cref="object.ToString"/> to deliver a formatted version of this patch. </summary>
    public override string ToString() =>
        $@"TargetScene: {TargetSceneName}
TargetObjects: {string.Join(", ", targetObjects)}
PatcherMethod: {patcherMethod.DeclaringType.FullName}.{patcherMethod.Name}({string.Join(", ", patcherMethod.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name))});";
}