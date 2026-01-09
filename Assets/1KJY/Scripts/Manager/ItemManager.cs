using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 1. 기본 아이템 3개 설정 (아이템보유)
/// 2. 수집된 아이템 CollectItemList에 넘겨주기
/// </summary>
public class ItemManager : MonoBehaviour
{
    CollectItemList collectItem;

    [Header("초기 지급 아이템 리스트")]
    [SerializeField] private List<Item> initialItems;

    public static ItemManager Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        collectItem = FindAnyObjectByType<CollectItemList>();

        if (collectItem != null)
        {
            GiveInitialItems();
        }
    }

    private void GiveInitialItems()
    {
        foreach (Item item in initialItems)
        {
            if (item != null)
            {
                // CollectItemList에 아이템 정보를 하나씩 넘겨줍니다.
                collectItem.AddItemList(item);
                Debug.Log($"{item.NAME}이(가) 기본 아이템으로 지급되었습니다.");
            }
        }
    }
}
