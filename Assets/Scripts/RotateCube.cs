using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCube : MonoBehaviour {

    [SerializeField]
    float rotationSpeed = 1f;

	void Update () {
        float speed = Time.deltaTime * rotationSpeed;
        transform.Rotate(speed, speed, speed);
	}
}
