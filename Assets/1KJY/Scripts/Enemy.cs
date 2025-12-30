using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(Image))]
public class Enemy : MonoBehaviour
{
    public Image hpBar;
    public Image shildBar;
    
    public float hp;       //ü��
    public float maxHp;    //�ִ�ü��
    public float shild;    //��
    public float maxSh;    //�ִ�پ���
    public float att1;     //�������ݷ�
    public float att2;     //�������ܷ�
    public bool death;
    public Animator animator;

    public void HpShildSet()
    {
        maxHp = 1000;
        maxSh = 1000;
        hp = 1000;
        shild = 1000;
        att1 = 0;
        att2 = 0;

        hpBar.fillAmount = hp / maxHp;
        shildBar.fillAmount = shild / maxSh;
        death = false;
    }

    //������Ʈ ����
    public void UpdateHpShildSet()
    {
        AnimDamage();
        hpBar.fillAmount = hp / maxHp;
        shildBar.fillAmount = shild / maxSh;


    }

    public void AnimDamage()
    {
        Animator animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("Damage");
    }

    public void Attack()
    {
        Animator animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("AttackTrigger");

        //10~99 ����
        int r = Random.Range(10, 100);
        att1 = r;
    }

    public void SpecialAttack()
    {
        Animator animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("SpecialATrigger");

        //60~90 ����
        int r = Random.Range(60, 91);
        att2 = r;
    }
    public void ShildRecover()
    {
        Animator animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("TalkTrigger");
    }

    public void Healing()
    {
        Animator animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("TalkTrigger");
    }

    public void Stuned()
    {
        Animator animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("StunedTrigger");
    }

    public void Death()
    { 
        Animator animator = GetComponentInChildren<Animator>();
        animator.SetBool("DeathBool" , death);
    }
}
