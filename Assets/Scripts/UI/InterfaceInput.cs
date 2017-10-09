using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceInput : MonoBehaviour {

	void Update () {
        if (Input.GetButtonDown("Pause"))
        {
            GameManager.TogglePause();
        }		
	}
}
