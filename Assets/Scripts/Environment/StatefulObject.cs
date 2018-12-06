using System.Collections;  
using UnityEngine;
using Sirenix.OdinInspector;

public class StatefulObject : SerializedMonoBehaviour {

    [SerializeField]
    Vector3 toggledPosition;
    [SerializeField]
    Vector3 toggledRotationEuler;

    [SerializeField]
    string stateBoolTag;

    bool hasMoved = false;
    float timeToMove = 1.0f;

    private void OnEnable()
    {
        if (MasterSerializer.GetObjectState(stateBoolTag))
        {
            hasMoved = true;
            StartCoroutine(ChangePosition());
        }
    }

    public string GetStateTag()
    {
        return stateBoolTag;
    }

    public void Move()
    {
        if (hasMoved)
        {
            return;
        }

        hasMoved = true;
        MasterSerializer.FlagSceneState(stateBoolTag);
        StartCoroutine(ChangePosition());
    }

    IEnumerator ChangePosition()
    {
        Vector3 initialPosition = transform.position;

        Vector3 destinationPosition = toggledPosition != Vector3.zero ?
            toggledPosition :
            initialPosition;

        Vector3 initialRotation = transform.rotation.eulerAngles;

        Vector3 destinationRotation = toggledRotationEuler != Vector3.zero ?
            toggledRotationEuler :
            initialRotation;

        float timeElapsed = 0.0f;

        while (timeElapsed < timeToMove)
        {
            float percentageComplete = timeElapsed / timeToMove;
            float curvedCompletion = GameManager.BelovedSwingCurve.Evaluate(percentageComplete);

            transform.position = Vector3.Lerp(initialPosition, toggledPosition, curvedCompletion);
            transform.rotation = Quaternion.Euler(Vector3.Lerp(initialRotation, destinationRotation, curvedCompletion));

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = destinationPosition;
        transform.rotation = Quaternion.Euler(destinationRotation);
    }
}
