using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource soundEffectsSource;
    [SerializeField] private AudioSource ambientMusicSource;

    [Header("Sound Clips")]
    [SerializeField] private AudioClip pressClip;
    [SerializeField] private AudioClip releaseClip;
    [SerializeField] private AudioClip enterClip;
    [SerializeField] private AudioClip exitClip;
    [SerializeField] private AudioClip errorClip;
    [SerializeField] private AudioClip deleteClip;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void PlayPressClip()
    {
        if (pressClip != null) soundEffectsSource.PlayOneShot(pressClip);
    }

    public void PlayReleaseClip()
    {
        if (releaseClip != null) soundEffectsSource.PlayOneShot(releaseClip);
    }

    public void PlayEnterClip()
    {
        if (enterClip != null) soundEffectsSource.PlayOneShot(enterClip);
    }

    public void PlayExitClip()
    {
        if (exitClip != null) soundEffectsSource.PlayOneShot(exitClip);
    }

    public void PlayErrorClip()
    {
        if (errorClip != null) soundEffectsSource.PlayOneShot(errorClip);
    }

    public void PlayDeleteClip()
    {
        if (deleteClip != null) soundEffectsSource.PlayOneShot(deleteClip);
    }
}