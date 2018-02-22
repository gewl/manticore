using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Modifiers/BaseModifier")]
public class Modifier : ScriptableObject {

    public ModifierType modifierType;
    public float baseDuration;

    public GameObject modifierEffectParticles;
    GameObject instantiatedParticles;

    EntityModifierHandler modifierHandler;
    float durationRemaining;
    public float DurationRemaining { get { return durationRemaining; } }

    public void Init(EntityModifierHandler _modifierHandler)
    {
        modifierHandler = _modifierHandler;
        durationRemaining = baseDuration;

        Transform modifierTarget = modifierHandler.transform;

        instantiatedParticles = Instantiate(modifierEffectParticles, modifierTarget.position, Quaternion.identity, modifierTarget);
        ParticleSystem instantiatedParticleSystem = instantiatedParticles.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule mainParticleSystem = instantiatedParticleSystem.main;

        mainParticleSystem.duration = baseDuration;
        instantiatedParticleSystem.Play();
    }

    public void UpdateModifierDuration(float deltaTime)
    {
        durationRemaining -= deltaTime;

        if (durationRemaining <= 0.0f)
        {
            modifierHandler.DeregisterModifier(this);
        }
    }
}
