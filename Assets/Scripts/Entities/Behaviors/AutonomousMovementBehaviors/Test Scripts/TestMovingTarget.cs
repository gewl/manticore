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
        if (toTarget.sqrMagnitude < 0.1f)
        {
            entityRigidbody.velocity = Vector3.zero;
            return;
        }
        toTarget = toTarget.normalized * speed;
        entityRigidbody.velocity = toTarget;
    }
}
