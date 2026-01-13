using TMPro;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(TextMeshPro))]
//[RequireComponent(typeof(Image))]
public class Player : MonoBehaviour
{

    public Image hpBar;
    public Image shildBar;
    public TextMeshProUGUI hpBarTxt;
    public TextMeshProUGUI shildBarTxt;
    public TextMeshProUGUI goldTxt;         //보유 골드 텍스트

    //�������� ���� ������Ʈ
    public TextMeshProUGUI poisonTxt;

    public float hp;       //ü��
    public float maxHp;    //�ִ�ü��
    public float shild;    //��
    public float maxSh;    //�ִ��
    public float att1;     //물리공격력
    public float att2;     //마법공격력
    public float poison;   //�� ����������
    public int gold;       //보유골드

    //�ʱ⼳��
    public void HpShildSet()
    {
        maxHp = 100;
        maxSh = 100;
        hp = 100;
        shild = 0;
        att1 = 0;
        att2 = 0;
        gold = 0;
        poison = 0;

        UpdateHpShildSet();
        UpdatePosion();
        UpdateGold();
    }

    //������Ʈ ����
    public void UpdateHpShildSet()
    {
        hpBarTxt.text = "♥ " + hp.ToString();
        shildBarTxt.text = "ⓞ " + shild.ToString();

        hpBar.fillAmount = hp / maxHp;
        shildBar.fillAmount = shild / maxSh;
    }

    public void UpdatePosion()
    {
        poisonTxt.text = poison.ToString();
    }

    public void UpdateGold()
    {
        goldTxt.text = gold.ToString();
    }
}