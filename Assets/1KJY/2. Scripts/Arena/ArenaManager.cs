
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering; // 상단에 추가 필요
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
public class ArenaManager : MonoBehaviour
{
    public Image hpBar;
    public Image shildBar;

    List<Item> allItemDatas;

    public SlotSpinner[] slotSpinner;
    SlotSpinner[] spawnedSlots;

    public Player player;
    public GameObject itemPrefab;
    public GameObject[] currentEffects;
    public Enemy enemy;

    public Button stopBtn;              //�������� ��ư

    public GameObject slotParent;       //���� ������ġ
    int slotCount;                      //�� ���� ���԰���

    string[] items;                     // ���� ���� ������ ���
    public bool playerTurn;
    public bool enemyTurn;
    bool playerSlotCheck;
    bool isEnemyturnning;

    public bool stuned1;    //대검
    public bool stuned2;    //고급도끼
    public bool stuned3;    //해골도끼
    public bool stuned4;    //천둥망치

    public TextMeshProUGUI turnTxt;
    public TextMeshProUGUI statusTxt;

    ScoreManager scoreManager;
    EnemyArenaManager enemyManager;
    ItemManager itemManager;
    StageManager stageManager;

    public GameObject goldParent;               //골드 프리팹 생성할 위치
    public GameObject goldPrefab;               //캔버스에 보여줄 프리팹(골드)

    public TextMeshProUGUI nextEnemyActionTxt;  // 적 다음 행동
    bool enemyActionCeheck = false;             // 1턴에 1번
    int actionEnemy;

    public TextMeshProUGUI roundTxt;
    public TextMeshProUGUI scoreTxt;
    int turn;

    //부활권 사용여부
    bool playerRevive = false;

    //슬롯 멈춤 체크용
    Coroutine spinStopCoroutine;

    Dictionary<string, int> itemDict = new Dictionary<string, int>()
    {
        {"고급도끼", 0},
        {"사과", 0},
        {"에너지", 0},
        {"물리에너지", 0},
        {"물리에너지대", 0},
        {"마법에너지", 0},
        {"마법에너지대", 0},
        {"특수에너지", 0},
        {"화염방패", 0},
        {"골드", 0},
        {"포도", 0},
        {"대검", 0},
        {"마법투구", 0},
        {"마법검", 0},
        {"마법봉", 0},
        {"고기", 0},
        {"독약", 0},
        {"독검", 0},
        {"일반도끼", 0},
        {"일반검", 0},
        {"마법반지", 0},
        {"흡혈반지", 0},
        {"독반지", 0},
        {"해골도끼", 0},
        {"해골방패", 0},
        {"원석", 0},
        {"딸기", 0},
        {"천둥망치", 0},
    };

    void Awake()
    {
        InitializeSceneObjects();
        roundTxt.text = $"{scoreManager.round} 라운드";
        scoreTxt.text = scoreManager.score.ToString();
    }

