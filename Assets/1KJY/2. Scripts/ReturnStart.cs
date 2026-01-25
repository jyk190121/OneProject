using UnityEngine;
using UnityEngine.InputSystem;

public class ReturnStart : MonoBehaviour
{
    void Update()
    {
        // Keyboard.current가 null인지 체크하는 것이 안전합니다 (키보드가 연결 안 된 경우 대비)
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame &&
            GameSceneManager.Instance.SceneName() != "BattleScene" && 
            GameSceneManager.Instance.SceneName() != "UpgradeStoreScene" &&
            GameSceneManager.Instance.SceneName() != "StartScene")
        {
            StartSceneCall();
        }
    }

    public void StartSceneCall()
    {
        GameSceneManager.Instance.LoadScene("StartScene");
    }
    public void StartSceneLoadingCall()
    {
        AudioManager.audioManager.StopBGM();
        GameSceneManager.Instance.LoadSceneAsync("StartScene");
    }

    public void StageSceneCall()
    {
        GameSceneManager.Instance.LoadScene("StageScene");
    }
}
