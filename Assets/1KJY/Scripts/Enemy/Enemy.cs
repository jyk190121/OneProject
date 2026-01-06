using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    [Header("UI References")]
    public Image hpBar;
    public Image shildBar;

    public float hp;                 // 적 체력
    public float maxHp;              // 적 최대체력
    public float shild;              // 적 방어도
    public float maxSh;              // 적 최대방어도
    public float att1;               // 적 물리공격력
    public float att2;               // 적 마법공격력
    public bool death;               // 적 죽음 확인
    public Animator animator;        // 적 애니메이션
    EnemyManager enemyManager;

    protected virtual void Awake()
    {
        // 매번 GetComponentInChildren을 호출하는 것은 성능에 좋지 않으므로 미리 캐싱합니다.
        enemyManager = FindAnyObjectByType<EnemyManager>();
        animator = GetComponentInChildren<Animator>();
        hpBar = enemyManager.hpBar;
        shildBar = enemyManager.shildBar;
    }

    protected virtual void Start()
    {
        // 인스펙터에 입력한 값을 기준으로 초기화만 진행
        hp = maxHp;
        shild = maxSh;
        UpdateUI();
    }
    
    //초기 수치를 설정하는 "추상 메서드"
    protected abstract void InitStats();

    virtual public void HpShildSet() 
    {
        // 1. 자식이 정의한 수치를 먼저 세팅
        InitStats();

        //maxHp = 1000;
        //maxSh = 1000;
        //hp = 1000;
        //shild = 1000;
        //att1 = 0;
        //att2 = 0;

        // 2. 공통 로직 실행 (UI 업데이트 등)
        hp = maxHp;
        shild = maxSh;

        //hpBar.fillAmount = hp / maxHp;
        //shildBar.fillAmount = shild / maxSh;
        UpdateUI();
        death = false;
    }

    public void UpdateUI() // UI 업데이트 로직 분리 (중복 제거)
    {
        if (hpBar != null) hpBar.fillAmount = hp / maxHp;
        if (shildBar != null) shildBar.fillAmount = shild / maxSh;
    }
    // 공통 애니메이션 함수 (자식에서 중복 코드를 작성할 필요가 없어짐)
    protected void PlayTrigger(string triggerName)
    {
        if (animator != null) animator.SetTrigger(triggerName);
    }

    // 이제 자식들은 내부 로직만 신경 쓰면 됨
    public virtual void Attack() { animator.SetTrigger("AttackTrigger"); }
    public virtual void SpecialAttack() { animator.SetTrigger("SpecialATrigger"); }
    public virtual void Death() { animator.SetTrigger("DeathTrigger"); death = true; }


    virtual public void UpdateHpShildSet()
    {
        AnimDamage();
        hpBar.fillAmount = hp / maxHp;
        shildBar.fillAmount = shild / maxSh;
    }

    virtual public void AnimDamage() 
    {
        //Animator animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("Damage");
    }

    //virtual public void Attack()
    //{
    //    Animator animator = GetComponentInChildren<Animator>();
    //    animator.SetTrigger("AttackTrigger");

    //    //10~99 물리공격
    //    int r = Random.Range(10, 100);
    //    att1 = r;
    //}

    //virtual public void SpecialAttack()
    //{
    //    Animator animator = GetComponentInChildren<Animator>();
    //    animator.SetTrigger("SpecialATrigger");

    //    //60~90 마법공격
    //    int r = Random.Range(60, 91);
    //    att2 = r;
    //}
    virtual public float ShildRecover()
    {
        //Animator animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("TalkTrigger");

        return 0;
    }

    virtual public float Healing()
    {
        //Animator animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("TalkTrigger");

        return 0;
    }

    virtual public void Stuned()
    {
        //Animator animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("StunedTrigger");
    }

    //virtual public void Death()
    //{
    //    //Animator animator = GetComponentInChildren<Animator>();
    //    animator.SetTrigger("DeathTrigger");
    //}
}
