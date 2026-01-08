using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//[RequireComponent(typeof(TextMeshPro))]
//[RequireComponent(typeof(Image))]
public class StartManager : MonoBehaviour
{
    //public static StartManager start;

    public Button gameStartBtn;                         // 게임 시작 버튼
    public Button endBtn;                               // 종료 팝업 호출 버튼
    public Button endY;                                 // 종료 확인 버튼 (예)
    public Button endN;                                 // 종료 취소 버튼 (아니오)
    public Button settingBtn;                           // 설정 버튼
    //public GameManager gameManagerPrefab;
    public Image EndImg;                                // 종료 확인 창 이미지
    public Image volSetImg;                             // 볼륨 설정 창 이미지
    public Button setCloseBtn;                          // 설정 창 닫기 버튼
    public Slider volumeSlider;
    public bool restart;
    public Button infoBtn;

    public Texture2D cursorTexture;                     // 변경할 커서 이미지
    public Vector2 hotSpot = Vector2.zero;              // 클릭 위치 (좌상단이 0,0)

    public RectTransform selectionOutline;              // 유저에게 보여줄 테두리 이미지
    public Button backgroundCancelBtn;                  // 배경 클릭 감지용 버튼 추가
    //private int selectedBtnIndex = -1;                // 현재 선택된 스테이지 (-1은 선택 없음)
    GameObject currentObj;

    private void Awake()
    {
        //if (start == null)
        //{
        //    start = this;
        //    DontDestroyOnLoad(gameObject);
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //게임 시작 시 포커스될 버튼
        gameStartBtn.Select();

        // 처음엔 테두리를 숨김
        if (selectionOutline != null) selectionOutline.gameObject.SetActive(false);

        // 커서 변경 실행
        // CursorMode.Auto는 시스템이 자동으로 하드웨어/소프트웨어 커서를 결정하게 합니다.
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.ForceSoftware);

        gameStartBtn.onClick.AddListener(GameStart);
        endBtn.onClick.AddListener(GameEndYorN);
        endY.onClick.AddListener(EndGame);
        endN.onClick.AddListener(EnterGame);

        settingBtn.onClick.AddListener(AudioSet);
        setCloseBtn.onClick.AddListener(EnterGame);
        infoBtn.onClick.AddListener(ShowInfo);

        EndImg.gameObject.SetActive(false);
        volSetImg.gameObject.SetActive(false);

