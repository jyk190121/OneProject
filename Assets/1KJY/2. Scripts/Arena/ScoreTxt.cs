using TMPro;
using UnityEngine;

public class ScoreTxt : MonoBehaviour
{
    public TextMeshProUGUI scoreTxt;

    private void Start()
    {
        scoreTxt.text = ScoreManager.Instance.score.ToString();
    }
}
