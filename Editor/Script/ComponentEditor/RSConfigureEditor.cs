using UnityEditor;
using UnityEngine;

namespace ActionFit.Framework.Addressable.Editor
{
    [CustomEditor(typeof(ResourceSystemConfigure))]
    public class RSConfigureEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var settings = ResourceSystemSettingSO.Instance;
            
            EditorGUILayout.Space(10);
            
            var headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUILayout.LabelField("Resource System Configure", headerStyle);
            
            EditorGUILayout.Space(10);

            var boxStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(10, 10, 10, 10),
                richText = true,
                fontSize = 12
            };

            string debugStatus = settings.UseDebugMode ? "<color=#00FF00>True</color>" : "<color=#FF6B6B>False</color>";
            string dontDestroyStatus = settings.UseDontDestroyOnLoad ? "<color=#00FF00>True</color>" : "<color=#FF6B6B>False</color>";

            EditorGUILayout.BeginVertical(boxStyle);
            
            EditorGUILayout.LabelField($"Debug Mode: {debugStatus}", new GUIStyle(EditorStyles.label) { richText = true });
            EditorGUILayout.LabelField($"DontDestroyOnLoad: {dontDestroyStatus}", new GUIStyle(EditorStyles.label) { richText = true });
            
            EditorGUILayout.Space(5);

            // 버튼을 중앙에 배치하고 너비를 박스의 1/2로 설정
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            var buttonStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };

            float boxWidth = EditorGUIUtility.currentViewWidth - 35; // 박스의 실제 너비 계산 (패딩 고려)
            float buttonWidth = boxWidth * 0.5f; // 버튼 너비를 박스 너비의 1/2로 설정
            
            if (GUILayout.Button("Open Settings", buttonStyle, GUILayout.Width(buttonWidth), GUILayout.Height(25)))
            {
                string[] guids = AssetDatabase.FindAssets("ResourceSystemSetting t:ScriptableObject");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    var settingsObject = AssetDatabase.LoadAssetAtPath<ResourceSystemSettingSO>(path);
                    Selection.activeObject = settingsObject;
                    EditorGUIUtility.PingObject(settingsObject);
                }
            }
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                Repaint();
            }
        }
    }
}