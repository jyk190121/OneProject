using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// 1. 기본 아이템 3개 설정 (아이템보유)
/// 2. 수집된 아이템 CollectItemList에 넘겨주기
/// </summary>

//InputManager처럼
using Key = UnityEngine.InputSystem.Key;
public class ItemManager : MonoBehaviour
{
    // 데이터 변경 시 UI 등에 알림을 주기 위한 이벤트
    public static event Action<Item> OnItemAdd;

    // 인스펙터에서 프로젝트에 있는 모든 아이템
    public List<Item> allItemDatas;

    // 인스펙터에서 초기 아이템 설정
    public List<Item> resetItem;

    // ItemSelectScene에서 3개 선택한 아이템 -> UpdateStore, Spinner에 반영
    List<Item> selectItems = new List<Item>();
    // GameOver 시 새로 획득한 아이템 -> OverScene에 반영
    List<Item> newItems = new List<Item>();

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
        if (!selectItems.Contains(item))
        {
            SetSelectItems(item);
        }

        if(!newItems.Contains(item))
        {
            SetNewItems(item);
        }

        if (!initialItems.Contains(item))
        {
            initialItems.Add(item);
            SaveItems(); // 아이템 리스트 전체 저장
            OnItemAdd?.Invoke(item);
        }
    }

    //아이템 판매
    public void SellItem(Item item, int price)
    {
        if (selectItems.Contains(item))
        {
            PlusGold(price);
            selectItems.Remove(item);
            item.ENHANCE = 0;
        }
    }

    // 아이템 리스트를 문자열로 변환하여 저장
    void SaveItems()
    {
        List<string> nameList = new List<string>();
        foreach (Item item in initialItems)
        {
            nameList.Add(item.NAME); // ID가 불안정하다면 NAME으로 저장
        }

        // 이름들을 "사과,포도,검" 형태의 문자열로 변환
        string saveData = string.Join(",", nameList);
        PlayerPrefs.SetString("SavedInventory", saveData);
        PlayerPrefs.Save();
    }
    private void LoadItems()
    {
        if (!PlayerPrefs.HasKey("SavedInventory")) return;

        string saveData = PlayerPrefs.GetString("SavedInventory");
        if (string.IsNullOrEmpty(saveData)) return;

        string[] names = saveData.Split(',');
        initialItems.Clear();

        foreach (string name in names)
        {
            // 전체 데이터베이스(allItemDatas)에서 이름이 같은 아이템을 찾아 추가
            Item foundItem = allItemDatas.Find(x => x.NAME == name);
            if (foundItem != null)
            {
                Init();
                initialItems.Add(foundItem);
            }
        }
    }
    //public void LoadItems()
    //{

    //    string savedData = PlayerPrefs.GetString("SavedItems", "");
    //    if (string.IsNullOrEmpty(savedData)) return;

    //    string[] ids = savedData.Split(',');
    //    initialItems.Clear();

    //    foreach (string idStr in ids)
    //    {
    //        int id = int.Parse(idStr);
    //        // 전체 아이템 리스트(allItems)에서 ID가 일치하는 아이템 검색
    //        Item foundItem = allItemDatas.Find(x => x.ID == id);
    //        if (foundItem != null)
    //        {
    //            initialItems.Add(foundItem);
    //            //모든 아이템 강화는 게임 시작 시 초기화
    //            LoadEnhanceLevel(foundItem);
    //        }
    //    }
    //}

    // 아이템 로드 시 호출
    //void LoadEnhanceLevel(Item item)
    //{
    //    int savedLevel = PlayerPrefs.GetInt($"Item_{item.NAME}_Enhance", 0);
    //    item.ENHANCE = savedLevel;
    //    gold = 0;
    //}

    ////구매
    //public void BuyItem(Item newItem)
    //{
    //    //플레이어 골드체크

    //    foreach (Item item in initialItems)
    //    {
    //        if (item.ID != newItem.ID)
    //        {
    //            AddItem(item);
    //        }
    //        else
    //        {
    //            ItemEnhance(item);
    //            //아이템 수치 증가 필요
    //        }
    //    }
    //}

    ////1강 보다 2강이 비싸고, 2강보다 3강이 비싸도록
    //public void ItemEnhance(Item item)
    //{
    //    if (item.ENHANCE < 3)
    //    {
    //        item.ENHANCE++;
    //    }
    //    else
    //    {
    //        print("최대 강화에 도달");
    //    }

    //}

    public void Init()
    {
        //플레이어 사망 등 아이템 초기화
        foreach (Item item in allItemDatas)
        {
            item.ENHANCE = 0;
            gold = 0;
        }
    }

    //게임 새로하기 시
    public void ResetItem()
    {
        initialItems.Clear();
        Init();

        //초기 아이템 설정
        foreach (Item item in resetItem)
        {
            AddItem(item);
        }
    }

    //3개 아이템 선택한 거
    public List<Item> GetSelectItems()
    {
        return selectItems; 
    }

    public void SetSelectItems(Item item)
    {
        selectItems.Add(item);
    }
    public void RemoveSelectItems(Item item)
    {
        selectItems.Remove(item);
    }
    public void RemoveSelectItems()
    {
        selectItems.RemoveAt(0);
    }
    public void ResetSelectItems()
    {
        selectItems.Clear();
    }
  
    public List<Item> GetNewItems()
    {
        return newItems; 
    }
    public void SetNewItems(Item item)
    {
        newItems.Add(item);
    }

    public void ResetNewItems()
    {
        newItems.Clear();
    }

    private void Update()
    {
        if (Input.GetKeyDown(Key.F1))
        {
            SetGold(2000);
            GameSceneManager.Instance.RestartScene();
        }

        if (Input.GetKeyDown(Key.F2))
        {
            //모든 아이템 득
            foreach (Item item in allItemDatas)
            {
                AddItem(item);
                GameSceneManager.Instance.RestartScene();
            }
        }

        //if (Input.GetKeyDown(Key.F5))
        //{
        //    print("아이템 초기화");
        //    ResetItem();
        //}
    }

    public int GetGold() { return gold; }
    public void SetGold(int gold) { this.gold = gold; }

    public void PlusGold(int gold) { this.gold += gold; }
    public void MinusGold(int gold) { this.gold -= gold; }

}
