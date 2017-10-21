using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleLifetime : MonoBehaviour {

    ParticleSystem particleSystem;

    float duration;
    float timeElapsed = 0.0f;

    void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        duration = particleSystem.main.duration;
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > duration)
        {
            Destroy(gameObject);
        }
    }
}
