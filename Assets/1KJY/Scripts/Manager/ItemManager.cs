using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// 1. 기본 아이템 3개 설정 (아이템보유)
/// 2. 수집된 아이템 CollectItemList에 넘겨주기
/// </summary>
public class ItemManager : MonoBehaviour
{
    // 인스펙터에서 프로젝트에 있는 모든 아이템
    public List<Item> allItemDatas; 

    [Header("보유 아이템 리스트")]
    [SerializeField] private List<Item> initialItems;

    public int gold;

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
        //Debug.Log($"{item}아이템 추가");
    }
    //구매
    public void BuyItem(Item newItem)
    {
        //플레이어 골드체크

        foreach (Item item in initialItems)
        {
            if (item.ID != newItem.ID)
            {
                AddItem(item);
            }
            else
            {
                ItemEnhance(item);
                //아이템 수치 증가 필요
            }
        }
    }

    //1강 보다 2강이 비싸고, 2강보다 3강이 비싸도록
    public void ItemEnhance(Item item)
    {
        if (item.ENHANCE < 3)
        {
            item.ENHANCE++;
        }
        else
        {
            print("최대 강화에 도달");
        }

    }

    public void Init()
    {
        //플레이어 사망 등 아이템 초기화
        foreach (Item item in initialItems)
        {
            item.ENHANCE = 0;
            gold = 0;
        }
    }

    private void Update()
    {
        if (Keyboard.current.f1Key.wasPressedThisFrame == true)
        {
            ItemManager.Instance.SetGold(10000);
        }
    }

    public int GetGold() { return gold; }
    public void SetGold(int gold) { this.gold = gold; }
    
    public void PlusGold(int gold) { this.gold += gold; }
    public void MinusGold(int gold) { this.gold -= gold; }

}
