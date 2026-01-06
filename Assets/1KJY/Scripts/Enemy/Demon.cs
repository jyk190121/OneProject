using UnityEngine;

public class Demon : Enemy
{
    // 자식은 이제 이 메서드에서 본인의 스탯만 정해주면 됩니다.
    protected override void InitStats()
    {
        maxHp = 2000;
        maxSh = 500;
        att1 = 10;
        att2 = 60;
    }

    // 공격 시 대미지 계산 방식이 다르다면 Override, 동일하다면 삭제해도 부모 것을 씀
    public override void Attack()
    {
        base.Attack();                  // 부모의 애니메이션 실행
        att1 = Random.Range(10, 100);   // 데몬만의 공격력 계산
    }

    override public void SpecialAttack()
    {
        base.SpecialAttack(); 
        att2 = Random.Range(60, 91); 
    }

    override public float ShildRecover()
    {
        base.ShildRecover();

        return 200f;
    }

    override public float Healing()
    {
        base.Healing();

        return 300f;
    }
}
