using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fracture : MonoBehaviour
{
    FractureHardware fractureHardware;
    LayerMask enemyBulletLayer;
    Rigidbody rigidbody;

    float timeToStop = 0.5f;
    float timeToDestroy = 0.51f;

    float timeElapsed = 0.0f;
    float destroyTime;
    bool hasStopped = false;

    int NumberOfProjectiles { get { return fractureHardware.ActiveNumberOfProjectiles; } }

    Vector3 initialDirection;
    float initialSpeed;

    private void Awake()
    {
        enemyBulletLayer = LayerMask.NameToLayer("EnemyBullet");
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        initialDirection = rigidbody.velocity.normalized;
        initialSpeed = rigidbody.velocity.magnitude;
        destroyTime = Time.time + timeToDestroy;
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed < timeToStop)
        {
            float percentageToStop = (timeElapsed / timeToStop);

            float updatedSpeed = initialSpeed * GameManager.EaseOutCurve.Evaluate(percentageToStop);
            rigidbody.velocity = initialDirection * updatedSpeed;
        }
        else if (!hasStopped)
        {
            hasStopped = true;
            rigidbody.velocity = Vector3.zero;
        }

        if (Time.time >= destroyTime)
        {
            Destroy(gameObject);
        }
    }

    public void PassReferenceToHardware(FractureHardware _fractureHardware)
    {
        fractureHardware = _fractureHardware;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Vector3 impactNormal = contact.normal * -1f;
        Vector3 impactPoint = contact.point;

        fractureHardware.FractureBullet(impactPoint, impactNormal);

        Destroy(collision.gameObject);
    }
}
