namespace ScenePatching;

using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> Static class full of really useful unity extensions that unity is just too lazy to add :3 </summary>
public static class UnityExtensions
{
    extension(Component comp)
    {
        /// <summary> Gets the gameObject parent of this component. </summary>
        public GameObject parent =>
            comp.transform.parent?.gameObject;

        /// <summary> Gets the RectTransform of the gameObject of a component. </summary>
        public RectTransform rectTransform =>
            comp.GetComponent<RectTransform>();

        /// <summary> Adds a component to the gameObject of a component. </summary>
        public T AddComponent<T>() where T : Component =>
            comp.gameObject.AddComponent<T>();

        /// <summary> Sets the gameObject of a component to be enabled </summary>
        public void SetActive(bool value) =>
            comp.gameObject.SetActive(value);

        /// <summary> Finds a child in a GameObject via its name/path. </summary>
        public GameObject Find(string childPath) =>
            comp.transform.Find(childPath)?.gameObject;

        /// <summary> Finds a child in a GameObject via its name/path, and gets a component from that child. </summary>
        public T Find<T>(string childPath) where T : Component =>
            comp.Find(childPath)?.GetComponent<T>();
    }

    extension(GameObject obj)
    {
        /// <summary> Gets the RectTransform of the gameObject. </summary>
        public RectTransform rectTransform =>
            obj.GetComponent<RectTransform>();

        /// <summary> The GameObject parent of this gameObject. </summary>
        public GameObject parent
        {
            get => obj.transform.parent.gameObject;
            set => obj.transform.parent = value.transform;
        }

        /// <summary> The scale of the GameObject relative to the GameObject's parent. </summary>
        public Vector3 scale
        {
            get => obj.transform.localScale;
            set => obj.transform.localScale = value;
        }

        /// <summary> The rotation of the GameObject as Euler angles in degrees. </summary>
        public Vector3 rotation
        {
            get => obj.transform.eulerAngles;
            set => obj.transform.eulerAngles = value;
        }

        /// <summary> The world space position of the GameObject. </summary>
        public Vector3 position
        {
            get => obj.transform.position;
            set => obj.transform.position = value;
        }

        /// <summary> Gets a component from a gameObject or if it doesnt exist yet, add it. </summary>
        public T GetOrAddComponent<T>() where T : Component =>
            obj.GetComponent<T>() ?? obj.AddComponent<T>();

        /// <summary> Finds a child in a GameObject via its name/path. </summary>
        public GameObject Find(string childPath) =>
            obj.transform.Find(childPath)?.gameObject;

        /// <summary> Finds a child in a GameObject via its name/path, and gets a component from that child. </summary>
        public T Find<T>(string childPath) where T : Component =>
            obj.Find(childPath)?.GetComponent<T>();

        /// <summary> Finds a GameObject based on name/path, doesnt matter if its enabled or not. </summary>
        public static GameObject FindObject(string Path, Scene? scene = null)
        {
            if (string.IsNullOrEmpty(Path))
                return null;

            Path = Path.Replace('\\', '/');
            string rootSearchObj = Path;
            if (Path.Contains('/'))
            {
                rootSearchObj = Path[..Path.IndexOf('/')];
                Path = Path[(Path.IndexOf('/') + 1)..];
            }

            var search = (scene ?? SceneManager.GetSceneAt(0)).GetRootGameObjects().FirstOrDefault(root => root.name == rootSearchObj);
            return rootSearchObj != Path // if root isnt the same as path then that means at the start it contained a / and got changed aka its got a child
                ? search.Find(Path)
                : search;
        }

        /// <summary> Creates a GameObject with the specified component. </summary>
        public static T Create<T>(string name) where T : Component =>
            new GameObject(name).AddComponent<T>();

        /// <summary> Creates a GameObject with the specified thingamajigs :3 </summary>
        public static T Create<T>(string name, Transform parent, bool active = true) where T : Component
        {
            GameObject gameObject = new(name);
            gameObject.SetActive(active);
            gameObject.transform.SetParent(parent, false);

            return gameObject.AddComponent<T>();
        }
    }
}