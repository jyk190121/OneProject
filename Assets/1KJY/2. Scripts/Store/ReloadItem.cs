using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 1회성 ReloadItem
/// </summary>
public class ReloadItem : MonoBehaviour
{
    public Image failImg;
    public TextMeshProUGUI failTxt;
    public Button failCheckBtn;
    Button reloadBtn;
    Popup popup;        //리로드 할건지 (3회)
    int chance;
    StageManager stageManager;
    ItemManager itemManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (stageManager == null)
        {
            stageManager = FindAnyObjectByType<StageManager>();
            itemManager = FindAnyObjectByType<ItemManager>();
        }
        popup = GetComponent<Popup>();
        reloadBtn = GetComponent<Button>();
        reloadBtn.onClick.AddListener(ReloadItemChcek);
        failCheckBtn.onClick.AddListener(() => failImg.gameObject.SetActive(false));
        chance = stageManager.ReloadChance;
    }

    void ReloadItemChcek()
    {
        if (chance > 0)
        {
            int reloadPrice = 0;
            switch (chance)
            {
                case 2:
                    reloadPrice = 500;
                    break;
                case 1:
                    reloadPrice = 5000;
                    break;
            }

            //아이템 리로드할건지 팝업노출
            popup.ShowConfirm(
                $"<color=blue>비용 {reloadPrice}골드</color>\n아이템 다시 불러오시겠습니까?\n<color=red>(이번라운드 남은기회 {chance}번)</color>",
                  () => {
                          if (itemManager.gold >= reloadPrice)
                          {
                              ExecuteNewGame(reloadPrice);
                          }
                          else
                          {
                              failTxt.text = "보유금액이 부족하다";
                              failImg.gameObject.SetActive(true);
                          }
                        }
                  );
        }
        else
        {
            failTxt.text = "남은 기회가 없다";
            failImg.gameObject.SetActive( true );
        }
    }

    void ExecuteNewGame(int reloadPrice)
    {
        StageManager.Instance.ReloadChance--;
        itemManager.MinusGold(reloadPrice);
        GameSceneManager.Instance.RestartScene();
    }
}