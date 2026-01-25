using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 1. Stage 버튼 선택 시 해당 스테이지로 이동
///  - 한번 선택 시 스테이지 명 노출
///  - 두번 선택 시 스테이지 진입
/// </summary>
public class StageUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI stageText;
    public Button[] stageButtons;                        // Stage 버튼을 인스펙터에서 할당
    public RectTransform selectionOutline;               // 유저에게 보여줄 테두리 이미지
    public Button backgroundCancelBtn;                   // 배경 클릭 감지용 버튼 추가

    //[Header("Stage Settings")]
    //public Transform[] stagePositions;                 // 기즈모를 표시할 스테이지별 위치

    int selectedStageIndex = -1;                         // 현재 선택된 스테이지 (-1은 선택 없음)
    int unlockedStageIndex;                              // 현재 해금된 최대 스테이지

    //StageManager stageManager;

    public Button backBtn;                              // 스테이지 뒤로 이동 버튼
    public Button frontBtn;                             // 스테이지 앞으로 이동 버튼

    // 1. 클래스 상단 변수에 현재 포커스된 인덱스 저장용 변수 추가(기존 selectedStageIndex 활용 가능)
    // 여기서는 이해를 돕기 위해 명시적으로 관리합니다.
    int currentFocusIndex = 0;

    void Awake()
    {
        //StageManager에서 넘겨준 스테이지 번호 확인
        //stageManager = FindAnyObjectByType<StageManager>();
        unlockedStageIndex = StageManager.Instance.UnlockedStage;
        print($"해금된 스테이지는 {unlockedStageIndex}");
        if (AudioManager.audioManager.GetCurrentBGM() == "")
        {
            AudioManager.audioManager.StopBGM();
            AudioManager.audioManager.PlayBGM("Intro");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 처음엔 테두리를 숨김
        if (selectionOutline != null) selectionOutline.gameObject.SetActive(false);

        ////stage1Btn.onClick.AddListener(() => SelectStage(1));
        //// 반복문을 통해 10개 버튼에 이벤트 등록
        //for (int i = 0; i < stageButtons.Length; i++)
        //{
        //    int index = i + 1; // 스테이지 번호 (1~10)
        //    stageButtons[i].onClick.AddListener(() => OnStageButtonClick(index));
        //}

        // 버튼 초기 설정
        for (int i = 0; i < stageButtons.Length; i++)
        {
            int stageNum = i + 1;

            // 기본 onClick은 제거하고 커스텀 이벤트를 등록합니다.
            stageButtons[i].onClick.RemoveAllListeners();

            EventTrigger trigger = stageButtons[i].gameObject.GetComponent<EventTrigger>();
            if (trigger == null) trigger = stageButtons[i].gameObject.AddComponent<EventTrigger>();

            // 1. 키보드 방향키로 "포커스"가 옮겨졌을 때 (선택 상태 업데이트)
            EventTrigger.Entry selectEntry = new EventTrigger.Entry();
            selectEntry.eventID = EventTriggerType.Select;
            selectEntry.callback.AddListener((data) => {
                // 마우스 클릭 중이 아닐 때만 (즉, 키보드/패드 이동일 때만) 인덱스 즉시 갱신
                if (!(Input.GetMouseButton(0) || Input.GetMouseButton(1)))
                {
                    SelectStage(stageNum);
                }
            });
            trigger.triggers.Add(selectEntry);


            // 2. 마우스로 "클릭"했을 때 (기존 로직: 선택 or 진입)
            EventTrigger.Entry clickEntry = new EventTrigger.Entry();
            clickEntry.eventID = EventTriggerType.PointerClick;
            clickEntry.callback.AddListener((data) =>
            {
            PointerEventData p = data as PointerEventData;
                if (p != null && p.button == PointerEventData.InputButton.Left)
                {
                    OnMouseClick(stageNum);
                }
            });

            trigger.triggers.Add(clickEntry);

            // 3. 키보드로 "엔터(Submit)"를 눌렀을 때 (즉시 진입)
            EventTrigger.Entry submitEntry = new EventTrigger.Entry();
            submitEntry.eventID = EventTriggerType.Submit;
            submitEntry.callback.AddListener((data) => { EnterStage(stageNum); });
            trigger.triggers.Add(submitEntry);


            // 해금되지 않은 스테이지 처리
            if (stageNum > unlockedStageIndex)
            {
                Image btnImage = stageButtons[i].GetComponent<Image>();

                // 알파값을 약 100/255 (0.4f) 정도로 낮춤
                Color color = btnImage.color;
                color.a = 0.4f;
                btnImage.color = color;
            }

            //// 모든 버튼은 일단 '클릭'은 가능하게 둠 (선택은 되어야 하므로)
            //stageButtons[i].interactable = true;
            //stageButtons[i].onClick.AddListener(() => OnStageButtonClick(stageNum));
        }

        //스테이지 진입 시 포커스될 버튼
        stageButtons[0].Select();

        // 배경 버튼 클릭 시 취소 함수 실행
        if (backgroundCancelBtn != null)
        {
            backgroundCancelBtn.onClick.AddListener(CancelSelection);
            //print("배경 클릭!");
        }

        backBtn.onClick.AddListener(OnClickBack);
        frontBtn.onClick.AddListener(OnClickFront);
    }
  
    // 마우스 클릭 시 호출
    void OnMouseClick(int stageNum)
    {
        // 1. 이미 선택된 스테이지를 다시 눌렀을 때 (진입)
        if (selectedStageIndex == stageNum)
        {
            EnterStage(stageNum);
        }
        // 2. 처음 누르거나 다른 스테이지를 눌렀을 때 (선택)
        else
        {
            SelectStage(stageNum);
        }
    }

    // 선택 취소 로직
    public void CancelSelection()
    {
        selectedStageIndex = -1;

        if (selectionOutline != null) selectionOutline.gameObject.SetActive(false);

        stageText.text = "";
        //Debug.Log("선택 취소됨");
    }


    void SelectStage(int stageNum)
    {
        //중복 호출 방지
        if (selectedStageIndex == stageNum) return;

        // 인덱스는 stageNum - 1
        currentFocusIndex = stageNum - 1;

        // ... 기존 로직 (텍스트 변경, 테두리 이동 등) ...
        selectedStageIndex = stageNum;

        bool isLocked = stageNum > unlockedStageIndex;

        //받아온 스테이지 선택된 상태
        if(stageNum != 10)
        {
            stageText.text = $"스테이지 {stageNum}";
        }
        else
        {
            stageText.text = $"스테이지 Final";
        }
        // --- 유저용 UI 테두리 처리 ---
        if (selectionOutline != null)
        {
            selectionOutline.gameObject.SetActive(true);

            Image outlineImg = selectionOutline.GetComponent<Image>();

            // 기존 이미지 색상
            Color targetColor = new Color(255, 255, 255);

            // 2. 잠긴 스테이지라면 이미지 알파값 낮춤
            if (isLocked)
            {
                targetColor.a = 0.4f; // 버튼 알파값과 동일하게 맞춰 일체감 부여
            }
            else
            {
                targetColor.a = 1.0f; // 해금된 스테이지는 선명하게
            }

            outlineImg.color = targetColor;

            // 선택된 버튼의 위치로 테두리 이동
            RectTransform btnRect = stageButtons[stageNum - 1].GetComponent<RectTransform>();
            selectionOutline.position = btnRect.position;

            // 테두리 크기를 버튼 크기에 맞춤
            selectionOutline.sizeDelta = btnRect.sizeDelta;
        }

        selectedStageIndex = stageNum;
    }

    void EnterStage(int stageNum)
    {
        //아이템 강화, 골드 초기화
        ItemManager.Instance.Init();
        bool isLocked = stageNum > unlockedStageIndex;
        //열린 스테이지 선택 시 해당 스테이지로 이동
        if (!isLocked)
        {
            StageManager.Instance.SelectedStage = stageNum;
            if(stageNum != 1) ItemManager.Instance.SetGold(stageNum * 20);
            //Debug.Log($"{stageManager.SelectedStage}번 스테이지로 진입합니다.");
            GameSceneManager.Instance.LoadSceneAsync("ItemSelectScene");
        }
        //잠긴 스테이지 선택 시 이동불가
        else
        {
            stageText.text = "진입 불가!";
        }
    }

    // 3. OnClickBack / OnClickFront 함수 수정
    public void OnClickBack()
    {
        // EventSystem 참조 대신, 우리가 저장해둔 currentFocusIndex를 사용합니다.
        if (currentFocusIndex > 0)
        {
            currentFocusIndex--;
            stageButtons[currentFocusIndex].Select();
        }
    }

    public void OnClickFront()
    {
        // stageButtons.Length 범위를 벗어나지 않도록 체크
        if (currentFocusIndex < stageButtons.Length - 1)
        {
            currentFocusIndex++;
            stageButtons[currentFocusIndex].Select();
        }
    }

}

