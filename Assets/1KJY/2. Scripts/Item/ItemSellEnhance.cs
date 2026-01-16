using Unity.VisualScripting;
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

            // 팝업 매니저에게 메시지와 '실행할 함수'를 전달
            sellItemPopup.ShowConfirm(
                $"<color=green>{itemData.NAME}</color>을(를) 판매하시겠습니깡?\n<color=yellow>{itemData.PRICE}</color>골드",
                () => ExecuteSell() // 'Yes'를 누르면 실행될 람다식(Action)
            );
        }
    }

    // 실제 판매 처리 로직
    private void ExecuteSell()
    {
        Debug.Log($"[판매완료] {itemData.NAME}을 판매하여 {itemData.PRICE} 골드를 획득했습니다.");
        ItemManager.Instance.SellItem(itemData);
        storeManager.UpdateUI();
        // 1. 골드 추가 로직 (예시)
        // InventoryManager.Instance.AddGold(itemData.PRICE / 2);

        // 2. 인벤토리에서 제거 및 UI 갱신
        // ItemManager.Instance.RemoveItem(itemData);
        // FindAnyObjectByType<StoreManager>().UpdateUI();

        // 3. 현재 이 슬롯 오브젝트 파괴 (또는 초기화)
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        Destroy(gameObject);
    }
}