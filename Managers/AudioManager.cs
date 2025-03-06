using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{

    private AudioSource bgmSource;
    private List<AudioSource> sfxSources = new List<AudioSource>();
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
    [SerializeField] private float bgmVolume = 0.4f;
    [SerializeField] private float sfxVolume = 1.0f;
    private const int MAX_SFX_SOURCES = 5;
    public void InitAudioSources()
    {
        // ��ʼ����������Դ
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;

        // ��ʼ����ЧԴ��
        for (int i = 0; i < MAX_SFX_SOURCES; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.volume = sfxVolume;
            //source.pitch = 1.2f; // ���ò����ٶ�Ϊ 1.5 ����
            sfxSources.Add(source);
        }
    }

    public void PlayBGM(string clipPath, bool fade = true)
    {
        AudioClip clip = LoadAudioClip(clipPath);
        if (clip == null) return;

        if (fade)
        {
            StartCoroutine(FadeBGM(clip));
        }
        else
        {
            bgmSource.clip = clip;
            bgmSource.Play();
        }
    }

    public void PlaySFX(string clipPath)
    {
        AudioClip clip = LoadAudioClip(clipPath);
        if (clip == null) return;

        // ���ҿ��õ���ЧԴ
        AudioSource source = sfxSources.Find(s => !s.isPlaying);
        if (source == null)
        {
            source = sfxSources[0]; // ���û�п��õģ�ʹ�õ�һ��
        }

        source.clip = clip;
        source.Play();
    }

    private AudioClip LoadAudioClip(string path)
    {
        if (audioClips.ContainsKey(path))
        {
            return audioClips[path];
        }

        AudioClip clip = ResourceManager.Instance.Load<AudioClip>(path);
        if (clip != null)
        {
            audioClips[path] = clip;
        }
        return clip;
    }

    private IEnumerator FadeBGM(AudioClip newClip)
    {
        float fadeTime = 1f;
        float timer = 0;

        if (bgmSource.clip != null)
        {
            // ������ǰ����
            while (timer < fadeTime)
            {
                timer += Time.deltaTime;
                bgmSource.volume = bgmVolume * (1 - timer / fadeTime);
                yield return null;
            }
        }

        bgmSource.clip = newClip;
        bgmSource.Play();
        timer = 0;

        // ����������
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            bgmSource.volume = bgmVolume * (timer / fadeTime);
            yield return null;
        }
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp(volume, 0f, 2.0f);
        foreach (var source in sfxSources)
        {
            source.volume = sfxVolume;
        }
    }
}
