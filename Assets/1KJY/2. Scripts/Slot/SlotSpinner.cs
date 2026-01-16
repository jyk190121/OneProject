using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotSpinner : MonoBehaviour
{
    public float speed;
    public bool isSpinning;

    //public List<Sprite> slotSprites;

    public List<Item> items;        //아이템 리스트

    [HideInInspector]
    public SpriteRenderer spriteRenderer;
    // 연출용 이미지 오브젝트 2개 (인스펙터에서 자식으로 생성해 할당하거나 코드로 생성)
    private SpriteRenderer[] renderers;

    int slotItemCount;
    ItemManager itemManager;

    float slotHeight = 100f; // 이미지 한 칸의 높이 (UI면 RectTransform.rect.height)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        itemManager = FindAnyObjectByType<ItemManager>();

        items = itemManager.GetSelectItems();

        slotItemCount = items.Count;
        speed = 1500f;

        spriteRenderer = GetComponent<SpriteRenderer>();

        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    // BattleManager의 Update에서 매번 호출되지 않도록 제어 변수 추가
    bool isCoroutineRunning = false;

    public void StartSpin()
    {
        if (isCoroutineRunning) return; // 이미 돌고 있으면 중복 실행 방지
        isSpinning = true;
        StartCoroutine(SpinCoroutine());
    }

    public void StopSpin()
    {
        //StartCoroutine(StopWithBounce());
        isSpinning = false; // 코루틴 내부 while문을 빠져나오게 함
    }

    IEnumerator SpinCoroutine()
    {
        // RectTransform childPos = transform.GetChild(0).GetComponent<RectTransform>(); // 첫 번째 자식 오브젝트 가져오기

        //while (isSpinning)
        //{
        //    // 스프라이트 변경
        //    slotItemCount = (slotItemCount + 1) % items.Count;
        //    //spriteRenderer.sprite = slotSprites[slotItemCount];
        //    spriteRenderer.sprite = items[slotItemCount].IMAGE;

        //    yield return new WaitForSeconds(speed * Time.deltaTime);
        //}


        isCoroutineRunning = true;
        float currentY = -254.88f;

        while (isSpinning)
        {
            // 위에서 아래로 이동
            currentY -= speed * Time.deltaTime;

            // 이미지가 한 칸 내려갔을 때 위치 리셋 및 이미지 교체
            if (currentY <= -slotHeight)
            {
                currentY += slotHeight;
                slotItemCount = (slotItemCount + 1) % items.Count;

                // 여기서 이미지를 미리 교체하여 흐르는 느낌을 줌
                renderers[0].sprite = items[slotItemCount].IMAGE;
                int nextIndex = (slotItemCount + 1) % items.Count;
                renderers[1].sprite = items[nextIndex].IMAGE;
            }

            // 실제 시각적 위치 업데이트 (두 렌더러의 위치를 조정)
            renderers[0].transform.localPosition = new Vector3(transform.localPosition.x, -204.88f + currentY, 0);
            renderers[1].transform.localPosition = new Vector3(transform.localPosition.x, -254.88f + currentY + slotHeight, 0);

            yield return null;
        }

        // 멈출 때 바운스 연출로 이어짐
        isCoroutineRunning = false;
        StartCoroutine(StopWithBounce());
    }

    IEnumerator StopWithBounce()
    {

        // 결과적으로 멈춘 시점의 이미지로 설정
        //spriteRenderer.sprite = slotSprites[slotItemCount];
        items[slotItemCount].IMAGE = spriteRenderer.sprite;

        // 정지 시 통통 튀는 효과 (바운스)
        float bounceTime = 1f;
        float elapsed = 0f;

        // 바운스 (위아래로 흔들리는 효과)
        while (elapsed < bounceTime)
        {
            elapsed += Time.deltaTime;
            float bounce = Mathf.Sin(elapsed * Mathf.PI * 3) * 10f; // 3회 진동
            transform.localPosition = new Vector3(transform.localPosition.x, -254.88f + bounce, 0);
            yield return null;
        }

        // 최종 위치 고정
        transform.localPosition = new Vector3(transform.localPosition.x, -254.88f, 0);

    }
}