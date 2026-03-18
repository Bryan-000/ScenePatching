namespace ScenePatching;

using BepInEx;
using UnityEngine;

/// <summary> Load the SceneModding handler. </summary>
[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
public class ScenePatchingPlugin : BaseUnityPlugin
{
    public void Awake()
    {
        gameObject.hideFlags = HideFlags.DontSaveInEditor;
        SceneModding.Load();
    }

    /// <summary> Information about the mod. </summary>
    public class PluginInfo
    {
        public const string GUID = "Bryan_-000-.ScenePatching";
        public const string Name = "ScenePatching";
        public const string Version = "1.0.1";
    }
}