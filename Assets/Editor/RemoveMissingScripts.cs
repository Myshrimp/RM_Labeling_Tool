using UnityEditor;
using UnityEngine;

public class RemoveMissingScripts : EditorWindow
{
    static int m_goCount;
    static int m_missingCount;

    [MenuItem("GameObject/Remove Missing Scripts")]
    static void Apply()
    {
        var beginTime = System.DateTime.UtcNow.Ticks;

        m_goCount = 0;
        m_missingCount = 0;
        foreach (var go in Selection.gameObjects)
            search(go);

        var endTime = System.DateTime.UtcNow.Ticks;
        var deltaTime = endTime - beginTime;

        Debug.Log($"Searched in {m_goCount} GameObjects, found and removed {m_missingCount} missing scripts. Took {deltaTime / 10000.0} ms.");
    }

    static void search(GameObject go)
    {
        m_goCount++;
        m_missingCount += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
        foreach (Transform child in go.transform)
            search(child.gameObject);
    }
}