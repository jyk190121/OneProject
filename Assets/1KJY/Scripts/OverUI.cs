using TMPro;
using UnityEngine;

public class OverUI : MonoBehaviour
{
    public TextMeshProUGUI goldText;            //죽기전에 모은 골드
    ItemManager itemManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemManager = FindAnyObjectByType<ItemManager>();

        if(itemManager != null)
        {
            goldText.text = itemManager.GetGold().ToString();
        }
    }
}
