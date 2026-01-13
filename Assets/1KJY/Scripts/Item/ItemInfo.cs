using UnityEngine;
using TMPro; // TextMeshPro를 사용한다고 가정합니다.

public class ItemInfo : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI descriptionText;

    void Start()
    {
        HideItemInfo();
    }

    public void ShowItemInfo(string name, string description)
    {
        itemNameText.text = name;
        descriptionText.color = Color.cyan;
        descriptionText.text = description;
    }
  
    public void HideItemInfo()
    {
        itemNameText.text = "";
        descriptionText.text = "";
    }
}