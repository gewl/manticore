using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fracture : MonoBehaviour
{
    FractureHardware fractureHardware;
    Rigidbody fractureRigidbody;

    public float TimeToStop;
    public float TimeToDestroy;

    float timeElapsed = 0.0f;
    bool hasStopped = false;

    int NumberOfProjectiles { get { return fractureHardware.ActiveNumberOfProjectiles; } }

    Vector3 initialDirection;
    float initialSpeed;

    const string TERRAIN_LAYER_ID = "Terrain";
    LayerMask terrainLayer;

    private void Awake()
    {
        terrainLayer = LayerMask.NameToLayer(TERRAIN_LAYER_ID);
        fractureRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        initialDirection = fractureRigidbody.velocity.normalized;
        initialSpeed = fractureRigidbody.velocity.magnitude;
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed < TimeToStop)
        {
            float percentageToStop = (timeElapsed / TimeToStop);

            float updatedSpeed = initialSpeed * GameManager.EaseOutCurve.Evaluate(percentageToStop);
            fractureRigidbody.velocity = initialDirection * updatedSpeed;
        }
        else if (!hasStopped)
        {
            Stop();
        }
        else if (timeElapsed >= TimeToDestroy)
        {
            Destroy(gameObject);
        }
    }

    void Stop()
    {
        hasStopped = true;
        timeElapsed = TimeToStop;
        fractureRigidbody.velocity = Vector3.zero;
        fractureRigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void PassReferenceToHardware(FractureHardware _fractureHardware)
    {
        fractureHardware = _fractureHardware;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == terrainLayer)
        {
            Stop();
            return;
        }

        ContactPoint contact = collision.contacts[0];
        Vector3 impactNormal = contact.normal * -1f;
        Vector3 impactPoint = contact.point;

        fractureHardware.FractureBullet(impactPoint, impactNormal);

        Destroy(collision.gameObject);
    }
}
