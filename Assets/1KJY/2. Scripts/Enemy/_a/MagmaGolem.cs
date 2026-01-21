using UnityEngine;

//[RequireComponent(typeof(Image))]
public class MagmaGolem : Enemy
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

    override public void ShildRecover()
    {
        base.ShildRecover();
    }

    override public void Healing()
    {
        base.Healing();
    }

    //override public void Attack()
    //{
    //    Animator animator = GetComponentInChildren<Animator>();
    //    animator.SetTrigger("AttackTrigger");

    //    int r = Random.Range(20, 30);
    //    att1 = r;
    //}
    //override public void SpecialAttack()
    //{
    //    Animator animator = GetComponentInChildren<Animator>();
    //    animator.SetTrigger("SpecialATrigger");

    //    int r = Random.Range(20, 41);
    //    att2 = r;
    //}

    //override public void ShildRecover()
    //{
    //    Animator animator = GetComponentInChildren<Animator>();
    //    animator.SetTrigger("TalkTrigger");
    //}

    //override public void Healing()
    //{
    //    Animator animator = GetComponentInChildren<Animator>();
    //    animator.SetTrigger("TalkTrigger");
    //}

    //override public void Stuned()
    //{
    //    Animator animator = GetComponentInChildren<Animator>();
    //    animator.SetTrigger("StunedTrigger");
    //}

    //override public void Death()
    //{ 
    //    Animator animator = GetComponentInChildren<Animator>();
    //    animator.SetTrigger("DeathTrigger");
    //}
}
