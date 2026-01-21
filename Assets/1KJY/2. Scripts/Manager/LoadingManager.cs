using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public Image progressBar;
    void Update()
    {
        // 매니저의 진행도를 UI에 반영만 함
        if (GameSceneManager.Instance != null)
        {
            progressBar.fillAmount = GameSceneManager.Instance.LoadingProgress;
        }
    }
}