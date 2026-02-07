using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverUI : MonoBehaviour
{
    public GameObject overEffect;               //칩도 주면서~ 이팩트
    public Image chipBg;
    public TextMeshProUGUI chipCount;

    public GameObject itemPrefabParent;         //프리팹 만들 위치
    public GameObject itemPrefab;               //아이템 프리팹
    List<GameObject> newItemPrefabs;            //죽기전까지 새로 산 아이템리스트
   
    public TextMeshProUGUI goldText;            //죽기전까지 모은 골드
    public TextMeshProUGUI scoreText;           //아레나모드 점수
    ItemManager itemManager;

    List<Item> newItemList;

    ScoreManager scoreManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemManager = FindAnyObjectByType<ItemManager>();
        scoreManager = FindAnyObjectByType<ScoreManager>();
        
        if (itemManager != null)
        {
            goldText.text = itemManager.GetGold().ToString();
            newItemList = itemManager.GetNewItems();
        }

        if(scoreManager != null)
        {
            scoreText.text = scoreManager.score.ToString();
        }

        // 1. 리스트가 null인 경우를 대비해 여기서 한 번 더 확인하거나 상단에서 초기화 필수
        if (newItemPrefabs == null) newItemPrefabs = new List<GameObject>();
        if (newItemList == null) return;

        // 2. 생성과 동시에 리스트에 추가
        foreach (Item item in newItemList)
        {
            GameObject itemObj = Instantiate(itemPrefab, itemPrefabParent.transform);
            newItemPrefabs.Add(itemObj);

            Image itemImg = itemObj.GetComponent<Image>();
            if (itemImg != null) itemImg.sprite = item.IMAGE;
        }

        // 3. 이제 리스트에 데이터가 있으므로 UpdateSlot 호출 가능
        for (int i = 0; i < newItemPrefabs.Count; i++)
        {
            UpdateSlot(newItemList[i], i);
        }

        chipBg.gameObject.SetActive(false);

        StartCoroutine(GetChipAnimation());
    }

    void UpdateSlot(Item item, int index)
    {
        ItemSlot slot = newItemPrefabs[index].gameObject.GetComponent<ItemSlot>();
        if (slot == null) slot = newItemPrefabs[index].gameObject.AddComponent<ItemSlot>();

        slot.Setup(item);
    }

    IEnumerator GetChipAnimation()
    {
        GameObject effect = Instantiate(overEffect, chipBg.transform.position , Quaternion.identity);

        int chip = ((StageManager.Instance.SelectedStage - 1) * 5) + StageManager.Instance.Round;

        PlayerManager.Instance.GetChip(chip);
        chipCount.text = chip.ToString();

        yield return new WaitForSeconds(2f);

        Destroy(effect, 1f);
        chipBg.gameObject.SetActive(true);
    }
}
