
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 게임 흐름제어만
/// 1. 분리작업 - 적 패턴 / 아이템 효과 
/// 2. 적 유형별 HP / Shild / 공격 패턴 만들기
///  - 스테이지(층) 1 : 일반몹 : 4마리 -> 보스
///  - 스테이지(층) 2 : 일반몹 : 4마리 -> 보스 ...
///  3. 몬스터 죽이면 보상 (현재 모든 아이템 갖고 시작)
///   - 3개 선택하여 시작
///   - 보상 : 골드
///   4. 업그레이드 상점
///   - 골드사용 : 아이템 구매 및 강화
///   5. 진행정보
///   - 아이템 수집 리스트
///   6. 플레이어 중도 사망 / 최종층 클리어
///   - 시작씬으로 ..
///   - 최종층 클리어보상 : ???(아레나 모드 오픈)
///   7. 아레나 모드
///   - 점수 표시
/// </summary>
public class GameManager : MonoBehaviour
{
    //List<Item> items;
    public SlotSpinner[] slotSpinner; 
    SlotSpinner[] spawnedSlots;       

    public Player player;
    public GameObject[] itemEffects;
    public GameObject itemPrefab;   
    public GameObject[] currentEffects;
    public Enemy enemy;
    public GameObject[] enemyObjects;   //적 프리팹


    public Button stopBtn;              //�������� ��ư

    public GameObject slotParent;       //���� ������ġ
    int slotCount;                      //�� ���� ���԰���

    string[] items;                     // ���� ���� ������ ���
    public bool playerTurn;
    public bool enemyTurn;
    bool playerSlotCheck;
    bool isEnemyturnning;

    bool cri1;              //ġ��Ÿ
    bool cri2;              //�ް�ġ��Ÿ

    public bool stuned1;    //��˽���
    public bool stuned2;    //��޵�������

    public TextMeshProUGUI turnTxt;
    public TextMeshProUGUI statusTxt;

    //아이템 이름, 콤보카운트 가져오기
    Dictionary<string, int> itemDict = new Dictionary<string, int>()
    {
        {"사과", 0},
        {"마법봉", 0},
        {"해골도끼", 0},
        {"포도", 0},
        {"마법검", 0},

        {"독약", 0},
        {"에너지", 0},
        {"일반검", 0},
        {"일반도끼", 0},
        {"대검", 0},
        {"고급도끼", 0},
    };

    public static GameManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //slotCount = 5;
        //spawnedSlots = new SlotSpinner[slotCount];
        //items = new string[slotCount];
        //itemPrefab = GetComponent<GameObject>();
        //itemArray = new GameObject[slotCount];
        //isEnemyturnning = false;
        //cri1 = false;
        //cri2 = false;

        //player.HpShildSet();
        //enemy.HpShildSet();

        //SpinSlotCreate();

        //EnemyCreate();

        //SpinStart();

