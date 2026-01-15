using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
//업그레이드 상점용
public class ItemEnhance : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Item itemData;
    private ItemInfo itemInfo;
    string description;

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
}