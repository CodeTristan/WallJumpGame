using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f,1f)]
    public float volume = 1;
    [Range(-3f, 3f)]
    public float pitch = 1;
    public bool isLoop = false;
}
public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public AudioSource source;
    [SerializeField] private SoundEffect soundEffectPrefab;
    public bool isMuted;
    public float MusicVolume;
    public float SoundVolume;
    public string CurrentMusic;

    public Sound[] musics;
    public Sound[] sounds;
    public List<SoundEffect> SoundEffects;
    public void Init()
    {
        instance = this;
        source.mute = isMuted;
        SoundEffects = new List<SoundEffect>();
    }
    public void PlayMusic(string name,bool CheckSameMusic = false)
    {
        if ((CheckSameMusic && name == CurrentMusic) || name == "NONE")
            return;
        Sound sound = null;
        foreach (var item in musics)
        {
            if(name == item.name)
                sound = item;
        }

        if(sound == null)
        {
            Debug.LogError("Sound not found --> " +  name);
            return;
        }
        CurrentMusic = name;
        StartCoroutine(MusicNumerator(sound));
    }

    public void PlayMusic(Sound sound)
    {
        if (sound == null)
        {
            Debug.LogError("Sound not found --> " + name);
            return;
        }
        CurrentMusic = name;
        StartCoroutine(MusicNumerator(sound));
    }

    public void StopMusic()
    {
        StartCoroutine(MusicNumerator(null));
    }

    private IEnumerator MusicNumerator(Sound sound)
    {
        for (int i = 0; i <= MusicVolume; i+=10)
        {
            source.volume -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        source.Stop();

        if (sound == null)
        {
            yield break;
        }

        source.pitch = sound.pitch;
        source.loop = sound.isLoop;
        source.clip = sound.clip;

        source.Play();
        for (int i = 0; i <= MusicVolume; i += 10)
        {
            source.volume += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        source.volume = MusicVolume / 100;
    }
    public void PlaySound(string name)
    {
        Sound sound = null;
        foreach (var item in sounds)
        {
            if (name == item.name)
                sound = item;
        }

        if (sound == null)
        {
            Debug.LogError("Sound not found --> " + name);
            return;
        }

        SoundEffect soundeffect = Instantiate(soundEffectPrefab,gameObject.transform);

        soundeffect.time = sound.clip.length;
        soundeffect.isLoop = sound.isLoop;
        soundeffect.source.volume = (SoundVolume / 100) * sound.volume;
        soundeffect.source.pitch = sound.pitch;
        soundeffect.source.loop = sound.isLoop;
        soundeffect.source.clip = sound.clip;

        soundeffect.source.Play();
        soundeffect.Init();
        SoundEffects.Add(soundeffect);
    }

    public void PlaySound(Sound sound)
    {
        //Debug.Log("Playing : " + sound.clip.name);
        SoundEffect soundeffect = Instantiate(soundEffectPrefab, gameObject.transform);

        soundeffect.time = sound.clip.length;
        soundeffect.isLoop = sound.isLoop;
        soundeffect.source.volume = (SoundVolume / 100) * sound.volume;
        soundeffect.source.pitch = sound.pitch;
        soundeffect.source.loop = sound.isLoop;
        soundeffect.source.clip = sound.clip;

        soundeffect.source.Play();
        soundeffect.Init();

        SoundEffects.Add(soundeffect);
    }

    public void KillAllSounds()
    {
        foreach (var item in SoundEffects.ToArray())
        {
            item.kill();
        }
        SoundEffects.Clear();
    }
}
