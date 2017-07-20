using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour {

    private float speed = 25f;
    private Rigidbody bulletRigidbody;

	void Start () {
        bulletRigidbody = GetComponent<Rigidbody>();

        bulletRigidbody.velocity = transform.forward * speed;
	}
	
	void Update () {
        Debug.Log(bulletRigidbody.velocity);
	}

    void Bounce() {
        bulletRigidbody.velocity = new Vector3(bulletRigidbody.velocity.x, 0f, -bulletRigidbody.velocity.z);
    }

}
