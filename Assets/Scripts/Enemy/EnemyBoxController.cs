using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoxController : MonoBehaviour {

    int nextBulletTimer = 100;

	void Start () 
    {
		
	}
	
	void Update () 
    {
        nextBulletTimer--;

        if (nextBulletTimer == 0)
        {
            nextBulletTimer = 100;
        }
    }
}
