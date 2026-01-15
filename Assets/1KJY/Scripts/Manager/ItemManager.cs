using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// 1. 기본 아이템 3개 설정 (아이템보유)
/// 2. 수집된 아이템 CollectItemList에 넘겨주기
/// </summary>
public class ItemManager : MonoBehaviour
{
    // 데이터 변경 시 UI 등에 알림을 주기 위한 이벤트
    public static event Action<Item> OnItemAdd;

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
            LoadItems();
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
        if (!initialItems.Contains(item))
        {
            initialItems.Add(item);
            SaveItems(); // 아이템 리스트 전체 저장
            OnItemAdd?.Invoke(item);
        }
    }

    // 아이템 리스트를 문자열로 변환하여 저장
    void SaveItems()
    {
        string itemIds = "";
        for (int i = 0; i < initialItems.Count; i++)
        {
            itemIds += initialItems[i].ID.ToString();
            if (i < initialItems.Count - 1) itemIds += ","; // ID 사이를 쉼표로 구분
        }
        PlayerPrefs.SetString("SavedItems", itemIds);
        PlayerPrefs.Save();
    }

    public void LoadItems()
    {
        
        string savedData = PlayerPrefs.GetString("SavedItems", "");
        if (string.IsNullOrEmpty(savedData)) return;

        string[] ids = savedData.Split(',');
        initialItems.Clear();

        foreach (string idStr in ids)
        {
            int id = int.Parse(idStr);
            // 전체 아이템 리스트(allItems)에서 ID가 일치하는 아이템 검색
            Item foundItem = allItemDatas.Find(x => x.ID == id);
            if (foundItem != null)
            {
                initialItems.Add(foundItem);
                //모든 아이템 강화는 게임 시작 시 초기화
                Init();
            }
        }
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
            ItemManager.Instance.SetGold(2000);
        }
    }

    public int GetGold() { return gold; }
    public void SetGold(int gold) { this.gold = gold; }
    
    public void PlusGold(int gold) { this.gold += gold; }
    public void MinusGold(int gold) { this.gold -= gold; }

}
