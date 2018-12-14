using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour {

    [SerializeField]
    AudioClip backgroundMusic;

    BackgroundMusicManager backgroundMusicManager;

    private void Awake()
    {
        backgroundMusicManager = Camera.main.GetComponentInChildren<BackgroundMusicManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        backgroundMusicManager.ChangeMusic(backgroundMusic);
    }

}
