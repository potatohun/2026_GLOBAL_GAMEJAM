using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Audio_Set
{
    public string code;
    public AudioClip clip;
}

public enum EAudioMixerType { Master, BGM, SFX }
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public List<Audio_Set> BGM_list;
    public List<Audio_Set> SFX_list;

    private Dictionary<string, AudioClip> sfxDict;
    private Dictionary<string, AudioClip> bgmDict;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource BGM_AudioSource;
    [SerializeField] private AudioSource SFX_AudioSource;

    private bool[] isMute = new bool[3];
    private float[] audioVolumes = new float[3];
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        sfxDict = new Dictionary<string, AudioClip>();
        foreach (var sfx in SFX_list)
            sfxDict[sfx.code] = sfx.clip;

        bgmDict = new Dictionary<string, AudioClip>();
        foreach (var bgm in BGM_list)
            bgmDict[bgm.code] = bgm.clip;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayClickSound()
    {
        SFX_AudioSource.PlayOneShot(sfxDict["UI_Click"]);
    }

    public void PlayOptionSound()
    {
        SFX_AudioSource.PlayOneShot(sfxDict["UI_Popup_Open"]);
    }

    public void PlayCloseOptionSound()
    {
        SFX_AudioSource.PlayOneShot(sfxDict["UI_Popup_Close"]);
    }

    public void PlayKnifeSound()
    {
        SFX_AudioSource.PlayOneShot(sfxDict["SFX_Ingame_ShapeCut"]);
    }


    public void PlayInGameBGM()
    {
        BGM_AudioSource.PlayOneShot(bgmDict["BGM_Ingame_Loop"]);
    }

    public void PlayTitleBGM()
    {
        BGM_AudioSource.PlayOneShot(bgmDict["BGM_Title_Loop"]);
    }

    public void StopBGM()
    {
        BGM_AudioSource.Stop();
    }

    public void PauseBGM()
    { 
        BGM_AudioSource.Pause(); 
    }

   
    public void SetAudioVolume(EAudioMixerType audioMixerType, float volume)
    {
        // 오디오 믹서의 값은 -80 ~ 0까지이기 때문에 0.0001 ~ 1의 Log10 * 20을 한다.
        audioMixer.SetFloat(audioMixerType.ToString(), Mathf.Log10(volume) * 20);
    }

    public void SetAudioMute(EAudioMixerType audioMixerType)
    {
        int type = (int)audioMixerType;
        if (!isMute[type]) // 뮤트 
        {
            isMute[type] = true;
            audioMixer.GetFloat(audioMixerType.ToString(), out float curVolume);
            audioVolumes[type] = curVolume;
            SetAudioVolume(audioMixerType, 0.001f);
        }
        else
        {
            isMute[type] = false;
            SetAudioVolume(audioMixerType, audioVolumes[type]);
        }
    }
}