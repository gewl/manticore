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

    //private void OnCollisionEnter(Collision co)
    //{
    //    if (co.gameObject.tag == "BouncyWall") 
    //    {
    //        Debug.Log(co.contacts[0].normal);
    //        rigidbody.velocity = Vector3.Reflect(rigidbody.velocity, co.contacts[0].normal);
    //    }
    //}
}
