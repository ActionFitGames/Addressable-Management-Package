using UnityEngine;
using UnityEditor;
using System.IO;

namespace ActionFit.Framework.Addressable.Editor
{
    public static class RSWindowEditor
    {
        private const string MENU_ITEM_PATH = "ActFit/ResourceSystem/";
        private const string CONTEXT_MENU_PATH = "ActFit/ResourceSystem/";
        private const string PREFAB_NAME = "ResourceSystem.prefab";
        private const string PREFAB_FOLDER = "Editor/PresetAssets";
        
        // Settings 메뉴
        [MenuItem(MENU_ITEM_PATH + "Open Settings", priority = 1)]
        public static void OpenSettings()
        {
            var settings = GetOrCreateSettings();
            Selection.activeObject = settings;
        }
        
        // 툴바 메뉴에서 리소스 시스템 생성
        [MenuItem(MENU_ITEM_PATH + "Create Resource System Object", priority = 2)]
        public static void CreateResourceSystemFromToolbar()
        {
            CreateResourceSystemObject();
        }
        
        // 하이어라키 컨텍스트 메뉴에서 리소스 시스템 생성
        [MenuItem("GameObject/" + CONTEXT_MENU_PATH + "Create Resource System Object", false, 10)]
        public static void CreateResourceSystemFromHierarchy()
        {
            CreateResourceSystemObject();
        }

        private static void CreateResourceSystemObject()
        {
            // 이미 씬에 존재하는지 체크
            var existingSystem = Object.FindObjectOfType<ResourceSystemComponent>();
            if (existingSystem != null)
            {
                Debug.LogWarning("Resource System already exists in the scene!");
                Selection.activeGameObject = existingSystem.gameObject;
                return;
            }

            // 프리팹 로드
            var prefab = GetResourceSystemPrefab();
            if (prefab == null)
            {
                Debug.LogError("Failed to load Resource System prefab!");
                return;
            }

            // 프리팹 인스턴스 생성
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.name = "ResourceSystem";
            
            // 생성된 오브젝트 선택
            Selection.activeGameObject = instance;
            
            // Undo 시스템에 등록
            Undo.RegisterCreatedObjectUndo(instance, "Create Resource System");
        }

        private static GameObject GetResourceSystemPrefab()
        {
            // 패키지 내의 프리팹 경로
            string[] guids = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(PREFAB_NAME) + " t:prefab");
            
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains(PREFAB_FOLDER) && path.EndsWith(PREFAB_NAME))
                {
                    return AssetDatabase.LoadAssetAtPath<GameObject>(path);
                }
            }

            // 프리팹이 없으면 생성
            return CreateResourceSystemPrefab();
        }

        private static GameObject CreateResourceSystemPrefab()
        {
            // 새 게임오브젝트 생성
            var go = new GameObject("ResourceSystem");
            go.AddComponent<ResourceSystemComponent>();
            go.AddComponent<ResourceSystemConfigure>();

            // 프리팹 저장을 위한 폴더 경로 확인 및 생성
            string packagePath = GetPackagePath();
            string fullPath = Path.Combine(packagePath, PREFAB_FOLDER);
            
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            // 프리팹 저장
            string prefabPath = Path.Combine(fullPath, PREFAB_NAME).Replace('\\', '/');
            prefabPath = prefabPath.Substring(prefabPath.IndexOf("Assets/"));
            
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            Object.DestroyImmediate(go);
            
            AssetDatabase.Refresh();
            
            return prefab;
        }

        private static ResourceSystemSettingSO GetOrCreateSettings()
        {
            var settings = Resources.Load<ResourceSystemSettingSO>("ResourceSystemSetting");
            if (settings == null)
            {
                // Settings 파일 생성
                settings = ScriptableObject.CreateInstance<ResourceSystemSettingSO>();
                
                string packagePath = GetPackagePath();
                string resourcesPath = Path.Combine(packagePath, "Editor/Resources");
                
                if (!Directory.Exists(resourcesPath))
                {
                    Directory.CreateDirectory(resourcesPath);
                }
                
                string assetPath = Path.Combine(resourcesPath, "ResourceSystemSetting.asset").Replace('\\', '/');
                assetPath = assetPath.Substring(assetPath.IndexOf("Assets/"));
                
                AssetDatabase.CreateAsset(settings, assetPath);
                AssetDatabase.SaveAssets();
            }
            
            return settings;
        }

        private static string GetPackagePath()
        {
            // 이 스크립트의 경로를 통해 패키지 루트 경로 찾기
            string[] guids = AssetDatabase.FindAssets("t:Script ResourceSystemEditorMenu");
            if (guids.Length == 0)
            {
                Debug.LogError("Cannot find ResourceSystemEditorMenu script!");
                return "";
            }
            
            string scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            return Path.GetDirectoryName(Path.GetDirectoryName(scriptPath)); // Editor 폴더의 상위 폴더
        }
    }
}