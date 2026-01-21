using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollController : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform stageImgRT; // Content 역할을 하는 StageImg
    public float lerpSpeed = 10f;

    private float targetX;
    private float viewportWidth;

    void Start()
    {
        viewportWidth = scrollRect.viewport.rect.width;
        targetX = stageImgRT.anchoredPosition.x;
    }

    void Update()
    {
        // 1. 키보드 네비게이션 감지
        HandleKeyboardNavigation();

        // 2. 부드러운 이동 적용
        Vector2 currentPos = stageImgRT.anchoredPosition;
        currentPos.x = Mathf.Lerp(currentPos.x, targetX, Time.deltaTime * lerpSpeed);
        stageImgRT.anchoredPosition = currentPos;
    }

    void HandleKeyboardNavigation()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        // 선택된 버튼이 StageImg의 자식인지 확인
        if (selected == null || selected.transform.parent != stageImgRT) return;

        RectTransform selectedRT = selected.GetComponent<RectTransform>();

        // 버튼의 위치를 기준으로 중앙에 오게 할 X 좌표 계산
        // 버튼의 anchoredPosition.x는 부모(StageImg) 기준 좌표입니다.
        float halfView = viewportWidth / 2f;
        float newTargetX = -selectedRT.anchoredPosition.x + halfView;

        // 이동 범위 제한 (Clamp)
        float contentWidth = stageImgRT.rect.width;
        float minX = -(contentWidth - viewportWidth);
        float maxX = 0;

        targetX = Mathf.Clamp(newTargetX, minX, maxX);
    }

    // 마우스 드래그가 끝났을 때 targetX를 현재 위치로 동기화 (튕김 방지)
    public void OnScrollDragEnd()
    {
        targetX = stageImgRT.anchoredPosition.x;
    }
}