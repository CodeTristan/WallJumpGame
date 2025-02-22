using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    public float time;
    public bool isLoop;
    public AudioSource source;

    public void Init()
    {
        if (!isLoop)
        {
            Invoke("kill",time);
        }
    }

    public void kill()
    {
        MusicManager.instance.SoundEffects.Remove(this);
        Destroy(gameObject);
    }
}
