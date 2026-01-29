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

}