    void Update()
    {
        if (player != null && enemy != null && stopBtn != null)
        {
            StatusTurn();

            if(Keyboard.current.enterKey.wasPressedThisFrame && playerSlotCheck ||
                Keyboard.current.spaceKey.wasPressedThisFrame && playerSlotCheck)
            {
                spinStopCoroutine = StartCoroutine(CoroutineSpinSlotbySlotStop());
            }
            else if (Keyboard.current.enterKey.wasReleasedThisFrame && playerSlotCheck ||
                Keyboard.current.spaceKey.wasReleasedThisFrame && playerSlotCheck)
            {
                if (spinStopCoroutine != null)
                {
                    StopCoroutine(spinStopCoroutine);
                    spinStopCoroutine = null;
                }
            }
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
        //StartPlayerTurn();

        if (!enemyActionCeheck)
        {
            turn++;
            //DetermineEnemyNextAction(); // 여기서 한 번만 결정
            enemyActionCeheck = true;
        }


        // 턴 게임이 시작되면 플레이어 턴으로
        playerTurn = true;
        enemyTurn = !playerTurn;
        enemyActionCeheck = false;

        // 슬롯 회전 시작
        foreach (SlotSpinner s in spawnedSlots)
        {
            if (s != null)
            {
                s.isSpinning = true;
                s.StartSpin();
            }
        }
    }
    void SpinStop()
    {
        // 슬롯 회전 시작
        foreach (SlotSpinner s in spawnedSlots)
        {
            if (s != null)
            {
                s.isSpinning = false;
                s.StopSpin();
            }
        }
    }

    void StartPlayerTurn()
    {
        // 1. 플레이어 슬롯 돌리기 준비
        playerSlotCheck = true;
        stopBtn.gameObject.SetActive(true);

        // 2. 적의 다음 행동 미리 결정 (플레이어 턴에 보여주기 위해)
        DetermineEnemyNextAction();
    }

    void DetermineEnemyNextAction()
    {
        actionEnemy = Random.Range(0, 10);

        if (enemy.type.Equals("E"))
        {
            if (actionEnemy <= 1) nextEnemyActionTxt.text = "마법공격";
            else if (actionEnemy == 7) nextEnemyActionTxt.text = $"방어도 {enemy.recovery} 회복";
            else if (actionEnemy == 8) nextEnemyActionTxt.text = $"체력 {enemy.heal} 회복";
            else nextEnemyActionTxt.text = "물리공격";
        }

        //마공 2회 (물공x, 체력 회복x)
        else if (enemy.type.Equals("D"))
        {
            if (actionEnemy <= 1) nextEnemyActionTxt.text = "마법공격 2회";
            else if (actionEnemy == 7) nextEnemyActionTxt.text = $"방어도 {enemy.recovery} 회복";
            //else if (actionEnemy == 8) nextEnemyActionTxt.text = $"체력 {enemy.heal} 회복";
            else nextEnemyActionTxt.text = "마법공격";
        }

        //마공 2회 (물공x, 방어도 회복x)
        else if (enemy.type.Equals("C"))
        {
            if (actionEnemy <= 1) nextEnemyActionTxt.text = "마법공격 2회";
            //else if (actionEnemy == 7) nextEnemyActionTxt.text = $"방어도 {enemy.recovery} 회복";
            else if (actionEnemy == 8) nextEnemyActionTxt.text = $"체력 {enemy.heal} 회복";
            else nextEnemyActionTxt.text = "마법공격";
        }

        //물공 2회 (마공x, 체력 회복x)
        else if (enemy.type.Equals("B"))
        {
            if (actionEnemy <= 1) nextEnemyActionTxt.text = "물리공격 2회";
            else if (actionEnemy == 7) nextEnemyActionTxt.text = $"방어도 {enemy.recovery} 회복";
            //else if (actionEnemy == 8) nextEnemyActionTxt.text = $"체력 {enemy.heal} 회복";
            else nextEnemyActionTxt.text = "물리공격";
        }

        //물공 2회 (마공x, 방어도 회복x)
        else if (enemy.type.Equals("A"))
        {
            if (actionEnemy <= 1) nextEnemyActionTxt.text = "물리공격 2회";
            //else if (actionEnemy == 7) nextEnemyActionTxt.text = $"방어도 {enemy.recovery} 회복";
            else if (actionEnemy == 8) nextEnemyActionTxt.text = $"체력 {enemy.heal} 회복";
            else nextEnemyActionTxt.text = "물리공격";
        }

    }

    void SpinSlotbySlotStop()
    {
        AudioManager.audioManager.PlaySFX("Button");

        // null 체크
        if (spawnedSlots == null || spawnedSlots.Length == 0) return;

        for (int i = 0; i < spawnedSlots.Length; i++)
        {
            //if (spawnedSlots[i] == null || spawnedSlots[i].spriteRenderer == null || spawnedSlots[i].spriteRenderer.sprite == null) continue;
            if (spawnedSlots[i] == null) continue;

            if (spawnedSlots[i].isSpinning)
            {
                // 회전 중인 슬롯이 있다면 멈춤
                if (spawnedSlots[i].isSpinning)
                {
                    string currentItemName = spawnedSlots[i].spriteRenderer.sprite.name;
                    spawnedSlots[i].isSpinning = false;
                    spawnedSlots[i].StopSpin();
                    items[i] = currentItemName;
                    // 마지막 슬롯까지 다 멈췄다면
                    if (i == spawnedSlots.Length - 1)
                    {
                        Debug.Log("전부 다 멈춤");
                        PlayerTurn();
                    }
                    break; // 한 번에 하나씩만 멈춤
                }
            }
        }
    }
    IEnumerator CoroutineSpinSlotbySlotStop()
    {
        // 키를 누르고 있는 동안 무한 반복
        while (true)
        {
            SpinSlotbySlotStop();

            if (IsAllSlotsStopped()) yield break;

            yield return new WaitForSeconds(0.3f);
        }
    }

    //IEnumerator CoroutineSpinSlotbySlotStopMO()
    //{
    //    while (isPointerDown) // 플래그를 조건으로 사용
    //    {
    //        SpinSlotbySlotStop();

    //        if (IsAllSlotsStopped())
    //        {
    //            // 모든 슬롯이 멈추면 즉시 루프 탈출
    //            isPointerDown = false;
    //            yield break;
    //        }

    //        yield return new WaitForSeconds(0.3f);
    //    }
    //}

    // 모든 슬롯이 멈췄는지 체크하는 함수
    bool IsAllSlotsStopped()
    {
        foreach (var slot in spawnedSlots)
        {
            if (slot.isSpinning) return false;
        }
        return true;
    }
    // 누르는 순간 호출
    public void OnStopBtnDown()
    {
        if (!playerSlotCheck) return;

        // 기존 코루틴이 돌고 있다면 중복 방지를 위해 정지
        if (spinStopCoroutine != null) StopCoroutine(spinStopCoroutine);
        spinStopCoroutine = StartCoroutine(CoroutineSpinSlotbySlotStop());
    }

    // 떼는 순간 호출
    public void OnStopBtnUp()
    {
        if (spinStopCoroutine != null)
        {
            StopCoroutine(spinStopCoroutine);
            spinStopCoroutine = null;
        }
    }
    void PlayerTurn()
    {
        if (!playerSlotCheck) return;

        playerSlotCheck = false; // 중복 실행 방지
        stopBtn.gameObject.SetActive(false);

        // 여기서 아이템 효과를 실행해야 확실하게 적 턴으로 넘어갑니다.
        StartCoroutine(ItemEffect(items));
    }

    // 애니메이션 효과 or 파티클 생성 + 데미지 계산
    IEnumerator ItemEffect(string[] items)
    {
        // 1. 초기화 로직 (딕셔너리 리셋 등 기존 코드 유지)
        List<string> keys = new List<string>(itemDict.Keys);
        foreach (string key in keys) itemDict[key] = 0;
        List<Item> matchedItems = new List<Item>();

        foreach (string name in items)
        {
            Item data = allItemDatas.Find(x => x.NAME == name);
            if (data != null) matchedItems.Add(data);
        }

        // 2. 아이템별 순차 처리
        string lastItem = null;

        int num = 0;
        string action = "";
        int energy1 = 0;
        int energy2 = 0;
        int energy3 = 0;
        int helmet = 0;
        int ring1 = 0;
        int ring2 = 0;
        int ring3 = 0;
        int goldIndex = 0;
        int stone = 0;

        // 슬롯 아이템에 따른 효과 적용
        if (matchedItems != null)
        {
            foreach (Item item in matchedItems)
            {
                // 콤보 계산 로직
                if (item.NAME == lastItem) itemDict[item.NAME]++;
                else itemDict[item.NAME] = 1;
                lastItem = item.NAME;

                // COUNT 설정 (1:일반, 2:치명, 3:메가)
                if (itemDict[item.NAME] < 3) item.COUNT = 1;
                else if (itemDict[item.NAME] < 5) item.COUNT = 2;
                else item.COUNT = 3;


                //if (itemDict[item.NAME] >= 0 &&
                //  itemDict[item.NAME] < 3)
                //{
                //    item.COUNT = 1;
                //}
                //else if (itemDict[item.NAME] >= 3 &&
                //         itemDict[item.NAME] < 5)
                //{
                //    item.COUNT = 2;
                //}
                //else if (itemDict[item.NAME] == 5)
                //{
                //    item.COUNT = 3;
                //}
                //else
                //{
                //    item.COUNT = 0;
                //}

                if (itemDict.TryGetValue(item.NAME, out int equalsCount))
                {
                    // 이펙트 생성 위치
                    Vector3 itemPos = Vector3.zero;

                    // 각 아이템에 맞는 애니메이션
                    if (item.NAME.Equals("사과"))
                    {
                        //itemPrefab = item.EFFECT;
                        // 이펙트 프리팹 생성 (Instantiate 코드 추가 필요)
                        if (item.EFFECT != null)
                        {
                            itemPrefab = item.EFFECT;
                        }

                        // 3. 연출을 위한 대기 시간 부여 (핵심!)
                        yield return new WaitForSeconds(0.8f);

                        itemPos = player.hpBar.transform.position;
                        float apple = item.ENHANCE_HP;

                        switch (item.COUNT)
                        {
                            // 일반
                            case 1:
                                action = $"체력 {apple} 회복";
                                break;

                            // 치명타
                            case 2:
                                apple *= 3;
                                action = $"치명타!\n체력 {apple} 회복";
                                break;
                            // 메가치명타
                            case 3:
                                apple *= 3;
                                action = $"메가치명타!\n체력 {apple} 회복";
                                break;
                        }

                        Apple(apple);
                        yield return new WaitForSeconds(0.5f);
                    }
                    if (item.NAME.Equals("포도"))
                    {
                        itemPos = player.hpBar.transform.position;

                        if (item.EFFECT != null)
                        {
                            itemPrefab = item.EFFECT;
                        }

                        float grape = item.ENHANCE_PLUS_HP;
                        yield return new WaitForSeconds(0.8f);

                        switch (item.COUNT)
                        {
                            // 일반
                            case 1:
                                action = $"최대 체력 {grape} 증가";
                                break;
                            case 2:
                                grape *= 3;
                                action = $"치명타!\n최대 체력 {grape} 증가";
                                break;
                            case 3:
                                grape *= 9;
                                action = $"메가치명타!\n최대 체력 {grape} 증가";
                                break;
                        }
                        Grape(grape);
                        yield return new WaitForSeconds(0.5f);
                    }

                    if (item.NAME.Equals("딸기"))
                    {
                        itemPos = player.hpBar.transform.position;

                        if (item.EFFECT != null)
                        {
                            itemPrefab = item.EFFECT;
                        }

                        float strowHp = item.ENHANCE_HP;
                        float strowMaxHp = item.ENHANCE_PLUS_HP;

                        yield return new WaitForSeconds(0.8f);

                        switch (item.COUNT)
                        {
                            // 일반
                            case 1:
                                action = $"체력 {strowHp} 회복\n최대 체력 {strowMaxHp} 증가";
                                break;
                            case 2:
                                strowHp *= 3;
                                strowMaxHp *= 3;
                                action = $"치명타!\n체력 {strowHp} 회복\n최대 체력 {strowMaxHp} 증가";
                                break;
                            case 3:
                                strowHp *= 9;
                                strowMaxHp *= 9;
                                action = $"메가치명타!\n체력 {strowHp} 회복\n최대 체력 {strowMaxHp} 증가";
                                break;
                        }
                        Grape(strowMaxHp);
                        Apple(strowHp);
                        yield return new WaitForSeconds(0.5f);
                    }

                    if (item.NAME.Equals("고기"))
                    {
                        itemPos = player.hpBar.transform.position;

                        if (item.EFFECT != null)
                        {
                            itemPrefab = item.EFFECT;
                        }

                        yield return new WaitForSeconds(0.8f);

                        float hp = item.ENHANCE_HP;
                        float shild = item.ENHANCE_SHILD;

                        switch (item.COUNT)
                        {
                            // 일반
                            case 1:
                                action = $"체력 {hp} 회복\n방어도 {shild} 회복";
                                break;
                            case 2:
                                hp *= 3;
                                shild *= 3;
                                action = $"치명타!\n체력 {hp} 회복\n방어도 {shild} 회복";
                                break;
                            case 3:
                                hp *= 9;
                                shild *= 9;
                                action = $"메가치명타!\n체력 {hp} 회복\n방어도 {shild} 회복";
                                break;
                        }
                        Meat(hp, shild);
                        yield return new WaitForSeconds(0.5f);
                    }

                    if (item.NAME.Equals("에너지") || item.NAME.Equals("특수에너지"))
                    {
                        for (int i = 0; i < matchedItems.Count; i++)
                        {
                            if (item == matchedItems[i])
                            {
                                int slotIndex = energy1 % spawnedSlots.Length;
                                // 슬롯의 위치에 이펙트 인덱스 설정
                                itemPos = spawnedSlots[slotIndex].transform.position;
                                if (item.EFFECT != null)
                                {
                                    itemPrefab = item.EFFECT;
                                }

                                yield return new WaitForSeconds(0.8f);
                                break;
                            }
                        }

                        float plus_att1 = item.ENHANCE_PLUSATK;
                        float plus_att2 = item.ENHANCE_PLUSMATK;

                        switch (item.COUNT)
                        {
                            // 일반
                            case 1:
                                action = $"물공 {plus_att1}\n마공 {plus_att2} 증가";
                                break;

                            case 2:
                                plus_att1 *= 3;
                                plus_att2 *= 3;
                                action = $"치명타!\n물공 {plus_att1}\n마공 {plus_att2} 증가";
                                break;

                            case 3:
                                plus_att1 *= 9;
                                plus_att2 *= 9;
                                action = $"메가치명타!\n물공 {plus_att1}\n마공 {plus_att2} 증가";
                                break;
                        }

                        Energy(plus_att1, plus_att2);
                        player.UpdateEnhanceUI();
                        yield return new WaitForSeconds(0.5f);
                    }

                    if (item.NAME.Equals("물리에너지") || item.NAME.Equals("물리에너지(대)"))
                    {
                        for (int i = 0; i < matchedItems.Count; i++)
                        {
                            if (item == matchedItems[i])
                            {
                                int slotIndex = energy2 % spawnedSlots.Length;
                                // 슬롯의 위치에 이펙트 인덱스 설정
                                itemPos = spawnedSlots[slotIndex].transform.position;
                                if (item.EFFECT != null)
                                {
                                    itemPrefab = item.EFFECT;
                                }

                                yield return new WaitForSeconds(0.8f);
                                break;
                            }
                        }

                        float plus_att1 = item.ENHANCE_PLUSATK;

                        switch (item.COUNT)
                        {
                            // 일반
                            case 1:
                                action = $"물공 {plus_att1} 증가";
                                break;

                            case 2:
                                plus_att1 *= 3;
                                action = $"치명타!\n물공 {plus_att1} 증가";
                                break;

                            case 3:
                                plus_att1 *= 9;
                                action = $"메가치명타!\n물공 {plus_att1} 증가";
                                break;
                        }

                        Energy(plus_att1, 0);
                        player.UpdateEnhanceUI();
                        yield return new WaitForSeconds(0.5f);
                    }

                    if (item.NAME.Equals("마법에너지") || item.NAME.Equals("마법에너지(대)"))
                    {
                        for (int i = 0; i < matchedItems.Count; i++)
                        {
                            if (item == matchedItems[i])
                            {
                                int slotIndex = energy3 % spawnedSlots.Length;
                                // 슬롯의 위치에 이펙트 인덱스 설정
                                itemPos = spawnedSlots[slotIndex].transform.position;
                                if (item.EFFECT != null)
                                {
                                    itemPrefab = item.EFFECT;
                                }

                                yield return new WaitForSeconds(0.8f);
                                break;
                            }
                        }

                        //print($"{item.NAME} 의 갯수 ?: {item.COUNT}");
                        float plus_att2 = item.ENHANCE_PLUSMATK;

                        switch (item.COUNT)
                        {
                            // 일반
                            case 1:
                                action = $"마공 {plus_att2} 증가";
                                break;

                            case 2:
                                plus_att2 *= 3;
                                action = $"치명타!\n마공 {plus_att2} 증가";
                                break;

                            case 3:
                                plus_att2 *= 9;
                                action = $"메가치명타!\n마공 {plus_att2} 증가";
                                break;
                        }

                        Energy(0, plus_att2);
                        player.UpdateEnhanceUI();
                        yield return new WaitForSeconds(0.5f);
                    }

                    if (item.NAME.Equals("독약"))
                    {
                        itemPos = Vector3.up;

                        if (item.EFFECT != null)
                        {
                            itemPrefab = item.EFFECT;
                        }

                        yield return new WaitForSeconds(0.8f);

                        float poison = item.ENHANCE_POISON;

                        //print("독 데미지 12");
                        switch (item.COUNT)
                        {
                            //일반
                            case 1:
                                action = $"독 중독 {poison}";
                                break;
                            case 2:
                                poison *= 3;
                                action = $"치명타!\n독 중독 {poison}";
                                break;
                            case 3:
                                poison *= 9;
                                action = $"메가치명타!\n독 중독 {poison}";
                                break;
                        }

                        player.poison += poison;
                        player.UpdatePosionUI();
                        yield return new WaitForSeconds(0.5f);
                    }

                    if (item.NAME.Equals("독검"))
                    {
                        itemPos = Vector3.up;

                        if (item.EFFECT != null)
                        {
                            itemPrefab = item.EFFECT;
                        }

                        yield return new WaitForSeconds(0.8f);

                        //물공 10~25
                        //독 중독5
                        float att = Random.Range(item.ENHANCE_MINATK, item.ENHANCE_ATK) + player.att1;

                        if (player.storeAtkMultiplier > 1f)
                        {
                            att *= player.storeAtkMultiplier;
                        }

                        float poison = item.ENHANCE_POISON;

                        switch (item.COUNT)
                        {
                            //일반
                            case 1:
                                action = $"물공 {att.ToIntString()}\n독 중독 {poison}";
                                break;
                            case 2:
                                att *= 3;
                                poison *= 3;
                                action = $"치명타!\n물공 {att.ToIntString()}\n독 중독 {poison}";
                                break;
                            case 3:
                                att *= 9;
                                poison *= 9;
                                action = $"메가치명타!\n물공 {att.ToIntString()}\n독 중독 {poison}";
                                break;
                        }
                        //물리데미지 적용
                        ApplyPhysicalDamageToEnemy(att);

                        player.poison += poison;
                        player.UpdatePosionUI();

                        //enemy.UpdateHpShildSet();
                        yield return new WaitForSeconds(0.5f);
                    }

                    if (item.NAME.Equals("마법검"))
                    {
                        //print("마법 공격 30");
                        float att = item.ENHANCE_MATK + player.att2;

                        if (player.storeMatkMultiplier > 1f)
                        {
                            att *= player.storeMatkMultiplier;
                        }

                        switch (item.COUNT)
                        {
                            case 1:
                                action = $"마공 {att.ToIntString()}";
                                break;
                            case 2:
                                att *= 3;
                                action = $"치명타!\n마공 {att.ToIntString()}";
                                break;
                            case 3:
                                att *= 9;
                                action = $"메가치명타!\n마공 {att.ToIntString()}";
                                break;
                        }

                        itemPos = Vector3.up;

                        if (item.EFFECT != null)
                        {
                            itemPrefab = item.EFFECT;
                        }

                        yield return new WaitForSeconds(0.8f);

                        ApplyMagicDamageToEnemy(att);
                        //enemy.hp -= att;
                        //scoreManager.AddScore((int)att);

                        //enemy.UpdateHpShildSet();
                        yield return new WaitForSeconds(0.5f);
                    }

                    if (item.NAME.Equals("해골도끼"))
                    {
                        //print("공격 20 공격 20");
                        float att1 = Random.Range(item.ENHANCE_MINATK, item.ENHANCE_ATK) + player.att1;

                        if (player.storeAtkMultiplier > 1f)
                        {
                            att1 *= player.storeAtkMultiplier;
                        }

                        float att2 = Random.Range(item.ENHANCE_MINMATK, item.ENHANCE_MATK) + player.att2;

                        if (player.storeMatkMultiplier > 1f)
                        {
                            att2 *= player.storeMatkMultiplier;
                        }

                        float blood = item.ENHANCE_BLOOD;
                        float r = (Random.Range(0f, 1f));

                        switch (item.COUNT)
                        {
                            case 1:
                                action = $"물공 {att1.ToIntString()} , 마공 {att2.ToIntString()}\n흡혈 {blood}";
                                break;
                            case 2:
                                att1 *= 3;
                                att2 *= 3;
                                blood *= 3;
                                action = $"치명타!\n물공 {att1.ToIntString()} , 마공 {att2.ToIntString()}\n흡혈 {blood}";
                                r = Random.Range(0.35f, 1);
                                break;
                            case 3:
                                att1 *= 9;
                                att2 *= 9;
                                blood *= 9;
                                action = $"메가치명타!\n물공 {att1.ToIntString()} , 마공 {att2.ToIntString()}\n흡혈 {blood}";
                                r = 1;
                                break;
                        }

                        //이미 스턴 상태이면 해제되지 않도록
                        if (!stuned3)
                        {
                            //print($"스턴 상태 정상동작? {r}");
                            // 1 - 스턴확률(0.2라면 0.8)보다 r이 크면 성공
                            if (r > (1f - item.ENHANCE_STUNED))
                            {
                                stuned3 = true;
                                nextEnemyActionTxt.text = "<color=yellow>기절상태</color>";
                            }
                        }

                        if (item.EFFECT != null)
                        {
                            itemPrefab = item.EFFECT;
                        }

                        yield return new WaitForSeconds(0.8f);

                        //흡혈데미지 적용
                        Blood(blood);

                        //물리데미지 적용
                        ApplyPhysicalDamageToEnemy(att1);

                        //마법데미지 적용
                        //enemy.hp -= att2;
                        //scoreManager.AddScore((int)att2);
                        ApplyMagicDamageToEnemy(att2);

                        //enemy.UpdateHpShildSet();
                        yield return new WaitForSeconds(0.5f);
                    }

                    if (item.NAME.Equals("마법봉"))
                    {
                        //print("공격 10");
                        float att = item.ENHANCE_MATK + player.att2;

                        if (player.storeMatkMultiplier > 1f)
                        {
                            att *= player.storeMatkMultiplier;
                        }

                        switch (item.COUNT)
                        {
                            case 1:
                                action = $"마공 {att.ToIntString()}";
                                break;
                            case 2:
                                att *= 3;
                                action = $"치명타!\n마공 {att.ToIntString()}";
                                break;
                            case 3:
                                att *= 9;
                                action = $"{item.NAME} 메가치명타\n마공 {att.ToIntString()}";
                                break;
                        }

                        itemPos = Vector3.up;

                        if (item.EFFECT != null)
                        {
                            itemPrefab = item.EFFECT;
                        }

                        yield return new WaitForSeconds(0.8f);

                        //마법데미지
                        ApplyMagicDamageToEnemy(att);
                        //enemy.hp -= att;
                        //scoreManager.AddScore((int)att);
                        //enemy.UpdateHpShildSet();
                        yield return new WaitForSeconds(0.5f);
                    }
                    if (item.NAME.Equals("일반검"))
                    {
                        //print("물공 10");
                        float att = item.ENHANCE_ATK + player.att1;

                        if (player.storeAtkMultiplier > 1f)
                        {
                            att *= player.storeAtkMultiplier;
                        }

                        switch (item.COUNT)
                        {
                            case 1:
                                action = $"물공 {att.ToIntString()}";
                                break;
                            case 2:
                                att *= 3;
                                action = $"치명타!\n물공 {att.ToIntString()}";
                                break;
                            case 3:
                                att *= 9;
                                action = $"메가치명타!\n물공 {att.ToIntString()}";
                                break;
                        }

                        itemPos = Vector3.up;

                        if (item.EFFECT != null)
                        {
                            itemPrefab = item.EFFECT;
                        }

                        yield return new WaitForSeconds(0.8f);

                        ApplyPhysicalDamageToEnemy(att);

                        //enemy.UpdateHpShildSet();
                        yield return new WaitForSeconds(0.5f);
                    }
                    if (item.NAME.Equals("일반도끼"))
                    {
                        //print("공격20 물리");
                        float att = Random.Range(item.ENHANCE_MINATK, item.ENHANCE_ATK) + player.att1;

                        if (player.storeAtkMultiplier > 1f)
                        {
                            att *= player.storeAtkMultiplier;
                        }

                        switch (item.COUNT)
                        {
                            case 1:
                                action = $"물공 {att.ToIntString()}";
                                break;
                            case 2:
                                att *= 3;
                                action = $"치명타!\n물공 {att.ToIntString()}";
                                break;
                            case 3:
                                att *= 9;
                                action = $"메가치명타!\n물공 {att.ToIntString()}";
                                break;
                        }

                        itemPos = Vector3.up;

                        if (item.EFFECT != null)
                        {
                            itemPrefab = item.EFFECT;
                        }

                        yield return new WaitForSeconds(0.8f);

                        ApplyPhysicalDamageToEnemy(att);

                        //enemy.UpdateHpShildSet();
                        yield return new WaitForSeconds(0.5f);
                    }
                    if (item.NAME.Equals("대검"))
                    {
                        //print("공격30 물리");
                        float att = item.ENHANCE_ATK + player.att1;

                        if (player.storeAtkMultiplier > 1f)
                        {
                            att *= player.storeAtkMultiplier;
                        }

                        float r = (Random.Range(0f, 1f));

                        switch (item.COUNT)
                        {
                            case 1:
                                action = $"물공 {att.ToIntString()}";
                                break;
                            case 2:
                                att *= 3;
                                action = $"치명타!\n물공 {att.ToIntString()}";
                                r = Random.Range(0.35f, 1);
                                break;
                            case 3:
                                att *= 9;
                                action = $"메가치명타!\n물공 {att.ToIntString()}";
                                r = 1;
                                break;
                        }

                        //print($"스턴체크 + {r > (1 - item.ENHANCE_STUNED)}");

                        //이미 스턴 상태이면 해제되지 않도록
                        if (!stuned1)
                        {
                            //print($"스턴 상태 정상동작? {r}");
                            // 1 - 스턴확률(0.2라면 0.8)보다 r이 크면 성공
                            if (r > (1f - item.ENHANCE_STUNED))
                            {
                                stuned1 = true;
                                nextEnemyActionTxt.text = "<color=yellow>기절상태</color>";
                            }
                        }


                        itemPos = Vector3.up;

                        if (item.EFFECT != null)
                        {
                            itemPrefab = item.EFFECT;
                        }

                        yield return new WaitForSeconds(0.8f);

                        //물리데미지
                        ApplyPhysicalDamageToEnemy(att);

                        //enemy.UpdateHpShildSet();
                        yield return new WaitForSeconds(0.5f);
                    }
                    if (item.NAME.Equals("고급도끼"))
                    {
                        //print("공격40 물리");
                        float att = Random.Range(item.ENHANCE_MINATK, item.ENHANCE_ATK) + player.att1;

                        if (player.storeAtkMultiplier > 1f)
                        {
                            att *= player.storeAtkMultiplier;
                        }

                        float r = (Random.Range(0f, 1f));

                        switch (item.COUNT)
                        {
                            case 1:
                                action = $"물공 {att.ToIntString()}";
                                break;
                            case 2:
                                att *= 3;
                                action = $"치명타!\n물공 {att.ToIntString()}";
                                r = Random.Range(0.35f, 1);
                                break;
                            case 3:
                                att *= 9;
                                action = $"메가치명타!\n물공 {att.ToIntString()}";
                                r = 1;
                                break;
                        }

                        //print($"스턴체크 + {r > (1 - item.ENHANCE_STUNED)}");

                        // 스턴 체크: 이미 스턴이 걸려있지 않을 때만 확률 검사
                        if (!stuned2)
                        {
                            print($"스턴 상태 정상동작? {r}");
                            if (r > (1f - item.ENHANCE_STUNED))
                            {
                                stuned2 = true;
                                nextEnemyActionTxt.text = "<color=yellow>기절상태</color>";
                            }
                        }

                        itemPos = Vector3.up;

                        if (item.EFFECT != null)
                        {
                            itemPrefab = item.EFFECT;
                        }

                        yield return new WaitForSeconds(0.8f);

                        ApplyPhysicalDamageToEnemy(att);

                        //enemy.UpdateHpShildSet();
                        yield return new WaitForSeconds(0.5f);
                    }

                    if (item.NAME.Equals("천둥망치"))
                    {
                        //마공 30~100
                        float r = (Random.Range(0f, 1f));
                        float att = Random.Range(item.ENHANCE_MINMATK, item.ENHANCE_MATK) + player.att2;

                        if (player.storeMatkMultiplier > 1f)
                        {
                            att *= player.storeMatkMultiplier;
                        }

                        switch (item.COUNT)
                        {
                            case 1:
                                action = $"마공 {att.ToIntString()}";
                                break;
                            case 2:
                                att *= 3;
                                action = $"치명타!\n마공 {att.ToIntString()}";
                                r = Random.Range(0.35f, 1);
                                break;
                            case 3:
                                att *= 9;
                                action = $"메가치명타!\n마공 {att.ToIntString()}";
                                r = 1;
                                break;
                        }

                        itemPos = Vector3.up;

                        if (!stuned4)
                        {
                            print($"스턴 상태 정상동작? {r}");
                            if (r > (1f - item.ENHANCE_STUNED))
                            {
                                stuned4 = true;
                                nextEnemyActionTxt.text = "<color=yellow>기절상태</color>";
                            }
                        }

                        if (item.EFFECT != null)
                        {
                            itemPrefab = item.EFFECT;
                        }

                        yield return new WaitForSeconds(0.8f);

                        //마법데미지
                        //enemy.hp -= att;
                        //scoreManager.AddScore((int)att);
                        //enemy.UpdateHpShildSet();

                        ApplyMagicDamageToEnemy(att);
                        yield return new WaitForSeconds(0.5f);
                    }

                    if (item.NAME.Equals("화염방패"))
                    {
                        float shild = item.ENHANCE_SHILD;
                        float plus_sh = item.ENHANCE_PLUS_SHILD;
                        switch (item.COUNT)
                        {
                            case 1:
                                action = $"방어도 {shild} 회복\n최대 방어도 {plus_sh}";
                                break;
                            case 2:
                                shild *= 3;
                                plus_sh *= 3;
                                action = $"치명타!\n방어도 {shild} 회복\n최대 방어도 {plus_sh}";
                                break;
                            case 3:
                                shild *= 9;
                                plus_sh *= 9;
                                action = $"메가치명타!\n방어도 {shild} 회복\n최대 방어도 {plus_sh}";
                                break;
                        }

                        itemPos = player.shildBar.transform.position;

                        if (item.EFFECT != null)
                        {
                            itemPrefab = item.EFFECT;
                        }

                        Shield(shild, plus_sh);
                        yield return new WaitForSeconds(0.8f);

                    }

                    if (item.NAME.Equals("해골방패"))
                    {
                        float shild = item.ENHANCE_SHILD;
                        float shildMax = item.ENHANCE_PLUS_SHILD;
                        switch (item.COUNT)
                        {
                            case 1:
                                action = $"방어도 {shild} 회복\n방어도 {shildMax} 증가";
                                break;
                            case 2:
                                shild *= 3;
                                shildMax *= 3;
                                action = $"치명타!\n방어도 {shild} 회복\n방어도 {shildMax} 증가";
                                break;
                            case 3:
                                shild *= 9;
                                shildMax *= 9;
                                action = $"메가치명타!\n방어도 {shild} 회복\n방어도 {shildMax} 증가";
                                break;
                        }
                        itemPos = player.shildBar.transform.position;

                        if (item.EFFECT != null)
                        {
                            itemPrefab = item.EFFECT;
                        }

                        Shield(shild, shildMax);

                        yield return new WaitForSeconds(0.8f);

                    }

                    if (item.NAME.Equals("골드"))
                    {
                        for (int i = 0; i < matchedItems.Count; i++)
                        {
                            if (item == matchedItems[i])
                            {
                                int slotIndex = goldIndex % spawnedSlots.Length;
                                // 슬롯의 위치에 이펙트 인덱스 설정
                                itemPos = spawnedSlots[slotIndex].transform.position;

                                if (item.EFFECT != null)
                                {
                                    itemPrefab = item.EFFECT;
                                }

                                yield return new WaitForSeconds(0.8f);
                                break;
                            }
                        }

                        int gold = item.ENHABCE_GOLD;
                        switch (item.COUNT)
                        {
                            case 1:
                                action = $"{gold}골드 획득";
                                break;
                            case 2:
                                gold *= 3;
                                action = $"치명타!\n{gold}골드 획득";
                                break;
                            case 3:
                                gold *= 9;
                                action = $"메가치명타!\n{gold}골드 획득";
                                break;
                        }


                        //골드 획득 시 프리팹 추가
                        GameObject goldEffect = Instantiate(goldPrefab, goldParent.transform);
                        SpriteRenderer goldEffectRander = goldEffect.GetComponent<SpriteRenderer>();
                        int r1 = Random.Range(-100, 130);
                        int r2 = Random.Range(-100, 130);
                        float ranX = Mathf.Clamp(r1, -100, 100);
                        float ranY = Mathf.Clamp(r2, -30, 120);
                        goldPrefab.transform.position = new Vector3(ranX, ranY, 0);
                        if (goldEffectRander != null)
                        {
                            goldEffectRander.sortingOrder = 50;
                        }

                        itemManager.PlusGold(gold);
                        player.UpdateGoldUI();
                        yield return new WaitForSeconds(0.5f);
                    }

                    if (item.NAME.Equals("원석"))
                    {
                        for (int i = 0; i < matchedItems.Count; i++)
                        {
                            if (item == matchedItems[i])
                            {
                                int slotIndex = stone % spawnedSlots.Length;
                                // 슬롯의 위치에 이펙트 인덱스 설정
                                itemPos = spawnedSlots[slotIndex].transform.position;

                                if (item.EFFECT != null)
                                {
                                    itemPrefab = item.EFFECT;
                                }

                                yield return new WaitForSeconds(0.8f);
                                break;
                            }
                        }

                        int gold = item.ENHABCE_GOLD;
                        switch (item.COUNT)
                        {
                            case 1:
                                action = $"{gold}골드 획득";
                                break;
                            case 2:
                                gold *= 3;
                                action = $"치명타!\n{gold}골드 획득";
                                break;
                            case 3:
                                gold *= 9;
                                action = $"메가치명타!\n{gold}골드 획득";
                                break;
                        }


                        //골드 획득 시 프리팹 추가
                        //x(-100 , 100) y(-30, 120)
                        GameObject goldEffect = Instantiate(goldPrefab, goldParent.transform);
                        int r1 = Random.Range(-100, 130);
                        int r2 = Random.Range(-100, 130);
                        float ranX = Mathf.Clamp(r1, -100, 100);
                        float ranY = Mathf.Clamp(r2, -30, 120);
                        goldPrefab.transform.position = new Vector3(ranX, ranY, 0);
                        SpriteRenderer goldEffectRander = goldEffect.GetComponent<SpriteRenderer>();
                        if (goldEffectRander != null)
                        {
                            goldEffectRander.sortingOrder = 50;
                        }

                        itemManager.PlusGold(gold);
                        player.UpdateGoldUI();
                        yield return new WaitForSeconds(0.5f);
                    }

                    if (item.NAME.Equals("마법투구") || item.NAME.Equals("마법반지"))
                    {
                        //증가치
                        float att1 = item.ENHANCE_PLUSATK;
                        float att2 = item.ENHANCE_PLUSMATK;

                        float hp = item.ENHANCE_HP;
                        float plus_hp = item.ENHANCE_PLUS_HP;
                        float shild = item.ENHANCE_SHILD;
                        float plus_sh = item.ENHANCE_PLUS_SHILD;

                        for (int i = 0; i < matchedItems.Count; i++)
                        {
                            if (item == matchedItems[i])
                            {
                                int slotIndex = helmet % spawnedSlots.Length;
                                //// 슬롯의 위치에 이펙트 인덱스 설정
                                //itemPos = spawnedSlots[slotIndex].transform.position;
                                // 1. 기본 슬롯 위치 가져오기
                                Vector3 slotPos = spawnedSlots[slotIndex].transform.position;

                                // 2. 변경점 적용: Y축 -3.3, Z축은 0 (혹은 원하는 절대값)으로 설정
                                itemPos = new Vector3(slotPos.x, -3.3f, 0f);

                                if (item.EFFECT != null)
                                {
                                    itemPrefab = item.EFFECT;
                                }

                                yield return new WaitForSeconds(0.8f);
                                break;
                            }
                        }

                        switch (item.COUNT)
                        {
                            case 1:
                                action = $"물공 {att1},마공 {att2} 증가\n" +
                                         $"최대체력{plus_hp}, 최대방어도{plus_sh}증가\n" +
                                         $"체력{hp}, 방어도{shild} 회복";
                                break;
                            case 2:
                                att1 *= 3;
                                att2 *= 3;
                                hp *= 3;
                                plus_hp *= 3;
                                shild *= 3;
                                plus_sh *= 3;

                                action = $"치명타!\n" +
                                         $"물공 {att1},마공 {att2} 증가\n" +
                                         $"최대체력{plus_hp}, 최대방어도{plus_sh}증가\n" +
                                         $"체력{hp}, 방어도{shild} 회복";
                                break;
                            case 3:
                                att1 *= 9;
                                att2 *= 9;
                                hp *= 9;
                                plus_hp *= 9;
                                shild *= 9;
                                plus_sh *= 9;
                                action = $"메가치명타!\n" +
                                         $"물공 {att1},마공 {att2} 증가\n" +
                                         $"최대체력{plus_hp}, 최대방어도{plus_sh}증가\n" +
                                         $"체력{hp}, 방어도{shild} 회복";
                                break;
                        }

                        Magic(att1, att2, hp, plus_hp, shild, plus_sh);

                        player.UpdateEnhanceUI();

                        yield return new WaitForSeconds(0.5f);
                    }

                    if (item.NAME.Equals("흡혈반지"))
                    {
                        //증가치
                        float shild = item.ENHANCE_SHILD;
                        float plus_sh = item.ENHANCE_PLUS_SHILD;

                        //흡혈
                        float blood = item.ENHANCE_BLOOD;

                        for (int i = 0; i < matchedItems.Count; i++)
                        {
                            if (item == matchedItems[i])
                            {
                                int slotIndex = ring2 % spawnedSlots.Length;
                                //itemPos = spawnedSlots[slotIndex].transform.position;
                                // 1. 기본 슬롯 위치 가져오기
                                Vector3 slotPos = spawnedSlots[slotIndex].transform.position;

                                // 2. 변경점 적용: Y축 -3.3, Z축은 0 (혹은 원하는 절대값)으로 설정
                                itemPos = new Vector3(slotPos.x, -3.3f, 0f);

                                if (item.EFFECT != null)
                                {
                                    itemPrefab = item.EFFECT;
                                }

                                yield return new WaitForSeconds(0.8f);
                                break;
                            }
                        }

                        switch (item.COUNT)
                        {
                            case 1:
                                action = $"최대방어도{plus_sh} 증가, 방어도{shild} 회복\n" +
                                         $"흡혈 {blood}";
                                break;
                            case 2:
                                shild *= 3;
                                plus_sh *= 3;
                                blood *= 3;

                                action = $"치명타!\n" +
                                         $"최대방어도{plus_sh} 증가, 방어도{shild} 회복\n" +
                                         $"흡혈 {blood}";
                                break;
                            case 3:
                                shild *= 9;
                                plus_sh *= 9;
                                blood *= 9;

                                action = $"메가치명타!\n" +
                                         $"최대방어도{plus_sh} 증가, 방어도{shild} 회복\n" +
                                         $"흡혈 {blood}";
                                break;

                        }

                        Shield(shild, plus_sh);
                        Blood(blood);

                        enemy.UpdateHpShildSet();
                        yield return new WaitForSeconds(0.5f);
                    }

                    if (item.NAME.Equals("독반지"))
                    {
                        //증가치
                        float hp = item.ENHANCE_HP;
                        float plus_hp = item.ENHANCE_PLUS_HP;

                        //독
                        float poison = item.ENHANCE_POISON;

                        for (int i = 0; i < matchedItems.Count; i++)
                        {
                            if (item == matchedItems[i])
                            {
                                int slotIndex = ring3 % spawnedSlots.Length;
                                //// 슬롯의 위치에 이펙트 인덱스 설정
                                //itemPos = spawnedSlots[slotIndex].transform.position;
                                // 1. 기본 슬롯 위치 가져오기
                                Vector3 slotPos = spawnedSlots[slotIndex].transform.position;

                                // 2. 변경점 적용: Y축 -3.3, Z축은 0 (혹은 원하는 절대값)으로 설정
                                itemPos = new Vector3(slotPos.x, -3.3f, 0f);

                                if (item.EFFECT != null)
                                {
                                    itemPrefab = item.EFFECT;
                                }

                                yield return new WaitForSeconds(0.8f);
                                break;
                            }
                        }

                        switch (item.COUNT)
                        {
                            case 1:
                                action = $"최대체력{plus_hp} 증가, 체력{hp} 회복\n" +
                                         $"독 중독 {poison}";
                                break;
                            case 2:
                                hp *= 3;
                                plus_hp *= 3;
                                poison *= 3;

                                action = $"치명타!\n" +
                                         $"최대체력{plus_hp} 증가, 체력{hp} 회복\n" +
                                         $"독 중독 {poison}";

                                break;
                            case 3:
                                hp *= 9;
                                plus_hp *= 9;
                                poison *= 9;

                                action = $"메가치명타!\n" +
                                         $"최대체력{plus_hp} 증가, 체력{hp} 회복\n" +
                                         $"독 중독 {poison}";

                                break;
                        }

                        Grape(plus_hp);
                        Apple(hp);

                        //독 데미지 부여 (매 턴마다 적에게 데미지를 입힌다)
                        player.poison += (poison);
                        player.UpdatePosionUI();
                        yield return new WaitForSeconds(0.5f);
                    }

                    //itemDict[item.NAME] = 1;
                    energy1++;
                    energy2++;
                    energy3++;
                    helmet++;
                    ring1++;
                    ring2++;
                    ring3++;
                    goldIndex++;
                    stone++;
                    //print($"{item.NAME} 확인");

                    currentEffects[num] = Instantiate(itemPrefab);
                    currentEffects[num].transform.position = itemPos;
                    //itemArray[num].GetComponent<SpriteRenderer>().sortingOrder = 11;

                    //ScoreUI업데이트
                    scoreTxt.text = scoreManager.score.ToString();

                    ////적 앞에 소환
                    //ParticleSystemRenderer effectRender = currentEffects[num].GetComponent<ParticleSystemRenderer>();
                    SpriteRenderer goldEffectRederer = currentEffects[num].GetComponent<SpriteRenderer>();
                    //if (effectRender != null)
                    //{
                    //    effectRender.sortingOrder = 500;
                    //}
                    if (goldEffectRederer != null)
                    {
                        goldEffectRederer.sortingOrder = 500;
                    }

                    SortingGroup group = currentEffects[num].GetComponent<SortingGroup>();
                    if (group != null)
                    {
                        group.sortingOrder = 500;
                    }

                    if (num < currentEffects.Length)
                    {
                        num++;
                        Status($"{item.NAME}\n{action}");
                    }

                    yield return new WaitForSeconds(1.5f);
                    //아이템 오브젝트 파괴
                    for (int i = 0; i < currentEffects.Length; i++)
                    {
                        if (currentEffects[i] != null)
                        {
                            Destroy(currentEffects[i]);
                        }
                    }
                    if (enemy.hp <= 0)
                    {
                        //적사망(승리)
                        StartCoroutine(EnemyDeath());
                        yield break;
                    }
                    //아이템이 한바퀴 돌았을 때
                    if (num == currentEffects.Length)
                    {
                        yield return new WaitForSeconds(0.5f);
                        statusTxt.text = ""; // 상태창 비우기
                        playerTurn = false;
                        enemyTurn = true;
                        isEnemyturnning = false;

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
            string enemyStatus = $"<color=white>적 체력 : {(int)enemy.hp} \n적 방어도 : {(int)enemy.shild}";
            statusTxt.color = Color.green;
            statusTxt.text = $"{action}\n\n{enemyStatus}";
        }

        else
        {
            nextEnemyActionTxt.text = "";
            statusTxt.color = Color.red;
            statusTxt.text = $"{action}";
        }
    }

    //적 캐릭터 생성
    //void EnemyCreate(int r)
    //{
    //    if (GameObject.FindWithTag("Enemy") == null) return;
    //    if (r < 0 || r > enemyObjects.Length)
    //    {
    //        print("enemy 생성못함");
    //        return;
    //    }

    //    Transform enemyPos = GameObject.FindWithTag("Enemy").transform;
    //    GameObject newObj = Instantiate(enemyObjects[r], enemyPos);

    //    enemy = newObj.GetComponent<Enemy>();

    //    newObj.transform.position = Vector3.zero;
    //    if (r == 1 || r == 2) { newObj.transform.position = Vector3.down; }
    //    newObj.transform.localScale = new Vector3(-1, 1, 1);
    //    print("적 생성 완료");
    //}

    void StartEnemyTurn()
    {
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        //isEnemyturnning = true;
        //EnemyAction currentAction = enemy.GetNextAction();

        //statusTxt.text = $"적의 {currentAction.actionName}!";

        //// 공격 로직
        //if (currentAction.damageMultiplier > 0)
        //{
        //    AttDamage(enemy.att1 * currentAction.damageMultiplier, currentAction.isMagic);
        //}

        //// 방어 로직
        //if (currentAction.shieldAmount > 0)
        //{
        //    enemy.shild += currentAction.shieldAmount;
        //    enemy.UpdateHpShildSet();
        //}

        isEnemyturnning = true;
        enemyTurn = false;
        statusTxt.text = "";

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(EnemyTypeAction());

        yield return new WaitForSeconds(1.5f);

        //플레이어의 독데미지 적용 시점
        if (player.poison > 0)
        {
            yield return new WaitForSeconds(1.8f);

            GameObject enemyEffect = Instantiate(enemyManager.enemyEffets[2]);
            enemyEffect.transform.position = enemy.transform.position;
            enemy.hp -= player.poison;
            scoreManager.AddScore((int)player.poison);
            scoreTxt.text = scoreManager.score.ToString();

            Status($"<color=yellow> 독 피해 : {player.poison}");
            player.poison -= 2f;
            if (player.poison <= 0)
            {
                player.poison = 0;
            }
            enemy.AnimDamage();

            enemy.UpdateHpShildSet();
            player.UpdatePosionUI();

            yield return new WaitForSeconds(1.5f);
            Destroy(enemyEffect);

            if (enemy.hp <= 0)
            {
                //적사망(승리)
                StartCoroutine(EnemyDeath());
                yield break;
            }
        }

        //stopBtn.gameObject.SetActive(true);
        Status(" ");

        //슬롯 재시작
        SpinStart();

        yield return new WaitForSeconds(1.0f);

        // 턴 종료 처리
        enemyTurn = false;
        isEnemyturnning = false;

        // 다시 플레이어 턴 시작 (여기서 새로운 행동이 결정됨)
        StartPlayerTurn();
    }

    IEnumerator EnemyTypeAction()
    {
        print($" 적타입 : {enemy.type} , 적행동 {actionEnemy}");

        if (enemy.type.Equals("E"))
        {
            if (stuned1 || stuned2 || stuned3 || stuned4)
            {
                Status("기절!");
                enemy.Stuned();
                yield return new WaitForSeconds(1.5f);

                stuned1 = false;
                stuned2 = false;
                stuned3 = false;
                stuned4 = false;

                yield return null;
            }
            else
            {
                if (actionEnemy <= 1)
                {
                    //특수공격
                    EnemySpecialAttack();
                }
                //방어도 회복(7)
                else if (actionEnemy == 7)
                {
                    EnemyShildRecover();
                }
                //체력 회복(8)
                else if (actionEnemy == 8)
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
                else
                {
                    //일반공격
                    EnemyAttack();
                }
            }


        }

        else if (enemy.type.Equals("D"))
        {
            if (stuned1 && actionEnemy > 1 ||
                stuned2 && actionEnemy > 1 ||
                stuned3 && actionEnemy > 1 ||
                stuned4 && actionEnemy > 1)
            {
                Status("기절!");
                enemy.Stuned();
                yield return new WaitForSeconds(1.5f);

                stuned1 = false;
                stuned2 = false;
                stuned3 = false;
                stuned4 = false;

                yield return null;
            }
            else
            {
                if (actionEnemy <= 1)
                {
                    //마법공격
                    if (stuned1 || stuned2 || stuned3 || stuned4)
                    {
                        enemy.Stuned();

                        stuned1 = false;
                        stuned2 = false;
                        stuned3 = false;
                        stuned4 = false;

                    }
                    else
                    {
                        EnemySpecialAttack();
                    }
                    yield return new WaitForSeconds(1.5f);

                    EnemySpecialAttack();

                }
                //방어도 회복(7)
                else if (actionEnemy == 7)
                {
                    EnemyShildRecover();
                }
                ////체력 회복(8)
                //else if (actionEnemy == 8)
                //{
                //    EnemyHealing();

                //    //독 회복
                //    if (player.poison > 4)
                //    {
                //        player.poison -= 4f;
                //        if (player.poison <= 0)
                //        {
                //            player.poison = 0;
                //        }
                //    }
                //}
                else
                {
                    //마법공격
                    EnemySpecialAttack();
                }
            }
        }
        else if (enemy.type.Equals("C"))
        {
            if (stuned1 && actionEnemy > 1 ||
                stuned2 && actionEnemy > 1 ||
                stuned3 && actionEnemy > 1 ||
                stuned4 && actionEnemy > 1)
            {
                Status("기절!");
                enemy.Stuned();
                yield return new WaitForSeconds(1.5f);

                stuned1 = false;
                stuned2 = false;
                stuned3 = false;
                stuned4 = false;

                yield return null;
            }
            else
            {
                if (actionEnemy <= 1)
                {
                    //마법공격
                    if (stuned1 || stuned2 || stuned3 || stuned4)
                    {
                        enemy.Stuned();

                        stuned1 = false;
                        stuned2 = false;
                        stuned3 = false;
                        stuned4 = false;

                    }
                    else
                    {
                        EnemySpecialAttack();
                    }
                    yield return new WaitForSeconds(1.5f);

                    EnemySpecialAttack();

                }
                ////방어도 회복(7)
                //else if (actionEnemy == 7)
                //{
                //    EnemyShildRecover();
                //}
                //체력 회복(8)
                else if (actionEnemy == 8)
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
                else
                {
                    //마법공격
                    EnemySpecialAttack();
                }
            }

        }
        else if (enemy.type.Equals("B"))
        {
            if (stuned1 && actionEnemy > 1 ||
              stuned2 && actionEnemy > 1 ||
              stuned3 && actionEnemy > 1 ||
              stuned4 && actionEnemy > 1)
            {
                Status("기절!");
                enemy.Stuned();
                yield return new WaitForSeconds(1.5f);

                stuned1 = false;
                stuned2 = false;
                stuned3 = false;
                stuned4 = false;

                yield return null;
            }
            else
            {
                if (actionEnemy <= 1)
                {
                    //물리공격
                    if (stuned1 || stuned2 || stuned3 || stuned4)
                    {
                        enemy.Stuned();

                        stuned1 = false;
                        stuned2 = false;
                        stuned3 = false;
                        stuned4 = false;

                    }
                    else
                    {
                        EnemyAttack();
                    }
                    yield return new WaitForSeconds(1.5f);

                    EnemyAttack();

                }
                //방어도 회복(7)
                else if (actionEnemy == 7)
                {
                    EnemyShildRecover();
                }
                ////체력 회복(8)
                //else if (actionEnemy == 8)
                //{
                //    EnemyHealing();

                //    //독 회복
                //    if (player.poison > 4)
                //    {
                //        player.poison -= 4f;
                //        if (player.poison <= 0)
                //        {
                //            player.poison = 0;
                //        }
                //    }
                //}
                else
                {
                    //물리공격
                    EnemyAttack();
                }
            }
        }
        else if (enemy.type.Equals("A"))
        {
            if (stuned1 && actionEnemy > 1 ||
              stuned2 && actionEnemy > 1 ||
              stuned3 && actionEnemy > 1 ||
              stuned4 && actionEnemy > 1)
            {
                Status("기절!");
                enemy.Stuned();
                yield return new WaitForSeconds(1.5f);

                stuned1 = false;
                stuned2 = false;
                stuned3 = false;
                stuned4 = false;

                yield return null;
            }
            else
            {
                if (actionEnemy <= 1)
                {
                    //물리공격
                    if (stuned1 || stuned2 || stuned3 || stuned4)
                    {
                        enemy.Stuned();

                        stuned1 = false;
                        stuned2 = false;
                        stuned3 = false;
                        stuned4 = false;

                    }
                    else
                    {
                        EnemyAttack();
                    }
                    yield return new WaitForSeconds(1.5f);

                    EnemyAttack();

                }
                ////방어도 회복(7)
                //else if (actionEnemy == 7)
                //{
                //    EnemyShildRecover();
                //}
                //체력 회복(8)
                else if (actionEnemy == 8)
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
                else
                {
                    //마법공격
                    EnemyAttack();
                }
            }
        }
    }

    void EnemyAttack()
    {
        //적 공격 Enermy.cs에서 작성예정 -애니메이션, 이펙트 (파티클?) 등
        enemy.Attack();
        StartCoroutine(AttackEffect());

        int enemyDam = (int)Random.Range(enemy.minAtt1, enemy.att1); 

        string action = $"물리공격 {enemyDam}";
        Status(action);

        TakeDamageFromEnemy(enemyDam, false);

        //if (player.shild >= enemyDam)
        //{
        //    player.shild -= enemyDam;
        //}
        //else if (player.shild < enemyDam)
        //{
        //    player.hp -= (enemyDam - player.shild);
        //    player.shild = 0;
        //}
        //else
        //{
        //    player.hp -= enemyDam;
        //}

        //player.UpdateHpShildSet();

        //if (player.hp <= 0)
        //{
        //    //플레이어 사망 (패배)
        //    StartCoroutine(playerDeath());
        //    return;
        //}
    }
    private void EnemySpecialAttack()
    {
        enemy.SpecialAttack();
        StartCoroutine(SpecialEffect());

        float enemyDam = Random.Range(enemy.minAtt2, enemy.att2);

        string action = $"마법공격 {(int)enemyDam}";
        Status(action);

        TakeDamageFromEnemy(enemyDam, true);

        //player.hp -= enemyDam;
        //player.UpdateHpShildSet();
        //if (player.hp <= 0)
        //{
        //    //플레이어 사망 (패배)
        //    StartCoroutine(playerDeath());
        //    return;
        //}
    }
    private void EnemyShildRecover()
    {
        enemy.ShildRecover();
        StartCoroutine(ShildEffect());

        string action = $"방어도 {enemy.recovery}회복";
        Status(action);
        enemy.shild += enemy.recovery;
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

        string action = $"체력 {enemy.heal}회복";
        Status(action);
        enemy.hp += enemy.heal;
        if (enemy.hp >= enemy.maxHp)
        {
            enemy.hp = enemy.maxHp;
        }

        enemy.UpdateHpShildSet();
    }

    IEnumerator AttackEffect()
    {
        GameObject enemyEffect = Instantiate(enemyManager.enemyEffets[3]);
        enemyEffect.transform.position = player.hpBar.transform.position;

        yield return new WaitForSeconds(1.5f);

        Destroy(enemyEffect);
    }

    IEnumerator SpecialEffect()
    {
        GameObject enemyEffect = Instantiate(enemyManager.enemyEffets[4]);
        enemyEffect.transform.position = player.hpBar.transform.position;

        yield return new WaitForSeconds(1.5f);

        Destroy(enemyEffect);
    }

    IEnumerator ShildEffect()
    {
        GameObject enemyEffect = Instantiate(enemyManager.enemyEffets[0]);
        enemyEffect.transform.position = enemy.transform.position;

        yield return new WaitForSeconds(2f);

        Destroy(enemyEffect);
    }

    IEnumerator HealEffect()
    {
        GameObject enemyEffect = Instantiate(enemyManager.enemyEffets[1]);
        enemyEffect.transform.position = enemy.transform.position;

        yield return new WaitForSeconds(2f);

        Destroy(enemyEffect);
    }

    IEnumerator EnemyDeath()
    {
        //Animator anim = enemy.GetComponent<Animator>();
        //anim.Play("Death");
        if (enemy.death) yield break;
        enemy.death = true;

        //적 죽음 애니메이션
        enemy.Death();

        float goldBonus = scoreManager.round * (1f + player.goldBonus);

        //빠른 클리어 보상
        if (turn < 5)
        {
            itemManager.PlusGold((int)(goldBonus  * 20));
        }
        //일반 보상
        else
        {
            itemManager.PlusGold((int)(goldBonus * 5));
        }

        yield return new WaitForSeconds(1.5f);

        playerTurn = false;
        enemyTurn = false;
        GameObject enemyEffect = Instantiate(enemyManager.enemyEffets[5]);
        enemyEffect.transform.position = enemy.transform.position;
        Destroy(enemy.gameObject);

        //라운드 확인 작업필요
        scoreManager.round ++;

        yield return new WaitForSeconds(1.5f);
        Destroy(enemyEffect);

        yield return new WaitForSeconds(1.5f);

        //끝났다면 업그레이드 상점으로 이동
        GameSceneManager.Instance.LoadScene("ArenaUpgradeStoreScene");

        yield return null;
    }

    IEnumerator playerDeath()
    {
        SpinStop();

        if (PlayerManager.Instance.hasRevive && !playerRevive)
        {
            playerRevive = true;

            //부활 연출
            GameObject effect = Instantiate(PlayerManager.Instance.reviveEffect, Vector2.zero, Quaternion.identity);

            yield return new WaitForSeconds(4f);

            Destroy(effect);
            player.HpShildSet();

            enemyTurn = false;
            playerTurn = true;

            DetermineEnemyNextAction();

            StartPlayerTurn();
            SpinStart();

            if(stageManager != null) stageManager.ReloadChance = 3;

            yield break;
        }


        //player.Death();
        playerTurn = false;
        enemyTurn = false;

        yield return new WaitForSeconds(2.5f);
        GameSceneManager.Instance.LoadScene("GameOverScene");
    }

    void Apple(float playerHp)
    {
        player.hp += playerHp;
        if (player.hp >= player.maxHp) player.hp = player.maxHp;
        player.UpdateHpShildSet();
    }

    void Grape(float playerMaxHp)
    {
        player.maxHp += playerMaxHp;
        player.UpdateHpShildSet();
    }

    void Meat(float hp, float shild)
    {
        player.hp += hp;
        player.shild += shild;
        if (player.hp >= player.maxHp) player.hp = player.maxHp;
        if (player.shild > player.maxSh) player.shild = player.maxSh;

        player.UpdateHpShildSet();
    }
    void Energy(float att1, float att2)
    {
        player.att1 += att1;
        player.att2 += att2;
    }

    void Shield(float playerSh, float playerMaxSh)
    {
        player.maxSh += playerMaxSh;
        player.shild += playerSh;
        if(player.shild > player.maxSh) player.shild = player.maxSh;
        player.UpdateHpShildSet();
    }

    void Magic(float att1, float att2, float hp,float plus_hp,float shild,float plus_sh)
    {
        player.att1 += att1;
        player.att2 += att2;
        player.maxHp += plus_hp;
        player.maxSh += plus_sh;

        player.hp += hp;
        player.shild += shild;
        if (player.hp >= player.maxHp) player.hp = player.maxHp;
        if (player.shild > player.maxSh) player.shild = player.maxSh;

        player.UpdateHpShildSet();
    }

    void Blood(float blood)
    {
        player.hp += blood;
        if (player.hp >= player.maxHp) player.hp = player.maxHp;

        //AttDamage(blood);
        enemy.hp = Mathf.Max(enemy.hp -= blood, 0);

        player.UpdateHpShildSet();

        scoreManager.AddScore((int)blood);
    }

    //void AttDamage(float att1)
    //{
    //    if (enemy.shild > 0 && enemy.shild >= att1)
    //    {
    //        enemy.shild -= att1;
    //    }
    //    else if (enemy.shild > 0 && enemy.shild < att1)
    //    {
    //        enemy.hp -= (att1 - enemy.shild);
    //        enemy.shild = 0;
    //    }
    //    else
    //    {
    //        enemy.hp -= att1;
    //    }

    //    scoreManager.AddScore((int)att1);
    //}


    void InitializeSceneObjects()
    {

        // UI 다시 찾기
        stopBtn = GameObject.FindWithTag("StopBtn")?.GetComponent<Button>();
        statusTxt = GameObject.FindWithTag("StatusTxt")?.GetComponent<TextMeshProUGUI>();
        turnTxt = GameObject.FindWithTag("TurnTxt")?.GetComponent<TextMeshProUGUI>();
        slotParent = GameObject.FindWithTag("Slot");

        // 매니저 참조 및 리스트 초기화
        itemManager = ItemManager.Instance;
        enemyManager = FindAnyObjectByType<EnemyArenaManager>();
        scoreManager = FindAnyObjectByType<ScoreManager>();
        stageManager = FindAnyObjectByType<StageManager>();
        player = FindFirstObjectByType<Player>();

        allItemDatas = itemManager.allItemDatas;

        enemyManager.SpawnEnemy(scoreManager.round);
        print($"현재라운드 : {scoreManager.round}");

        enemy = enemyManager.currentEnemy;

        if (player == null || enemy == null || stopBtn == null)
        {
            Debug.LogWarning("필수 오브젝트 없음!!");
            return;
        }

        //턴 초기화
        turn = 0;

        //초기 배열 및 카운트 재설정
        slotCount = 5;
        items = new string[slotCount];

        itemPrefab = GetComponent<GameObject>();
        currentEffects = new GameObject[slotCount];
        isEnemyturnning = false;
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

        StartPlayerTurn();

        SpinStart();

        if (stopBtn != null)
        {
            stopBtn.onClick.RemoveAllListeners();
            //stopBtn.onClick.AddListener(SpinSlotbySlotStop);
            // EventTrigger 컴포넌트가 버튼에 있어야 합니다.
            EventTrigger trigger = stopBtn.gameObject.GetComponent<EventTrigger>();
            if (trigger == null) trigger = stopBtn.gameObject.AddComponent<EventTrigger>();

            // PointerDown (누르기 시작)
            EventTrigger.Entry downEntry = new EventTrigger.Entry();
            downEntry.eventID = EventTriggerType.PointerDown;
            downEntry.callback.AddListener((data) => { OnStopBtnDown(); });
            trigger.triggers.Add(downEntry);

            // PointerUp (손을 뗌)
            EventTrigger.Entry upEntry = new EventTrigger.Entry();
            upEntry.eventID = EventTriggerType.PointerUp;
            upEntry.callback.AddListener((data) => { OnStopBtnUp(); });
            trigger.triggers.Add(upEntry);
        }

        //// 플레이어 턴부터 시작
        //playerTurn = true;
        //playerSlotCheck = true;

        //전체 매니저 관리가 없어 임시용
        if (AudioManager.audioManager == null) return;
        float value = AudioManager.audioManager.bgmVolume;

        //print("볼륨값" + value); 
        if (AudioManager.audioManager.GetCurrentBGM() != "Battle")
        {
            AudioManager.audioManager.StopBGM();
            AudioManager.audioManager.PlayBGM("Battle", value);
        }

        if (stageManager != null) stageManager.ReloadChance = 3;

        Debug.Log("씬 오브젝트들 초기화 완료");
    }

    //물공 증강수치적용
    void ApplyPhysicalDamageToEnemy(float att)
    {
        //if (player.storeAtkMultiplier > 1f)
        //{
        //    att *= player.storeAtkMultiplier;
        //}

        //float finalDamage = att;

        enemy.shild -= att;
        if (enemy.shild < 0)
        {
            enemy.hp += enemy.shild;
            enemy.shild = 0;
        }
        enemy.UpdateHpShildSet();

        scoreManager.AddScore((int)att);
    }

    //마공 증강수치적용
    void ApplyMagicDamageToEnemy(float att)
    {
        //if (player.storeMatkMultiplier > 1f)
        //{
        //    att *= player.storeMatkMultiplier;
        //}

        //float finalDamage = att;

        enemy.hp -= att;
        enemy.UpdateHpShildSet();

        scoreManager.AddScore((int)att);
    }

    // 물리/마법 저항적용
    public void TakeDamageFromEnemy(float damage, bool isMagic)
    {
        float finalDamage = 0;

        if (isMagic)
        {
            // 마법 저항력 % 감소 또는 고정치 감소 적용
            // 예: 데미지의 20%를 줄여줌 (storeMatkResist가 0.2f일 경우)
            finalDamage = damage * (1f - player.storeMatkResist);
            player.hp -= Mathf.Max(0, finalDamage);
        }
        else
        {
            // 물리 저항력 적용
            finalDamage = damage * (1f - player.storeAtkResist);

            // 실드 먼저 깎기
            player.shild -= Mathf.Max(0, finalDamage);
            if (player.shild < 0)
            {
                player.hp += player.shild;
                player.shild = 0;
            }
        }

        player.UpdateHpShildSet();

        if (player.hp <= 0)
        {
            //플레이어 사망 (패배)
            StartCoroutine(playerDeath());
            return;
        }
    }
}