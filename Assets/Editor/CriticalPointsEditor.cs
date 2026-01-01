using Label;
using UnityEditor;
using UnityEngine;

namespace Robo.Editor
{
    [CustomEditor(typeof(CriticalPoints))]
    public class CriticalPointsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            CriticalPoints cp = (CriticalPoints)target;
            if (GUILayout.Button("获取父节点朝向", GUILayout.Width(300)))
            {
                cp.SetBoxNormalEqualToParent();
                EditorUtility.SetDirty(cp);
            }
            if (GUILayout.Button("获取父节点位置", GUILayout.Width(300)))
            {
                cp.SetBoxCenterOnParent();
                EditorUtility.SetDirty(cp);
            }
            if (GUILayout.Button("重置", GUILayout.Width(300)))
            {
                cp.SetBoxNormalEqualToParent();
                cp.SetBoxCenterOnParent();
                EditorUtility.SetDirty(cp);
            }
            base.OnInspectorGUI();
        }
    }
}