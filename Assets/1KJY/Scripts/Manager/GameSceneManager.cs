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