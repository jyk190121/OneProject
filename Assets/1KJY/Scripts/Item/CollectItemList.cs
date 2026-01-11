using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 수집된 아이템 생성하여 보여주기
/// </summary>
public class CollectItemList : MonoBehaviour
{
    public List<Item> items;            //가지고 있는 아이템리스트
    public GameObject itemBGPrefab;     //아이템 배경 프리팹
    List<GameObject> itemsBG = new List<GameObject>();
    public GameObject itemPrefab;       //아이템 프리펩
    public GameObject itemListParent;   //가지고 있는 아이템 프리펩 부모 오브젝트
    int itemCollectCount = 28;

    ItemManager itemManager;

    void Awake()
    {
        itemManager = FindAnyObjectByType<ItemManager>();

        items = itemManager.CurrentItems();

        //배경슬롯 준비
        for (int i = 0; i < itemCollectCount; i++)
        {
            GameObject bg = Instantiate(itemBGPrefab, itemListParent.transform);
            itemsBG.Add(bg);
        }

        for(int i =0; i< items.Count; i++)
        {
            UpdateSlot(i, items[i]);
        }
    }

    //public void AddItemList(Item newItem)
    //{
    //    if (items.Count >= itemCollectCount)
    //    {
    //        print("아이템 모두 수집"); 
    //        return;
    //    }

    //    //수집한 아이템 리스트에 추가
    //    items.Add(newItem);

    //    //// 새로 추가된 아이템만 해당 순서의 슬롯에 그려줍니다.
    //    //UpdateSlot(items.Count - 1, newItem);
    //}

    void UpdateSlot(int index, Item item)
    {
        //// 1. 해당 슬롯(배경)의 자식으로 아이템 프리팹 생성
        //GameObject newItemObj = Instantiate(itemPrefab, itemsBG[index].transform);

        //// 2. 프리팹 원본이 아닌, 방금 생성된 복제본(newItemObj)의 Image를 가져옵니다.
        //Image itemImg = newItemObj.GetComponent<Image>();

        //if (itemImg != null && item != null)
        //{
        //    itemImg.sprite = item.IMAGE;
        //}

        if (items.Count > itemCollectCount)
        {
            print("아이템 모두 수집");
            return;
        }

        // 1. 프리팹 생성 (worldPositionStays를 false로 설정하여 UI 좌표 체계 유지)
        GameObject newItemObj = Instantiate(itemPrefab, itemsBG[index].transform, false);

        // 2. RectTransform 설정 강제 초기화
        RectTransform rect = newItemObj.GetComponent<RectTransform>();
        if (rect != null)
        {
            // 부모(배경)의 크기에 꽉 차게 만들고 싶을 때 (앵커 프리셋: Stretch-Stretch)
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            //rect.offsetMin = Vector2.zero; // Left, Bottom 0
            //rect.offsetMax = Vector2.zero; // Right, Top 0
            float padding = 10f;
            rect.offsetMin = new Vector2(padding, padding);   // Left, Bottom 여백
            rect.offsetMax = new Vector2(-padding, -padding); // Right, Top 여백 (마이너스 값 필수)
            rect.localScale = Vector3.one;                    // 간혹 스케일이 0으로 생성되는 버그 방지
        }

        // 3. 이미지 설정
        Image itemImg = newItemObj.GetComponent<Image>();
        if (itemImg != null && item != null)
        {
            itemImg.sprite = item.IMAGE;
            //itemImg.color = Color.white;
        }

        // 추가: ItemSlot 컴포넌트를 가져와서 데이터 세팅
        ItemSlot slot = newItemObj.GetComponent<ItemSlot>();
        if (slot == null) slot = newItemObj.AddComponent<ItemSlot>();

        slot.Setup(item);
    }
}
