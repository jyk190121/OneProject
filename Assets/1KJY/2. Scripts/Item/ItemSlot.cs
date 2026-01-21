using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Item itemData;
    private ItemInfo itemInfo;

    public void Setup(Item newItem)
    {
        itemData = newItem;
        itemInfo = FindAnyObjectByType<ItemInfo>();
    }

    // 마우스를 올렸을 때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemData != null && itemInfo != null)
        {
            itemInfo.ShowItemInfo(itemData.NAME, itemData.EXPLAIN);
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
}