using UnityEngine;
using UnityEngine.PostProcessing;

public class CameraManager : MonoBehaviour {

    PostProcessingProfile currentProfile;
    PostProcessingBehaviour postProcessingBehaviour;
 
    [SerializeField]
    PostProcessingProfile pauseProfile;

    void Start()
    {
        postProcessingBehaviour = GetComponent<PostProcessingBehaviour>();
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
