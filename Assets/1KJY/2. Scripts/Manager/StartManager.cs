using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//[RequireComponent(typeof(TextMeshPro))]
//[RequireComponent(typeof(Image))]
public class StartManager : MonoBehaviour
{
    public Button gameStartBtn;                         // 게임 시작 버튼
    public Button endBtn;                               // 종료 팝업 호출 버튼
    public Button endY;                                 // 종료 확인 버튼 (예)
    public Button endN;                                 // 종료 취소 버튼 (아니오)
    public Button settingBtn;                           // 설정 버튼
    public Image endImg;                                // 종료 확인 창 이미지
    public Image volSetImg;                             // 볼륨 설정 창 이미지
    public Button setCloseBtn;                          // 설정 창 닫기 버튼

    public Slider volumeSliderBGM;
    public Slider volumeSliderSFX;

    public Button infoBtn;
    public Image infoImg;
    public Button infoCloseBtn;
    public Button itemListBtn;                          // 진행정보 진입 버튼


    public Texture2D cursorTexture;                     // 변경할 커서 이미지
    public Vector2 hotSpot = Vector2.zero;              // 클릭 위치 (좌상단이 0,0)

    public RectTransform selectionOutline;              // 유저에게 보여줄 테두리 이미지
    public Button backgroundCancelBtn;                  // 배경 클릭 감지용 버튼 추가
    //private int selectedBtnIndex = -1;                // 현재 선택된 스테이지 (-1은 선택 없음)
    GameObject currentObj;

    public Button newStartBtn;                          // 새로 시작 버튼 
    Popup newStartpopup;                                // 새로운 게임 시작 시 호출할 팝업

    public Button arenaBtn;                             // 아레나모드 버튼 (10스테이지 클리어 후 활성화)

    public Button modeBtn;                              // 불법모드 구매상점

    AudioManager audioManager;                          // 오디오매니저
    float sfxValue = 0f;                                // 버튼음

    GameSceneManager gameSceneManager;

    [SerializeField] Button hidenCheatBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = AudioManager.audioManager;
        sfxValue = audioManager.sfxVolume;

        gameSceneManager = GameSceneManager.Instance;

        //아레나모드 진행 중 스타트씬으로 돌아와졌을 때 아레나 점수 및 라운드 초기화(게임튕김 등)
        ScoreManager.Instance.Init();

        //아이템 강화 및 골드 초기화
        ItemManager.Instance.Init();

        //리로드 초기화
        StageManager.Instance.Init();

        //게임 시작 시 포커스될 버튼
        newStartBtn.Select();

        newStartpopup = FindAnyObjectByType<Popup>();

        // 처음엔 테두리를 숨김
        if (selectionOutline != null) selectionOutline.gameObject.SetActive(false);