        //stopBtn.onClick.AddListener(SpinSlotbySlotStop);
    }
    void Update()
    {
        if (player != null && enemy != null && stopBtn != null)
        {
            StatusTurn();

            if (Input.GetKeyDown(KeyCode.Space) && playerSlotCheck ||
                Input.GetKeyDown(KeyCode.Return) && playerSlotCheck)
            {
                SpinSlotbySlotStop();
            }

            foreach (SlotSpinner s in spawnedSlots)
            {
                if (s.isSpinning) s.StartSpin();
            }

            // 플레이어 턴
            if (spawnedSlots[spawnedSlots.Length - 1].isSpinning == false && playerSlotCheck)
            {
                // ComboCount 계산
                ComboCri(ComboCount(items));

                // 플레이어 아이템 효과 발동 및 생성
                StartCoroutine(ItemEffect(items));

                // 중복 실행 방지 (아이템이 다 돌았는지 체크)
                playerSlotCheck = false;

                // 스톱 버튼 비활성화
                stopBtn.gameObject.SetActive(false);
            }

            // 적 턴
            // 공격 1회 or 특수능력 1회 or 방어 or 체력회복 
            if (enemyTurn && !isEnemyturnning)
            {
                if (enemy.hp <= 0)
                {
                    // 적 사망(승리)
                    StartCoroutine(EnemyDeath());
                }
                else
                {
                    StartEnemyTurn();
                }
            }
        }
    }

    // 콤보 확인
    string ComboCount(string[] itmes)
    {
        string lastItem = null;

        foreach (string item in itmes)
        {
            if (itemDict.TryGetValue(item, out int equalsCount))
            {
                if (item == lastItem && lastItem != null)
                {
                    itemDict[item]++;
                }
                else
                {
                    itemDict[item] = 1;
                }
            }
            lastItem = item;
        }

        // 치명타, 메가치명타 여부 확인
        for (int i = 0; i < items.Length; i++)
        {
            if (itemDict.TryGetValue(items[i], out int equalsCount))
            {
                if (equalsCount >= 3 && equalsCount != 5)
                {
                    print($"{items[i]} 치명타 ");
                    cri1 = true;
                    return items[i];
                }

                if (equalsCount == 5)
                {
                    print($"{items[i]} 메가치명타");
                    cri2 = true;
                    return items[i];
                }
            }
        }

        // 콤보 횟수 초기화
        for (int i = 0; i < items.Length; i++)
        {
            itemDict[items[i]] = 1;
        }

        return null;
    }

    void ComboCri(string item)
    {
        if (cri1 == true)
        {
            itemDict[item] = 2;
        }
        else if (cri2 == true)
        {
            itemDict[item] = 3;
        }
    }

    // 플레이어 슬롯 스피너 생성
    void SpinSlotCreate()
    {
        for (int i = 0; i < slotCount; i++)
        {
            SlotSpinner slot = Instantiate(slotSpinner[i], slotParent.transform);

            if (slot.spriteRenderer == null)
            {
                slot.spriteRenderer = slot.GetComponent<SpriteRenderer>();
                if (slot.spriteRenderer == null)
                {
                    Debug.LogError("스프라이트가 없음!");
                }
            }

            spawnedSlots[i] = slot;
            slot.transform.localPosition = new Vector3(-432f + i * 216f, -254.88f, 0);
        }
    }

    void SpinStart()
    {
        // 턴 게임이 시작되면 플레이어 턴으로
        playerTurn = true;
        enemyTurn = !playerTurn;

        playerSlotCheck = true;

        // 슬롯 회전 시작
        foreach (SlotSpinner s in spawnedSlots)
        {
            if (s != null) s.isSpinning = true;
        }
    }

    void SpinSlotbySlotStop()
    {
        // null 체크
        if (spawnedSlots == null || spawnedSlots.Length == 0) return;

        for (int i = 0; i < spawnedSlots.Length; i++)
        {
            if (spawnedSlots[i] == null || spawnedSlots[i].spriteRenderer == null || spawnedSlots[i].spriteRenderer.sprite == null) continue;

            if (playerTurn)
            {
                string currentItemName = spawnedSlots[i].spriteRenderer.sprite.name;

                // 회전 중인 슬롯이 있다면 멈춤
                if (spawnedSlots[i].isSpinning)
                {
                    spawnedSlots[i].isSpinning = false;
                    spawnedSlots[i].StopSpin();
                    items[i] = currentItemName;
                    break; // 한 번에 하나씩만 멈춤
                }
                // 마지막 슬롯까지 다 멈췄다면
                else if (i == spawnedSlots.Length - 1)
                {
                    Debug.Log("전부 다 멈춤");
                    items[i] = currentItemName;
                    spawnedSlots[i].StopSpin();
                }
            }
        }
    }

    // 애니메이션 효과 or 파티클 생성
    IEnumerator ItemEffect(string[] items)
    {
        int num = 0;
        string action = "";
        int energy = 0;

        // 슬롯 아이템에 따른 효과 적용

        if (items != null)
        {
            foreach (string item in items)
            {
                if (itemDict.TryGetValue(item, out int equalsCount))
                {
                    // 이펙트 생성 위치
                    Vector3 itemPos = Vector3.zero;

                    // 각 아이템에 맞는 애니메이션
                    if (item.Equals("사과"))
                    {
                        itemPrefab = itemEffects[0];
                        itemPos = player.hpBar.transform.position;
                        switch (itemDict[item])
                        {
                            // 일반
                            case 1:
                                action = "체력 10 회복";
                                Apple(10, 0);
                                break;

                            // 치명타
                            case 2:
                                action = $"치명타!\nMAX 체력 10증가 + 10회복";
                                Apple(10, 10);
                                cri1 = false;
                                break;
                            // 메가치명타
                            case 3:
                                action = $"메가치명타!\nMAX 체력 30증가 + 30회복";
                                Apple(30, 30);
                                cri2 = false;
                                break;
                        }
                    }
                    if (item.Equals("포도"))
                    {
                        itemPrefab = itemEffects[1];
                        itemPos = player.shildBar.transform.position;
                        switch (itemDict[item])
                        {
                            // 일반
                            case 1:
                                action = "방어도 30 회복";
                                Grape(30, 0);
                                break;
                            case 2:
                                action = $"{item} 치명타!\n최대 방어 30증가 + 방어도 30 회복";
                                Grape(30, 30);
                                cri1 = false;
                                break;
                            case 3:
                                action = $"{item} 메가치명타!\n최대 방어 100증가 + 방어도 50 회복";
                                Grape(50, 100);
                                cri2 = false;
                                break;
                        }
                    }
                    if (item.Equals("에너지"))
                    {
                        for (int i = 0; i < items.Length; i++)
                        {
                            if (item == items[i])
                            {
                                int slotIndex = energy % spawnedSlots.Length;
                                // 슬롯의 위치에 이펙트 인덱스 설정
                                itemPos = spawnedSlots[slotIndex].transform.position;
                                itemPrefab = itemEffects[2]; // 효과 프리팹
                                break;
                            }
                        }

                        switch (itemDict[item])
                        {
                            // 일반
                            case 1:
                                action = "물공 10 증가";
                                Energy(10, 0);
                                break;

                            case 2:
                                action = $"{item} 치명타!\n물공/마공 10 증가";
                                Energy(10, 10);
                                cri1 = false;
                                break;

                            case 3:
                                action = $"{item} 메가치명타!\n물공/마공 30 증가";
                                Energy(30, 30);
                                cri2 = false;
                                break;
                        }
                    }

                    if (item.Equals("독약"))
                    {
                        itemPrefab = itemEffects[3];
                        itemPos = Vector3.up * 2;
                        //print("독 데미지 12");
                        switch (itemDict[item])
                        {
                            //일반
                            case 1:
                                action = "독 중독 12";
                                //독 데미지 부여 (매 턴마다 적에게 데미지를 입힌다)
                                player.poison += 12;
                                player.UpdatePosion();
                                break;
                            case 2:
                                action = $"{item} 치명타!\n독 중독 36";
                                player.poison += 36;
                                player.UpdatePosion();
                                cri1 = false;
                                break;
                            case 3:
                                action = $"{item} 메가치명타!\n독 중독 100";
                                player.poison += 100;
                                player.UpdatePosion();
                                cri2 = false;
                                break;
                        }
                    }

                    if (item.Equals("마법검"))
                    {
                        //print("마법 공격 30");
                        int att = 30;

                        switch (itemDict[item])
                        {
                            case 1:
                                att = 30;
                                action = $"마공 {player.att2 + att}";
                                break;
                            case 2:
                                att *= 3;
                                action = $"치명타!\n마공 {player.att2 + att}";
                                cri1 = false;
                                break;
                            case 3:
                                att *= 10;
                                action = $"메가치명타!\n마공 {player.att2 + att}";
                                cri2 = false;
                                break;
                        }

                        itemPrefab = itemEffects[4];
                        itemPos = Vector3.up * 2;

                        enemy.hp -= (player.att2 + att);

                        enemy.UpdateHpShildSet();

                    }

                    if (item.Equals("해골도끼"))
                    {
                        //print("공격 20 공격 20");
                        int att1 = Random.Range(5, 35);
                        int att2 = Random.Range(5, 35);

                        switch (itemDict[item])
                        {
                            case 1:
                                //att1 = 20;
                                //att2 = 20;
                                action = $"물공 {player.att1 + att1} , 마공 {player.att2 + att2}";
                                break;
                            case 2:
                                att1 *= 3;
                                att2 *= 3;
                                action = $"치명타!\n물공 {player.att1 + att1} , 마공 {player.att2 + att2}";
                                cri1 = false;
                                break;
                            case 3:
                                att1 *= 10;
                                att2 *= 10;
                                action = $"메가치명타!\n물공 {player.att1 + att1} , 마공 {player.att2 + att2}";
                                cri2 = false;
                                break;
                        }

                        itemPrefab = itemEffects[5];
                        itemPos = Vector3.up * 2;

                        //물리데미지 적용
                        AttDamage(att1);

                        //마법데미지 적용
                        enemy.hp -= (player.att2 + att2);
                        enemy.UpdateHpShildSet();
                    }
                    if (item.Equals("마법봉"))
                    {
                        //print("공격 10");
                        int att = 10;

                        switch (itemDict[item])
                        {
                            case 1:
                                att = 10;
                                action = $"마공 {player.att2 + att}";
                                break;
                            case 2:
                                att *= 3;
                                action = $"치명타!\n마공 {player.att2 + att}";
                                cri1 = false;
                                break;
                            case 3:
                                att *= 10;
                                action = $"메가치명타\n마공 {player.att2 + att}";
                                cri2 = false;
                                break;
                        }

                        itemPrefab = itemEffects[6];
                        itemPos = Vector3.up * 2;

                        enemy.hp -= (player.att2 + att);
                        enemy.UpdateHpShildSet();

                    }
                    if (item.Equals("일반검"))
                    {
                        //print("물공 10");
                        int att = 10;
                        switch (itemDict[item])
                        {
                            case 1:
                                att = 10;
                                action = $"물공 {player.att1 + att}";
                                break;
                            case 2:
                                att *= 3;
                                action = $"치명타!\n물공 {player.att1 + att}";
                                cri1 = false;
                                break;
                            case 3:
                                att *= 10;
                                action = $"메가치명타!\n물공 {player.att1 + att}";
                                cri2 = false;
                                break;
                        }

                        itemPrefab = itemEffects[7];
                        itemPos = Vector3.up * 2;

                        AttDamage(att);

                        enemy.UpdateHpShildSet();



                    }
                    if (item.Equals("일반도끼"))
                    {
                        //print("공격20 물리");
                        int att = Random.Range(5, 15);

                        switch (itemDict[item])
                        {
                            case 1:
                                //att = 20;
                                action = $"물공 {player.att1 + att}";
                                break;
                            case 2:
                                att *= 3;
                                action = $"치명타!\n물공 {player.att1 + att}";
                                cri1 = false;
                                break;
                            case 3:
                                att *= 10;
                                action = $"메가치명타!\n물공 {player.att1 + att}";
                                cri2 = false;
                                break;
                        }

                        itemPrefab = itemEffects[8];
                        itemPos = Vector3.up * 2;

                        AttDamage(att);

                        enemy.UpdateHpShildSet();

                    }
                    if (item.Equals("대검"))
                    {
                        //print("공격30 물리");
                        int att = 30;
                        int r = Random.Range(40, 100);

                        switch (itemDict[item])
                        {
                            case 1:
                                att = 30;
                                action = $"물공 {player.att1 + att}";
                                break;
                            case 2:
                                att *= 3;
                                action = $"치명타!\n물공 {player.att1 + att}";
                                cri1 = false;
                                r = 99;
                                break;
                            case 3:
                                att *= 10;
                                action = $"메가치명타!\n물공 {player.att1 + att}";
                                cri2 = false;
                                r = 99;
                                break;
                        }

                        print($"스턴체크 + {r > 90}");

                        //이미 스턴 상태이면 해제되지 않도록
                        if (!stuned1)
                        {
                            if (r > 90)
                            {
                                stuned1 = true;
                            }
                            else
                            {
                                stuned1 = false;
                            }
                        }


                        itemPrefab = itemEffects[9];
                        itemPos = Vector3.up * 2;

                        AttDamage(att);

                        enemy.UpdateHpShildSet();

                    }
                    if (item.Equals("고급도끼"))
                    {
                        //print("공격40 물리");
                        int att = Random.Range(20, 60);
                        int r = Random.Range(80, 100);

                        switch (itemDict[item])
                        {
                            case 1:
                                //att = 40;
                                action = $"물공 {player.att1 + att}";
                                break;
                            case 2:
                                att *= 3;
                                action = $"치명타!\n물공 {player.att1 + att}";
                                cri1 = false;
                                r = Random.Range(85, 100);
                                break;
                            case 3:
                                att *= 10;
                                action = $"메가치명타!\n물공 {player.att1 + att}";
                                cri2 = false;
                                r = 99;
                                break;
                        }

                        print($"스턴체크 + {r > 90}");

                        if (!stuned2)
                        {
                            if (r > 90)
                            {
                                stuned2 = true;
                            }
                            else
                            {
                                stuned2 = false;
                            }
                        }
                        itemPrefab = itemEffects[10];
                        itemPos = Vector3.up * 2;

                        AttDamage(att);

                        enemy.UpdateHpShildSet();

                    }

                    itemDict[item] = 1;

                    energy++;

                    currentEffects[num] = Instantiate(itemPrefab);
                    currentEffects[num].transform.position = itemPos;
                    //itemArray[num].GetComponent<SpriteRenderer>().sortingOrder = 11;

                    //적 앞에 소환
                    ParticleSystemRenderer effectRender = currentEffects[num].GetComponent<ParticleSystemRenderer>();
                    if (effectRender != null)
                    {
                        effectRender.sortingOrder = 11;
                    }

                    if (num < currentEffects.Length)
                    {
                        num++;
                        Status($"{item}\n{action}");

                    }

                    yield return new WaitForSeconds(1.5f);

                    //아이템이 한바퀴 돌았을 때
                    if (num == currentEffects.Length)
                    {

                        //턴 넘기기
                        playerTurn = false;
                        isEnemyturnning = false;
                    }

                    //아이템 오브젝트 파괴
                    for (int i = 0; i < currentEffects.Length; i++)
                    {
                        if (currentEffects[i] != null)
                        {
                            Destroy(currentEffects[i]);
                        }
                    }
                }
            }

        }
    }


    //턴 바뀔 때 텍스트 업데이트
    void StatusTurn()
    {
        enemyTurn = !playerTurn;

        if (playerTurn) turnTxt.text = "Player Turn";
        else turnTxt.text = "Enemy Turn";
    }

    //에너지나 회복 텍스트 업데이트
    void Status(string action)
    {
        if (playerTurn)
        {
            string enemyStatus = $"<color=white>적 체력 : {enemy.hp} \n적 방어도 : {enemy.shild}";
            statusTxt.color = Color.green;
            statusTxt.text = $"{action}\n\n{enemyStatus}";
        }

        else
        {
            statusTxt.color = Color.red;
            statusTxt.text = $"{action}";
        }
    }

    //적 캐릭터 생성
    void EnemyCreate(int r)
    {
        Transform enemyPos = GameObject.FindWithTag("Enemy").transform;
        GameObject newObj = Instantiate(enemyObjects[r], enemyPos);
        enemy = newObj.GetComponent<Enemy>();
        newObj.transform.position = Vector3.zero;
        if (r == 1 || r == 2) { newObj.transform.position = Vector3.down; }
        newObj.transform.localScale = new Vector3(-1, 1, 1);
        print("적 생성 완료");
    }


    void StartEnemyTurn()
    {
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        isEnemyturnning = true;
        enemyTurn = false;
        statusTxt.text = "";

        yield return new WaitForSeconds(1.5f);

        if (stuned1 || stuned2)
        {
            Status("기절!");
            enemy.Stuned();
            yield return new WaitForSeconds(1.5f);

            stuned1 = false;
            stuned2 = false;
        }
        else
        {
            int r = Random.Range(0, 10); //0~9
            print("적 행동 " + r);
            //공격확률(0~6)
            if (r < 7)
            {
                if (r == 0)
                {
                    //특수공격
                    EnemySpecialAttack();
                }
                else
                {
                    //일반공격
                    EnermyAttack();
                }
            }
            //방어도 회복(6~7)
            else if (r > 5 && r < 8)
            {
                EnemyShildRecover();
            }
            //체력 회복(8~9)
            else
            {
                EnemyHealing();

                //독 회복
                if (player.poison > 4)
                {
                    player.poison -= 4f;
                    if (player.poison <= 0)
                    {
                        player.poison = 0;
                    }
                }
            }
        }

        yield return new WaitForSeconds(1.5f);

        //플레이어의 독데미지 적용 시점
        if (player.poison > 0)
        {
            enemy.hp -= player.poison;
            Status($"<color=yellow> 독 피해 : {player.poison}");
            player.poison -= 2f;
            enemy.AnimDamage();

            enemy.UpdateHpShildSet();
            player.UpdatePosion();


            if (enemy.hp <= 0)
            {
                //적사망(승리)
                StartCoroutine(EnemyDeath());
                yield break;
            }
        }

        yield return new WaitForSeconds(1.5f);

        playerTurn = true;
        playerSlotCheck = true;
        stopBtn.gameObject.SetActive(true);
        Status(" ");

        //슬롯 재시작
        foreach (SlotSpinner s in spawnedSlots)
        {
            if (s != null) s.isSpinning = true;
        }

    }

    void EnermyAttack()
    {
        //적 공격 Enermy.cs에서 작성예정 -애니메이션, 이펙트 (파티클?) 등
        enemy.Attack();
        StartCoroutine(AttackEffect());
        string action = $"공격 {enemy.att1}";
        Status(action);

        if (player.shild >= enemy.att1)
        {
            player.shild -= enemy.att1;
        }
        else if (player.shild < enemy.att1)
        {
            player.hp -= (enemy.att1 - player.shild);
            player.shild = 0;
        }
        else
        {
            player.hp -= enemy.att1;
        }

        player.UpdateHpShildSet();

        if (player.hp <= 0)
        {
            //플레이어 사망 (패배)
            StartCoroutine(playerDeath());
            return;
        }
    }
    private void EnemySpecialAttack()
    {
        enemy.SpecialAttack();
        StartCoroutine(SpecialEffect());

        string action = $"특수공격 {enemy.att2}";
        Status(action);

        player.hp -= enemy.att2;
        player.UpdateHpShildSet();
        if (player.hp <= 0)
        {
            //플레이어 사망 (패배)
            StartCoroutine(playerDeath());
            return;
        }
    }
    private void EnemyShildRecover()
    {
        enemy.ShildRecover();
        StartCoroutine(ShildEffect());

        string action = $"방어도 {enemy.ShildRecover()}회복";
        Status(action);
        enemy.shild += enemy.ShildRecover();
        if (enemy.shild >= enemy.maxSh)
        {
            enemy.shild = enemy.maxSh;
        }

        enemy.UpdateHpShildSet();
    }
    private void EnemyHealing()
    {
        enemy.Healing();
        StartCoroutine(HealEffect());

        string action = $"체력 {enemy.Healing()}회복";
        Status(action);
        enemy.hp += enemy.Healing();
        if (enemy.hp >= enemy.maxHp)
        {
            enemy.hp = enemy.maxHp;
        }

        enemy.UpdateHpShildSet();
    }

    IEnumerator AttackEffect()
    {
        GameObject enemyEffect = Instantiate(itemEffects[11]);
        enemyEffect.transform.position = player.hpBar.transform.position;

        yield return new WaitForSeconds(1.5f);

        Destroy(enemyEffect);
    }

    IEnumerator SpecialEffect()
    {
        GameObject enemyEffect = Instantiate(itemEffects[12]);
        enemyEffect.transform.position = player.hpBar.transform.position;

        yield return new WaitForSeconds(1.5f);

        Destroy(enemyEffect);
    }

    IEnumerator ShildEffect()
    {
        GameObject enemyEffect = Instantiate(itemEffects[13]);
        enemyEffect.transform.position = enemy.transform.position;

        yield return new WaitForSeconds(1.5f);

        Destroy(enemyEffect);
    }

    IEnumerator HealEffect()
    {
        GameObject enemyEffect = Instantiate(itemEffects[14]);
        enemyEffect.transform.position = enemy.transform.position;

        yield return new WaitForSeconds(1.5f);

        Destroy(enemyEffect);
    }

    IEnumerator EnemyDeath()
    {
        //Animator anim = enemy.GetComponent<Animator>();
        //anim.Play("Death");
        if (enemy.death) yield break;
        //enemy.death = true;

        //적 죽음 애니메이션
        enemy.Death();

        yield return new WaitForSeconds(1.5f);

        playerTurn = false;
        enemyTurn = false;
        GameObject enemyEffect = Instantiate(itemEffects[15]);
        enemyEffect.transform.position = enemy.transform.position;
        Destroy(enemy.gameObject);

        //승리 결과창 추가?
        yield return new WaitForSeconds(1.5f);
        Destroy(enemyEffect);

        yield return new WaitForSeconds(3f);
        //AudioManager.audioManager.StopBGM();

        //Destroy(gameObject);
        SceneManager.LoadScene("StartScene", LoadSceneMode.Single);

        yield return null;
    }

    IEnumerator playerDeath()
    {
        //player.Death();
        playerTurn = false;
        enemyTurn = false;

        yield return new WaitForSeconds(3f);

        //SceneManager.LoadScene("GameOverScene", LoadSceneMode.Additive);
        //Destroy(gameObject);

        //AudioManager.audioManager.StopBGM();
        //AudioManager.audioManager.PlayBGM("Intro");
        SceneManager.LoadScene("GameOverScene");
        //SceneManager.LoadScene("StartScene");
    }

    void Apple(int playerHp, int playerMaxHp)
    {
        player.maxHp += playerMaxHp;
        player.hp += playerHp;
        if (player.hp >= player.maxHp) player.hp = player.maxHp;
        player.UpdateHpShildSet();
    }

    void Grape(int playerSh, int playerMaxSh)
    {
        player.maxSh += playerMaxSh;
        player.shild += playerSh;
        if (player.shild >= player.maxSh) player.shild = player.maxSh;
        player.UpdateHpShildSet();
    }

    void Energy(int att1, int att2)
    {
        player.att1 += att1;
        player.att2 += att2;
    }

    void AttDamage(int att1)
    {
        if (enemy.shild > 0 && enemy.shild >= (player.att1 + att1))
        {
            enemy.shild -= (player.att1 + att1);
        }
        else if (enemy.shild > 0 && enemy.shild < (player.att1 + att1))
        {
            enemy.hp = (player.att1 + att1) - enemy.shild;
            enemy.shild = 0;
        }
        else
        {
            enemy.hp -= (player.att1 + att1);
        }
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 바뀌면 UI와 연결된 parent를 찾아서 다시 세팅하는 함수
        InitializeSceneObjects();

    }

    void InitializeSceneObjects()
    {
        //if (AudioManager.audioManager.IsPlaying("Intro"))
        //{
        //    AudioManager.audioManager.StopBGM();
        //}
        //AudioManager.audioManager.SetBGMOnlyVol(AudioManager.audioManager.bgmSource.volume);

        // UI 다시 찾기
        stopBtn = GameObject.FindWithTag("StopBtn")?.GetComponent<Button>();
        statusTxt = GameObject.FindWithTag("StatusTxt")?.GetComponent<TextMeshProUGUI>();
        turnTxt = GameObject.FindWithTag("TurnTxt")?.GetComponent<TextMeshProUGUI>();
        slotParent = GameObject.FindWithTag("Slot");

        int r = Random.Range(0, enemyObjects.Length);

        // Player / Enemy 다시 찾기
        player = FindFirstObjectByType<Player>();

        //Enemy[] enemies = new Enemy[enemyObjects.Length];

        //enemies[r] = FindAnyObjectByType<Enemy>();

        //enemy = enemies[r];

        EnemyCreate(r);
         
        if (player == null || enemy == null || stopBtn == null)
        {
            Debug.LogWarning("필수 오브젝트 없음!!");
            return;
        }

        //초기 배열 및 카운트 재설정
        slotCount = 5;
        items = new string[slotCount];
       
        itemPrefab = GetComponent<GameObject>();
        currentEffects = new GameObject[slotCount];
        isEnemyturnning = false;
        cri1 = false;
        cri2 = false;
        stuned1 = false;
        stuned2 = false;

        // hp bar 등 UI는 켜져야 EnemyTurn에서 에러 안 생김
        player.HpShildSet();
        enemy.HpShildSet();

        // 슬롯 다시 생성
        if (slotParent != null)
        {
            slotCount = 5;
            spawnedSlots = new SlotSpinner[slotCount];
            SpinSlotCreate();
        }


        SpinStart();

        if (stopBtn != null)
        {
            stopBtn.onClick.RemoveAllListeners();
            stopBtn.onClick.AddListener(SpinSlotbySlotStop);
        }

        // 플레이어 턴부터 시작
        playerTurn = true;
        playerSlotCheck = true;

        //전체 매니저 관리가 없어 임시용
        if (AudioManager.audioManager == null) return;
        float value = AudioManager.audioManager.bgmVolume;

        print("볼륨값" + value);
        AudioManager.audioManager.PlayBGM("Battle", value);

        Debug.Log("씬 오브젝트들 초기화 완료");
    }
}