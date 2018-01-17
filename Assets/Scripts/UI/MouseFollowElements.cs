using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollowElements : MonoBehaviour {

    float xOffset = 10f;
    float zOffset = 10f;

	void Update () {
        Vector3 newPosition = Input.mousePosition;

        newPosition.x += xOffset;
        newPosition.z -= zOffset;

        transform.position = newPosition;
	}
}
