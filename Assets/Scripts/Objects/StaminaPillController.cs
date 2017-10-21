using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaPillController : MonoBehaviour {

    [SerializeField]
    float recoveryAmount;
    [SerializeField]
    float lifeDuration = 1.0f;
    [SerializeField]
    float bounceDuration = 0.5f;
    [SerializeField]
    float timeBetweenBlinks = 1f;
    [SerializeField]
    PhysicMaterial stillMaterial;

    [SerializeField]
    AudioClip collectionClip;
    [SerializeField]
    GameObject collectionParticles;

    bool bouncing = true;
    Collider collider;
    Renderer renderer;
    float blinkPoint;

    void Awake()
    {
        collider = GetComponent<Collider>();
        renderer = GetComponent<Renderer>();
        blinkPoint = lifeDuration / 2f;

        StartCoroutine("PillLifecycle");
    }

    void ToggleRenderer()
    {
        if (renderer.enabled == true)
        {
            renderer.enabled = false;
        }
        else
        {
            renderer.enabled = true;
        }
    }

    IEnumerator PillLifecycle()
    {
        float timeElapsed = 0.0f;
        float blinkTimer = 0.0f;

        while (timeElapsed <= lifeDuration)
        {
            timeElapsed += Time.smoothDeltaTime;

            if (bouncing && timeElapsed >= bounceDuration)
            {
                bouncing = false;
                collider.material = stillMaterial;
            }
            else if (timeElapsed >= blinkPoint)
            {
                blinkTimer += Time.deltaTime;
                if (blinkTimer >= timeBetweenBlinks)
                {
                    ToggleRenderer();
                    blinkTimer = 0.0f;
                }
            }

            yield return null;
        }
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject collisionObject = collision.gameObject;
        if (collisionObject.CompareTag("Player"))
        {
            collisionObject.GetComponent<ManticoreAudioComponent>().PlayOutsideSound(collectionClip);
            collisionObject.GetComponent<EntityStaminaComponent>().ChangeStamina(recoveryAmount);
            Instantiate(collectionParticles, collisionObject.transform.position, Quaternion.Euler(-90f, 0f, 0f), collisionObject.transform);
            Destroy(gameObject);
        }
    }
}
