using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    const string SPAWN_POINT_ID = "SpawnPoint";

    Transform spawnPoint;
    public Transform SpawnPoint { get { return spawnPoint; } }

    private void Awake()
    {
    }

    private void OnEnable()
    {
        spawnPoint = transform.Find(SPAWN_POINT_ID);
        GameManager.RegisterCurrentLevel(this);
    }
}
