using System;
using UnityEngine;


public enum SoundType
{
    missHit,
    goodHit,
    PerfectHit,
}
[RequireComponent(typeof(AudioSource))]

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance { get; private set; }
    private AudioSource audioSource;

    [SerializeField] private AudioClip[] sounds;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);   // optional
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType sound, float volume = 1.0f)
    {
        if(GameManager.Instance.IsGameOver) return;
        if (Instance == null || Instance.audioSource == null) return;

        switch(sound)
        {
            case SoundType.missHit:
                Instance.audioSource.PlayOneShot(Instance.sounds[0], volume);
                break;
            case SoundType.goodHit:
                Instance.audioSource.PlayOneShot(Instance.sounds[1], volume);
                break;
            case SoundType.PerfectHit:
                Instance.audioSource.PlayOneShot(Instance.sounds[2], volume);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(sound), sound, null);
        }

    }

}


