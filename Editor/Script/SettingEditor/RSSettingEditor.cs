using UnityEditor;
using UnityEngine;

namespace ActionFit.Framework.Addressable.Editor
{
    [CustomEditor(typeof(ResourceSystemSettingSO))]
    public class ResourceSystemSettingSOEditor : UnityEditor.Editor
    {
        private bool showScript = false;
        private Texture2D bottomImage;
        
        private void OnEnable()
        {
            // Resources 폴더에서 이미지 로드
            bottomImage = Resources.Load<Texture2D>("ActFitFrameworkTitle");
        }
        
        public override void OnInspectorGUI()
        {
            var settings = (ResourceSystemSettingSO)target;
            
            using (new EditorGUI.DisabledScope(true))
            {
                if (showScript)
                {
                    base.OnInspectorGUI();
                }
            }
            
            EditorGUILayout.Space(10);
            
            var headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };
            EditorGUILayout.LabelField("<color=#FFD700>Resource System Settings</color>", headerStyle);
            
            EditorGUILayout.Space(10);

            // 설정 박스
            var boxStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(10, 10, 10, 10),
                richText = true
            };

            EditorGUILayout.BeginVertical(boxStyle);

            DrawToggleButton("Debug Mode", settings.UseDebugMode, (value) =>
            {
                settings.UseDebugMode = value;
                GUI.changed = true;
            });

            EditorGUILayout.Space(5);

            DrawToggleButton("DontDestroyOnLoad", settings.UseDontDestroyOnLoad, (value) =>
            {
                settings.UseDontDestroyOnLoad = value;
                GUI.changed = true;
            });

            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);
            
            // 설명 박스
            EditorGUILayout.BeginVertical(boxStyle);

            // 설명 헤더
            EditorGUILayout.LabelField("[ Description ]", headerStyle);
            EditorGUILayout.Space(10);
            
            var titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 12,
                richText = true,
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0)
            };
            
            var contentStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
                richText = true,
                fontSize = 11,
                padding = new RectOffset(10, 10, 0, 10)
            };

            // Debug Mode 섹션
            EditorGUILayout.LabelField("<color=#4A90E2>▪ Debug Mode</color>", titleStyle);
            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField(
                "Debug mode logs all operations from initialization to errors in the addressable system. " +
                "<color=#FF6B6B>Please ensure this option is disabled in the final build.</color>", 
                contentStyle);
            
            EditorGUILayout.Space(8);
            
            // DontDestroyOnLoad 섹션
            EditorGUILayout.LabelField("<color=#4A90E2>▪ DontDestroyOnLoad</color>", titleStyle);
            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField(
                "If there is no permanent scene in your game, use this option to maintain " +
                "the resource system throughout the entire game session.", 
                contentStyle);
            
            EditorGUILayout.EndVertical();
            
            // 하단 이미지 추가
            EditorGUILayout.Space(15);
            if (bottomImage != null)
            {
                float aspectRatio = (float)bottomImage.width / bottomImage.height;
                float desiredWidth = EditorGUIUtility.currentViewWidth - 30; // 여백을 위해 30픽셀 뺌
                float desiredHeight = desiredWidth / aspectRatio;
                
                Rect imageRect = GUILayoutUtility.GetRect(desiredWidth, desiredHeight);
                // 이미지를 중앙 정렬하기 위한 계산
                imageRect.x = (EditorGUIUtility.currentViewWidth - desiredWidth) * 0.5f;
                imageRect.width = desiredWidth;
                
                GUI.DrawTexture(imageRect, bottomImage, ScaleMode.ScaleToFit);
            }
            else
            {
                EditorGUILayout.HelpBox("Failed to load ActFitFrameworkTitle image", MessageType.Warning);
            }
        }

        private void DrawToggleButton(string label, bool value, System.Action<bool> onValueChanged)
        {
            EditorGUILayout.BeginHorizontal();
            
            var labelStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12,
                richText = true
            };
            
            var buttonWidth = EditorGUIUtility.currentViewWidth - 35;
            var toggleRect = GUILayoutUtility.GetRect(buttonWidth, 20);
            var contentRect = new Rect(toggleRect);

            contentRect.width = 150;
            EditorGUI.LabelField(contentRect, label, labelStyle);

            contentRect.x += 150;
            contentRect.width = 50;
            var statusStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleRight,
                normal = { textColor = value ? new Color(0.35f, 0.85f, 0.35f) : new Color(1f, 0.4f, 0.4f) },
                fontStyle = FontStyle.Bold
            };
            EditorGUI.LabelField(contentRect, value ? "True" : "False", statusStyle);

            contentRect.x += 60;
            contentRect.width = 70;
            if (GUI.Button(contentRect, value ? "Disable" : "Enable", new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold }))
            {
                onValueChanged?.Invoke(!value);
            }

            EditorGUILayout.EndHorizontal();
        }
        
        private void OnDisable()
        {
            // 리소스 해제
            if (bottomImage != null)
            {
                Resources.UnloadAsset(bottomImage);
            }
        }
    }
}