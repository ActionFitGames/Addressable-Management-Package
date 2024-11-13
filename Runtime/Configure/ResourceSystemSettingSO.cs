using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ActionFit.Framework.Addressable
{
    public class ResourceSystemSettingSO : ScriptableObject
    {
        public const string SettingExportPath = "AddressableSystem/Resources/ResourceSystemSetting.asset";
        private const string ResourcePath = "ResourceSystemSetting";
        
        private static ResourceSystemSettingSO instance;
        
        [SerializeField] private bool useDebugMode;
        [SerializeField] private bool useDontDestroyOnLoad;
        [SerializeField] private int batchReleaseThreshold = 5;
        [SerializeField] private float memoryCleanupInterval = 30f;

        public static ResourceSystemSettingSO Instance
        {
            get
            {
                if (instance)
                {
                    return instance;
                }
                
#if UNITY_EDITOR
                var guids = AssetDatabase.FindAssets("ResourceSystemSetting t:ScriptableObject");
                if (guids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    instance = AssetDatabase.LoadAssetAtPath<ResourceSystemSettingSO>(path);
                }
                else
                {
                    var settingsFilePath = $"Assets/{SettingExportPath}";
                    var settingsDir = System.IO.Path.GetDirectoryName(settingsFilePath);
                        
                    if (!System.IO.Directory.Exists(settingsDir))
                    {
                        System.IO.Directory.CreateDirectory(settingsDir);
                    }

                    instance = CreateInstance<ResourceSystemSettingSO>();
                    AssetDatabase.CreateAsset(instance, settingsFilePath);
                    Debug.Log($"{Define.DefaultMessage}Created settings at: {settingsFilePath}");
                }
#else
                instance = Resources.Load<ResourceSystemSettingSO>(ResourcePath);
                if (instance == null)
                {
                    instance = CreateInstance<ResourceSystemSettingSO>();
                    Debug.LogWarning($"{Define.DefaultMessage}Failed to load settings from path: {ResourcePath}");
                }
#endif
                return instance;
            }
        }

        public bool UseDebugMode
        {
            get => Instance.useDebugMode;
            set
            {
                if (Instance.useDebugMode == value) return;
                Instance.useDebugMode = value;
#if UNITY_EDITOR
                UpdateDebugSymbol();
#endif
                SaveSettings();
            }
        }

        public bool UseDontDestroyOnLoad
        {
            get => Instance.useDontDestroyOnLoad;
            set
            {
                if (Instance.useDontDestroyOnLoad == value) return;
                Instance.useDontDestroyOnLoad = value;
                SaveSettings();
            }
        }
        
        public int BatchReleaseThreshold
        {
            get => Instance.batchReleaseThreshold;
            set
            {
                if (Instance.batchReleaseThreshold == value) return;
                Instance.batchReleaseThreshold = Mathf.Clamp(value, 1, 100);
                SaveSettings();
            }
        }

        public float MemoryCleanupInterval
        {
            get => Instance.memoryCleanupInterval;
            set
            {
                if (Mathf.Approximately(Instance.memoryCleanupInterval, value)) return;
                Instance.memoryCleanupInterval = Mathf.Clamp(value, 1f, 300f);
                SaveSettings();
            }
        }

        private void SaveSettings()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssets();
#endif
        }

#if UNITY_EDITOR
        private void UpdateDebugSymbol()
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            bool hasDebugSymbol = defines.Contains(Define.DebugSymbol);

            if (useDebugMode && !hasDebugSymbol)
            {
                defines = defines + ";" + Define.DebugSymbol;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
            }
            else if (!useDebugMode && hasDebugSymbol)
            {
                defines = defines.Replace(Define.DebugSymbol, "")
                    .Replace(";;", ";")
                    .Trim(';');
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
            }
        }

        private void OnValidate()
        {
            UpdateDebugSymbol();
        }
    }
#endif
}