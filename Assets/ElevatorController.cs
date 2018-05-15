using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour {

    float baseHeight;
    [SerializeField]
    float alternateHeight;

    [SerializeField]
    float travelTime;

    bool hasMoved = false;

    private void Awake()
    {
        baseHeight = transform.localPosition.z;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasMoved)
        {
            hasMoved = true;

            StartCoroutine(MoveElevator());
        };
    }

    IEnumerator MoveElevator()
    {
        float timeElapsed = 0.0f;

        while (timeElapsed < travelTime)
        {
            float percentageComplete = timeElapsed / travelTime;

            float curvedPercentage = GameManager.BelovedSwingCurve.Evaluate(percentageComplete);

            Vector3 newPosition = transform.localPosition;

            newPosition.z = Mathf.Lerp(baseHeight, alternateHeight, curvedPercentage);

            transform.localPosition = newPosition;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, alternateHeight);
    }

}
