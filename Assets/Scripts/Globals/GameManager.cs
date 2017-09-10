using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Global GameManager exposes information about gamestate & references to GameObjects to prevent expensive Find calls.
/// </summary>
public class GameManager : MonoBehaviour {

    private static GameManager instance = null;

    public static GameManager Instance
    {
        get 
        {
            if (instance == null)
            {
                GameObject managerObject = new GameObject();
                instance = managerObject.AddComponent<GameManager>();
                managerObject.name = "Game Manager Object";
            }
            return instance;
        }
    }

    GameObject playerObject;
    Transform playerTransform;

    void Awake()
    {
        playerObject = GameObject.Find("Manticore");
        playerTransform = playerObject.transform;
    }

    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }

    public Vector3 GetPlayerPosition()
    {
        return playerTransform.position;
    }
}
