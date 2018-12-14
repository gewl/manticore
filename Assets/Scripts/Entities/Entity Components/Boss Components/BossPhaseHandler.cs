using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BossPhaseHandler : SerializedMonoBehaviour {

    [SerializeField]
    List<MonoBehaviour> firstPhaseComponentsToDisable;
    [SerializeField]
    List<MonoBehaviour> secondPhaseComponentsToEnable;

    [SerializeField]
    Transform secondPhasePosition;

    [SerializeField]
    GameObject floatingLetter;

    [SerializeField]
    float timeToTransition = 1.5f;

    public void EndFirstPhase()
    {
        foreach (MonoBehaviour component in firstPhaseComponentsToDisable)
        {
            component.enabled = false;
        }

        StartCoroutine(MoveToSecondPhasePosition());
    }

    IEnumerator MoveToSecondPhasePosition()
    {
        Vector3 initialPosition = transform.position;
        Vector3 initialRotation = transform.rotation.eulerAngles;

        Vector3 destinationPosition = secondPhasePosition.position;
        Vector3 destinationRotation = secondPhasePosition.rotation.eulerAngles;

        float timeElapsed = 0.0f;

        while (timeElapsed < timeToTransition)
        {
            float percentageComplete = timeElapsed / timeToTransition;
            float curvedCompletion = GameManager.BelovedSwingCurve.Evaluate(percentageComplete);

            transform.position = Vector3.Lerp(initialPosition, destinationPosition, curvedCompletion);
            transform.rotation = Quaternion.Euler(Vector3.Lerp(initialRotation, destinationRotation, curvedCompletion));
                
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = destinationPosition;
        transform.rotation = Quaternion.Euler(destinationRotation);

        foreach (MonoBehaviour component in secondPhaseComponentsToEnable)
        {
            component.enabled = true;
        }
        floatingLetter.SetActive(true);
    }
}
