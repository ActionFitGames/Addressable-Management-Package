
using UnityEditor;
using UnityEngine;

namespace ActionFit.Framework.Addressable.Editor
{
    [CustomEditor(typeof(ResourceSystemComponent))]
    public class RSComponentEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawCustomMessage();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawCustomMessage()
        {
            var toolTipStyle = new GUIStyle(EditorStyles.helpBox)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(10, 10, 10, 10),
                richText = true
            };

            string message = "Component responsible for the lifecycle of the Resource System.\n" +
                             "Utilizes core logic and MonoBehaviour lifecycle events.\n" +
                             "Uses the main thread dispatcher if needed.";
            EditorGUILayout.LabelField(message, toolTipStyle, GUILayout.Height(60));
            
            var boxStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(0, 0, 10, 10)
            };

            EditorGUILayout.BeginVertical(boxStyle);
            
            var linkStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,  
                fontStyle = FontStyle.Bold,
                richText = true
            };
            
            EditorGUILayout.LabelField("Learn more about Resource System, <color=#00ffff>Click!!</color>", linkStyle);
            
            Rect rect = GUILayoutUtility.GetLastRect();
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
            if (Event.current.type == EventType.MouseUp && rect.Contains(Event.current.mousePosition))
            {
                Application.OpenURL("https://github.com/ActionFitGames/Addressable-Management-Package");
            }

            EditorGUILayout.EndVertical();
            
            var footerStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleRight,
                fontSize = 10,
                fontStyle = FontStyle.Normal,
                padding = new RectOffset(10, 10, 5, 5),
                normal = { textColor = Color.gray }
            };

            string footer = "Resource Management System (ActionFit-Framework)";
            EditorGUILayout.LabelField(footer, footerStyle, GUILayout.Height(30));
        }
    }
}