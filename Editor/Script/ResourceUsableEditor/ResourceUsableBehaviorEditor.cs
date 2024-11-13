
using UnityEditor;
using UnityEngine;

namespace ActionFit.Framework.Addressable.Editor
{
    [CustomEditor(typeof(ResourceUsableBehavior), true)]
    [CanEditMultipleObjects]
    public class ResourceUsableBehaviorEditor : UnityEditor.Editor
    {
        private SerializedProperty _toReleaseProperty;
        private SerializedProperty _scriptProperty;

        private void OnEnable()
        {
            _toReleaseProperty = serializedObject.FindProperty("_toRelease");
            _scriptProperty = serializedObject.FindProperty("m_Script");
        }

        private void DrawSeparator()
        {
            EditorGUILayout.Space(10);
            var rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
            EditorGUILayout.Space(10);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            // Title Box
            var boxStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(0, 0, 0, 0)
            };

            EditorGUILayout.BeginVertical(boxStyle);
            
            var headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.5f, 1f, 0.5f) }
            };
            
            EditorGUILayout.LabelField("This is Resource Usable Component", headerStyle);
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(5);
            
            // Settings Box
            EditorGUILayout.BeginVertical(boxStyle);
            
            // Release Option in horizontal layout
            EditorGUILayout.BeginHorizontal();
            var labelStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 11
            };
            EditorGUILayout.LabelField("Release Option", labelStyle, GUILayout.Width(90));
            EditorGUILayout.PropertyField(_toReleaseProperty, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();

            DrawSeparator();

            // Script field (read-only)
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(_scriptProperty);
            }

            // Rest of properties
            DrawPropertiesExcluding(serializedObject, "_toRelease", "m_Script");

            serializedObject.ApplyModifiedProperties();
        }
    }
}