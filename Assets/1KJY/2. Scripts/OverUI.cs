using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverUI : MonoBehaviour
{
    public GameObject itemPrefabParent;         //프리팹 만들 위치
    public GameObject itemPrefab;               //아이템 프리팹
    List<GameObject> newItemPrefabs;            //죽기전까지 새로 산 아이템리스트
   
    public TextMeshProUGUI goldText;            //죽기전까지 모은 골드
    ItemManager itemManager;

    List<Item> newItemList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemManager = FindAnyObjectByType<ItemManager>();

        if (itemManager != null)
        {
            goldText.text = itemManager.GetGold().ToString();
            newItemList = itemManager.GetNewItems();
        }

        foreach (Item item in newItemList)
        {
            GameObject itemObj = Instantiate(itemPrefab, itemPrefabParent.transform);
            newItemPrefabs.Add(itemObj);

            Image itemImg = itemObj.GetComponent<Image>();
            itemImg.sprite = item.IMAGE;
        }
        for (int i = 0; i < newItemPrefabs.Count; i++)
        {
            UpdateSlot(newItemList[i], i);
        }
    }

    void UpdateSlot(Item item, int index)
    {
        ItemSlot slot = newItemPrefabs[index].gameObject.GetComponent<ItemSlot>();
        if (slot == null) slot = newItemPrefabs[index].gameObject.AddComponent<ItemSlot>();

        slot.Setup(item);
    }
}
