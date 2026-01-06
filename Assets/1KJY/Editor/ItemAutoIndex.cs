#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class ItemAutoIndex : AssetPostprocessor
{
    //에셋이 생성된 후 자동으로 호출되는 함수
    static void OnPostprocessAllAssets(string[] importedAssets)
    {
        foreach (string path in importedAssets)
        {
            //생성된 에셋이 Item 타입인지 확인
            Item item = AssetDatabase.LoadAssetAtPath<Item>(path);
            
            //ID가 부여되지 않은 상태(기본값 0)일때
            if(item != null && item.ID == 0)
            {
                int nextID = GenerateNextID();
                item.SetID(nextID);

                //변경사항 저장
                EditorUtility.SetDirty(item);
                AssetDatabase.SaveAssets();
                Debug.Log($"[ItemManager] 새 아이템 생성됨: {item.name}, 할당된 ID: {nextID}");
            }
        }

        // 프로젝트 내의 모든 Item 에셋을 찾아 가장 높은 ID + 1을 반환
        static int GenerateNextID()
        {
            string[] guids = AssetDatabase.FindAssets("t:Item");
            int maxID = 0;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Item item = AssetDatabase.LoadAssetAtPath<Item>(path);

                if (item != null && item.ID > maxID)
                {
                    maxID = item.ID;
                }
            }

            return maxID + 1;
        }
    }
}
#endif
