using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 아이템
/// 이름/가격/갯수/물공/마공
/// 능력
/// - 일반 / 치명타 / 메가치명타
/// </summary>
/// 
[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    // 외부에서 수정 불가능하게 하되, 시리얼라이즈는 되도록 설정
    [SerializeField, HideInInspector]
    private int id;

    // 읽기 전용 프로퍼티
    public int ID => id * (ENHANCE + 1);        //아이템 ID (오브젝트 생성 시 자동부여, 쓸일이 있을지는..)
    public string NAME;         //아이템 이름
    public Sprite IMAGE;        //아이템 이미지

    [Header("업그레이드 상점")]
    public int PRICE;           //아이탬 가격 (업그레이드 상점용)
    public int ENHANCE;         //강화 횟수(최대 3강) 임의로
    public int GOLD;            //골드 (아이템으로 얻게되는 화폐)

    [Header("아이템 기본 스텟")]
    public int MINATK;          //최소 물리공격력 스텟
    public int MINMATK;         //최소 마법공격력 스텟
    public int ATK;             //(최대)물리공격력 스텟
    public int MATK;            //(최대)마법공격력 스텟
    public int PLUSATK;         //물리공격력 증가 스텟
    public int PLUSMATK;        //마법공격력 증가 스텟
    public float PLUS_HP;       //(최대) 체력증가 스텟
    public float PLUS_SHILD;    //(최대) 방어도증가 스텟
    public float HP;            //체력회복 스텟
    public float SHILD;         //방어도회복 스텟
    public float POISON;        //독 누적
    public float BLOOD;         //흡열 수치(HP 강탈)
    public int COUNT;           //치명타, 메가치명타 (콤보카운트)

    [Header("기절확률")]
    [Range(0, 1)]
    public float STUNED;


    //강화수치 계산
    [Header("강화된 아이템 스텟")]
    public int ENHANCE_MINATK => MINATK * (ENHANCE+1);
    public int ENHANCE_MINMATK => MINMATK * (ENHANCE + 1);
    public int ENHANCE_ATK => ATK * (ENHANCE + 1);
    public int ENHANCE_MATK => MATK * (ENHANCE + 1);
    public int ENHANCE_PLUSATK => PLUSATK * (ENHANCE + 1);
    public int ENHANCE_PLUSMATK => PLUSMATK * (ENHANCE + 1);
    public float ENHANCE_PLUS_HP => PLUS_HP * (ENHANCE + 1);
    public float ENHANCE_PLUS_SHILD => PLUS_SHILD * (ENHANCE + 1);
    public float ENHANCE_HP => HP * (ENHANCE + 1);
    public float ENHANCE_SHILD => SHILD * (ENHANCE + 1);
    public float ENHANCE_POISON => POISON * (ENHANCE + 1);
    public float ENHANCE_BLOOD => BLOOD * (ENHANCE + 1);
    public float ENHANCE_STUNED => STUNED + ((ENHANCE / 10));

    [Header("아이템 이팩트")]
    public GameObject EFFECT;

    //public List<ItemEffect> effects; // 이 아이템이 가진 기능들 (여러 개 가능)

    //public void Use(GameObject user)
    //{
    //    foreach (ItemEffect effect in effects)
    //    {
    //        effect.Execute(user, this);
    //    }
    //}


    [Header("아이템 설명")]
    [TextArea(minLines: 1, maxLines: 10)]
    public string EXPLAIN;

    public void SetID(int newID)
    {
        id = newID;
    }
}
