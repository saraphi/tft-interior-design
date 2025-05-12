using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource soundSource;

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

    public void PlayPressSound()
    {
        if (pressClip != null) soundSource.PlayOneShot(pressClip);
    }

    public void PlayReleaseSound()
    {
        if (releaseClip != null) soundSource.PlayOneShot(releaseClip);
    }

    public void PlayEnterSound()
    {
        if (enterClip != null) soundSource.PlayOneShot(enterClip);
    }

    public void PlayExitSound()
    {
        if (exitClip != null) soundSource.PlayOneShot(exitClip);
    }

    public void PlayErrorSound()
    {
        if (errorClip != null) soundSource.PlayOneShot(errorClip);
    }

    public void PlayDeleteSound()
    {
        if (deleteClip != null) soundSource.PlayOneShot(deleteClip);
    }
}