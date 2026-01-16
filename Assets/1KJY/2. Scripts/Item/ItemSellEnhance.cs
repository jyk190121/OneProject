using UnityEngine;
using UnityEngine.EventSystems;

//업그레이드 상점용
public class ItemSellEnhance : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Item itemData;
    private ItemInfo itemInfo;
    string description;
    SellItemPopup sellItemPopup;
    StoreManager storeManager;

    public void Setup(Item newItem)
    {
        itemData = newItem;
        itemInfo = FindAnyObjectByType<ItemInfo>();
        sellItemPopup = FindAnyObjectByType<SellItemPopup>();
        storeManager = FindAnyObjectByType<StoreManager>();
    }

    // 마우스를 올렸을 때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemData != null && itemInfo != null)
        {
            description = $"강화수치 : +{itemData.ENHANCE}\n{itemData.EXPLAIN}";
                    
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

            if (itemData == null) return;

            int price = itemData.PRICE / 3;
            if (price <= 0) price = 1;

            // 팝업 매니저에게 메시지와 '실행할 함수'를 전달
                sellItemPopup.ShowConfirm(
                $"<color=green>{itemData.NAME}</color>을(를) 판매하시겠습니깡?\n<color=yellow>{price}</color>골드",
                () => ExecuteSell(price) // 'Yes'를 누르면 실행될 람다식(Action)
            );
        }
    }

    // 실제 판매 처리 로직
    private void ExecuteSell(int p)
    {
        Debug.Log($"[판매완료] {itemData.NAME}을 판매하여 {p} 골드를 획득했습니다.");
        ItemManager.Instance.SellItem(itemData,p);
        storeManager.UpdateUI();

        // 2. [추가] 상점 UI(BuyItem)의 강화 이미지도 꺼줘야 함
        BuyItem buyItemScript = FindAnyObjectByType<BuyItem>();
        if (buyItemScript != null)
        {
            buyItemScript.RemoveEnhanceImg(itemData);
        }

        // 3. 인벤토리 UI 갱신
        storeManager.UpdateUI();

        // 4. 오브젝트 파괴
        if (transform.parent != null) Destroy(transform.parent.gameObject);
        Destroy(gameObject);
    }
}