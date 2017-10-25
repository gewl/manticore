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
    float playerXOffset = 15f;
    [SerializeField]
    float playerZOffset = -15f;
    [SerializeField]
    float smoothTime = 0.05f;
    [SerializeField]
    float distanceToMouse = 0.3f;

    Vector3 dampVelocity = Vector3.zero;
    Camera mainCamera;

    void Start()
    {
        postProcessingBehaviour = GetComponent<PostProcessingBehaviour>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        Vector3 playerPosition = GameManager.GetPlayerPosition();
        playerPosition.x += playerXOffset;
        playerPosition.z += playerZOffset;

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        Vector3 nextCameraPosition = Vector3.Lerp(playerPosition, mousePosition, distanceToMouse);

        nextCameraPosition.y = playerPosition.y + yDistance;

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
