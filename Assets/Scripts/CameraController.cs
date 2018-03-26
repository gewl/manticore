﻿using System.Collections;
using UnityEngine;
using UnityEngine.PostProcessing;

public class CameraController : MonoBehaviour {

    PostProcessingProfile currentProfile;
    PostProcessingBehaviour postProcessingBehaviour;
 
    [SerializeField]
    PostProcessingProfile pauseProfile;
    [SerializeField]
    float yDistance = 21f;
    [SerializeField]
    float entityXOffset = 15f;
    [SerializeField]
    float entityZOffset = -15f;
    [SerializeField]
    float smoothTime = 0.05f;
    [SerializeField]
    float distanceToMouse = 0.3f;
    [SerializeField]
    Transform followEntity;

    float defaultJoltMagnitude = 0.4f;
    [SerializeField]
    float shakeMagnitude = 1.0f;

    Vector3 dampVelocity = Vector3.zero;
    Camera mainCamera;
    bool isShaking = false;

    void Start()
    {
        postProcessingBehaviour = GetComponent<PostProcessingBehaviour>();
        mainCamera = Camera.main;
        if (followEntity == null)
        {
            followEntity = GameManager.GetPlayerTransform();
        }
    }

    void Update()
    {
        Vector3 nextCameraPosition = GetNextCameraPosition();

        transform.position = Vector3.SmoothDamp(transform.position, nextCameraPosition, ref dampVelocity, smoothTime);
    }

    Vector3 GetNextCameraPosition()
    {
        Vector3 entityPosition = followEntity.position;
        entityPosition.x += entityXOffset;
        entityPosition.z += entityZOffset;

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        Vector3 nextCameraPosition = Vector3.Lerp(entityPosition, mousePosition, distanceToMouse);

        if (isShaking)
        {
            nextCameraPosition += (Random.insideUnitSphere * shakeMagnitude);
        }

        nextCameraPosition.y = entityPosition.y + yDistance;

        return nextCameraPosition;
    }

    public void ApplyPauseFilter()
    {
        currentProfile = postProcessingBehaviour.profile;

        postProcessingBehaviour.profile = pauseProfile;
    }

    public void RevertToOriginalProfile()
    {
        postProcessingBehaviour.profile = currentProfile;
    }

    // ShakeScreen is for aimless juddering.
    public void ShakeScreen(float duration)
    {
        isShaking = true;

        Invoke("StopShaking", duration);
    }

    void StopShaking()
    {
        isShaking = false;
    }

    // JoltScreen is for a sudden, directional movement.
    public void JoltScreen(Vector3 direction, float joltMagnitude = 0.4f)
    {
        transform.position += (direction.normalized * joltMagnitude);
    }
}
