using UnityEngine;
using UnityEngine.SceneManagement;

public class OverManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("SceneLoad", 2f);
    }

    void SceneLoad()
    {
        SceneManager.LoadScene("StartScene", LoadSceneMode.Single);
    }
}
