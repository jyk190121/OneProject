
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
public class BattleManager : MonoBehaviour
{
    
    List<Item> allItemDatas;

    public SlotSpinner[] slotSpinner;
    SlotSpinner[] spawnedSlots;

    public Player player;
    public GameObject itemPrefab;
    public GameObject[] currentEffects;
    public Enemy enemy;
    //public List<Enemy> enemies;

    public Button stopBtn;              //�������� ��ư

    public GameObject slotParent;       //���� ������ġ
    int slotCount;                      //�� ���� ���԰���

    string[] items;                     // ���� ���� ������ ���
    public bool playerTurn;
    public bool enemyTurn;
    bool playerSlotCheck;
    bool isEnemyturnning;

    //bool cri1;              //ġ��Ÿ
    //bool cri2;              //�ް�ġ��Ÿ

    public bool stuned1;    //��˽���
    public bool stuned2;    //��޵�������

    public TextMeshProUGUI turnTxt;
    public TextMeshProUGUI statusTxt;

    StageManager stageManager;              //현재 진입한 스테이지
    int currentStageIndex;

    EnemyManager enemyManager;
    ItemManager itemManager;

    public GameObject goldParent;               //골드 프리팹 생성할 위치
    public GameObject goldPrefab;               //캔버스에 보여줄 프리팹(골드)

    public TextMeshProUGUI nextEnemyActionTxt;  // 적 다음 행동
    bool enemyActionCeheck = false;             // 1턴에 1번
    int r;

    public TextMeshProUGUI stageTxt;
    public TextMeshProUGUI roundTxt;
    //public TextMeshProUGUI enemyName;

    Dictionary<string, int> itemDict = new Dictionary<string, int>()
    {
        {"고급도끼", 0},
        {"사과", 0},
        {"에너지", 0},
        {"물리에너지", 0},
        {"물리에너지_대", 0},
        {"마법에너지", 0},
        {"마법에너지_대", 0},
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
        stageTxt.text = $"스테이지 {stageManager.SelectedStage}";
        if (stageManager.Round != 5) roundTxt.text = $"{stageManager.Round} 라운드";
        else roundTxt.text = "보스";
    }

