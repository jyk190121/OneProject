using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Scene { get; private set; }
    private void Awake()
    {
        // 싱글톤 핵심 로직 수정
        if (Scene == null)
        {
            Scene = this;
            DontDestroyOnLoad(gameObject); // 이 객체를 씬이 바뀌어도 보존
        }
        else
        {
            Destroy(gameObject); // 중복 생성된 객체는 제거
        }
    }

    void OnEnable()
    {
        //씬 로드 완료 이벤트 구축
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        //이벤트 구축 해제(메모리 누수 및 에러 방지)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void InitializeBattle()
    {
        //StageManager에서 넘겨준 스테이지 번호 확인
        int stage = StageManager.CurrentStage;

        //MonsterSpawnerManager.Instance.Spawn(stage)
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "BattleScene")
        {
            InitializeBattle();
        }
    }

    // 기본 로드 방식 (동기)
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // 비동기 로드 방식 (추천: 로딩 화면 구현 시 유리)
    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        while (!op.isDone)
        {
            // 여기서 로딩 바(UI)를 업데이트할 수 있습니다.
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            Debug.Log($"Loading progress: {progress * 100}%");

            yield return null;
        }
    }

    // 현재 씬 재시작
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}