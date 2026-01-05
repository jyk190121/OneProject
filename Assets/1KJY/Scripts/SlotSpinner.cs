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

    int slotItemCount;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        slotItemCount = 11;
        speed = 50f;

        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    public void StartSpin()
    {
        StartCoroutine(SpinCoroutine());
    }

    public void StopSpin()
    {
        StartCoroutine(StopWithBounce());
    }

    IEnumerator SpinCoroutine()
    {
        // RectTransform childPos = transform.GetChild(0).GetComponent<RectTransform>(); // 첫 번째 자식 오브젝트 가져오기

        while (isSpinning)
        {
            // 스프라이트 변경
            slotItemCount = (slotItemCount + 1) % items.Count;
            //spriteRenderer.sprite = slotSprites[slotItemCount];
            spriteRenderer.sprite = items[slotItemCount].IMAGE;

            yield return new WaitForSeconds(speed * Time.deltaTime);
        }

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
            float bounce = Mathf.Sin(elapsed * Mathf.PI * 3) * 3f; // 3회 진동
            transform.localPosition = new Vector3(transform.localPosition.x, -254.88f + bounce, 0);
            yield return null;
        }

        // 최종 위치 고정
        transform.localPosition = new Vector3(transform.localPosition.x, -254.88f, 0);

    }
}