        AudioManager.audioManager.PlayBGM("Intro");
        //SetBGMVol(volumeSlider.value);
        // 현재 이벤트 시스템에서 포커스된 오브젝트를 가져옵니다.
    }

    private void Update()
    {
        //// 1. 키를 누르는 순간 (GetKeyDown과 동일)
        //if (Keyboard.current.spaceKey.wasPressedThisFrame)
        //{
        //    Debug.Log("스페이스바를 눌렀습니다!");
        //}

        //// 2. 키를 누르고 있는 상태 (GetKey와 동일)
        //if (Keyboard.current.spaceKey.isPressed)
        //{
        //    // 지속적인 로직 (예: 기 모으기)
        //}

        //// 3. 키를 떼는 순간 (GetKeyUp과 동일)
        //if (Keyboard.current.spaceKey.wasReleasedThisFrame)
        //{
        //    Debug.Log("스페이스바를 뗐습니다!");
        //}

        if (AudioManager.audioManager.IsPlaying("Battle"))
        {
            AudioManager.audioManager.StopBGM();
            AudioManager.audioManager.PlayBGM("Intro");
        }
        if (Keyboard.current.escapeKey.wasPressedThisFrame == true)
        {
            GameEndYorN();
            return;
        }

        //현재 선택된 오브젝트를 가져옴
        currentObj = EventSystem.current.currentSelectedGameObject;

        Keyboard key = Keyboard.current;
        if (key == null) return;

        //Enter는 EventSystem이 자동으로 Invoke해주기 때문에 처리가 필요없음
        
        //// 1. 엔터 키 입력 감지(현재 버튼 실행)
        //if (Keyboard.current.enterKey.wasPressedThisFrame == true)
        //{
        //    // 2. 현재 이벤트 시스템에서 선택된(포커스된) 오브젝트 가져오기
        //    GameObject currentSelected = currentObj;

        //    if (currentSelected != null)
        //    {
        //        // 3. 해당 오브젝트에 버튼 컴포넌트가 있는지 확인
        //        Button btn = currentSelected.GetComponent<Button>();
        //        if (btn != null)
        //        {
        //            // 4. 버튼의 onClick 이벤트 실행
        //            btn.onClick.Invoke();
        //            Debug.Log($"{currentSelected.name} 실행");
        //        }
        //    }
        //}
        
        //선택된 버튼 확인
        UpdateSelectionOutline();

        // WASD와 화살표 입력을 하나로 합침
        //Vector2 moveInput = Vector2.zero;

        //if (key.wKey.wasPressedThisFrame || key.upArrowKey.wasPressedThisFrame) moveInput.y = 1;
        //else if (key.sKey.wasPressedThisFrame || key.downArrowKey.wasPressedThisFrame) moveInput.y = -1;

        //// Update 함수 내의 입력 부분 수정
        //if (key.aKey.wasPressedThisFrame == true || key.leftArrowKey.wasPressedThisFrame == true)
        //{
        //    // 왼쪽 끝(StartBtn)에서 왼쪽 입력 시 -> 오른쪽 끝(EndBtn)으로 이동
        //    if (current == gameStartBtn.gameObject)
        //    {
        //        EventSystem.current.SetSelectedGameObject(endBtn.gameObject);
        //        return; // 직접 이동시켰으므로 이후 로직 중단
        //    }
        //}
        //else if (key.dKey.wasPressedThisFrame == true || key.rightArrowKey.wasPressedThisFrame == true)
        //{
        //    // 오른쪽 끝(EndBtn)에서 오른쪽 입력 시 -> 왼쪽 끝(StartBtn)으로 이동
        //    if (current == endBtn.gameObject)
        //    {
        //        EventSystem.current.SetSelectedGameObject(gameStartBtn.gameObject);
        //        return; // 직접 이동시켰으므로 이후 로직 중단
        //    }
        //}
        HandleLoopNavigation(key);
    }

    void GameStart()
    {
        GameSceneManager.Instance.LoadSceneAsync("StageScene");
    }

    void GameEndYorN()
    {
        volSetImg.gameObject.SetActive(false);

        if (!EndImg.gameObject.activeSelf)
        {
            EndImg.gameObject.SetActive(true);
            endN.Select();
            return;
        }
        else
        {
            //EndImg.gameObject.SetActive(false);
            EnterGame();
        }
    }

    void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void EnterGame()
    {
        EndImg.gameObject.SetActive(false);
        volSetImg.gameObject.SetActive(false);
        //gameStartBtn.Select();
        GameSceneManager.Instance.RestartScene();
    }

    public void AudioSet()
    {
        EndImg.gameObject.SetActive(false);

        if (!volSetImg.gameObject.activeSelf)
        {
            volSetImg.gameObject.SetActive(true);
            setCloseBtn.Select();
            return;
        }
        else
        {
            volSetImg.gameObject.SetActive(false);
        }
    }

    //public void SetBGMVol(float value)
    //{
    //    AudioManager.audioManager.SetBGMOnlyVol(volumeSlider.value);
    //}

    public void OnSliderChanged()
    {
        AudioManager.audioManager.SetBGMOnlyVol(volumeSlider.value);
        AudioManager.audioManager.bgmVolume = volumeSlider.value;
    }

    //void SelectBtn(Keyboard key)
    //{
    //    if (key == null || currentObj == null) return;

    //    if (currentObj != null)
    //    {
    //        // 선택된 오브젝트의 이름 출력
    //        Debug.Log($"현재 선택된 UI: {currentObj.name}");

    //        // 만약 버튼 컴포넌트가 있는지 확인하고 싶다면
    //        if (currentObj.TryGetComponent<UnityEngine.UI.Button>(out var button))
    //        {
    //            // 여기서 버튼에 대한 추가 처리가 가능합니다.
    //            // --- 유저용 UI 테두리 처리 ---
    //            if (selectionOutline != null)
    //            {
    //                selectionOutline.gameObject.SetActive(true);

    //                // 선택된 버튼의 위치로 테두리 이동
    //                RectTransform btnRect = button.GetComponent<RectTransform>();
    //                selectionOutline.position = btnRect.position;

    //                // 테두리 크기를 버튼 크기에 맞춤
    //                selectionOutline.sizeDelta = btnRect.sizeDelta;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("현재 선택된 UI가 없습니다.");
    //    }

    //}

    void UpdateSelectionOutline()
    {
        if (currentObj != null && currentObj.TryGetComponent<RectTransform>(out RectTransform targetRect))
        {
            if (selectionOutline != null)
            {
                selectionOutline.gameObject.SetActive(true);
                selectionOutline.position = targetRect.position;
                selectionOutline.sizeDelta = targetRect.sizeDelta;
            }
        }
    }

    void HandleLoopNavigation(Keyboard key)
    {
        //// A 또는 왼쪽 화살표: 시작 버튼에서 왼쪽 누르면 종료 버튼으로
        //if (key.aKey.wasPressedThisFrame == true || key.leftArrowKey.wasPressedThisFrame == true)
        //{
        //    if (currentObj == gameStartBtn.gameObject)
        //    {
        //        EventSystem.current.SetSelectedGameObject(endBtn.gameObject);
        //        return;
        //    }
        //}
        //// D 또는 오른쪽 화살표: 종료 버튼에서 오른쪽 누르면 시작 버튼으로
        //else if (key.dKey.wasPressedThisFrame == true || key.rightArrowKey.wasPressedThisFrame == true)
        //{
        //    if (currentObj == endBtn.gameObject)
        //    {
        //        EventSystem.current.SetSelectedGameObject(gameStartBtn.gameObject);
        //        return;
        //    }
        //}

        // 이동 로직 (Navigation이 None일 때만 정상 작동)
        if (key.aKey.wasPressedThisFrame || key.leftArrowKey.wasPressedThisFrame)
        {
            if (currentObj == gameStartBtn.gameObject)
                EventSystem.current.SetSelectedGameObject(endBtn.gameObject);
            else if (currentObj == endBtn.gameObject)
                EventSystem.current.SetSelectedGameObject(infoBtn.gameObject);
            else if(currentObj == settingBtn.gameObject)
                EventSystem.current.SetSelectedGameObject(gameStartBtn.gameObject);
            else if (currentObj == infoBtn.gameObject)
                EventSystem.current.SetSelectedGameObject(settingBtn.gameObject);
        }
        else if (key.dKey.wasPressedThisFrame || key.rightArrowKey.wasPressedThisFrame)
        {
            if (currentObj == endBtn.gameObject)
                EventSystem.current.SetSelectedGameObject(gameStartBtn.gameObject);
            else if (currentObj == gameStartBtn.gameObject)
                EventSystem.current.SetSelectedGameObject(settingBtn.gameObject);
            else if (currentObj == infoBtn.gameObject)
                EventSystem.current.SetSelectedGameObject(endBtn.gameObject);
            else if (currentObj == settingBtn.gameObject)
                EventSystem.current.SetSelectedGameObject(infoBtn.gameObject);
        }
       
    }

    void ShowInfo()
    {
        print("아직 안만듬 인포창");
    }
}