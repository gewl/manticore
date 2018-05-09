using System.Collections;  
using UnityEngine;
using Sirenix.OdinInspector;

public class StatefulObject : SerializedMonoBehaviour {

    [SerializeField]
    Vector3 destinationPosition;

    bool hasMoved = false;
    float timeToMove = 1.0f;

    public void Move()
    {
        if (hasMoved)
        {
            return;
        }

        hasMoved = true;
        Debug.Log("starting coroutine");
        StartCoroutine(ChangePosition());
    }

    IEnumerator ChangePosition()
    {
        Vector3 initialPosition = transform.localPosition;

        float timeElapsed = 0.0f;

        while (timeElapsed < timeToMove)
        {
            float percentageComplete = timeElapsed / timeToMove;
            float curvedCompletion = GameManager.BelovedSwingCurve.Evaluate(percentageComplete);

            transform.localPosition = Vector3.Lerp(initialPosition, destinationPosition, curvedCompletion);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = destinationPosition;
    }
}
