using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy Data")]
public class EnemyObject : ScriptableObject
{
    [Header("Enemy Info")]
    public string enemyName;
    public GameObject enemyPrefab; // 적의 외형 프리팹 (필요시)

    [Header("Base Stack")]
    public float maxHp;
    public float maxSh;
    [Header("물리공격력")]
    public float minAtt1;
    public float baseAtt1;
    [Header("마법공격력")]
    public float minAtt2;
    public float baseAtt2;
    [Header("회복")]
    public float recovery;
    public float heal;

    // 필요하다면 특정 적 전용 사운드나 이펙트도 여기에 추가
    // public AudioClip crySound;
    //public GameObject[] enemyEffets; // 사용할 이팩트
}