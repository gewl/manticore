using System.Collections;
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

    [SerializeField]
    float joltRecoveryTime = 0.3f;
    [SerializeField]
    float joltMagnitude = 1.0f;

    Vector3 dampVelocity = Vector3.zero;
    Camera mainCamera;

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
    public void ShakeScreen()
    {
    
    }

    // JoltScreen is for a sudden, directional movement.
    public void JoltScreen(Vector3 direction)
    {
        transform.position += (direction.normalized * joltMagnitude);
    }

    IEnumerator JoltCameraPosition()
    {
        yield return null;
    }

}
