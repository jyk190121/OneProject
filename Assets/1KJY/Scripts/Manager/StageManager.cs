using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 1. Stage 버튼 선택 시 해당 스테이지로 이동
///  - 한번 선택 시 스테이지 명 노출
///  - 두번 선택 시 스테이지 진입
/// </summary>
public class StageManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI stageText;
    public Button[] stageButtons;           // Stage 버튼을 인스펙터에서 할당
    public RectTransform selectionOutline;  // 유저에게 보여줄 테두리 이미지
    public Button backgroundCancelBtn;      // 배경 클릭 감지용 버튼 추가

    //[Header("Stage Settings")]
    //public Transform[] stagePositions; // 기즈모를 표시할 스테이지별 위치

    private int selectedStageIndex = -1; // 현재 선택된 스테이지 (-1은 선택 없음)

    public static int CurrentStage { get; private set; } // 다른 씬(BattleScene)에서 참조할 정적 변수

    //public TextMeshProUGUI stageText;
    //bool stageSelect = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 처음엔 테두리를 숨김
        if (selectionOutline != null) selectionOutline.gameObject.SetActive(false);

        //stage1Btn.onClick.AddListener(() => SelectStage(1));
        // 반복문을 통해 10개 버튼에 이벤트 등록
        for (int i = 0; i < stageButtons.Length; i++)
        {
            int index = i + 1; // 스테이지 번호 (1~10)
            stageButtons[i].onClick.AddListener(() => OnStageButtonClick(index));
        }
        // 배경 버튼 클릭 시 취소 함수 실행
        if (backgroundCancelBtn != null)
        {
            backgroundCancelBtn.onClick.AddListener(CancelSelection);

            print("배경 클릭!");
        }
    }

  
    void OnStageButtonClick(int stageNum)
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
        selectedStageIndex = stageNum;
        //받아온 스테이지 선택된 상태
        stageText.text = $"스테이지 {stageNum}";

        // --- 유저용 UI 테두리 처리 ---
        if (selectionOutline != null)
        {
            selectionOutline.gameObject.SetActive(true);

            // 선택된 버튼의 위치로 테두리 이동
            RectTransform btnRect = stageButtons[stageNum - 1].GetComponent<RectTransform>();
            selectionOutline.position = btnRect.position;

            // 테두리 크기를 버튼 크기에 맞춤
            selectionOutline.sizeDelta = btnRect.sizeDelta;
        }
    }

    void EnterStage(int stageNum)
    {
        CurrentStage = stageNum;
        Debug.Log($"{stageNum}번 스테이지로 진입합니다.");

        GameSceneManager.Scene.LoadScene("BattleScene");
    }
}

