using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueMenuController : MonoBehaviour {

    MenuManager menuManager;

    private void Awake()
    {
        menuManager = GetComponentInParent<MenuManager>();
    }

    void Start () {
		
	}
	
	void Update () {
		
	}
}
