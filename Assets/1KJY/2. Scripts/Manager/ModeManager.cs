using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 모드 업그레이드 칩 여부 확인
/// - 버튼별 업그레이드 내용 소개
/// - 플레이어가 Round를 깰때마다 모드를 구매할 수 있는 별도 칩을 줌
/// </summary>
public class ModeManager : MonoBehaviour
{
    public Button[] modeBtns;
    //강화 성공 이미지
    public Image succesImg;
    //강화 실패 이미지 (돈 부족 or 조건 미달)
    public Image failImg;
    //강화 여부 팝업
    Popup popup;

    int modeEnhance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        popup = GetComponent<Popup>();

        //성공실패이미지는 끄자
        succesImg.gameObject.SetActive(false);
        failImg.gameObject.SetActive(false);

        //8개의 모드 버튼
        //1회만 강화가능한 버튼 2개 / 나머지는 4단계까지
        modeBtns[0].onClick.AddListener(PhysicalResistance);
        modeBtns[1].onClick.AddListener(MagicResistance);
        modeBtns[2].onClick.AddListener(PhysicalEnhancement);
        modeBtns[3].onClick.AddListener(MagicEnhancement);
        modeBtns[4].onClick.AddListener(BonusGold);
        modeBtns[5].onClick.AddListener(StrongHP);
        modeBtns[6].onClick.AddListener(Revive);
        modeBtns[7].onClick.AddListener(Rings);
    }

    //물리저항 단계별 5%씩 (최대 20%)
    void PhysicalResistance()
    {

    }

    //마법저항 단계별 5%씩 (최대 20%)
    void MagicResistance()
    {

    }
    //물리공격력 단계별 25% (최대 100% - 2배)
    void PhysicalEnhancement()
    {

    }
    //마법공격력 단계별 25% (최대 100% - 2배)
    void MagicEnhancement()
    {

    }
    //동전추가(Round별 동전 20%추가) (최대 80%)
    void BonusGold()
    {

    }
    //시작체력 100 / 200 / 300 / 400
    void StrongHP()
    {


    }
    //플레이어 부활 1회 (게임마다 자동사용)
    void Revive()
    {

    }
    //반지셋 (흡혈,독,마법) 3가지 모아서 구매가능 (스테이지 모든적 체력/방어도 50% 감소)
    void Rings()
    {

    }
}
