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
    public int ID => id;
    public string NAME;
    public int PRICE;
    public int COUNT;
    public int ATK;
    public int MATK;


    public void SetID(int newID)
    {
        id = newID;
    }
}
