using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Audio
{
    public string name;         // 키값
    public AudioClip clip;      // 재생할 오디오 파일

    [Range(0f, 1f)]
    public float volume;        // 볼륨
    [Range(0f, 2f)]
    public float pitch;         // 피치 (재생속도, 높낮이)

    public bool loop;           // 반복 재생

    [HideInInspector]
    public AudioSource source;  // 실제 재생할 오디오소스 
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager audioManager { get; private set; }

    [Header("사운드 목록")]
    [Tooltip("여기에 사운드를 추가!")]
    public Audio[] audios;

    Dictionary<string, Audio> audioDic;

    [Header("BGM 설정")]
    [Range(0f, 1f)]
    public AudioSource bgmSource;
    string currentBGM = "";

    [Header("볼륨 설정")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    public float bgmVolume = 1f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    void Awake()
    {
        // 싱글톤 초기화
        if (audioManager == null)
        {
            audioManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 딕셔너리 초기화
        audioDic = new Dictionary<string, Audio>();
        // 모든 사운드에 AudioSource 추가
        foreach (Audio s in audios)
        {
            // 빈 사운드 오브젝트를 자식으로 생성하여 오디오소스 추가관리
            GameObject soundObject = new GameObject("Audio_" + s.name);
            soundObject.transform.SetParent(transform);

            s.source = soundObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            // 딕셔너리에 추가
            audioDic.Add(s.name, s);
        }

        // BGM 전용 오디오소스 생성
        GameObject bgmObject = new GameObject("BGM");
        bgmObject.transform.SetParent(transform);
        bgmSource = bgmObject.AddComponent<AudioSource>();
        bgmSource.loop = true;

        print($"총 {audios.Length}개 로드됨");

    }

    ///// <summary>
    ///// 효과음 재생 (루프는 기본으로 안된다)
    ///// </summary>
    ///// <param name="name"></param>
    //public void Play(string name, float volumeScale = 1f)
    //{
    //    // 딕셔너리에서 사운드 찾기
    //    if (!soundDictionary.ContainsKey(name))
    //    {
    //        // BGM1, bmg1, Bgm1 정확한 키값이 필요하다
    //        print($"사운드 '{name}'를 찾을 수 없음");
    //        return;
    //    }

    //    // 딕셔너리에서 키값으로 찾아서 실제 클래스를 넘겨받음
    //    Sound sound = soundDictionary[name];

    //    // 볼륨 설정
    //    sound.source.volume = masterVolume * sfxVolume * sound.volume * volumeScale;

    //    // 플레이
    //    sound.source.Play();

    //    print($"효과음 재생: {name}");
    //}

    /// <summary>
    /// 효과음 정지 (안쓸수 있음)
    /// </summary>
    /// <param name="name"></param>
    //public void Stop(string name)
    //{
    //    // 딕셔너리에서 사운드 찾기
    //    if (!soundDictionary.ContainsKey(name))
    //    {
    //        // BGM1, bmg1, Bgm1 정확한 키값이 필요하다
    //        print($"사운드 '{name}'를 찾을 수 없음");
    //        return;
    //    }

    //    // 딕셔너리에서 오디오소스를 정지시키면 끝!!
    //    soundDictionary[name].source.Stop();
    //}

    /// <summary>
    /// BGM 재생
    /// </summary>
    /// <param name="name"></param>
    public void PlayBGM(string name, float volumeScale = 1f)
    {
        // 딕셔너리에서 사운드 찾기
        if (!audioDic.ContainsKey(name))
        {
            // BGM1, bmg1, Bgm1 정확한 키값이 필요하다
            print($"사운드 '{name}'를 찾을 수 없음");
            return;
        }

        // 이미 같은 BGM이 재생 중이라면 리턴
        if (currentBGM == name && bgmSource.isPlaying)
        {
            print($"BGM '{name}'은 이미 재생 중입니다.");
            return;
        }

        // 딕셔너리에서 키값으로 찾아서 실제 클래스를 넘겨받음
        Audio bgm = audioDic[name];

        bgmSource.clip = bgm.clip;
        bgmSource.volume = masterVolume * bgmVolume * bgm.volume * volumeScale;
        bgmSource.Play();

        currentBGM = name;
        print($"BGM 재생: {name}");
    }

    /// <summary>
    /// BGM 정지
    /// </summary>
    public void StopBGM()
    {
        bgmSource.Stop();
        currentBGM = "";
        print("BGM 정지");
    }

    /// <summary>
    /// BGM 페이드 아웃 후 정지
    /// </summary>
    /// <param name="duration"></param>
    public void StopFadeOutBGM(float duration = 1f)
    {
        StartCoroutine(FadeOut(duration));
    }

    IEnumerator FadeOut(float duration)
    {
        float startVolume = bgmSource.volume;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }
        // BGM 정지 후 볼륨 원상복귀
        bgmSource.Stop();
        bgmSource.volume = startVolume;
        currentBGM = "";

        print("BGM 페이드 아웃 완료");
    }

    /// <summary>
    /// BGM 페이드 인 후 플레이
    /// </summary>
    /// <param name="name"></param>
    /// <param name="duration"></param>
    public void PlayFadeInBGM(string name, float duration = 1f)
    {
        StartCoroutine(FadeInBGM(name, duration));
    }

    IEnumerator FadeInBGM(string name, float duration)
    {
        // 키값으로 BGM을 못찾으면 그대로 기능 안함
        if (!audioDic.ContainsKey(name))
        {
            print($"BGM {name}을 찾을 수 없음");
            yield break;
        }

        Audio bgm = audioDic[name];
        bgmSource.clip = bgm.clip;
        bgmSource.volume = 0f;
        bgmSource.Play();

        float targetVolume = masterVolume * bgmVolume * bgm.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / duration);
            yield return null;
        }

        currentBGM = name;
        print($"BGM 페이드 인 완료: {name}");
    }

    /// <summary>
    /// BGM 크로스 페이드 (A -> B로 부드럽게 전환)
    /// </summary>
    /// <param name="name"></param>
    /// <param name="duration"></param>
    public void CrossFadeBGM(string name, float duration = 2f)
    {
        StartCoroutine(CrossFade(name, duration));
    }

    IEnumerator CrossFade(string name, float duration)
    {
        // 키값으로 BGM을 못찾으면 그대로 기능 안함
        if (!audioDic.ContainsKey(name))
        {
            print($"BGM {name}을 찾을 수 없음");
            yield break;
        }

        // 페이드 아웃 시작
        float elapsed = 0f;
        float startVolume = bgmSource.volume;

        // 임시로 생성할 BGM용 오디오소스
        AudioSource tempSource = gameObject.AddComponent<AudioSource>();
        Audio newAudio = audioDic[name];
        tempSource.clip = newAudio.clip;
        tempSource.volume = 0f;
        tempSource.loop = true;
        tempSource.Play();

        float targetVolume = masterVolume * bgmVolume * newAudio.volume;

        // 크로스 페이드
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // 이전 BGM 페이드 아웃
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            // 새 BGM 페이드 인
            tempSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / duration);

            yield return null;
        }

        // 이전 BGM 정지
        bgmSource.Stop();

        // 새 BGM을 메인 BGM으로 교체
        bgmSource.clip = tempSource.clip;
        bgmSource.volume = tempSource.volume;
        bgmSource.Play();

        // 임시 오디오소스 삭제
        Destroy(tempSource);

        currentBGM = name;
        print($"BGM 크로스페이드 완료: {name}");
    }

    /// <summary>
    /// 마스터 볼륨 설정
    /// </summary>
    /// <param name="volume"></param>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);

        if (bgmSource.isPlaying && !string.IsNullOrEmpty(currentBGM))
        {
            Audio bgm = audioDic[currentBGM];
            bgmSource.volume = masterVolume * bgmVolume * bgm.volume;
        }
    }

    /// <summary>
    /// BGM 볼륨 설정
    /// </summary>
    /// <param name="volume"></param>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);

        if (bgmSource.isPlaying && !string.IsNullOrEmpty(currentBGM))
        {
            Audio bgm = audioDic[currentBGM];
            bgmSource.volume = masterVolume * bgmVolume * bgm.volume;
        }
    }

    public void SetBGMOnlyVol(float volume)
    {
        bgmSource.volume = volume;
    }

    ///// <summary>
    ///// 효과음 볼륨 설정
    ///// </summary>
    ///// <param name="volume"></param>
    //public void SetSFXVolume(float volume)
    //{
    //    sfxVolume = Mathf.Clamp01(volume);
    //    // 효과음은 재생시 볼륨이 결정되므로 자동으로 적용된다
    //}

    /// <summary>
    /// 사운드가 재생 중인지 확인
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool IsPlaying(string name)
    {
        if (!audioDic.ContainsKey(name)) return false;

        return audioDic[name].source.isPlaying;
    }

    /// <summary>
    /// 현재 재생 중인 BGM 이름 반환
    /// </summary>
    /// <returns></returns>
    public string GetCurrentBGM()
    {
        return currentBGM;
    }
}