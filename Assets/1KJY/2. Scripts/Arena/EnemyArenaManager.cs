using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 스테이지별 몬스터 설정
/// </summary>


[RequireComponent(typeof(EnemyArenaManager))]
public class EnemyArenaManager : MonoBehaviour
{
    public EnemyObject data;

    public GameObject[] enemyEffets;                // 사용할 이팩트

    public Transform spawnPoint;                    // 몬스터가 나타날 위치
    public GameObject enemyPrefab;                  // 적의 외형이 될 기본 프리팹
    public Enemy currentEnemy { get; private set; }
   
    [System.Serializable]
    public struct ArenaRoundConfig // 이름 변경 (구조체 타입)
    {
        public List<EnemyObject> enemyPool; // 무한 라운드 순환용 적들
    }

    public ArenaRoundConfig roundConfig; // 실제 인스펙터에서 할당할 변수

    public void SpawnEnemy(int round)
    {
        // ... (이전 코드와 동일하게 처리하되 roundConfig.enemyPool 참조)
        int enemyIdx = (round - 1) % roundConfig.enemyPool.Count;
        EnemyObject targetData = roundConfig.enemyPool[enemyIdx];

        if (targetData == null) return;
        // 3. 프리팹 및 스폰포인트 확인
        if (enemyPrefab == null || spawnPoint == null)
        {
            Debug.Log("EnemyPrefab 또는 SpawnPoint가 EnemyManager에 할당되지 않았습니다!");
            return;
        }


        GameObject obj = Instantiate(targetData.enemyPrefab, spawnPoint);
        currentEnemy = obj.GetComponent<Enemy>();
        obj.transform.position = Vector3.down;
        obj.transform.localScale = new Vector3(-1, 1, 1);

        // 데이터 주입
        currentEnemy.Setup(targetData);

        // 라운드마다 체력 10%씩 증가 예시
        float multiplier = 1f + (round - 1) * 0.1f;
        currentEnemy.hp *= multiplier;
        //currentEnemy.hp = currentEnemy.maxHp;
        currentEnemy.UpdateUI();
    }
}