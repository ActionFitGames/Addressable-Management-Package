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

            // Settings box
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

            DrawSettingsSliders();

            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);
            
            // Description box
            EditorGUILayout.BeginVertical(boxStyle);
            
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

            // ... (existing description sections)

            EditorGUILayout.Space(8);
            
            // Batch Release Threshold section
            EditorGUILayout.LabelField("<color=#4A90E2>▪ Batch Release Threshold</color>", titleStyle);
            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField(
                "Determines how many assets need to accumulate in the release queue before triggering " +
                "a batch release operation. Lower values mean more frequent releases but potentially " +
                "more overhead. Higher values batch more operations together but may temporarily hold " +
                "more memory.", 
                contentStyle);
            
            EditorGUILayout.Space(8);
            
            // Memory Cleanup Interval section
            EditorGUILayout.LabelField("<color=#4A90E2>▪ Memory Cleanup Interval</color>", titleStyle);
            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField(
                "Defines the minimum time (in seconds) between automatic memory cleanup operations. " +
                "More frequent cleanup (lower interval) keeps memory usage lower but may impact " +
                "performance. Less frequent cleanup (higher interval) has less performance impact but " +
                "may allow more memory accumulation.", 
                contentStyle);
            
            EditorGUILayout.EndVertical();
            
            // Bottom image with reduced size
            EditorGUILayout.Space(15);
            if (bottomImage != null)
            {
                float aspectRatio = (float)bottomImage.width / bottomImage.height;
                float desiredWidth = EditorGUIUtility.currentViewWidth - 60; // Increased margin for smaller image
                float desiredHeight = desiredWidth / aspectRatio;
                
                Rect imageRect = GUILayoutUtility.GetRect(desiredWidth, desiredHeight);
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
        
        private void DrawSettingsSliders()
        {
            var settings = (ResourceSystemSettingSO)target;
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.BeginHorizontal();
            
            // Batch Release Threshold
            EditorGUILayout.BeginVertical(GUILayout.Width(EditorGUIUtility.currentViewWidth / 2 - 10));
            var thresholdLabel = new GUIContent("Batch Release Threshold", 
                "Number of assets that trigger automatic batch release");
            EditorGUILayout.LabelField(thresholdLabel, EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            float sliderWidth = (EditorGUIUtility.currentViewWidth / 2) - 70; // 슬라이더 폭 조정
            var newThreshold = settings.BatchReleaseThreshold;
            
            newThreshold = (int)EditorGUILayout.Slider(newThreshold, 1, 100, GUILayout.Width(sliderWidth));
            newThreshold = Mathf.Clamp(newThreshold, 1, 100);
            
            if (newThreshold != settings.BatchReleaseThreshold)
            {        
                settings.BatchReleaseThreshold = newThreshold;
                GUI.changed = true;
            }
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            
            // Memory Cleanup Interval
            EditorGUILayout.BeginVertical(GUILayout.Width(EditorGUIUtility.currentViewWidth / 2 - 10));
            var intervalLabel = new GUIContent("Memory Cleanup Interval", 
                "Time interval (in seconds) between memory cleanup operations");
            EditorGUILayout.LabelField(intervalLabel, EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            var newInterval = settings.MemoryCleanupInterval;
            
            newInterval = EditorGUILayout.Slider(newInterval, 1f, 300f, GUILayout.Width(sliderWidth));                     
            newInterval = Mathf.Clamp(newInterval, 1f, 300f);
            
            if (!Mathf.Approximately(newInterval, settings.MemoryCleanupInterval))
            {     
                settings.MemoryCleanupInterval = newInterval;
                GUI.changed = true;
            }
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();
        }
    }
}