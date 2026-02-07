using TMPro;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(TextMeshPro))]
//[RequireComponent(typeof(Player))]
public class Player : MonoBehaviour
{

    public Image hpBar;
    public Image shildBar;
    public TextMeshProUGUI hpBarTxt;
    public TextMeshProUGUI shildBarTxt;
    public TextMeshProUGUI goldTxt;         //보유 골드 텍스트

    public TextMeshProUGUI poisonTxt;
    public TextMeshProUGUI atkTxt;
    public TextMeshProUGUI matkTxt;

    public float hp;       //ü��
    public float maxHp;    //�ִ�ü��
    public float shild;    //��
    public float maxSh;    //�ִ��
    public float att1;     //물리공격력
    public float att2;     //마법공격력
    public float poison;   //�� ����������

    [Header("Base Stats from Store")]
    public float storeAtkMultiplier = 1.0f;     // 상점 물공 강화 (예: 1.2 = 20% 증폭)
    public float storeMatkMultiplier = 1.0f;
    public float storeAtkResist = 0f;           // 물리 저항
    public float storeMatkResist = 0f;          // 마법 저항

    //새로하기
    public void HpShildSet()
    {
        PlayerManager pm = PlayerManager.Instance;

        // 상점 매니저(Singleton)에서 영구 강화 값을 가져옴
        storeAtkMultiplier = 1.0f + pm.physicalAtkBonus;
        storeMatkMultiplier = 1.0f + pm.magicAtkBonus;

        storeAtkResist = pm.physicalResist;
        storeMatkResist = pm.magicResist;

        // 기본값 + 강화값
        if (pm.maxHP == 0) maxHp = 50f;
        else maxHp = pm.maxHP;

        hp = maxHp;
        //maxHp = 300;
        //hp = 300;
        maxSh = 0;
        shild = 0;
        att1 = 0f;
        att2 = 0f;
        poison = 0;

        UpdateHpShildSet();
        UpdatePosionUI();
        UpdateGoldUI();
        UpdateEnhanceUI();
    }

    public void UpdateHpShildSet()
    {
        hpBarTxt.text = "♥ " + ((int)hp).ToString();
        shildBarTxt.text = "ⓞ " + ((int)shild).ToString();

        hpBar.fillAmount = hp / maxHp;
        shildBar.fillAmount = shild / maxSh;
    }

    public void UpdatePosionUI()
    {
        poisonTxt.text = poison.ToString();
    }

    public void UpdateEnhanceUI()
    {
        atkTxt.text = att1.ToString();
        matkTxt.text= att2.ToString();
    }


    public void UpdateGoldUI()
    {
        goldTxt.text = ItemManager.Instance.GetGold().ToString();
    }

}