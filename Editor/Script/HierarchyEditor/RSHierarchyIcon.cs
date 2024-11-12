using UnityEngine;
using UnityEditor;

namespace ActionFit.Framework.Addressable.Editor
{
    [InitializeOnLoad]
    public class RSHierarchyIcon
    {
        private static Texture2D iconTexture;
        
        // 에디터가 시작될 때 실행
        static RSHierarchyIcon()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
            LoadIcon();
        }
        
        private static void LoadIcon()
        {
            if (iconTexture == null)
            {
                iconTexture = Resources.Load<Texture2D>("ActFitFrameworkIcon");
            }
        }

        private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            
            if (gameObject != null && gameObject.GetComponent<ResourceSystemComponent>() != null)
            {
                // 아이콘을 표시할 위치 계산
                Rect iconRect = new Rect(selectionRect);
                iconRect.x = selectionRect.x - 20; // 게임오브젝트 이름 앞에 위치하도록 조정
                iconRect.width = 16;
                iconRect.height = 16;
                
                // y 위치를 중앙에 맞추기 위한 조정
                iconRect.y += (selectionRect.height - 16) * 0.5f;
                
                if (iconTexture != null)
                {
                    // 아이콘 그리기
                    GUI.DrawTexture(iconRect, iconTexture);
                }
            }
        }
    }
}