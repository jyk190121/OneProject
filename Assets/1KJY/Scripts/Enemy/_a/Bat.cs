using UnityEngine;

public class Bat : Enemy
{
    protected override void InitStats()
    {
        maxHp = 100;
        maxSh = 50;
        att1 = 10;
        att2 = 10;
    }

    public override void Attack()
    {
        base.Attack();                 
        att1 = Random.Range(10, 20);   
    }

    override public void SpecialAttack()
    {
        base.SpecialAttack();
        att2 = Random.Range(10, 41);
    }

    override public float ShildRecover()
    {
        base.ShildRecover();

        return 20f;
    }

    override public float Healing()
    {
        base.Healing();

        return 30f;
    }

    //override public void Attack()
    //{
    //    Animator animator = GetComponentInChildren<Animator>();
    //    animator.SetTrigger("AttackTrigger");

    //    int r = Random.Range(10, 20);
    //    att1 = r;
    //}
    //override public void SpecialAttack()
    //{
    //    Animator animator = GetComponentInChildren<Animator>();
    //    animator.SetTrigger("SpecialATrigger");

    //    int r = Random.Range(10, 41);
    //    att2 = r;
    //}

}
