using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
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
            //보유아이템일 때
            List<Item> items = ItemManager.Instance.GetSelectItems();
            bool selectItem = items.Find(x => x.NAME == itemData.NAME);
            string itemExp = "";

            switch (itemData.ENHANCE)
            {
                case 0:
                    if(!selectItem)
                    {
                        itemExp = itemData.EXPLAIN;
                        description = $"구매\n{itemExp}";
                        itemInfo.ShowItemInfo(itemData.NAME, description);
                    }
                    else
                    {
                        itemExp = itemData.ENHANCE1_EXPLAIN;
                        description = $"강화: +{itemData.ENHANCE + 1}\n{itemExp}";
                        itemInfo.ShowItemInfo(itemData.NAME, description);
                    }
                    break;
                case 1:
                    itemExp = itemData.ENHANCE2_EXPLAIN;
                    description = $"강화: +{itemData.ENHANCE + 1}\n{itemExp}";
                    itemInfo.ShowItemInfo(itemData.NAME, description);
                    break;
                case 2:
                    itemExp = itemData.ENHANCE3_EXPLAIN;
                    description = $"강화: +{itemData.ENHANCE + 1}\n{itemExp}";
                    itemInfo.ShowItemInfo(itemData.NAME, description);
                    break;
                 case 3:
                    description = "";
                    break;
            }
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