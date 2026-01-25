using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
    public GameObject currentPanel;                     //현재 보유한 아이템 보여줄 판넬
    public GameObject itemInvenPrefab;                  //보유한 아이템 이미지(프리팹)
    public GameObject itemInvenBGPrefab;                //보유한 아이템 이미지 배경(프리팹)
    List<GameObject> itemsBG = new List<GameObject>();
    public GameObject sellFailPanel;                    //판매실패 시 보여줄 판넬
    public Button sellFailCheckBtn;                     //판매실패 확인 버튼

    List<Item> items;

    ItemManager itemManager;
    StageManager stageManager;

    public TextMeshProUGUI goldTxt;
    public Button nextBtn;
    public Button prevBtn;
    Popup popup;

    void Awake()
    {
        itemManager = FindAnyObjectByType<ItemManager>();
        stageManager = FindAnyObjectByType<StageManager>();
        popup = GetComponent<Popup>();

        if (itemManager != null)
        {
            //아이템 구매 시에도 업데이트 필요
            UpdateUI();
            //items = itemManager.CurrentItems();
            //goldTxt.text = itemManager.GetGold().ToString();
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemsBG.Clear();

        //배경슬롯 준비
        for (int i=0; i< items.Count; i++)
        {
            GameObject bg = Instantiate(itemInvenBGPrefab, currentPanel.transform);
            itemsBG.Add(bg);
        }


        if (stageManager != null && GameSceneManager.Instance.SceneName() != "ArenaUpgradeStoreScene")
        {
            nextBtn.onClick.AddListener(() => NextStage(stageManager.SelectedStage));
            CurrentItemUpdate();
        }
        else
        {
            nextBtn.onClick.AddListener(() => NextArena());
            CurrentItemUpdate();
        }

        if(itemManager != null)
        {
            sellFailCheckBtn.onClick.AddListener(() => CloseFailUI());
        }

        if(GameSceneManager.Instance.SceneName() != "ArenaUpgradeStoreScene")
        {
            // 스테이지 선택창으로 돌아가기
            prevBtn.onClick.AddListener(() => BackStageScene());
        }
    }
    public void UpdateSlot(int index, Item item)
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
        ItemSellEnhance slot = newItemObj.GetComponent<ItemSellEnhance>();
        if (slot == null) slot = newItemObj.AddComponent<ItemSellEnhance>();

        slot.Setup(item);
    }

    void NextStage(int stageNum)
    {
        GameSceneManager.Instance.LoadSceneAsync("BattleScene");
    }
    void NextArena()
    {
        GameSceneManager.Instance.LoadSceneAsync("ArenaScene");
    }


    //public void UpdateUI()
    //{
    //    items = itemManager.CurrentItems();
    //    goldTxt.text = itemManager.GetGold().ToString();
    //}

    //public void CurrentItemUpdate()
    //{
    //    //받아온 아이템 인벤토리에 보여주기
    //    for (int i = 0; i < items.Count; i++)
    //    {
    //        UpdateSlot(i, items[i]);
    //    }
    //}

    public void UpdateUI()
    {
        items = itemManager.GetSelectItems();
        goldTxt.text = itemManager.GetGold().ToString();

        //// UI 갱신 시 인벤토리 아이템들도 다시 그려줍니다.
        //CurrentItemUpdate();
    }

    public void CurrentItemUpdate()
    {
        // 1. 기존에 생성된 아이템 오브젝트들을 모두 제거 (중복 생성 방지)
        foreach (GameObject bg in itemsBG)
        {
            if (bg != null) Destroy(bg);
        }
        itemsBG.Clear();

        // 2. 현재 보유한 아이템 개수만큼 배경(Slot)과 아이템 이미지를 생성
        for (int i = 0; i < items.Count; i++)
        {
            // 배경 생성
            GameObject bg = Instantiate(itemInvenBGPrefab, currentPanel.transform);
            itemsBG.Add(bg);

            // 아이템 이미지 생성 (UpdateSlot 호출)
            UpdateSlot(i, items[i]);
        }
    }


    public void SellFailUI()
    {
        //// 이미 켜져 있다면 끄고 다시 켜서 '재실행' 효과를 줌
        //if (sellFailPanel.activeSelf)
        //{
        //    sellFailPanel.SetActive(false);
        //}

        sellFailPanel.SetActive(true);
    }

    void CloseFailUI()
    {
        sellFailPanel.gameObject.SetActive(false);
    }

    void BackStageScene()
    {
        popup.ShowConfirm(
                  $"스테이지 선택 화면으로 나가시겠습니까??\n<color=red>아이템 강화 및 현재 보유골드는 초기화됩니다</color>",
                  () => ExecuteNewGame() // 'Yes'를 누르면 실행될 람다식(Action)
                  );
    }

    void ExecuteNewGame()
    {
        itemManager.Init();
        stageManager.ReloadChance = 1;
        GameSceneManager.Instance.LoadSceneAsync("StageScene");
        AudioManager.audioManager.StopBGM();
    }
}
