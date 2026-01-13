using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
/// <summary>
/// 1. 보유 아이템 인벤토리 노출
/// 2. 상점
/// - 보유아이템 : 강화 표시
///     주의할점 : 강화된 아이템은 아이템 설명란에도 강화수치로 보여주어야 함
/// - 미보유아이템 : 구매 시 아이템리스트에 추가해줌(강화 표시 미노출)
/// 
/// 
/// </summary>
public class StoreManager : MonoBehaviour
{
    public GameObject CurrentPanel;                     //현재 보유한 아이템 보여줄 판넬
    public GameObject itemInvenPrefab;                  //보유한 아이템 이미지(프리팹)
    public GameObject itemInvenBGPrefab;                //보유한 아이템 이미지 배경(프리팹)
    List<GameObject> itemsBG = new List<GameObject>();

    List<Item> items;

    ItemManager itemManager;

    Player player;

    void Awake()
    {
        itemManager = FindAnyObjectByType<ItemManager>();
        player = FindAnyObjectByType<Player>();

        if(itemManager != null)
        {
            items = itemManager.CurrentItems();
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemsBG.Clear();

        //배경슬롯 준비
        for (int i=0; i< items.Count; i++)
        {
            GameObject bg = Instantiate(itemInvenBGPrefab, CurrentPanel.transform);
            itemsBG.Add(bg);
        }

        //받아온 아이템 인벤토리에 보여주기
        for (int i = 0; i < items.Count; i++)
        {
            UpdateSlot(i, items[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateSlot(int index, Item item)
    {
        GameObject newItemObj = Instantiate(itemInvenPrefab, itemsBG[index].transform, false);

        RectTransform rect = newItemObj.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            float padding = 10f;
            rect.offsetMin = new Vector2(padding, padding);  
            rect.offsetMax = new Vector2(-padding, -padding);
            rect.localScale = Vector3.one;                   
        }

        Image itemImg = newItemObj.GetComponent<Image>();
        if (itemImg != null && item != null)
        {
            itemImg.sprite = item.IMAGE;
        }
        ItemSlot slot = newItemObj.GetComponent<ItemSlot>();
        if (slot == null) slot = newItemObj.AddComponent<ItemSlot>();

        slot.Setup(item);
    }
}
