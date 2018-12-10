using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEventAudioPlayer : MonoBehaviour {

    AudioSource audioSource;

    [SerializeField]
    AudioClip newMomentumPointClip;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        GlobalEventEmitter.OnGameStateEvent += GameStateEventHandler;
    }

    private void OnDisable()
    {
        GlobalEventEmitter.OnGameStateEvent -= GameStateEventHandler;
    }

    void GameStateEventHandler(GlobalConstants.GameStateEvents gameStateEvent, string eventInformation)
    {
        switch (gameStateEvent)
        {
            case GlobalConstants.GameStateEvents.NewMomentumPoint:
                PlayAudioClip(newMomentumPointClip);
                return;
            default:
                return;
        }
    }

    void PlayAudioClip(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
