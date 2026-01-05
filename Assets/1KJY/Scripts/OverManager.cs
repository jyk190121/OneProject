using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SceneLoad());
    }

    IEnumerator SceneLoad()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("StartScene", LoadSceneMode.Single);
    }
}
