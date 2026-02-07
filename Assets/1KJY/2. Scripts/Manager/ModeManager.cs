using System;
using System.Collections;
using TMPro;
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
    public TextMeshProUGUI[] modePrices;
    //강화 성공 이미지
    public Image succesImg;
    //강화 실패 이미지 (돈 부족 or 조건 미달)
    public Image failImg;
    public TextMeshProUGUI failTxt;
    //강화 여부 팝업
    Popup popup;

    //int modeEnhance;
    public TextMeshProUGUI chipCountTxt;

    //모드정보
    public TextMeshProUGUI modeNameText;
    public TextMeshProUGUI descriptionText;

    // 현재 강화 단계를 저장 (실제 서비스 시에는 PlayerManager나 데이터 매니저에서 가져와야 함)
    // 인덱스: 0 물리저항, 1 마법저항, 2 물공, 3 마공, 4 골드, 5 체력
    private int[] upgradeLevels = new int[8];
    private int[] maxLevels = { 4, 4, 4, 4, 4, 4, 1, 1 };

    // 가격표 데이터
    private int[][] priceTables = new int[][]
    {
        new int[] { 25, 35, 45, 70 },   // 물리저항
        new int[] { 25, 35, 45, 70 },   // 마법저항
        new int[] { 25, 35, 45, 70 },   // 물공
        new int[] { 25, 35, 45, 70 },   // 마공
        new int[] { 5, 15, 25, 50 },    // 보너스골드
        new int[] { 35, 45, 55, 80 },   // 추가HP
        new int[] { 400 },              // 부활권
        new int[] { 250 }               // 반지셋
    };

    void Start()
    {
        popup = GetComponent<Popup>();
        chipCountTxt.text = PlayerManager.Instance.chip.ToString();

        //이미지
        succesImg.gameObject.SetActive(false);
        failImg.gameObject.SetActive(false);

        HideItemInfo();

        //for(int i=0; i < modePrices.Length; i++)
        //{
        //    modePrices[i].text = priceTables[i][0].ToString();
        //}
        ModePriceUpdateUI();

        //8개의 모드 버튼
        //1회만 강화가능한 버튼 2개 / 나머지는 4단계까지
        modeBtns[0].onClick.AddListener(() => TryUpgrade(0, PhysicalResistance));
        modeBtns[1].onClick.AddListener(() => TryUpgrade(1, MagicResistance));
        modeBtns[2].onClick.AddListener(() => TryUpgrade(2, PhysicalEnhancement));
        modeBtns[3].onClick.AddListener(() => TryUpgrade(3, MagicEnhancement));
        modeBtns[4].onClick.AddListener(() => TryUpgrade(4, BonusGold));
        modeBtns[5].onClick.AddListener(() => TryUpgrade(5, StrongHP));
        modeBtns[6].onClick.AddListener(() => TryUpgrade(6, Revive));
        modeBtns[7].onClick.AddListener(Rings);
    }

    ////물리저항 단계별 5%씩 (최대 20%)
    ////가격표 25 -> 35 -> 45 -> 70
    //void PhysicalResistance()
    //{
    //    print("물리저항 강화");
    //}

    ////마법저항 단계별 5%씩 (최대 20%)
    ////가격표 25 -> 35 -> 45 -> 70
    //void MagicResistance()
    //{

    //}
    ////물리공격력 단계별 25% (최대 100% - 2배)
    ////가격표 25 -> 35 -> 45 -> 70
    //void PhysicalEnhancement()
    //{

    //}
    ////마법공격력 단계별 25% (최대 100% - 2배)
    ////가격표 25 -> 35 -> 45 -> 70
    //void MagicEnhancement()
    //{

    //}
    ////동전추가(Round별 동전 20%추가) (최대 80%)
    ////가격표 5 -> 15 -> 25 -> 50
    //void BonusGold()
    //{

    //}
    ////시작체력 100 / 200 / 300 / 400
    ////가격표 35 -> 45 -> 55 -> 80
    //void StrongHP()
    //{


    //}
    ////플레이어 부활 1회 (게임마다 자동사용)
    ////가격표 400
    //void Revive()
    //{

    //}
    ////반지셋 (흡혈,독,마법) 3가지 모아서 구매가능 (스테이지 모든적 체력/방어도 50% 감소)
    ////가격표 250
    //void Rings()
    //{ 
    //}


    // 공통 강화 시도 로직
    void TryUpgrade(int index, System.Action upgradeAction)
    {
        popup.ShowConfirm(
                 $"해당 모드를 업그레이드 하시겠습니까",
                 () => Excute(index, upgradeAction) // 'Yes'를 누르면 실행될 람다식(Action)
                 );
    }

    void Excute(int index, System.Action upgradeAction)
    {
        if (upgradeLevels[index] >= maxLevels[index])
        {
            failTxt.text = "최대 강화 단계입니다 (강화불가)";
            StartCoroutine(ShowFeedback(failImg));
            //modeBtns[index].interactable = false;
            //modePrices[index].text = "";
            return;
        }
       
        int currentPrice = priceTables[index][upgradeLevels[index]];

        if (PlayerManager.Instance.chip >= currentPrice)
        {
            PlayerManager.Instance.chip -= currentPrice;
            upgradeLevels[index]++;
            //modePrices[index].text = currentPrice.ToString();
            chipCountTxt.text = PlayerManager.Instance.chip.ToString();
            upgradeAction.Invoke();
            StartCoroutine(ShowFeedback(succesImg));

            ModePriceUpdateUI(); // 여기서 전체 UI를 현재 단계에 맞게 새로고침

            //PlayerManager.Instance.SavePlayerData(); // 저장 잊지 마세요!
            //ModePriceUpdateUI();
        }
        else
        {
            failTxt.text = "칩이 모자랍니다 (강화실패)";
            StartCoroutine(ShowFeedback(failImg));
        }
    }


    IEnumerator ShowFeedback(Image img)
    {
        img.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        img.gameObject.SetActive(false);
    }

    // --- 강화 로직 상세 ---

    void PhysicalResistance()
    {
        // 단계별 5%씩 적용
        PlayerManager.Instance.physicalResist = upgradeLevels[0] * 0.05f;
        Debug.Log($"물리저항 강화: {PlayerManager.Instance.physicalResist * 100}%");
    }

    void MagicResistance()
    {
        PlayerManager.Instance.magicResist = upgradeLevels[1] * 0.05f;
    }

    void PhysicalEnhancement()
    {
        // 단계별 25% 증폭
        PlayerManager.Instance.physicalAtkBonus = upgradeLevels[2] * 0.25f;
    }

    void MagicEnhancement()
    {
        PlayerManager.Instance.magicAtkBonus = upgradeLevels[3] * 0.25f;
    }

    void BonusGold()
    {
        PlayerManager.Instance.goldBonus = upgradeLevels[4] * 0.20f;
    }

    void StrongHP()
    {
        PlayerManager.Instance.maxHP = upgradeLevels[5] * 100f;
    }

    void Revive()
    {
        if (PlayerManager.Instance.hasRevive) return;

        PlayerManager.Instance.hasRevive = true;
        StartCoroutine(ShowFeedback(succesImg));
        modeBtns[6].interactable = false; // 1회성 구매 제한
    }

    void Rings()
    {
        popup.ShowConfirm(
                $"해당 모드를 업그레이드 하시겠습니까",
                () => ExcuteRing() // 'Yes'를 누르면 실행될 람다식(Action)
                );
    }

    void ExcuteRing()
    {
        if (PlayerManager.Instance.chip >= 250 && PlayerManager.Instance.hasRings)
        {
            PlayerManager.Instance.chip -= 250;
            PlayerManager.Instance.enemyHalf = true;
            StartCoroutine(ShowFeedback(succesImg));
            //modePrices[7].text = "";
            //modeBtns[7].interactable = false;
            ModePriceUpdateUI();
            //PlayerManager.Instance.SavePlayerData();
        }
        else if (PlayerManager.Instance.chip >= 250 && !PlayerManager.Instance.hasRings)
        {
            failTxt.text = "모든 반지를 모으지 못했습니다 (강화실패)\n흡혈반지, 독반지, 마법반지 필요";
            StartCoroutine(ShowFeedback(failImg));
            return;
        }else if (PlayerManager.Instance.chip >= 250 && PlayerManager.Instance.enemyHalf)
        {
            failTxt.text = "최대 강화 단계입니다 (강화불가)";
            StartCoroutine(ShowFeedback(failImg));
            return;
        }
        else
        {
            failTxt.text = "칩이 모자랍니다 (강화실패)";
            StartCoroutine(ShowFeedback(failImg));
        }
    }

    public void ModePriceUpdateUI()
    {
        //for (int i = 0; i < modePrices.Length; i++)
        //{
        //    modePrices[i].text = priceTables[i][0].ToString();
        //}

        for (int i = 0; i < modePrices.Length; i++)
        {
            int currentLvl = upgradeLevels[i];
            int maxLvl = maxLevels[i];

            if (currentLvl >= maxLvl)
            {
                modePrices[i].text = "";
                modeBtns[i].interactable = false;
            }
            else
            {
                // i번째 항목의 현재 레벨(upgradeLevels[i])에 해당하는 가격 표시
                modePrices[i].text = priceTables[i][currentLvl].ToString();
                modeBtns[i].interactable = true;
            }
        }
    }

    public void ShowItemInfo(string name, string description)
    {
        modeNameText.text = name;
        descriptionText.color = Color.green;
        descriptionText.text = description;
    }

    public void HideItemInfo()
    {
        modeNameText.text = "";
        descriptionText.text = "";
    }

}
