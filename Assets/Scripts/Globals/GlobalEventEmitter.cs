using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEventEmitter : MonoBehaviour {

    public delegate void GameStateEventDelegate(GlobalConstants.GameStateEvents gameStateEvent, string eventInformation = "");
    public static GameStateEventDelegate FireGameStateEvent;
}
