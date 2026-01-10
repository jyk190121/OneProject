using UnityEngine;

public class ReturnStart : MonoBehaviour
{
   public void StartSceneCall()
    {
        GameSceneManager.Instance.LoadScene("StartScene");
    }
    public void StartSceneLoadingCall()
    {
        GameSceneManager.Instance.LoadSceneAsync("StartScene");
    }
}
