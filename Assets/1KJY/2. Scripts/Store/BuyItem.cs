using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 랜덤하게 아이템 판매 노출
/// - 안뜰 확률도 있어야댐
/// </summary>

public class BuyItem : MonoBehaviour
{
    StoreManager storeManager;

    ItemManager itemManager;
    List<Item> allItems;
    List<Item> items;                           //보유 아이템
    List<Item> storeItems = new List<Item>();   //상점에 팔 아이템
    //List<int> usedIndices = new List<int>(); // 이번 상점에서 이미 뽑힌 아이템 인덱스 추적
    //Player player;

    public Button[] buyItemBtns;                //구매 버튼
    public Image[] buyItemImgs;                 //구매 아이템 이미지
    public Image[] enhanceImgs;                 //가지고 있는 아이템 강화 이미지
    public TextMeshProUGUI[] buyItemPrices;     //구매 가격 텍스트

    public Image successImg;                    //구매 성공 이미지
    public TextMeshProUGUI successTxt;          //구매 성공 텍스트
    public Image failImg;                       //구매 실패 이미지
    public TextMeshProUGUI failTxt;             //구매 실패 텍스트
    public Button[] buyBtn;                     //해당 UI닫기

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        storeManager = FindAnyObjectByType<StoreManager>();

        itemManager = FindAnyObjectByType<ItemManager>();
        if (itemManager == null) return;

        items = itemManager.GetSelectItems();
        allItems = itemManager.allItemDatas;

        storeItems.Clear();
        List<int> usedIndices = new List<int>();

        for (int i = 0; i < buyItemBtns.Length; i++)
        {
            // 클릭 리스너 중복 방지를 위해 이전 리스너 제거
            buyItemBtns[i].onClick.RemoveAllListeners();

            enhanceImgs[i].gameObject.SetActive(false);
            buyItemBtns[i].interactable = false;

            int r = -1;
            int maxAttempts = 10;
            while (maxAttempts > 0)
            {
                r = Random.Range(-1, allItems.Count);
                if (r != -1 && usedIndices.Contains(r))
                {
                    maxAttempts--;
                    continue;
                }
                break;
            }

            if (r == -1)
            {
                storeItems.Add(null); // 인덱스 유지를 위해 null 추가
                buyItemImgs[i].gameObject.SetActive(false);
                buyItemPrices[i].text = "";
            }
            else
            {
                usedIndices.Add(r);
                Item selectedItem = allItems[r];
                storeItems.Add(selectedItem); // 리스트에 아이템 추가 (딱 한 번만 실행)

                buyItemImgs[i].gameObject.SetActive(true);
                buyItemImgs[i].sprite = selectedItem.IMAGE;
                buyItemBtns[i].interactable = true;

                Item myItem = items.Find(x => x.NAME == selectedItem.NAME);
                int finalPrice = selectedItem.PRICE;

                if (myItem != null)
                {
                    enhanceImgs[i].gameObject.SetActive(true);

                    if (myItem.ENHANCE >= 3)
                    {
                        // 이미 storeItems.Add를 했으므로 여기서 다시 Add하면 안 됨!
                        // 대신 해당 칸을 무효화 처리
                        storeItems[i] = null;
                        buyItemImgs[i].gameObject.SetActive(false);
                        enhanceImgs[i].gameObject.SetActive(false);
                        buyItemPrices[i].text = "";
                        //buyItemPrices[i].GetComponentInParent<Image>().gameObject.SetActive(false);
                        buyItemBtns[i].interactable = false;
                    }
                    else
                    {
                        //buyItemPrices[i].GetComponentInParent<Image>().gameObject.SetActive(true);
                        finalPrice *= (int)Mathf.Pow(2, myItem.ENHANCE);
                        buyItemPrices[i].text = $"{finalPrice}";
                    }
                }
                else
                {
                    buyItemPrices[i].text = $"{finalPrice}";
                }

                // --- 람다식 에러 해결 핵심 부분 ---
                int index = i; // 현재의 루프 인덱스 고정
                int priceForButton = finalPrice; // 현재 계산된 가격 고정
                //storeManager.UpdateSlot(index, storeItems[index]);
                UpdateInfo(storeItems[index] ,index);

                buyItemBtns[i].onClick.AddListener(() => {
                    // storeItems[index]를 통해 안전하게 아이템 참조
                    if (storeItems[index] != null)
                    {
                        SelectBuyItem(storeItems[index], priceForButton, index);
                    }
                });
            }
        }