    void Update()
    {
        if (player != null && enemy != null && stopBtn != null)
        {
            StatusTurn();

            if(!enemyActionCeheck)
            {
                r = Random.Range(0, 10); //0~9
                if (stuned1 || stuned2) nextEnemyActionTxt.text = "기절상태";
                else if (r == 0) nextEnemyActionTxt.text = "특수공격(마법)";
                else if (r < 7 && r != 0) nextEnemyActionTxt.text = "일반공격(물리)";
                else if (r > 5 && r < 8) nextEnemyActionTxt.text = $"방어도 {enemy.ShildRecover()} 회복";
                else nextEnemyActionTxt.text = $"체력 {enemy.Healing()} 회복";

                enemyActionCeheck = true;
            }

            Keyboard key = Keyboard.current;
            if (key == null) return;

            if (key.enterKey.wasPressedThisFrame && playerSlotCheck ||
                key.spaceKey.wasPressedThisFrame && playerSlotCheck)
            {
                SpinSlotbySlotStop();
            }

            //foreach (SlotSpinner s in spawnedSlots)
            //{
            //    if (s.isSpinning) s.StartSpin();
            //}

            // 플레이어 턴
            if (spawnedSlots[spawnedSlots.Length - 1].isSpinning == false && playerSlotCheck)
            {
                //SpinStart();

                // ComboCount 계산
                //ComboCri(ComboCount(items));

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
                    StartEnemyTurn(r);
                }
            }
        }
    }

    //// 콤보 확인
    //string ComboCount(string[] itmes)
    //{
    //    string lastItem = null;

    //    foreach (string item in itmes)
    //    {
    //        if (itemDict.TryGetValue(item, out int equalsCount))
    //        {
    //            if (item == lastItem && lastItem != null)
    //            {
    //                itemDict[item]++;
    //            }
    //            else
    //            {
    //                itemDict[item] = 1;
    //            }
    //        }
    //        lastItem = item;
    //    }

    //    // 치명타, 메가치명타 여부 확인
    //    for (int i = 0; i < items.Length; i++)
    //    {
    //        if (itemDict.TryGetValue(items[i], out int equalsCount))
    //        {
    //            if (equalsCount >= 3 && equalsCount != 5)
    //            {
    //                print($"{items[i]} 치명타 ");
    //                cri1 = true;
    //                return items[i];
    //            }

    //            if (equalsCount == 5)
    //            {
    //                print($"{items[i]} 메가치명타");
    //                cri2 = true;
    //                return items[i];
    //            }
    //        }
    //    }

    //    // 콤보 횟수 초기화
    //    for (int i = 0; i < items.Length; i++)
    //    {
    //        itemDict[items[i]] = 1;
    //    }

    //    return null;
    //}

    //void ComboCri(string item)
    //{
    //    if (cri1 == true)
    //    {
    //        itemDict[item] = 3;
    //    }
    //    else if (cri2 == true)
    //    {
    //        itemDict[item] = 5;
    //    }
    //}

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
        enemyActionCeheck = false;

        playerSlotCheck = true;

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

    // 애니메이션 효과 or 파티클 생성 + 데미지 계산
    IEnumerator ItemEffect(string[] items)
    {
        //이전 아이템명과 동일한지 체크
        string lastItem = null;

        // [단계 1] 중복 갯수 초기화 및 계산
        // 딕셔너리의 모든 값을 0으로 리셋
        List<string> keys = new List<string>(itemDict.Keys);
        foreach (string key in keys) itemDict[key] = 0;

        List<Item> matchedItems = new List<Item>();

        foreach (string name in items)
        {
            if (string.IsNullOrEmpty(name)) continue;

            //if (name == lastItem)
            //{
            //    // 갯수 누적
            //    if (itemDict.ContainsKey(name)) itemDict[name]++;
            //}
            //else
            //{
            //    if (itemDict.ContainsKey(name)) itemDict[name] = 1;
            //}

            // [단계 2] 이름에 맞는 실제 Item 데이터 찾기 (allItemDatas에서)
            Item data = allItemDatas.Find(x => x.NAME == name);
            if (data != null)
            {
                matchedItems.Add(data);
            }

            //print($"{itemDict[name]} 콤보확인");
            //lastItem = name;
        }

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
                if (item.NAME == lastItem)
                {
                    // 갯수 누적
                    if (itemDict.ContainsKey(item.NAME)) itemDict[item.NAME]++;
                }
                else
                {
                    if (itemDict.ContainsKey(item.NAME)) itemDict[item.NAME] = 1;
                }

                lastItem = item.NAME;

                //print($"개수 카운트{itemDict[item.NAME]}");

                if (itemDict[item.NAME] >= 0 &&
                  itemDict[item.NAME] < 3)
                {
                    item.COUNT = 1;
                }
                else if (itemDict[item.NAME] >= 3 &&
                         itemDict[item.NAME] < 5)
                {
                    item.COUNT = 2;
                }
                else if (itemDict[item.NAME] == 5)
                {
                    //item.COUNT = 3;
                    item.COUNT = 3;
                }
                else
                {
                    item.COUNT = 0;
                }

                if (itemDict.TryGetValue(item.NAME, out int equalsCount))
                {
                    // 이펙트 생성 위치
                    Vector3 itemPos = Vector3.zero;

                    // 각 아이템에 맞는 애니메이션
                    if (item.NAME.Equals("사과"))
                    {
                        itemPrefab = item.EFFECT;

                        itemPos = player.hpBar.transform.position;

                        switch (item.COUNT)
                        {
                            // 일반
                            case 1:
                                action = $"체력 {item.ENHANCE_HP} 회복";
                                Apple(item.ENHANCE_HP);
                                break;

                            // 치명타
                            case 2:
                                action = $"치명타!\n체력 {item.ENHANCE_HP * 3} 회복";
                                Apple(item.ENHANCE_HP * 3);
                                break;
                            // 메가치명타
                            case 3:
                                action = $"메가치명타!\n체력 {item.ENHANCE_HP * 9} 회복";
                                Apple(item.ENHANCE_HP * 9);
                                break;
                        }
                    }
                    if (item.NAME.Equals("포도"))
                    {
                        itemPrefab = item.EFFECT;
                        itemPos = player.hpBar.transform.position;
                        switch (item.COUNT)
                        {
                            // 일반
                            case 1:
                                action = $"최대 체력 {item.ENHANCE_PLUS_HP} 증가";
                                Grape(item.ENHANCE_PLUS_HP);
                                break;
                            case 2:
                                action = $"치명타!\n최대 체력 {item.ENHANCE_PLUS_HP * 3} 증가";
                                Grape(item.ENHANCE_PLUS_HP * 3);
                                break;
                            case 3:
                                action = $"메가치명타!\n최대 체력 {item.ENHANCE_PLUS_HP * 9} 증가";
                                Grape(item.ENHANCE_PLUS_HP * 9);
                                break;
                        }
                    }

                    if (item.NAME.Equals("딸기"))
                    {
                        itemPrefab = item.EFFECT;
                        itemPos = player.hpBar.transform.position;
                        switch (item.COUNT)
                        {
                            // 일반
                            case 1:
                                action = $"체력 {item.ENHANCE_HP} 회복\n최대 체력 {item.ENHANCE_PLUS_HP} 증가";
                                Apple(item.ENHANCE_HP);
                                Grape(item.ENHANCE_PLUS_HP);
                                break;
                            case 2:
                                action = $"치명타!\n체력 {item.ENHANCE_HP * 3} 회복\n최대 체력 {item.ENHANCE_PLUS_HP * 3} 증가";
                                Apple(item.ENHANCE_HP * 3);
                                Grape(item.ENHANCE_PLUS_HP * 3);
                                break;
                            case 3:
                                action = $"메가치명타!\n체력 {item.ENHANCE_HP * 9} 회복\n최대 체력 {item.ENHANCE_PLUS_HP * 9} 증가";
                                Apple(item.ENHANCE_HP * 9);
                                Grape(item.ENHANCE_PLUS_HP * 9);
                                break;
                        }
                    }

                    if (item.NAME.Equals("고기"))
                    {
                        itemPrefab = item.EFFECT;
                        itemPos = player.hpBar.transform.position;

                        float hp = item.ENHANCE_HP;
                        float shild = item.ENHANCE_SHILD;


                        switch (item.COUNT)
                        {
                            // 일반
                            case 1:
                                action = $"체력 {hp} 회복\n방어도 {shild} 회복";
                                Meat(hp, shild);
                                break;
                            case 2:
                                hp *= 3;
                                shild *= 3;
                                action = $"치명타!\n체력 {hp} 회복\n방어도 {shild} 회복";
                                Meat(hp, shild);
                                break;
                            case 3:
                                hp *= 9;
                                shild *= 9;
                                action = $"메가치명타!\n체력 {hp} 회복\n방어도 {shild} 회복";
                                Meat(hp, shild);
                                break;
                        }
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
                                itemPrefab = item.EFFECT;
                                break;
                            }
                        }

                        switch (item.COUNT)
                        {
                            // 일반
                            case 1:
                                action = $"물공 {item.ENHANCE_PLUSATK}\n마공 {item.ENHANCE_PLUSMATK} 증가";
                                Energy(item.ENHANCE_PLUSATK, item.ENHANCE_PLUSMATK);
                                break;

                            case 2:
                                action = $"치명타!\n물공 {item.ENHANCE_PLUSATK * 3}\n마공 {item.ENHANCE_PLUSMATK * 3} 증가";
                                Energy(item.ENHANCE_PLUSATK * 3, item.ENHANCE_PLUSMATK * 3);
                                break;

                            case 3:
                                action = $"메가치명타!\n물공 {item.ENHANCE_PLUSATK * 9}\n마공 {item.ENHANCE_PLUSMATK * 9} 증가";
                                Energy(item.ENHANCE_PLUSATK * 9, item.ENHANCE_PLUSMATK * 9);
                                break;
                        }
                    }

                    if (item.NAME.Equals("물리에너지") || item.NAME.Equals("물리에너지_대"))
                    {
                        for (int i = 0; i < matchedItems.Count; i++)
                        {
                            if (item == matchedItems[i])
                            {
                                int slotIndex = energy2 % spawnedSlots.Length;
                                // 슬롯의 위치에 이펙트 인덱스 설정
                                itemPos = spawnedSlots[slotIndex].transform.position;
                                itemPrefab = item.EFFECT;
                                break;
                            }
                        }

                        switch (item.COUNT)
                        {
                            // 일반
                            case 1:
                                action = $"물공 {item.ENHANCE_PLUSATK} 증가";
                                Energy(item.ENHANCE_PLUSATK, 0);
                                break;

                            case 2:
                                action = $"치명타!\n물공 {item.ENHANCE_PLUSATK * 3} 증가";
                                Energy(item.ENHANCE_PLUSATK * 3, 0);
                                break;

                            case 3:
                                action = $"메가치명타!\n물공 {item.ENHANCE_PLUSATK * 9} 증가";
                                Energy(item.ENHANCE_PLUSATK * 9, 0);
                                break;
                        }
                    }

                    if (item.NAME.Equals("마법에너지") || item.NAME.Equals("마법에너지_대"))
                    {
                        for (int i = 0; i < matchedItems.Count; i++)
                        {
                            if (item == matchedItems[i])
                            {
                                int slotIndex = energy3 % spawnedSlots.Length;
                                // 슬롯의 위치에 이펙트 인덱스 설정
                                itemPos = spawnedSlots[slotIndex].transform.position;
                                itemPrefab = item.EFFECT;
                                break;
                            }
                        }

                        switch (item.COUNT)
                        {
                            // 일반
                            case 1:
                                action = $"마공 {item.ENHANCE_PLUSMATK} 증가";
                                Energy(0, item.ENHANCE_PLUSMATK);
                                break;

                            case 2:
                                action = $"치명타!\n마공 {item.ENHANCE_PLUSMATK * 3} 증가";
                                Energy(0, item.ENHANCE_PLUSMATK * 3);
                                break;

                            case 3:
                                action = $"메가치명타!\n마공 {item.ENHANCE_PLUSMATK * 9} 증가";
                                Energy(0, item.ENHANCE_PLUSMATK * 9);
                                break;
                        }
                    }


                    if (item.NAME.Equals("독약"))
                    {
                        itemPrefab = item.EFFECT;
                        itemPos = Vector3.up;
                        //print("독 데미지 12");
                        switch (item.COUNT)
                        {
                            //일반
                            case 1:
                                action = $"독 중독 {item.ENHANCE_POISON}";
                                //독 데미지 부여 (매 턴마다 적에게 데미지를 입힌다)
                                player.poison += item.ENHANCE_POISON;
                                player.UpdatePosionUI();
                                break;
                            case 2:
                                action = $"치명타!\n독 중독 {item.ENHANCE_POISON * 3}";
                                player.poison += (item.ENHANCE_POISON * 3);
                                player.UpdatePosionUI();
                                break;
                            case 3:
                                action = $"메가치명타!\n독 중독  {item.ENHANCE_POISON * 9}";
                                player.poison += (item.ENHANCE_POISON * 9);
                                player.UpdatePosionUI();
                                break;
                        }
                    }

                    if (item.NAME.Equals("독검"))
                    {
                        itemPrefab = item.EFFECT;
                        itemPos = Vector3.up;
                        //물공 10~25
                        //독 중독5
                        float att = Random.Range(item.ENHANCE_MINATK, item.ENHANCE_ATK) + player.att1;
                        switch (item.COUNT)
                        {
                            //일반
                            case 1:
                                action = $"물공 {att}\n독 중독 {item.ENHANCE_POISON}";
                                //독 데미지 부여 (매 턴마다 적에게 데미지를 입힌다)
                                player.poison += item.ENHANCE_POISON;
                                player.UpdatePosionUI();
                                break;
                            case 2:
                                att *= 3;
                                action = $"치명타!\n물공 {att}\n독 중독 {item.ENHANCE_POISON * 3}";
                                player.poison += (item.ENHANCE_POISON * 3);
                                player.UpdatePosionUI();
                                break;
                            case 3:
                                att *= 9;
                                action = $"메가치명타!\n물공 {att}\n독 중독 {item.ENHANCE_POISON * 9}";
                                player.poison += (item.ENHANCE_POISON * 9);
                                player.UpdatePosionUI();
                                break;
                        }
                        //물리데미지 적용
                        AttDamage(att);

                        enemy.UpdateHpShildSet();
                    }

                    if (item.NAME.Equals("마법검"))
                    {
                        //print("마법 공격 30");
                        float att = 0;

                        switch (item.COUNT)
                        {
                            case 1:
                                att = player.att2 + item.ENHANCE_MATK;
                                action = $"마공 {att}";
                                break;
                            case 2:
                                att = player.att2 + (item.ENHANCE_MATK * 3);
                                action = $"치명타!\n마공 {att}";
                                break;
                            case 3:
                                att = player.att2 + (item.ENHANCE_MATK * 9);
                                action = $"메가치명타!\n마공 {att}";
                                break;
                        }

                        itemPrefab = item.EFFECT;
                        itemPos = Vector3.up;

                        enemy.hp -= att;

                        enemy.UpdateHpShildSet();

                    }

                    if (item.NAME.Equals("해골도끼"))
                    {
                        //print("공격 20 공격 20");
                        float att1 = 0;
                        float att2 = 0;

                        switch (item.COUNT)
                        {
                            case 1:
                                att1 = Random.Range(item.ENHANCE_MINATK, item.ENHANCE_ATK) + player.att1;
                                att2 = Random.Range(item.ENHANCE_MINMATK, item.ENHANCE_MATK) + player.att2;
                                action = $"물공 {att1} , 마공 {att2}";
                                break;
                            case 2:
                                att1 = (Random.Range(item.ENHANCE_MINATK, item.ENHANCE_ATK) * 3) + player.att1;
                                att2 = (Random.Range(item.ENHANCE_MINMATK, item.ENHANCE_MATK) * 3) + player.att2;
                                action = $"치명타!\n물공 {att1} , 마공 {att2}";
                                break;
                            case 3:
                                att1 = (Random.Range(item.ENHANCE_MINATK, item.ENHANCE_ATK) * 9) + player.att1;
                                att2 = (Random.Range(item.ENHANCE_MINMATK, item.ENHANCE_MATK) * 9) + player.att2;
                                action = $"메가치명타!\n물공 {att1} , 마공 {att2}";
                                break;
                        }

                        itemPrefab = item.EFFECT;
                        itemPos = Vector3.up;

                        //물리데미지 적용
                        AttDamage(att1);

                        //마법데미지 적용
                        enemy.hp -= att2;
                        enemy.UpdateHpShildSet();
                    }

                    if (item.NAME.Equals("마법봉"))
                    {
                        //print("공격 10");
                        float att = 0;

                        switch (item.COUNT)
                        {
                            case 1:
                                att = item.ENHANCE_MATK + player.att2;
                                action = $"마공 {att}";
                                break;
                            case 2:
                                att = (item.ENHANCE_MATK * 3) + player.att2;
                                action = $"치명타!\n마공 {att}";
                                break;
                            case 3:
                                att = (item.ENHANCE_MATK * 9) + player.att2;
                                action = $"{item.NAME} 메가치명타\n마공 {att}";
                                break;
                        }

                        itemPrefab = item.EFFECT;
                        itemPos = Vector3.up;

                        //마법데미지
                        enemy.hp -= att;
                        enemy.UpdateHpShildSet();

                    }
                    if (item.NAME.Equals("일반검"))
                    {
                        //print("물공 10");
                        float att = 0;
                        switch (item.COUNT)
                        {
                            case 1:
                                att = item.ENHANCE_ATK + player.att1;
                                action = $"물공 {att}";
                                break;
                            case 2:
                                att = (item.ENHANCE_ATK * 3) + player.att1;
                                action = $"치명타!\n물공 {att}";
                                break;
                            case 3:
                                att = (item.ENHANCE_ATK * 9) + player.att1; ;
                                action = $"메가치명타!\n물공 {att}";
                                break;
                        }

                        itemPrefab = item.EFFECT;
                        itemPos = Vector3.up;

                        AttDamage(att);

                        enemy.UpdateHpShildSet();

                    }
                    if (item.NAME.Equals("일반도끼"))
                    {
                        //print("공격20 물리");
                        float att = 0;

                        switch (item.COUNT)
                        {
                            case 1:
                                att = Random.Range(item.ENHANCE_MINATK, item.ENHANCE_ATK) + player.att1;
                                action = $"물공 {att}";
                                break;
                            case 2:
                                att = (Random.Range(item.ENHANCE_MINATK, item.ENHANCE_ATK) * 3) + player.att1;
                                action = $"치명타!\n물공 {att}";
                                break;
                            case 3:
                                att = (Random.Range(item.ENHANCE_MINATK, item.ENHANCE_ATK) * 9) + player.att1;
                                action = $"메가치명타!\n물공 {att}";
                                break;
                        }

                        itemPrefab = item.EFFECT;
                        itemPos = Vector3.up;

                        AttDamage(att);

                        enemy.UpdateHpShildSet();

                    }
                    if (item.NAME.Equals("대검"))
                    {
                        //print("공격30 물리");
                        float att = 0;
                        float r = (Random.Range(0, 1));

                        switch (item.COUNT)
                        {
                            case 1:
                                att = item.ENHANCE_ATK + player.att1;
                                action = $"물공 {att}";
                                break;
                            case 2:
                                att = (item.ENHANCE_ATK * 3) + player.att1;
                                action = $"치명타!\n물공 {att}";
                                r = 1;
                                break;
                            case 3:
                                att = (item.ENHANCE_ATK * 9) + player.att1;
                                action = $"메가치명타!\n물공 {att}";
                                r = 1;
                                break;
                        }

                        //print($"스턴체크 + {r > (1 - item.ENHANCE_STUNED)}");

                        //이미 스턴 상태이면 해제되지 않도록
                        if (!stuned1)
                        {
                            // 1 - 스턴확률(0.2라면 0.8)보다 r이 크면 성공
                            if (r > (1f - item.ENHANCE_STUNED))
                            {
                                stuned1 = true;
                            }
                        }


                        itemPrefab = item.EFFECT;
                        itemPos = Vector3.up;

                        //물리데미지
                        AttDamage(att);

                        enemy.UpdateHpShildSet();

                    }
                    if (item.NAME.Equals("고급도끼"))
                    {
                        //print("공격40 물리");
                        float att = 0;
                        float r = (Random.Range(0, 1));

                        switch (item.COUNT)
                        {
                            case 1:
                                att = Random.Range(item.ENHANCE_MINATK, item.ENHANCE_ATK) + player.att1;
                                action = $"물공 {att}";
                                break;
                            case 2:
                                att = (Random.Range(item.ENHANCE_MINATK, item.ENHANCE_ATK) * 3) + player.att1;
                                action = $"치명타!\n물공 {att}";
                                r = Random.Range(0.35f, 1);
                                break;
                            case 3:
                                att = (Random.Range(item.ENHANCE_MINATK, item.ENHANCE_ATK) * 9) + player.att1;
                                action = $"메가치명타!\n물공 {att}";
                                r = 1;
                                break;
                        }

                        //print($"스턴체크 + {r > (1 - item.ENHANCE_STUNED)}");

                        // 스턴 체크: 이미 스턴이 걸려있지 않을 때만 확률 검사
                        if (!stuned2)
                        {
                            if (r > (1f - item.ENHANCE_STUNED))
                            {
                                stuned2 = true;
                                print("스턴 성공!");
                            }
                        }

                        itemPrefab = item.EFFECT;
                        itemPos = Vector3.up;

                        AttDamage(att);

                        enemy.UpdateHpShildSet();

                    }

                    if (item.NAME.Equals("천둥망치"))
                    {
                        //마공 30~100
                        float att = 0;

                        switch (item.COUNT)
                        {
                            case 1:
                                att = Random.Range(item.ENHANCE_MINMATK, item.ENHANCE_MATK) + player.att2;
                                action = $"마공 {att}";
                                break;
                            case 2:
                                att = (Random.Range(item.ENHANCE_MINMATK, item.ENHANCE_MATK) * 3) + player.att2;
                                action = $"치명타!\n마공 {att}";
                                break;
                            case 3:
                                att = (Random.Range(item.ENHANCE_MINMATK, item.ENHANCE_MATK) * 9) + player.att2;
                                action = $"메가치명타!\n마공 {att}";
                                break;
                        }

                        itemPrefab = item.EFFECT;
                        itemPos = Vector3.up;

                        //마법데미지
                        enemy.hp -= att;
                        enemy.UpdateHpShildSet();

                    }

                    if (item.NAME.Equals("화염방패"))
                    {
                        float shild = item.ENHANCE_SHILD;
                        float plus_sh = item.ENHANCE_PLUS_SHILD;
                        switch (item.COUNT)
                        {
                            case 1:
                                action = $"방어도 {shild} 회복\n최대 방어도 {plus_sh}";
                                Shield(shild, plus_sh);
                                break;
                            case 2:
                                shild *= 3;
                                plus_sh *= 3;
                                action = $"치명타!\n방어도 {shild} 회복\n최대 방어도 {plus_sh}";
                                Shield(shild, plus_sh);
                                break;
                            case 3:
                                shild *= 9;
                                plus_sh *= 9;
                                action = $"메가치명타!\n방어도 {shild} 회복\n최대 방어도 {plus_sh}";
                                Shield(shild, plus_sh);
                                break;
                        }


                        itemPrefab = item.EFFECT;
                        itemPos = player.shildBar.transform.position;

                    }

                    if (item.NAME.Equals("해골방패"))
                    {
                        float shild = item.ENHANCE_SHILD;
                        switch (item.COUNT)
                        {
                            case 1:
                                action = $"방어도 {shild} 회복";
                                Shield(shild, 0);
                                break;
                            case 2:
                                shild *= 3;
                                action = $"치명타!\n방어도 {shild} 회복";
                                Shield(shild, 0);
                                break;
                            case 3:
                                shild *= 9;
                                action = $"메가치명타!\n방어도 {shild} 회복";
                                Shield(shild, 0);
                                break;
                        }


                        itemPrefab = item.EFFECT;
                        itemPos = player.shildBar.transform.position;

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
                                itemPrefab = item.EFFECT;
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
                                itemPrefab = item.EFFECT;
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
                    }

                    if (item.NAME.Equals("마법투구"))
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
                                // 슬롯의 위치에 이펙트 인덱스 설정
                                itemPos = spawnedSlots[slotIndex].transform.position;
                                itemPrefab = item.EFFECT;
                                break;
                            }
                        }

                        switch (item.COUNT)
                        {
                            case 1:
                                action = $"물공 {att1},마공 {att2} 증가\n" +
                                         $"최대체력{plus_hp}, 최대방어도{plus_sh}증가\n" +
                                         $"체력{hp}, 방어도{shild} 회복";

                                Magic(att1, att2, hp, plus_hp, shild, plus_sh);

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

                                Magic(att1, att2, hp, plus_hp, shild, plus_sh);

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

                                Magic(att1, att2, hp, plus_hp, shild, plus_sh);

                                break;
                        }


                    }

                    if (item.NAME.Equals("마법반지"))
                    {
                        //증가치
                        float hp = item.ENHANCE_HP;
                        float plus_hp = item.ENHANCE_PLUS_HP;
                        float shild = item.ENHANCE_SHILD;
                        float plus_sh = item.ENHANCE_PLUS_SHILD;

                        for (int i = 0; i < matchedItems.Count; i++)
                        {
                            if (item == matchedItems[i])
                            {
                                int slotIndex = ring1 % spawnedSlots.Length;
                                // 슬롯의 위치에 이펙트 인덱스 설정
                                itemPos = spawnedSlots[slotIndex].transform.position;
                                itemPrefab = item.EFFECT;
                                break;
                            }
                        }

                        switch (item.COUNT)
                        {
                            case 1:
                                action = $"최대체력{plus_hp}, 최대방어도{plus_sh}증가\n" +
                                         $"체력{hp}, 방어도{shild} 회복";

                                Magic(0, 0, hp, plus_hp, shild, plus_sh);

                                break;
                            case 2:
                                hp *= 3;
                                plus_hp *= 3;
                                shild *= 3;
                                plus_sh *= 3;

                                action = $"치명타!\n" +
                                         $"최대체력{plus_hp}, 최대방어도{plus_sh}증가\n" +
                                         $"체력{hp}, 방어도{shild} 회복";

                                Magic(0, 0, hp, plus_hp, shild, plus_sh);

                                break;
                            case 3:
                                hp *= 9;
                                plus_hp *= 9;
                                shild *= 9;
                                plus_sh *= 9;
                                action = $"메가치명타!\n" +
                                         $"최대체력{plus_hp}, 최대방어도{plus_sh}증가\n" +
                                         $"체력{hp}, 방어도{shild} 회복";

                                Magic(0, 0, hp, plus_hp, shild, plus_sh);

                                break;
                        }
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
                                // 슬롯의 위치에 이펙트 인덱스 설정
                                itemPos = spawnedSlots[slotIndex].transform.position;
                                itemPrefab = item.EFFECT;
                                break;
                            }
                        }

                        switch (item.COUNT)
                        {
                            case 1:
                                action = $"최대방어도{plus_sh} 증가, 방어도{shild} 회복\n" +
                                         $"흡혈 {blood}";
                                Shield(shild, plus_sh);
                                Blood(blood);
                                break;
                            case 2:
                                shild *= 3;
                                plus_sh *= 3;
                                blood *= 3;

                                action = $"치명타!\n" +
                                         $"최대방어도{plus_sh} 증가, 방어도{shild} 회복\n" +
                                         $"흡혈 {blood}";

                                Shield(shild, plus_sh);
                                Blood(blood);
                                break;
                            case 3:
                                shild *= 9;
                                plus_sh *= 9;
                                blood *= 9;

                                action = $"메가치명타!\n" +
                                         $"최대방어도{plus_sh} 증가, 방어도{shild} 회복\n" +
                                         $"흡혈 {blood}";

                                Shield(shild, plus_sh);
                                Blood(blood);
                                break;
                        }
                    }

                    if (item.NAME.Equals("독반지"))
                    {
                        //증가치
                        float shild = item.ENHANCE_SHILD;
                        float plus_sh = item.ENHANCE_PLUS_SHILD;

                        //독
                        //float poison = item.ENHANCE_POISON;

                        for (int i = 0; i < matchedItems.Count; i++)
                        {
                            if (item == matchedItems[i])
                            {
                                int slotIndex = ring3 % spawnedSlots.Length;
                                // 슬롯의 위치에 이펙트 인덱스 설정
                                itemPos = spawnedSlots[slotIndex].transform.position;
                                itemPrefab = item.EFFECT;
                                break;
                            }
                        }

                        switch (item.COUNT)
                        {
                            case 1:
                                action = $"최대방어도{plus_sh} 증가, 방어도{shild} 회복\n" +
                                         $"독 중독 {item.ENHANCE_POISON}";
                                Shield(shild, plus_sh);

                                //독 데미지 부여 (매 턴마다 적에게 데미지를 입힌다)
                                player.poison += item.ENHANCE_POISON;
                                player.UpdatePosionUI();
                                break;
                            case 2:
                                shild *= 3;
                                plus_sh *= 3;

                                action = $"치명타!\n" +
                                         $"최대방어도{plus_sh} 증가, 방어도{shild} 회복\n" +
                                         $"독 중독 {item.ENHANCE_POISON}";

                                Shield(shild, plus_sh);
                                //독 데미지 부여 (매 턴마다 적에게 데미지를 입힌다)
                                player.poison += (item.ENHANCE_POISON * 3);
                                player.UpdatePosionUI();
                                break;
                            case 3:
                                shild *= 9;
                                plus_sh *= 9;

                                action = $"메가치명타!\n" +
                                         $"최대방어도{plus_sh} 증가, 방어도{shild} 회복\n" +
                                         $"독 중독 {item.ENHANCE_POISON}";

                                Shield(shild, plus_sh);
                                //독 데미지 부여 (매 턴마다 적에게 데미지를 입힌다)
                                player.poison += (item.ENHANCE_POISON * 9);
                                player.UpdatePosionUI();
                                break;
                        }
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

                    currentEffects[num] = Instantiate(itemPrefab);
                    currentEffects[num].transform.position = itemPos;
                    //itemArray[num].GetComponent<SpriteRenderer>().sortingOrder = 11;

                    //적 앞에 소환
                    ParticleSystemRenderer effectRender = currentEffects[num].GetComponent<ParticleSystemRenderer>();
                    SpriteRenderer goldEffectRederer = currentEffects[num].GetComponent<SpriteRenderer>();
                    if (effectRender != null)
                    {
                        effectRender.sortingOrder = 50;
                    }
                    if (goldEffectRederer != null)
                    {
                        goldEffectRederer.sortingOrder = 500;
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
                        //턴 넘기기
                        playerTurn = false;
                        isEnemyturnning = false;
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

    void StartEnemyTurn(int r)
    {
        int actionInt = r;
        StartCoroutine(EnemyTurn(actionInt));
    }

    IEnumerator EnemyTurn(int action)
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
            //int r = Random.Range(0, 10); //0~9
            //print("적 행동 " + r);
            //공격확률(0~6)
            if (action < 7)
            {
                if (action == 0)
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
            else if (action > 5 && action < 8)
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
            GameObject enemyEffect = Instantiate(enemyManager.enemyEffets[2]);
            enemyEffect.transform.position = enemy.transform.position;
            enemy.hp -= player.poison;
            Status($"<color=yellow> 독 피해 : {player.poison}");
            player.poison -= 2f;
            if (player.poison <= 0)
            {
                player.poison = 0;
            }
            enemy.AnimDamage();

            enemy.UpdateHpShildSet();
            player.UpdatePosionUI();


            if (enemy.hp <= 0)
            {
                //적사망(승리)
                StartCoroutine(EnemyDeath());
                yield break;
            }

            yield return new WaitForSeconds(1.5f);
            Destroy(enemyEffect);
        }

        playerTurn = true;
        playerSlotCheck = true;
        stopBtn.gameObject.SetActive(true);
        Status(" ");

        //슬롯 재시작
        SpinStart();

    }

    void EnermyAttack()
    {
        //적 공격 Enermy.cs에서 작성예정 -애니메이션, 이펙트 (파티클?) 등
        enemy.Attack();
        StartCoroutine(AttackEffect());

        int enemyDam = (int)Random.Range(enemy.minAtt1, enemy.att1); 

        string action = $"공격 {enemyDam}";
        Status(action);

        if (player.shild >= enemyDam)
        {
            player.shild -= enemyDam;
        }
        else if (player.shild < enemyDam)
        {
            player.hp -= (enemyDam - player.shild);
            player.shild = 0;
        }
        else
        {
            player.hp -= enemyDam;
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

        float enemyDam = Random.Range(enemy.minAtt2, enemy.att2);

        string action = $"특수공격 {(int)enemyDam}";
        Status(action);

        player.hp -= enemyDam;
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
        //enemy.ShildRecover();
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
        //enemy.Healing();
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

    //Enemy ScriptableObject만든 후 재작업
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
        enemy.ShildRecover();
        GameObject enemyEffect = Instantiate(enemyManager.enemyEffets[0]);
        enemyEffect.transform.position = enemy.transform.position;

        yield return new WaitForSeconds(2f);

        Destroy(enemyEffect);
    }

    IEnumerator HealEffect()
    {
        enemy.Healing();
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

        yield return new WaitForSeconds(1.5f);

        playerTurn = false;
        enemyTurn = false;
        GameObject enemyEffect = Instantiate(enemyManager.enemyEffets[5]);
        enemyEffect.transform.position = enemy.transform.position;
        Destroy(enemy.gameObject);

        //라운드 확인 작업필요
        stageManager.Round ++;

        if (stageManager.Round > 5)
        {
            //아이템 강화, 골드 초기화
            itemManager.Init();

            currentStageIndex++;
            stageManager.UnlockNextStage(currentStageIndex + 1);
        }
        //enemyManager.SpawnEnemy(currentStageIndex, round);

        yield return new WaitForSeconds(1.5f);
        Destroy(enemyEffect);

        yield return new WaitForSeconds(1.5f);

        //안끝났다면 다음 라운드 이동
        print($"현재 스테이지 {currentStageIndex + 1}, 현재 라운드 {stageManager.Round}");

        //스테이지 상승 후 Round 1로 초기화 및 StartScene로
        if (stageManager.Round > 5)
        {
            stageManager.Round = 1;

            GameSceneManager.Instance.LoadSceneAsync("StageScene");
            yield return null;
        }
        else
        {
            //끝났다면 업그레이드 상점으로 이동
            GameSceneManager.Instance.LoadScene("UpgradeStoreScene");
        }

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

        stageManager.Round = 1;

        GameSceneManager.Instance.LoadScene("GameOverScene");
        //SceneManager.LoadScene("StartScene");
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

        AttDamage(blood);
        player.UpdateHpShildSet();
    }

    void AttDamage(float att1)
    {
        if (enemy.shild > 0 && enemy.shild >= att1)
        {
            enemy.shild -= att1;
        }
        else if (enemy.shild > 0 && enemy.shild < att1)
        {
            enemy.hp -= (att1 - enemy.shild);
            enemy.shild = 0;
        }
        else
        {
            enemy.hp -= att1;
        }
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
        stageManager = FindAnyObjectByType<StageManager>();
        //enemyManager = FindAnyObjectByType<EnemyManager>();
        
        enemyManager = EnemyManager.Instance;
        itemManager = ItemManager.Instance;
        player = FindFirstObjectByType<Player>();
        //int r = Random.Range(0, enemyObjects.Length);

        allItemDatas = itemManager.allItemDatas;

        if (stageManager != null)
        {
            currentStageIndex = stageManager.SelectedStage  - 1 ;
        }

        //스테이지 값 받아서 해당 스테이지 몬스터만 출현
        //int r = currentStageIndex;
        //Enemy[] enemies = new Enemy[enemyObjects.Length];
        //enemies[r] = FindAnyObjectByType<Enemy>();
        //enemy = enemies[r];
        //EnemyCreate(currentStageIndex);

        enemyManager.SpawnEnemy(currentStageIndex, stageManager.Round);

        enemy = enemyManager.currentEnemy;

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