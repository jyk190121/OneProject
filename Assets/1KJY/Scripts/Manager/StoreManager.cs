using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
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
    StageManager stageManager;

    public TextMeshProUGUI goldTxt;
    public Button nextBtn;

    void Awake()
    {
        itemManager = FindAnyObjectByType<ItemManager>();
        stageManager = FindAnyObjectByType<StageManager>();

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
            GameObject bg = Instantiate(itemInvenBGPrefab, CurrentPanel.transform);
            itemsBG.Add(bg);
        }


        if (stageManager != null)
        {
            nextBtn.onClick.AddListener(() => NextStage(stageManager.SelectedStage));
            CurrentItemUpdate();
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
        ItemEnhance slot = newItemObj.GetComponent<ItemEnhance>();
        if (slot == null) slot = newItemObj.AddComponent<ItemEnhance>();

        slot.Setup(item);
    }

    void NextStage(int stageNum)
    {
        GameSceneManager.Instance.LoadSceneAsync("BattleScene");
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
        items = itemManager.CurrentItems();
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
            GameObject bg = Instantiate(itemInvenBGPrefab, CurrentPanel.transform);
            itemsBG.Add(bg);

            // 아이템 이미지 생성 (UpdateSlot 호출)
            UpdateSlot(i, items[i]);
        }
    }

}
