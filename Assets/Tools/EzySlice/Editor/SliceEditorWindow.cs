using UnityEditor;
using UnityEngine;

namespace EzySlice
{
    public class SliceEditorWindow : UnityEditor.EditorWindow
    {
        private GameObject sliceObject;
        private GameObject slicePlane;
        private Material crossSecMaterial;
        [UnityEditor.MenuItem("Tools/Slice")]
        private static void ShowWindow()
        {
            var window = GetWindow<SliceEditorWindow>();
            window.titleContent = new UnityEngine.GUIContent("Slice Helper");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("GameObject 绑定工具", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
        
            // 第一个 GameObject 绑定区域
            DrawGameObjectField("slice object:", ref sliceObject);
        
            EditorGUILayout.Space(15);
        
            // 第二个 GameObject 绑定区域
            DrawGameObjectField("plane:", ref slicePlane);
        
            EditorGUILayout.Space(20);
        
            // 操作按钮区域
            DrawActionButtons();
        
            // 显示当前选择的信息
            DrawSelectionInfo();
        }
        
         void DrawGameObjectField(string label, ref GameObject targetObject)
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField(label, GUILayout.Width(60));
            
            // 使用 ObjectField 绑定 GameObject
            targetObject = (GameObject)EditorGUILayout.ObjectField(
                targetObject, 
                typeof(GameObject), 
                true,  // 允许场景对象
                GUILayout.Height(20)
            );
            
            // 快速选择按钮
            if (GUILayout.Button("选择", GUILayout.Width(50)))
            {
                if (Selection.activeGameObject != null)
                {
                    targetObject = Selection.activeGameObject;
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "请在场景中选择一个 GameObject", "确定");
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        
        // 显示对象信息
        if (targetObject != null)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(65);
            EditorGUILayout.LabelField(
                $"名称: {targetObject.name}", 
                GUILayout.Width(150)
            );
            
            // 显示激活状态
            GUI.enabled = false;
            EditorGUILayout.Toggle("激活", targetObject.activeSelf, GUILayout.Width(80));
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }
    }
    
    // 绘制操作按钮
    void DrawActionButtons()
    {
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            
            // 清空按钮
            if (GUILayout.Button("清空", GUILayout.Width(80)))
            {
                sliceObject = null;
                slicePlane = null;
            }
            
            // 交换按钮
            if (GUILayout.Button("交换", GUILayout.Width(80)))
            {
                GameObject temp = sliceObject;
                sliceObject = slicePlane;
                slicePlane = temp;
            }
            
            // 聚焦按钮
            if (GUILayout.Button("聚焦选中", GUILayout.Width(100)))
            {
                if (sliceObject != null)
                {
                    Selection.activeGameObject = sliceObject;
                    SceneView.lastActiveSceneView.FrameSelected();
                }
            }
            
            // 交换按钮
            if (GUILayout.Button("切割", GUILayout.Width(80)))
            {
                SlicedHull hull = sliceObject.Slice(slicePlane.transform.position, slicePlane.transform.up);
                crossSecMaterial = sliceObject.GetComponent<MeshRenderer>().sharedMaterial;
                GameObject upper = hull.CreateUpperHull(sliceObject, crossSecMaterial);
                GameObject lower = hull.CreateLowerHull(sliceObject, crossSecMaterial);
                UnityEditor.PrefabUtility.SaveAsPrefabAsset(upper, "Assets/Models/upper.prefab");
                UnityEditor.PrefabUtility.SaveAsPrefabAsset(lower, "Assets/Models/HalfIndicator.prefab");
            }
            
            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();
    }
    
    // 显示当前选择的信息
    void DrawSelectionInfo()
    {
        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("信息", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            // 显示当前选择的 GameObject
            GameObject selected = Selection.activeGameObject;
            string selectedName = selected != null ? selected.name : "无";
            EditorGUILayout.LabelField($"当前选中: {selectedName}");
            
            // 显示绑定状态
            EditorGUILayout.LabelField($"对象1绑定: {(sliceObject != null ? sliceObject.name : "未绑定")}");
            EditorGUILayout.LabelField($"对象2绑定: {(slicePlane != null ? slicePlane.name : "未绑定")}");
            
            // 显示关系（如果两个对象都已绑定）
            if (sliceObject != null && slicePlane != null)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("关系:", EditorStyles.miniBoldLabel);
                
                // 检查是否为同一个对象
                if (sliceObject == slicePlane)
                {
                    EditorGUILayout.HelpBox("绑定的是同一个对象", MessageType.Info);
                }
                else
                {
                    // 检查父子关系
                    bool isChild = sliceObject.transform.IsChildOf(slicePlane.transform);
                    bool isParent = slicePlane.transform.IsChildOf(sliceObject.transform);
                    
                    if (isChild)
                        EditorGUILayout.LabelField("对象1是对象2的子级", EditorStyles.miniLabel);
                    else if (isParent)
                        EditorGUILayout.LabelField("对象1是对象2的父级", EditorStyles.miniLabel);
                    else
                        EditorGUILayout.LabelField("对象间无直接层级关系", EditorStyles.miniLabel);
                }
            }
        }
        EditorGUILayout.EndVertical();
    }
    
    // 当场景中选中对象变化时更新窗口
    void OnSelectionChange()
    {
        // 重新绘制窗口
        Repaint();
    }
    
    // 添加工具提示
    void DrawTooltip(string tooltip)
    {
        if (Event.current.type == EventType.Repaint && 
            GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
        {
            GUI.tooltip = tooltip;
        }
    }
    }
}

