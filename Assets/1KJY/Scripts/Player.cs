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

    //�������� ���� ������Ʈ
    public TextMeshProUGUI poisonTxt;

    public float hp;       //ü��
    public float maxHp;    //�ִ�ü��
    public float shild;    //��
    public float maxSh;    //�ִ��
    public float att1;     //�������ݷ�
    public float att2;     //�������ܷ�
    public float poison;   //�� ����������
    public int gold;

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

}