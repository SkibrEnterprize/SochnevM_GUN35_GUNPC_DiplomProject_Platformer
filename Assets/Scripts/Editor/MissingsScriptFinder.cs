using UnityEditor;
using UnityEngine;

public class MissingScriptsFinder : EditorWindow
{
    [MenuItem("Tools/Find Missing Scripts")]
    public static void Find()
    {
        // Ищем на сцене
        var objects = GameObject.FindObjectsOfType<GameObject>(true);
        foreach (var go in objects)
        {
            var components = go.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    Debug.LogError($"Missing Script найден на СЦЕНЕ: {go.name}", go);
                }
            }
        }

        // Ищем в префабах (Assets)
        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
        foreach (string path in allAssetPaths)
        {
            if (path.EndsWith(".prefab"))
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    var components = prefab.GetComponentsInChildren<Component>(true);
                    foreach (var c in components)
                    {
                        if (c == null)
                        {
                            Debug.LogError($"Missing Script найден в ПРЕФАБЕ: {path}", prefab);
                        }
                    }
                }
            }
        }
        Debug.Log("Поиск завершен.");
    }
}