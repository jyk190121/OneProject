using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 1회성 ReloadItem
/// </summary>
public class ReloadItem : MonoBehaviour
{
    public Image failImg;
    public Button failCheckBtn;
    Button reloadBtn;
    Popup popup;        //리로드 할건지 (1회)
    int chance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        popup = GetComponent<Popup>();
        reloadBtn = GetComponent<Button>();
        reloadBtn.onClick.AddListener(ReloadItemChcek);
        failCheckBtn.onClick.AddListener(() => failImg.gameObject.SetActive(false));
        chance = StageManager.Instance.ReloadChance;
    }

    void ReloadItemChcek()
    {
        if (chance > 0)
        {
            //아이템 리로드할건지 팝업노출
            popup.ShowConfirm(
                $"아이템 다시 불러오시겠습니까?\n<color=red>(기회 1번)</color>",
                  () => ExecuteNewGame() // 'Yes'를 누르면 실행될 람다식(Action)
                  );

        }
        else
        {
            //아이템 리로드 기회없음 이미지 노출
            failImg.gameObject.SetActive( true );
        }
    }

    void ExecuteNewGame()
    {
        StageManager.Instance.ReloadChance--;
        GameSceneManager.Instance.RestartScene();
    }
}