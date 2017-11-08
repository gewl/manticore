using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovingTarget : MonoBehaviour {

    Rigidbody entityRigidbody;
    [SerializeField]
    float speed = 10f;
    [SerializeField]
    Transform targetWaypoint;

    void Awake()
    {
        entityRigidbody = GetComponent<Rigidbody>();

        SetVelocity();
    }

    void FixedUpdate()
    {
        SetVelocity();
    }

    void SetVelocity()
    {
        Vector3 toTarget = targetWaypoint.position - transform.position;
        toTarget = toTarget.normalized * speed;
        entityRigidbody.velocity = toTarget;
    }
}
