using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    const string SPAWN_POINT_ID = "SpawnPoint";

    Transform spawnPoint;
    public Transform SpawnPoint { get { return spawnPoint; } }

    const string LEVEL_ENEMIES_ID = "Enemies";

    private void Awake()
    {
        //if (enemiesPrefab == null)
        //{
        //    Debug.LogError("Enemies prefab not assigned.");
        //}
    }

    private void OnEnable()
    {
        spawnPoint = transform.Find(SPAWN_POINT_ID);

        GameManager.RegisterCurrentLevel(this);

        GlobalEventEmitter.OnGameStateEvent += HandleGameStateEvent;
    }

    private void OnDisable()
    {
        GlobalEventEmitter.OnGameStateEvent -= HandleGameStateEvent;
    }

    void HandleGameStateEvent(GlobalConstants.GameStateEvents gameStateEvent, string eventInformation)
    {
        if (gameStateEvent == GlobalConstants.GameStateEvents.PlayerDied)
        {
        }
    }

    //void ClearAndRespawnEnemies()
    //{
    //    Transform currentEnemies = transform.Find(LEVEL_ENEMIES_ID);

    //    if (currentEnemies != null)
    //    {
    //        Destroy(currentEnemies.gameObject);
    //    }

    //    GameObject newEnemies = Instantiate(enemiesPrefab);
    //    newEnemies.name = LEVEL_ENEMIES_ID;
    //}
}