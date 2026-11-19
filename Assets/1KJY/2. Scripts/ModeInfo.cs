
using UnityEngine;
using UnityEngine.EventSystems;

public class ModeInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int modeIndex;
    public string modeName;
    public string description;
    //public int[] costs;
    //public int currentLevel = 0;
    //public int maxLevel;
    ModeManager modeManager;

    //퍼센트
    [Range(0f, 1f)]
    public float per;

    void Start()
    {
        modeManager = FindAnyObjectByType<ModeManager>();
        if (modeManager != null)
        {
            modeManager.HideItemInfo();
        }
    }

    // 마우스를 올렸을 때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(modeManager != null)
        {
            //string modePullName = $"{modeName} : {currentLevel}단계";
            //string newDescription;
            //if (per == 0) newDescription = $"{description}";
            //else newDescription = $"{description} {per*100}%";

            modeManager.ShowItemInfo(modeIndex, modeName, description, per);
        }
    }
    // 마우스가 벗어났을 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        if (modeManager != null)
        {
            modeManager.HideItemInfo();
        }
    }

}