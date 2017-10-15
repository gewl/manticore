﻿using UnityEngine;
using UnityEngine.PostProcessing;

public class CameraController : MonoBehaviour {

    PostProcessingProfile currentProfile;
    PostProcessingBehaviour postProcessingBehaviour;
 
    [SerializeField]
    PostProcessingProfile pauseProfile;
    [SerializeField]
    float yDistance = 21f;
    [SerializeField]
    float playerZOffset = -24f;
    [SerializeField]
    float smoothTime = 0.05f;

    Vector3 dampVelocity = Vector3.zero;

    void Start()
    {
        postProcessingBehaviour = GetComponent<PostProcessingBehaviour>();
    }

    void Update()
    {
        Vector3 playerPosition = GameManager.GetPlayerPosition();
        playerPosition.z += playerZOffset;

        Vector3 mousePosition = GameManager.GetMousePositionInWorldSpace();

        Vector3 nextCameraPosition = Vector3.Lerp(playerPosition, mousePosition, 0.3f);

        nextCameraPosition.y = yDistance;

        //transform.position = nextCameraPosition;
        transform.position = Vector3.SmoothDamp(transform.position, nextCameraPosition, ref dampVelocity, smoothTime);
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
}
