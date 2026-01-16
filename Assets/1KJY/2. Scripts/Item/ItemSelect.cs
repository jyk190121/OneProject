using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemSelect : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI itemNameText;                 // 선택한 아이템 이름
    public TextMeshProUGUI itemDescriptionText;          // 선택하 아이템 설명
    public Button[] itemButtons;                         // 10개 아이템 버튼을 인스펙터에서 할당
    public RectTransform[] selectionOutlines;            // 유저에게 보여줄 테두리 이미지
    public RectTransform selectKeyboardOutline;          // 키보드로 포커싱한 테두리 이미지
    List<int> selectedIndexs = new List<int>();          // 선택된 아이템의 인덱스를 순서대로 저장 (최대 3개)
    public Button nextBtn;                               // 배틀 씬으로 이동(1) or 업그레이드 상점으로 이동(2이상)
    int itemSelectedCount = 3;                           // 반드시 3개 선택

    List<Item> items;                                   // 현재 내가 가지고 있는 아이템 (최대 10개까지 불러오기)

    ItemManager itemManager;
    StageManager stageManager;

    void Awake()
    {
        itemManager = FindAnyObjectByType<ItemManager>();

        items = itemManager.CurrentItems();

        if(itemManager.GetSelectItems() != null)
        {
            //선택한 아이템 초기화
            itemManager.ResetSelectItems();
        }
      
        if(itemManager.GetNewItems() != null)
        {
            //새로 득템한 아이템리스트 초기화
            itemManager.ResetNewItems();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(selectKeyboardOutline != null)
        {
            selectKeyboardOutline.gameObject.SetActive(false);
        }
      
        // 처음엔 테두리를 숨김
        if (selectionOutlines != null)
        {
            for (int i = 0; i < selectionOutlines.Length; i++)
            {
                selectionOutlines[i].gameObject.SetActive(false);
            }
        }

        // 버튼 초기 설정
        for (int i = 0; i < itemButtons.Length; i++)
        {
            int index = i;
            Image btnImage = itemButtons[i].GetComponent<Image>();

            // 해금되지 않은 아이템 처리
            if (i > items.Count - 1)
            {
                itemButtons[i].gameObject.SetActive(false);
            }
            else
            {
                itemButtons[i].gameObject.SetActive(true);
                btnImage.sprite = items[i].IMAGE;
            }

            //itemButtons[i].onClick.AddListener(() => SelectItem(items, index));

            // EventTrigger 추가
            EventTrigger trigger = itemButtons[i].gameObject.GetComponent<EventTrigger>();
            if (trigger == null) trigger = itemButtons[i].gameObject.AddComponent<EventTrigger>();

            // [Select] 키보드로 포커스가 갔을 때 실행
            EventTrigger.Entry selectEntry = new EventTrigger.Entry();
            selectEntry.eventID = EventTriggerType.Select;
            selectEntry.callback.AddListener((data) => {
                // 1. 키보드 테두리 위치 이동
                UpdateKeyboardOutline(itemButtons[index].GetComponent<RectTransform>());
                // 2. 선택하지 않아도 정보를 미리 보여주고 싶다면 호출
                ShowItemInfo(index);
            });
            trigger.triggers.Add(selectEntry);

            // 클릭 리스너
            itemButtons[i].onClick.AddListener(() => SelectItem(items, index));
        }

        // 해금된 아이템이 있다면 첫 번째 버튼에 포커스
        if (items.Count > 0)
        {
            // EventSystem이 버튼을 선택하게 함
            EventSystem.current.SetSelectedGameObject(itemButtons[0].gameObject);
            // 시각적으로 키보드 테두리 업데이트 (처음 위치 잡아주기)
            UpdateKeyboardOutline(itemButtons[0].GetComponent<RectTransform>());
        }

        itemNameText.text = "";
        itemDescriptionText.text = "";

        stageManager = FindAnyObjectByType<StageManager>();
        int stageNum = stageManager.SelectedStage;

        nextBtn.onClick.AddListener(() => NextSceneSelect(stageNum));
    }

    void Update()
    {
        // 아무것도 선택되어 있지 않은데 키보드 입력을 하면 다시 첫 번째 버튼 선택
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (Keyboard.current.aKey.wasPressedThisFrame ||
                Keyboard.current.dKey.wasPressedThisFrame ||
                Keyboard.current.leftArrowKey.wasPressedThisFrame ||
                Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                EventSystem.current.SetSelectedGameObject(itemButtons[0].gameObject);
            }
        }
    }
    void SelectItem(List<Item> items, int index)
    {
        // --- 유저용 UI 테두리 처리 ---
        //if (selectionOutlines != null)
        //{
        //    for (int i = 0; i < selectionOutlines.Length; i++)
        //    {
        //        RectTransform btnRect = itemButtons[index].GetComponent<RectTransform>();

        //        //3개 이하 선택 시
        //        if (selectionOutlines[i].gameObject.activeSelf == false && itemSelectedCount > 0)
        //        {
        //            selectionOutlines[i].gameObject.SetActive(true);
        //            // 선택된 버튼의 위치로 테두리 이동
        //            selectionOutlines[i].position = btnRect.position;
        //            // 테두리 크기를 버튼 크기에 맞춤
        //            selectionOutlines[i].sizeDelta = btnRect.sizeDelta;
        //            itemSelectedCount--;
        //            break;
        //        }
        //        //3개 이상 선택 시
        //        else if (itemSelectedCount == 0)
        //        {
        //            selectionOutlines[0].position = btnRect.position;
        //        }

        //    }


        // 이미 선택된 아이템을 다시 누르면 취소하는 로직 (선택 사항)
        if (selectedIndexs.Contains(index))
        {
            selectedIndexs.Remove(index);
            //itemManager.selectItem.Remove(items[index]);
            itemManager.RemoveSelectItems(items[index]);
            UpdateUI();
            return;
        }

        // 4번째 아이템을 선택하면 첫 번째(0번 인덱스) 제거
        if (selectedIndexs.Count >= 3)
        {
            selectedIndexs.RemoveAt(0);
            //itemManager.selectItem.RemoveAt(0);
            itemManager.RemoveSelectItems();
        }

        // 새로운 아이템 추가
        selectedIndexs.Add(index);
        //itemManager.selectItem.Add(items[index]);
        itemManager.SetSelectItems(items[index]);

        // UI 업데이트
        UpdateUI();
        itemNameText.text = $"{items[index].NAME}";
        itemDescriptionText.color = Color.cyan;
        itemDescriptionText.text = $"{items[index].EXPLAIN}";
    }

    void UpdateUI()
    {
        // 먼저 모든 테두리를 끈다
        foreach (RectTransform outline in selectionOutlines) outline.gameObject.SetActive(false);

        // 현재 선택된 리스트 순서대로 테두리 배치
        for (int i = 0; i < selectedIndexs.Count; i++)
        {
            int itemIdx = selectedIndexs[i];
            RectTransform targetBtn = itemButtons[itemIdx].GetComponent<RectTransform>();

            selectionOutlines[i].gameObject.SetActive(true);
            selectionOutlines[i].position = targetBtn.position;
            selectionOutlines[i].sizeDelta = targetBtn.sizeDelta;
        }
    }

    void NextSceneSelect(int stageNum)
    {
        // Item을 반드시 3개 선택
        if (selectedIndexs.Count != itemSelectedCount)
        {
            itemNameText.text = "3개 선택!";
            itemDescriptionText.text = "";
            return;
        }
        if (stageNum == 1)
        {
            // 배틀씬으로 바로이동(stage1)
            GameSceneManager.Instance.LoadSceneAsync("BattleScene");
        }
        else
        {
            // 업그레이드 상점으로 이동
            GameSceneManager.Instance.LoadSceneAsync("UpgradeStoreScene");
        }
        print(stageNum);
    }
    void UpdateKeyboardOutline(RectTransform targetRect)
    {
        if (selectKeyboardOutline == null) return;

        selectKeyboardOutline.gameObject.SetActive(true);
        selectKeyboardOutline.position = targetRect.position;
        selectKeyboardOutline.sizeDelta = targetRect.sizeDelta;

    }

    // 아이템 정보만 미리 보여주는 함수 (선택 전)
    void ShowItemInfo(int index)
    {
        if (index < 0 || index >= items.Count) return;
        itemNameText.text = items[index].NAME;
        itemDescriptionText.color = Color.gray; // 선택 전에는 회색으로 표시하는 등 구분 가능
        itemDescriptionText.text = items[index].EXPLAIN;
    }

}

