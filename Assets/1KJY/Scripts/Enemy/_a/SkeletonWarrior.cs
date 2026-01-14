using UnityEngine;

public class SkeletonWarrior : Enemy
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

        return 60f;
    }

    override public float Healing()
    {
        base.Healing();

        return 40f;
    }
}
