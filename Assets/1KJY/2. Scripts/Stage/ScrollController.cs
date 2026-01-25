using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollController : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform stageImgRT; // Content 역할을 하는 StageImg
    public float lerpSpeed = 10f;

    //private bool isDragging = false; // 드래그 중인지 체크하는 플래그

    private float targetX;
    private float viewportWidth;

    void Start()
    {
        viewportWidth = scrollRect.viewport.rect.width;
        targetX = stageImgRT.anchoredPosition.x;
    }

    void Update()
    {
        //// 1. 드래그 중이 아닐 때만 Lerp 이동 수행
        //if (!isDragging)
        //{
        //    // 키보드/버튼 네비게이션 감지
        //    HandleKeyboardNavigation();

        //    Vector2 currentPos = stageImgRT.anchoredPosition;
        //    // 현재 위치와 targetX가 거의 같으면 연산을 멈춰서 자원을 아낍니다.
        //    if (Mathf.Abs(currentPos.x - targetX) > 0.1f)
        //    {
        //        currentPos.x = Mathf.Lerp(currentPos.x, targetX, Time.deltaTime * lerpSpeed);
        //        stageImgRT.anchoredPosition = currentPos;
        //    }
        //}
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

   
    //// 마우스 드래그 시작 시 호출
    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    isDragging = true;
    //}

    //// 마우스 드래그 종료 시 호출
    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    isDragging = false;
    //    // 드래그가 끝난 지점의 위치를 새로운 targetX로 설정하여 튕김 방지
    //    targetX = stageImgRT.anchoredPosition.x;

    //    // 추가 팁: 드래그가 끝난 후 가장 가까운 페이지로 "자석 효과(Snapping)"를 
    //    // 주고 싶다면 여기서 targetX를 계산하여 할당하면 됩니다.
    //}
}