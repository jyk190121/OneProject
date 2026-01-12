using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 1. 기본 아이템 3개 설정 (아이템보유)
/// 2. 수집된 아이템 CollectItemList에 넘겨주기
/// </summary>
public class ItemManager : MonoBehaviour
{
    [Header("보유 아이템 리스트")]
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

    //현재 가지고 있는 아이템 리스트 넘겨줌
    public List<Item> CurrentItems()
    {
        return initialItems;
    }
    //업그레이드 상점에서 새로운 아이템 구매 시 추가
    public void AddItem(Item item)
    {
        initialItems.Add(item);
        Debug.Log($"{item}아이템 추가");
    }
}