        buyBtn[0].onClick.AddListener(() => SuccessCheck());
        buyBtn[1].onClick.AddListener(() => FailCheck());
    }


    void SelectBuyItem(Item item, int storePrice, int index)
    {
        //강화표시 여부로 확인(보유아이템)
        bool selectItem = items.Find(x => x.NAME == item.NAME);

        //플레이어 골드 보유량 체크
        if (itemManager.GetGold() >= storePrice)
        {
            itemManager.MinusGold(storePrice);

            storeItems[index] = null;                                     // 리스트 데이터 Null 처리
            buyItemBtns[index].interactable = false;                      // 버튼 클릭 방지
            buyItemImgs[index].color = new Color(0.5f, 0.5f, 0.5f, 0.5f); // (선택) 반투명하게 '품절' 느낌 주기

            storeManager.UpdateUI();

            //미보유 아이템의 경우
            if (!selectItem)
            {
                BuyItem_Success(item);
            }
            else
            {
                BuyItem_Enhance(item, storePrice);
                enhanceImgs[index].gameObject.SetActive(false);
            }
        }

        else
        {
            failTxt.text = $"<color=green>{item.NAME}</color> 구매불가 : 골드 부족";

            BuyItem_Fail();
        }

    }

    void BuyItem_Success(Item item)
    {
        itemManager.AddItem(item);

        //구매 성공 표시 UI
        //print("구매성공 : 새로운 아이템");
        successTxt.text = $"<color=green>{item.NAME}</color> 아이템 구매에 성공하였다";
        successImg.gameObject.SetActive(true);

        storeManager.CurrentItemUpdate();
    }

    void BuyItem_Enhance(Item item, int price)
    {
        if (item.ENHANCE < 3)
        {
            item.ENHANCE++;

            //아이템 강화 시 능력치 변경

            //구매된 아이템은 Null로 변경
            //print("구매성공 : 강화");
            //item = null;
            successTxt.text = $"<color=green>{item.NAME}</color> 아이템 강화에 성공하였다";
            successImg.gameObject.SetActive(true);
        }
        else
        {
            //강화 불가 표시 UI 및 골드 회수
            print("구매불가 : 강화불가");
            successTxt.text = $"<color=green>{item.NAME}</color> 최대강화 초과";
            itemManager.PlusGold(price);
        }
    }

    void BuyItem_Fail()
    {
        failImg.gameObject.SetActive(true);

        //구매 실패 표시 UI
        //print("구매불가 : 돈 부족");
    }


    void SuccessCheck()
    {
        successImg.gameObject.SetActive(false);
    }

    void FailCheck()
    {
        failImg.gameObject.SetActive(false);
    }

    void UpdateInfo(Item item, int index)
    {
        ItemEnhance slot = buyItemBtns[index].gameObject.GetComponent<ItemEnhance>();
        if (slot == null) slot = buyItemBtns[index].gameObject.AddComponent<ItemEnhance>();

        slot.Setup(item);
    }

    public void RemoveEnhanceImg(Item itemData)
    {
        // 중요: items(보유리스트)가 아니라 storeItems(상점에 노출된 리스트)와 비교해야 합니다.
        for (int i = 0; i < storeItems.Count; i++)
        {
            // 상점 슬롯에 아이템이 있고, 그 이름이 방금 판 아이템 이름과 같다면
            if (storeItems[i] != null && storeItems[i].NAME.Equals(itemData.NAME))
            {
                // 해당 상점 슬롯의 강화 이미지를 비활성화
                if (enhanceImgs[i] != null)
                {
                    enhanceImgs[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
