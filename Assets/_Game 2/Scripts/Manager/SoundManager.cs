using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField]
    AudioSource AudioSourceBackground;

    [SerializeField]
    AudioSource AudioSourceEffect;

    private void Awake()
    {
        Instance = this;
    }
    public void playSound(AudioClip clip)
    {
        AudioSourceEffect.clip = clip;
        AudioSourceEffect.Play();
    }
    public void stopSound()
    {
        AudioSourceEffect.Stop();
    }
}
