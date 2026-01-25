using UnityEngine;
using UnityEngine.EventSystems;

//업그레이드 상점용
public class ItemSellEnhance : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Item itemData;
    private ItemInfo itemInfo;
    string description;
    Popup sellItemPopup;
    StoreManager storeManager;
    ItemManager itemManager;
    public void Setup(Item newItem)
    {
        itemData = newItem;
        itemInfo = FindAnyObjectByType<ItemInfo>();
        sellItemPopup = FindAnyObjectByType<Popup>();
        storeManager = FindAnyObjectByType<StoreManager>();
        itemManager = FindAnyObjectByType<ItemManager>();
    }

    // 마우스를 올렸을 때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemData != null && itemInfo != null)
        {
            string itemExp = "";
            switch (itemData.ENHANCE)
            {
                case 0:
                    itemExp = itemData.EXPLAIN;
                    break;
                case 1:
                    itemExp = itemData.ENHANCE1_EXPLAIN;
                    break;
                case 2:
                    itemExp = itemData.ENHANCE2_EXPLAIN;
                    break;
                case 3:
                    itemExp = itemData.ENHANCE3_EXPLAIN;
                    break;
            }

            description = $"강화수치 : +{itemData.ENHANCE}\n{itemExp}";
                    
            itemInfo.ShowItemInfo(itemData.NAME, description);
        }
    }

    // 마우스가 벗어났을 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        if (itemInfo != null)
        {
            itemInfo.HideItemInfo();
        }
    }

    //마우스로 선택하였을 때 호출
    public void OnPointerClick(PointerEventData eventData)
    {
        //판매 UI 호출 (판매하시겠습니까? Yes / No 선택 Action)
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (eventData.button != PointerEventData.InputButton.Left || itemData == null) return;

            //보유 아이템 최소 3개은 되도록
            if (itemManager.GetSelectItems().Count < 4)
            {
                //판매불가 이미지 노출
                storeManager.SellFailUI();
                return;
            }
            else
            {
                int price = Mathf.Max(1, itemData.PRICE / 3);

                // 팝업 매니저에게 메시지와 '실행할 함수'를 전달
                sellItemPopup.ShowConfirm(
                $"<color=green>{itemData.NAME}</color>을(를) 판매하시겠습니깡?\n<color=yellow>{price}</color>골드",
                () => ExecuteSell(price) // 'Yes'를 누르면 실행될 람다식(Action)
                );
            }

            //var currentItems = itemManager.GetSelectItems();

            //// 실제 유효한 아이템 개수 계산
            //int actualItemCount = currentItems.FindAll(x => x != null).Count;

            //// 조건 확인: 3개 이하일 때 (보유량이 3, 2, 1개라면 판매 불가)
            //if (actualItemCount <= 3)
            //{
            //    //Debug.LogWarning("아이템 3개 이하 - 판매 불가 UI 호출");

            //    // 중요: UI가 이미 켜져있더라도 다시 '깜빡'이거나 초기화되도록 처리
            //    if (storeManager != null)
            //    {
            //        storeManager.SellFailUI();
            //    }
            //    return;
            //}

            //// 3. 4개 이상일 때: 판매 확인 팝업 호출
            //int price = Mathf.Max(1, itemData.PRICE / 3);
            //if (sellItemPopup != null)
            //{
            //    sellItemPopup.ShowConfirm(
            //        $"<color=green>{itemData.NAME}</color>을(를) 판매하시겠습니깡?\n<color=yellow>{price}</color>골드",
            //        () => ExecuteSell(price)
            //    );
            //}
        }
    }

    // 실제 판매 처리 로직
    private void ExecuteSell(int p)
    {
        print($"[판매완료] {itemData.NAME}을 판매하여 {p} 골드를 획득했습니다.");
        itemManager.SellItem(itemData,p);

        // 상점 UI(BuyItem)의 강화 이미지도 꺼줘야 함
        BuyItem buyItem = FindAnyObjectByType<BuyItem>();
        if (buyItem != null)
        {
            buyItem.RemoveEnhanceImg(itemData);
        }

        // 인벤토리 UI 갱신
        storeManager.UpdateUI();

        // 오브젝트 파괴
        if (transform.parent != null) Destroy(transform.parent.gameObject);
        Destroy(gameObject);
    }
}