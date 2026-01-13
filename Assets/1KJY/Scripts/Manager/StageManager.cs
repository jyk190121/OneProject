using System;
using UnityEngine;
/// <summary>
/// 현재 해금된 스테이지 값저장
/// </summary>
public class StageManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static StageManager Instance { get; private set; }

    // 데이터 변경 시 UI 등에 알림을 주기 위한 이벤트
    public static event Action<int> OnStageUnlocked;

    public int UnlockedStage { get; private set; }  //  현재 해금된 최대 스테이지
    public int SelectedStage { get; set; }          // 실제 플레이 중인 스테이지

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // 싱글톤 설정 및 씬 전환 시 파괴 방지
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadData()
    {
        UnlockedStage = PlayerPrefs.GetInt("UnlockedStageIndex", 1);
    }

    // 스테이지 클리어 시 호출
    public void UnlockNextStage(int stageNum)
    {
        if (stageNum > UnlockedStage)
        {
            UnlockedStage = stageNum;
            PlayerPrefs.SetInt("UnlockedStageIndex", UnlockedStage);
            PlayerPrefs.Save();

            OnStageUnlocked?.Invoke(UnlockedStage);
        }
    }

    // 선택한 스테이지가 해금된 상태인지 (스테이지 씬에서 체크)
    //public bool SelectStage(int stageNum)
    //{
    //    bool isLocked = stageNum > UnlockedStage;

    //    return isLocked;
    //}

}

