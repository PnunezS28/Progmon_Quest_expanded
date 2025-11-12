using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioSource sfxPlayer;
    [SerializeField] float fadeDuration=.5f;

    [SerializeField] List<AudioData> sfxList;

    float originalMusicVolume;
    Dictionary<AudioId, AudioData> sfxLookup;

    [SerializeField] float sfxVolume;

    public static AudioManager i { get; private set; }

    private void Awake()
    {
        Debug.Log("AudioManager Awake");

        if (i == null)
        {
            i = this;
        }
        originalMusicVolume = GlobalSettings.i.DefaultBGMVolumen;
        //musicPlayer.volume = GlobalSettings.i.DefaultBGMVolumen;
        sfxVolume = GlobalSettings.i.DefaultSFXVolumen;
        sfxLookup=sfxList.ToDictionary(x=>x.id);
    }

    private void Start()
    {
        Debug.Log("AudioManager Start");
    }

    public void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;
        sfxPlayer.volume = sfxVolume;
        sfxPlayer.PlayOneShot(clip);
    }

    public void PlaySfx(AudioId id)
    {
        if (!sfxLookup.ContainsKey(id)) return;
        var audioClip= sfxLookup[id];
        PlaySfx(audioClip.clip);
    }

    public void PlayMusic(AudioClip clip, bool loop = true, bool fade=false,float volumeOffset=0)
    {
        if (clip == null || musicPlayer.clip==clip) return;

        StartCoroutine(PlayMusicAsync(clip, loop, fade));
    }

    IEnumerator PlayMusicAsync(AudioClip clip, bool loop, bool fade, float volumeOffset = 0)
    {
        if (fade)
        {
            //realizar fade antes de cmabiar
            yield return musicPlayer.DOFade(0, fadeDuration).WaitForCompletion();
        }
        musicPlayer.clip = clip;
        musicPlayer.loop = loop;
        musicPlayer.Play();

        if (fade)
        {
            //realizar fade antes de cmabiar
            yield return musicPlayer.DOFade(originalMusicVolume+volumeOffset, fadeDuration).WaitForCompletion();
        }
        else
        {
            musicPlayer.volume = originalMusicVolume+volumeOffset;
        }
    }

    public void SetBGMVolume(float v)
    {
        originalMusicVolume = v;
        musicPlayer.volume = v;
    }

    public void SetSFXVolume(float v)
    {
        sfxVolume = v;
        sfxPlayer.volume = v;
    }
}

public enum AudioId { Interact, CreatureAttack,CreatureFaint,itemUse,OpenMenu,CloseMenu,getItem,TrainerExclamation,StartCreatureBox,SaveCompleted}