using UnityEngine;

//[RequireComponent(typeof(Image))]
public class Butcher : Enemy
{
    protected override void InitStats()
    {
        maxHp = 300;
        maxSh = 300;
        att1 = 20;
        att2 = 20;
    }

    public override void Attack()
    {
        base.Attack();
        att1 = Random.Range(20, 40);
    }

    override public void SpecialAttack()
    {
        base.SpecialAttack();
        att2 = Random.Range(20, 61);
    }
    override public float ShildRecover()
    {
        base.ShildRecover();

        return 50f;
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

    //    int r = Random.Range(20, 40);
    //    att1 = r;
    //}
    //override public void SpecialAttack()
    //{
    //    Animator animator = GetComponentInChildren<Animator>();
    //    animator.SetTrigger("SpecialATrigger");

    //    int r = Random.Range(20, 61);
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
