using UnityEngine;

//[RequireComponent(typeof(Image))]
public class CrystalGolem : Enemy
{
    protected override void InitStats()
    {
        maxHp = 1000;
        maxSh = 500;
        att1 = 20;
        att2 = 20;
    }

    public override void Attack()
    {
        base.Attack();
        att1 = Random.Range(20, 30);
    }

    override public void SpecialAttack()
    {
        base.SpecialAttack();
        att2 = Random.Range(20, 41);
    }

    override public float ShildRecover()
    {
        base.ShildRecover();

        return 100f;
    }

    override public float Healing()
    {
        base.Healing();

        return 100f;
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
