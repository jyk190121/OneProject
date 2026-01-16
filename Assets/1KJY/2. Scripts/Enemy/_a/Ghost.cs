using UnityEngine;

public class Ghost : Enemy
{
    protected override void InitStats() { }

    public override void Attack()
    {
        base.Attack();                 
    }

    override public void SpecialAttack()
    {
        base.SpecialAttack();
    }

    override public float ShildRecover()
    {
        base.ShildRecover();

        return 120f;
    }

    override public float Healing()
    {
        base.Healing();

        return 300f;
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
