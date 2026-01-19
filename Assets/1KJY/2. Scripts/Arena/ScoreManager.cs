using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; } // 싱글톤 선언

    //아레나 라운드
    public int round;
    public int score;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    public void Init()
    {
        round = 1;
        score = 0;
    }


    // 적을 때리거나 죽였을 때 점수를 추가하는 함수
    public void AddScore(int amount)
    {
        score += amount;
        // UI 업데이트 로직 추가 가능 (ScoreTxt.text = totalScore.ToString();)
    }
}
