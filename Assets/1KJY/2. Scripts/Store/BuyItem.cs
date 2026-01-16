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
        //itemManager = FindAnyObjectByType<ItemManager>();
        ////player = FindAnyObjectByType<Player>();
        //if (itemManager == null) return;

        //items = itemManager.CurrentItems();
        //allItems = itemManager.allItemDatas;

        //// [수정 2] 상점 아이템 리스트 초기화 (이전 찌꺼기 제거)
        //storeItems.Clear();

        //// 버튼 5개 각각 랜덤 아이템 노출
        //for (int i = 0; i < buyItemBtns.Length; i++)
        //{
        //    // 1. 초기 상태: 강화 표시 끄고 버튼 비활성화
        //    enhanceImgs[i].gameObject.SetActive(false);
        //    buyItemBtns[i].interactable = false;

        //    // 2. 랜덤 인덱스 결정 (중복 방지 포함)
        //    int r = -1;
        //    int maxAttempts = 10; // 무한 루프 방지용

        //    while (maxAttempts > 0)
        //    {
        //        r = Random.Range(-1, allItems.Count);

        //        // 빈 칸(-1)이 아니면서 이미 뽑힌 아이템이면 다시 뽑기
        //        if (r != -1 && usedIndices.Contains(r))
        //        {
        //            maxAttempts--;
        //            continue;
        //        }
        //        break;
        //    }

        //    if (r == -1)
        //    {
        //        storeItems.Add(null);
        //        buyItemImgs[i].gameObject.SetActive(false);
        //        buyItemPrices[i].text = "";
        //    }
        //    else
        //    {
        //        usedIndices.Add(r); // 뽑힌 아이템 기록
        //        Item selectedItem = allItems[r];
        //        storeItems.Add(selectedItem);

        //        enhanceImgs[i].gameObject.SetActive(false);

        //        // UI 세팅
        //        buyItemImgs[i].gameObject.SetActive(true);
        //        buyItemImgs[i].sprite = selectedItem.IMAGE;
        //        buyItemBtns[i].interactable = true;

        //        // 3. 강화 표시 로직 (중요: 보유 아이템 전체에서 검색)
        //        // 인벤토리(items) 리스트에서 상점 아이템과 ID가 같은 아이템을 찾음
        //        Item myItem = items.Find(x => x.NAME == selectedItem.NAME);

        //        int finalPrice = selectedItem.PRICE;

        //        if (myItem != null)
        //        {
        //            Debug.Log($"[상점] {selectedItem.NAME} 발견! 내 인벤토리 강화도: {myItem.ENHANCE}");
        //            // [결과] 내 인벤토리에 이 아이템이 있을 때만 실행됨
        //            enhanceImgs[i].gameObject.SetActive(true);

        //            if (myItem.ENHANCE >= 3)
        //            {
        //                //아이템 미노출로 변경
        //                storeItems.Add(null);
        //                buyItemImgs[i].gameObject.SetActive(false);
        //                buyItemPrices[i].text = "";
        //                buyItemBtns[i].interactable = false;
        //            }
        //            else
        //            {
        //                // 강화 비용 계산 (예: 강화 수치만큼 가격 상승)
        //                finalPrice *= (int)Mathf.Pow(2, myItem.ENHANCE);
        //                buyItemPrices[i].text = $"{finalPrice}";
        //            }
        //        }
        //        else
        //        {
        //            Debug.Log($"[상점] {selectedItem.NAME}는 내 인벤토리에 없음.");
        //            // [결과] 보유하지 않은 아이템은 기본 가격 표시
        //            enhanceImgs[i].gameObject.SetActive(false);

        //            buyItemPrices[i].text = $"{finalPrice}";
        //        }

        //        //4.버튼 선택 시 플레이어 골드보유량 체크해서 구매여부 확인
        //        buyItemBtns[i].onClick.AddListener(() => SelectBuyItem(storeItems[i] , finalPrice));
        //    }
        //}
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
                        buyItemPrices[i].text = "MAX";
                        buyItemBtns[i].interactable = false;
                    }
                    else
                    {
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
            failTxt.text = $"{item.NAME} 구매불가 : 골드 부족";

            BuyItem_Fail();
        }

    }

    void BuyItem_Success(Item item)
    {
        itemManager.AddItem(item);

        //구매 성공 표시 UI
        //print("구매성공 : 새로운 아이템");
        successTxt.text = $"{item.NAME} 아이템 구매에 성공하였다";
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
            successTxt.text = $"{item.NAME} 아이템 강화에 성공하였다";
            successImg.gameObject.SetActive(true);
        }
        else
        {
            //강화 불가 표시 UI 및 골드 회수
            print("구매불가 : 강화불가");
            successTxt.text = $"{item.NAME} 최대강화 초과";
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
}
