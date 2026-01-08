using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(TextMeshPro))]
//[RequireComponent(typeof(Image))]
public class StartManager : MonoBehaviour
{
    public Button gameStartBtn;         // 게임 시작 버튼
    public Button endBtn;               // 종료 팝업 호출 버튼
    public Button endY;                 // 종료 확인 버튼 (예)
    public Button endN;                 // 종료 취소 버튼 (아니오)
    public Button settingBtn;           // 설정 버튼
    public static StartManager start;
    //public GameManager gameManagerPrefab;
    public Image EndImg;                // 종료 확인 창 이미지
    public Image volSetImg;             // 볼륨 설정 창 이미지
    public Button setCloseBtn;          // 설정 창 닫기 버튼
    public Slider volumeSlider;
    public bool restart;

    public Texture2D cursorTexture;        // 변경할 커서 이미지
    public Vector2 hotSpot = Vector2.zero; // 클릭 위치 (좌상단이 0,0)
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
        // 커서 변경 실행
        // CursorMode.Auto는 시스템이 자동으로 하드웨어/소프트웨어 커서를 결정하게 합니다.
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.ForceSoftware);

        if (gameStartBtn != null)
        {
            gameStartBtn.onClick.AddListener(GameStart);
            endBtn.onClick.AddListener(GameEndYorN);
            endY.onClick.AddListener(EndGame);
            endN.onClick.AddListener(EnterGame);

            settingBtn.onClick.AddListener(AudioImg);
            setCloseBtn.onClick.AddListener(EnterGame);
        }

        EndImg.gameObject.SetActive(false);
        volSetImg.gameObject.SetActive(false);

        AudioManager.audioManager.PlayBGM("Intro");
        //SetBGMVol(volumeSlider.value);
    }

    private void Update()
    {
        if (AudioManager.audioManager.IsPlaying("Battle"))
        {
            AudioManager.audioManager.StopBGM();
            AudioManager.audioManager.PlayBGM("Intro");
        }

        if (Input.GetKeyDown(KeyCode.Return) && gameStartBtn != null ||
            Input.GetKeyDown(KeyCode.Space) && gameStartBtn != null)
        {
            GameStart();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameEndYorN();
        }
    }

    public void GameStart()
    {
        //currentScene = SceneManager.GetActiveScene().buildIndex;
        //SceneManager.LoadScene(currentScene + 1);
        //SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
        //SceneManager.sceneLoaded += OnSceneLoaded;
        //SceneManager.LoadScene("BattleScene", LoadSceneMode.Single);
        GameSceneManager.Instance.LoadSceneAsync("StageScene");
    }

    //void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    if (scene.name == "BattleScene")
    //    {
    //        if (GameManager.instance == null)
    //        {
    //            GameManager gameManager = FindFirstObjectByType<GameManager>();
    //            if (gameManager != null)
    //            {
    //                GameManager.instance = gameManager;
    //                DontDestroyOnLoad(gameManager.gameObject);
    //                SceneManager.sceneLoaded -= OnSceneLoaded; // 다시 호출되지 않도록 구독 해제
    //            }
    //            else
    //            {
    //                Debug.LogError("GameManager가 존재하지 않습니다!");
    //            }
    //        }
    //    }
    //}

    void GameEndYorN()
    {
        volSetImg.gameObject.SetActive(false);

        if (!EndImg.gameObject.activeSelf)
        {
            EndImg.gameObject.SetActive(true);
        }
        else
        {
            EndImg.gameObject.SetActive(false);
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
        EndImg.gameObject.SetActive(false);
        volSetImg.gameObject.SetActive(false);
    }

    void AudioImg()
    {
        EndImg.gameObject.SetActive(false);

        if (!volSetImg.gameObject.activeSelf)
        {
            volSetImg.gameObject.SetActive(true);
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
}