        // 커서 변경 실행
        // CursorMode.Auto는 시스템이 자동으로 하드웨어/소프트웨어 커서를 결정하게 합니다.
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.ForceSoftware);

        gameStartBtn.onClick.AddListener(GameStart);
        newStartBtn.onClick.AddListener(NewGameStart);      // 새로하기
        modeBtn.onClick.AddListener(ModeStore);             // 모드상점 진입
        endBtn.onClick.AddListener(GameEndYorN);
        endY.onClick.AddListener(EndGame);
        endN.onClick.AddListener(EnterGame);

        settingBtn.onClick.AddListener(AudioSet);
        setCloseBtn.onClick.AddListener(EnterGame);
        infoBtn.onClick.AddListener(ShowInfo);
        infoCloseBtn.onClick.AddListener(EnterGame);
        itemListBtn.onClick.AddListener(EnterItemList);

        hidenCheatBtn.onClick.AddListener(CheatExcute);

        // [추가] 이미 치트를 썼거나 특정 스테이지 이상이라면 버튼을 비활성화 상태로 시작
        if (StageManager.Instance.UnlockedStage >= 11)
        {
            hidenCheatBtn.interactable = false;
            // 또는 아예 안 보이게 하려면
            // hidenCheatBtn.gameObject.SetActive(false);
        }

        endImg.gameObject.SetActive(false);
        volSetImg.gameObject.SetActive(false);
        infoImg.gameObject.SetActive(false);

        //AudioManager.audioManager.PlayBGM("Intro");
        //SetBGMVol(volumeSlider.value);
        // 배경 버튼 클릭 시 취소 함수 실행
        if (backgroundCancelBtn != null)
        {
            backgroundCancelBtn.onClick.AddListener(CancelSelection);
        }

        if (StageManager.Instance.UnlockedStage <= 10)
        {
            Image arenaBtnImg = arenaBtn.GetComponent<Image>();
            arenaBtnImg.color = new Color(0.5f, 0.5f, 0.5f, 0.5f); // 반투명하게
        }
        else
        {
            Image arenaBtnImg = arenaBtn.GetComponent<Image>();
            arenaBtnImg.color = new Color(1f, 1f, 1f, 1f);
            arenaBtn.onClick.AddListener(ArenaStart);
        }

        if(audioManager.GetCurrentBGM() == "")
        {
            audioManager.StopBGM();
            audioManager.PlayBGM("Intro");
        }
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

        // 1. 스페이스 키 입력 감지(현재 버튼 실행)
        if (key.spaceKey.wasPressedThisFrame == true)
        {
            // 2. 현재 이벤트 시스템에서 선택된(포커스된) 오브젝트 가져오기
            GameObject currentSelected = currentObj;

            if (currentSelected != null)
            {
                // 3. 해당 오브젝트에 버튼 컴포넌트가 있는지 확인
                Button btn = currentSelected.GetComponent<Button>();
                if (btn != null)
                {
                    // 4. 버튼의 onClick 이벤트 실행
                    btn.onClick.Invoke();
                    //Debug.Log($"{currentSelected.name} 실행");
                }
            }
        }

        //선택된 버튼 확인
        UpdateSelectionOutline();

        HandleLoopNavigation(key);
    }

    void GameStart()
    {
        audioManager.PlaySFX("Button", sfxValue);
        gameSceneManager.LoadSceneAsync("StageScene");
    }

    //새로하기
    void NewGameStart()
    {
        audioManager.PlaySFX("Button", sfxValue);

        newStartpopup.yesBtn.Select();
        //새로 시작하시겠습니까?? (이전 진행정보가 모두 사라집니다) Yes/No
        // 팝업 매니저에게 메시지와 '실행할 함수'를 전달
        newStartpopup.ShowConfirm(
                    $"새로 시작하시겠습니까??\n<color=red>이전 진행정보가 모두 사라집니다</color>",
                    () => ExecuteNewGame() // 'Yes'를 누르면 실행될 람다식(Action)
                    );

    }

    void ExecuteNewGame()
    {
        //아이템, 스테이지 정보 초기화
        ItemManager.Instance.ResetItem();
        StageManager.Instance.ResetStage();
        PlayerManager.Instance.ResetData();

        //gameSceneManager.LoadSceneAsync("StageScene");
        gameSceneManager.RestartScene();

    }


    void GameEndYorN()
    {

        volSetImg.gameObject.SetActive(false);
        infoImg.gameObject.SetActive(false);

        if (!endImg.gameObject.activeSelf)
        {
            endImg.gameObject.SetActive(true);
            endN.Select();
            return;
        }
        else
        {
            //endImg.gameObject.SetActive(false);
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

    void EnterGame()
    {
        endImg.gameObject.SetActive(false);
        volSetImg.gameObject.SetActive(false);
        infoImg.gameObject.SetActive(false);
        //gameStartBtn.Select();
        gameSceneManager.RestartScene();
    }

    void EnterItemList()
    {
        audioManager.PlaySFX("Button", sfxValue);

        gameSceneManager.LoadSceneAsync("ItemListScene");
    }

    public void AudioSet()
    {
        volumeSliderBGM.value = audioManager.bgmVolume;
        volumeSliderSFX.value = sfxValue;

        if (endImg.gameObject.activeSelf || infoImg.gameObject.activeSelf) return;

        if (!volSetImg.gameObject.activeSelf)
        {

            volSetImg.gameObject.SetActive(true);
            setCloseBtn.Select();
            return;
        }
        else
        {
            volSetImg.gameObject.SetActive(false);
            EnterGame();
        }
    }
    void ShowInfo()
    {
        if (endImg.gameObject.activeSelf || volSetImg.gameObject.activeSelf) return;

        if (!infoImg.gameObject.activeSelf)
        {
            infoImg.gameObject.SetActive(true);
            infoCloseBtn.Select();
            return;
        }
        else
        {
            infoImg.gameObject.SetActive(false);
            EnterGame();
        }
    }

    //public void SetBGMVol(float value)
    //{
    //    AudioManager.audioManager.SetBGMOnlyVol(volumeSlider.value);
    //}

    public void OnSliderChangedBGM()
    {
        audioManager.SetBGMOnlyVol(volumeSliderBGM.value);
        audioManager.bgmVolume = volumeSliderBGM.value;
    }

    public void OnSliderChangedSFX()
    {
        audioManager.SetSFXOnlyVol(volumeSliderSFX.value);
        audioManager.sfxVolume = volumeSliderSFX.value;
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
            if (selectionOutline != null && currentObj != backgroundCancelBtn )
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
        // isPressed 상태일때는 한번 선택에도 빠르게 이동
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
            else if (currentObj == null)
                EventSystem.current.SetSelectedGameObject(gameStartBtn.gameObject);
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
            else if (currentObj == null)
                EventSystem.current.SetSelectedGameObject(gameStartBtn.gameObject);
        }
       
    }

    void CancelSelection()
    {
        if (selectionOutline != null) selectionOutline.gameObject.SetActive(false);
    }

    void ArenaStart()
    {
        audioManager.PlaySFX("Button", sfxValue);

        gameSceneManager.LoadSceneAsync("ArenaItemSelectScene");
    }

    void ModeStore()
    {
        audioManager.PlaySFX("Button", sfxValue);
        gameSceneManager.LoadSceneAsync("ModeStoreScene");
    }

    void CheatExcute()
    {
        hidenCheatBtn.interactable = false;

        PlayerManager.Instance.GetChip(500);
        ItemManager.Instance.PlusGold(10000);
        StageManager.Instance.UnlockNextStage(11);

        //hidenCheatBtn.gameObject.SetActive(false);
        hidenCheatBtn.onClick.RemoveListener(CheatExcute);

        gameSceneManager.RestartScene();
    }

    private void OnDisable()
    {
        gameStartBtn.onClick.RemoveListener(GameStart);
        newStartBtn.onClick.RemoveListener(NewGameStart);      
        modeBtn.onClick.RemoveListener(ModeStore);
        endBtn.onClick.RemoveListener(GameEndYorN);
        endY.onClick.RemoveListener(EndGame);
        endN.onClick.RemoveListener(EnterGame);

        settingBtn.onClick.RemoveListener(AudioSet);
        setCloseBtn.onClick.RemoveListener(EnterGame);
        infoBtn.onClick.RemoveListener(ShowInfo);
        infoCloseBtn.onClick.RemoveListener(EnterGame);
        itemListBtn.onClick.RemoveListener(EnterItemList);

    }
}