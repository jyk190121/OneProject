using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSelect : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI itemNameText;                 // 선택한 아이템 이름
    public TextMeshProUGUI itemDescriptionText;          // 선택하 아이템 설명
    public Button[] itemButtons;                         // 10개 아이템 버튼을 인스펙터에서 할당
    public RectTransform[] selectionOutlines;            // 유저에게 보여줄 테두리 이미지
    List<int> selectedIndexs = new List<int>();          // 선택된 아이템의 인덱스를 순서대로 저장 (최대 3개)
    public Button nextBtn;                               // 배틀 씬으로 이동(1) or 업그레이드 상점으로 이동(2이상)
    int itemSelectedCount = 3;                           // 반드시 3개 선택

    public Item[] items;                                 // 현재 내가 가지고 있는 아이템

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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
            if (i > items.Length - 1)
            {
                itemButtons[i].gameObject.SetActive(false);
            }
            else
            {
                itemButtons[i].gameObject.SetActive(true);
                btnImage.sprite = items[i].IMAGE;
            }

            itemButtons[i].onClick.AddListener(() => SelectItem(items, index));
        }

        itemNameText.text = "";
        itemDescriptionText.text = "";
        int stageNum = StageManager.CurrentStage;
        nextBtn.onClick.AddListener(() => NextSceneSelect(stageNum));
    }


    void SelectItem(Item[] items, int index)
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
            UpdateUI();
            return;
        }

        // 4번째 아이템을 선택하면 첫 번째(0번 인덱스) 제거
        if (selectedIndexs.Count >= 3)
        {
            selectedIndexs.RemoveAt(0);
        }

        // 새로운 아이템 추가
        selectedIndexs.Add(index);

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
            print("스테이지 1");
            GameSceneManager.Instance.LoadSceneAsync("BattleScene");
        }
        else
        {
            // 업그레이드 상점으로 이동
            GameSceneManager.Instance.LoadSceneAsync("UpgradeStoreScene");
        }
    }
}

