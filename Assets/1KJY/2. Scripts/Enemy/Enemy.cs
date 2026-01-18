using UnityEngine;
using UnityEngine.UI;

//[System.Serializable]
//public struct EnemyAction
//{
//    public string actionName;
//    public float damageMultiplier;
//    public bool isMagic;
//    public float shieldAmount;
//}

public abstract class Enemy : MonoBehaviour
{

    //public List<EnemyAction> patternList;
    //int currentPatternIndex = 0;

    //public EnemyAction GetNextAction()
    //{
    //    var action = patternList[currentPatternIndex];
    //    currentPatternIndex = (currentPatternIndex + 1) % patternList.Count;
    //    return action;
    //}

    [Header("Data Reference")]
    public EnemyObject data;        // 이 적의 데이터 설계도

    [Header("UI References")]
    public Image hpBar;
    public Image shildBar;

    public float hp;                 // 적 체력
    public float maxHp;              // 적 최대체력
    public float shild;              // 적 방어도
    public float maxSh;              // 적 최대방어도
    public float minAtt1;            // 적 최소 물리공격력
    public float att1;               // 적 물리공격력
    public float minAtt2;            // 적 최소 마법공격력
    public float att2;               // 적 마법공격력
    public float recovery;           // 적 방어도 회복력
    public float heal;               // 적 체력 회복력

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

        // 데이터가 할당되어 있다면 초기화
        if (data != null)
        {
            InitFromData();
        }
    }

    //protected virtual void Start()
    //{
    //    // 인스펙터에 입력한 값을 기준으로 초기화만 진행
    //    hp = maxHp;
    //    shild = maxSh;
    //    UpdateUI();
    //}

    public void Setup(EnemyObject newData)
    {
        this.data = newData; // 외부(Manager)에서 데이터를 넣어줌

        if (data == null)
        {
            Debug.LogError($"{gameObject.name}에 EnemyObject 데이터가 없습니다!");
            return;
        }

        // 데이터가 확실히 있을 때 변수 할당
        maxHp = data.maxHp;
        maxSh = data.maxSh;
        hp = maxHp;
        shild = maxSh;

        // 이 변수들이 Enemy.cs 상단에 선언되어 있는지 확인 필수!
        minAtt1 = data.minAtt1;
        att1 = data.baseAtt1;
        minAtt2 = data.minAtt2;
        att2 = data.baseAtt2;
        recovery = data.recovery;
        heal = data.heal;

        UpdateUI();
    }

    //초기 수치를 설정하는 "추상 메서드"
    // 이제 각 자식 클래스(Slime, Boss 등)에서 
    // 일일이 수치를 적지 않아도 InitFromData가 처리합니다.
    protected abstract void InitStats();

    private void InitFromData()
    {
        maxHp = data.maxHp;
        maxSh = data.maxSh;
        hp = maxHp;
        shild = maxSh;
        minAtt1 = data.minAtt1;
        att1 = data.baseAtt1;
        minAtt2 = data.minAtt2;
        att2 = data.baseAtt2;
        recovery = data.recovery;
        heal = data.heal;
    }

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

        // 공통 로직 실행 (UI 업데이트 등)
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
    virtual public void Attack() { animator.SetTrigger("AttackTrigger"); }
    virtual public void SpecialAttack() { animator.SetTrigger("SpecialATrigger"); }
    virtual public void Death() { animator.SetTrigger("DeathTrigger"); }


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

        if (hp < 0)
        {
            hp = 0;
            Death();
        }
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
    virtual public void ShildRecover()
    {
        //Animator animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("TalkTrigger");
    }

    virtual public void Healing()
    {
        //Animator animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("JumpTrigger");
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
