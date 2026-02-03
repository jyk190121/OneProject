using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    // 강화 수치를 저장할 변수들
    public float maxHP = 0;
    public float physicalAtkBonus = 0; // % 단위 (예: 0.25f = 25%)
    public float magicAtkBonus = 0;
    public float physicalResist = 0;   // % 단위
    public float magicResist = 0;
    public float goldBonus = 0;
    public bool hasRevive = false;     // 부활권 소유여부
    public int chip;                   // 모드구매 화폐
    public bool hasRings = false;      // 모든링을 모았는지 (ItemManager를 통해 가져올 수 있음)
    public bool enemyHalf = false;     // 반지셋 구매여부

    void Awake()
    {
        // 싱글톤 설정 및 씬 전환 시 파괴 방지
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
