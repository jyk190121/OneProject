using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Header("Upgrade Levels")]
    public int[] upgradeLevels = new int[8]; // 0~7단계 관리

    // 강화 수치를 저장할 변수들
    public float maxHP = 0;
    public float physicalAtkBonus = 0; // 물리공격력 % 단위 (예: 0.25f = 25%)
    public float magicAtkBonus = 0;    // 마법공격력
    public float physicalResist = 0;   // 물리저항력
    public float magicResist = 0;      // 마법저항력
    public float goldBonus = 0;        // 골드추가률 % 단위
    public bool hasRevive = false;     // 부활권 소유여부
    public bool hasRings = false;      // 모든링을 모았는지 (ItemManager를 통해 가져올 수 있음)
    public bool enemyHalf = false;     // 반지셋 구매여부
    public int chip;                   // 모드구매 화폐

    public GameObject reviveEffect;    // 부활 이펙트

    void Awake()
    {
        // 싱글톤 설정 및 씬 전환 시 파괴 방지
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPlayerData(); // 시작 시 데이터 로드
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //테스트용 치트
    void Update()
    {
        if(Keyboard.current.f7Key.wasPressedThisFrame == true)
        {
            GetChip(500);
            GameSceneManager.Instance.RestartScene();
        }
    }

    // 데이터를 PlayerPrefs에 저장 (강화 성공 시마다 호출 권장)
    public void SavePlayerData()
    {
        for (int i = 0; i < upgradeLevels.Length; i++)
        {
            PlayerPrefs.SetInt($"UpgradeLevel_{i}", upgradeLevels[i]);
        }

        PlayerPrefs.SetFloat("maxHP", maxHP);
        PlayerPrefs.SetFloat("PhysicalAtkBonus", physicalAtkBonus);
        PlayerPrefs.SetFloat("MagicAtkBonus", magicAtkBonus);
        PlayerPrefs.SetFloat("PhysicalResist", physicalResist);
        PlayerPrefs.SetFloat("MagicResist", magicResist);
        PlayerPrefs.SetFloat("GoldBonus", goldBonus);

        // bool 타입은 0(false)과 1(true)로 저장
        PlayerPrefs.SetInt("HasRevive", hasRevive ? 1 : 0);
        PlayerPrefs.SetInt("HasRings", hasRings ? 1 : 0);
        PlayerPrefs.SetInt("EnemyHalf", enemyHalf ? 1 : 0);

        PlayerPrefs.SetInt("ChipCount", chip);

        PlayerPrefs.Save(); // 디스크에 즉시 기록
        Debug.Log("플레이어 데이터가 저장되었습니다.");
    }

    // 데이터를 PlayerPrefs에서 로드
    public void LoadPlayerData()
    {
        // 1. 단계 로드
        for (int i = 0; i < upgradeLevels.Length; i++)
        {
            upgradeLevels[i] = PlayerPrefs.GetInt($"UpgradeLevel_{i}", 0);
        }

        maxHP = PlayerPrefs.GetFloat("maxHP", 0);
        physicalAtkBonus = PlayerPrefs.GetFloat("PhysicalAtkBonus", 0);
        magicAtkBonus = PlayerPrefs.GetFloat("MagicAtkBonus", 0);
        physicalResist = PlayerPrefs.GetFloat("PhysicalResist", 0);
        magicResist = PlayerPrefs.GetFloat("MagicResist", 0);
        goldBonus = PlayerPrefs.GetFloat("GoldBonus", 0);

        hasRevive = PlayerPrefs.GetInt("HasRevive", 0) == 1;
        hasRings = PlayerPrefs.GetInt("HasRings", 0) == 1;
        enemyHalf = PlayerPrefs.GetInt("EnemyHalf", 0) == 1;

        chip = PlayerPrefs.GetInt("ChipCount", 0);

        Debug.Log("플레이어 데이터를 로드했습니다.");
    }

    // 초기화가 필요한 경우 사용
    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        LoadPlayerData();
    }

    public void GetChip(int count)
    {
        chip += count;
        print($"칩 획득 : {count}");
        //GameSceneManager.Instance.RestartScene();
        SavePlayerData();
    }
}

public static class MathExtensions
{
    // float을 소수점 없이 문자열로 변환하는 확장 메서드
    public static string ToIntString(this float value)
    {
        return value.ToString("F0");
    }
}