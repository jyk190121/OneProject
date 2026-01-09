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
    public int ID => id;        //아이템 ID (오브젝트 생성 시 자동부여, 쓸일이 있을지는..)
    public string NAME;         //아이템 이름
    public Sprite IMAGE;        //아이템 이미지

    [Header("업그레이드 상점")]
    public int PRICE;           //아이탬 가격 (업그레이드 상점용)
    public int ENHANCE;         //강화 횟수(최대 3강) 임의로
    public int GOLD;            //골드 (아이템으로 얻게되는 화폐)

    [Header("아이템 스텟")]
    public int MINATK;          //최소 물리공격력 스텟
    public int MINMATK;         //최소 마법공격력 스텟
    public int ATK;             //(최대)물리공격력 스텟
    public int MATK;            //(최대)마법공격력 스텟
    public int PLUSATK;         //물리공격력 증가 스텟
    public int PLUSMATK;        //마법공격력 증가 스텟
    public float HP;            //체력회복 스텟
    public float SHILD;         //방어도회복 스텟
    public float POISON;        //독 누적
    //public int COUNT;           //치명타, 메가치명타

    [Header("기절확률")]
    [Range(0, 1)]
    public float STUNED;

    [Header("아이템 설명")]
    [TextArea(minLines: 1, maxLines: 10)]
    public string EXPLAIN;

    public void SetID(int newID)
    {
        id = newID;
    }
}
