using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicManager : MonoBehaviour {

    [SerializeField]
    float fadeTime = 0.5f;

    AudioSource audioSource;
    float initialVolume;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        initialVolume = audioSource.volume;
    }

    public void ChangeMusic(AudioClip audioClip)
    {
        StopAllCoroutines();

        StartCoroutine(FadeMusic(audioClip));
    }

    IEnumerator FadeMusic(AudioClip newClip)
    {
        if (newClip == audioSource.clip)
        {
            yield break;
        }

        bool hasToFadeOut = audioSource.clip != null;

        float timeElapsed = 0.0f;

        while (hasToFadeOut && timeElapsed < fadeTime)
        {
            float percentageComplete = timeElapsed / fadeTime;

            float newVolume = Mathf.Lerp(initialVolume, 0.0f, percentageComplete);

            audioSource.volume = newVolume;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        audioSource.clip = newClip;
        audioSource.Play();
        timeElapsed = 0.0f;

        if (newClip == null)
        {
            yield break;
        }

        while (timeElapsed < fadeTime)
        {
            float percentageComplete = timeElapsed / fadeTime;

            float newVolume = Mathf.Lerp(0.0f, initialVolume, percentageComplete);

            audioSource.volume = newVolume;

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
