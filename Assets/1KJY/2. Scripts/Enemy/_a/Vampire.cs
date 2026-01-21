using UnityEngine;
//흡혈기능 추가하고싶다
public class Vampire : Enemy
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

    public void blooding()
    {

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
