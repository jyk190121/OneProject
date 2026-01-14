using UnityEngine;

public class Spider : Enemy
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

        return 10f;
    }

    override public float Healing()
    {
        base.Healing();

        return 10f;
    }
}
