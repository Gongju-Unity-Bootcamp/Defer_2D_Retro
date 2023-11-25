using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [Header("Manager")]
    public static SoundManager instance;

    [Header("Audio Mixers")]
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    [Header("Audio Clips")]
    public AudioClip[] bgmClips;
    public AudioClip[] playerSfxClips;
    public AudioClip[] monsterSfxClips;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // BGM 클립 선택 및 재생
        PlayBGM(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 각 슬라이더에 audioMixer의 값(master, bgm, sfx 볼륨값)을 설정
    /// </summary>
    public void SetSliderValue()
    {
        float masterVolume;

        if (audioMixer.GetFloat("Master", out masterVolume))
        {
            masterSlider.value = Mathf.Pow(10f, masterVolume / 20f);
        }

        float BGMVolume;
        if (audioMixer.GetFloat("BGM", out BGMVolume))
        {
            bgmSlider.value = Mathf.Pow(10f, BGMVolume / 20f);
        }

        float SFXVolume;
        if (audioMixer.GetFloat("SFX", out SFXVolume))
        {
            sfxSlider.value = Mathf.Pow(10f, SFXVolume / 20f);
        }
    }

    /// <summary>
    /// BGM 재생
    /// </summary>
    /// <param name="index"></param>
    public void PlayBGM(int index)
    {
        if (index >= 0 && index < bgmClips.Length)
        {
            bgmSource.clip = bgmClips[index];
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    /// <summary>
    /// SFX 재생
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySFX(string clip)
    {
        switch (clip)
        {
            case "PlayerAttack":
                sfxSource.PlayOneShot(playerSfxClips[0]);
                break;
            case "PlayerHit":
                sfxSource.PlayOneShot(playerSfxClips[1]);
                break;
            case "PlayerMove":
                sfxSource.PlayOneShot(playerSfxClips[2]);
                break;
            case "PlayerJump":
                sfxSource.PlayOneShot(playerSfxClips[3]);
                break;
            case "PlayerDead":
                sfxSource.PlayOneShot(playerSfxClips[4]);
                break;
            case "MonsterAttack":
                sfxSource.PlayOneShot(monsterSfxClips[0]);
                break;
            case "MonsterHit":
                sfxSource.PlayOneShot(monsterSfxClips[1]);
                break;
        }
    }

    /// <summary>
    /// BGM 중지
    /// </summary>
    public void PauseBGM()
    {
        bgmSource.Pause();
    }

    /// <summary>
    /// BGM 이어서 재생
    /// </summary>
    public void ResumeBGM()
    {
        bgmSource.UnPause();
    }

    /// <summary>
    /// 마스터 볼륨 조절
    /// </summary>
    /// <param name="volume"></param>
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }

    /// <summary>
    /// BGM 볼륨 조절
    /// </summary>
    /// <param name="volume"></param>
    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
    }

    /// <summary>
    /// SFX 볼륨 조절
    /// </summary>
    /// <param name="volume"></param>
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }
}
