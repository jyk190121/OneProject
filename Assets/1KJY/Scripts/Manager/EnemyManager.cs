using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 스테이지별 몬스터 설정
/// </summary>
public class EnemyManager : MonoBehaviour
{
    public Image hpBar;
    public Image shildBar;

    public GameObject[] enemyEffets;                          // 사용할 이팩트
    public static EnemyManager Instance { get; private set; } // 싱글톤 선언

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    [System.Serializable]
    public struct StageData
    {
        public string stageName;
        public List<EnemyObject> normalEnemies; // 1~4라운드용 적들
        public EnemyObject bossEnemy;           // 보스
    }

    public List<StageData> stages;  // 인스펙터에서 스테이지별로 적 설정
    public GameObject enemyPrefab;  // 적의 외형이 될 기본 프리팹
    public Transform spawnPoint;    // 몬스터가 나타날 위치

    public Enemy currentEnemy { get; private set; }

    // BattleManager가 호출할 함수
    public void SpawnEnemy(int stageIdx, int round)
    {
        // 1. 단계 데이터 존재 확인
        if (stages == null || stageIdx >= stages.Count)
        {
            Debug.LogError($"Stage {stageIdx} 데이터가 EnemyManager에 설정되지 않았습니다!");
            return;
        }

        EnemyObject targetData;
        StageData currentStageData = stages[stageIdx];

        // 2. 라운드 데이터 존재 확인
        if (round <= 4)
        {
            if (currentStageData.normalEnemies == null || currentStageData.normalEnemies.Count == 0)
            {
                Debug.LogError($"{stageIdx}번 스테이지의 일반 몬스터 리스트가 비어있습니다!");
                return;
            }
            int enemyIdx = (round - 1) % currentStageData.normalEnemies.Count;
            targetData = currentStageData.normalEnemies[enemyIdx];
        }
        else
        {
            targetData = currentStageData.bossEnemy;
        }

        if (targetData == null)
        {
            Debug.LogError("소환하려는 EnemyObject(SO)가 할당되지 않았습니다!");
            return;
        }
        enemyPrefab = targetData.enemyPrefab;

        // 3. 프리팹 및 스폰포인트 확인
        if (enemyPrefab == null || spawnPoint == null)
        {
            Debug.Log("EnemyPrefab 또는 SpawnPoint가 EnemyManager에 할당되지 않았습니다!");
            return;
        }

        GameObject obj = Instantiate(enemyPrefab, spawnPoint);
        currentEnemy = obj.GetComponent<Enemy>();
        obj.transform.position = Vector3.zero;
        if (targetData.enemyName == "Butcher" || targetData.enemyName == "CrystalGolem") { obj.transform.position = Vector3.down; }
        obj.transform.localScale = new Vector3(-1, 1, 1);

        // Enemy.cs에 만든 Setup 함수 호출 (데이터 주입)
        currentEnemy.Setup(targetData);
    }
}
