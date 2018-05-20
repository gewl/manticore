using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour {

    public bool isUsingZ = false;

    float baseHeight;
    [SerializeField]
    float alternateHeight;

    [SerializeField]
    float travelTime;

    bool hasMoved = false;

    private void Awake()
    {
        baseHeight = transform.localPosition.y;
        if (isUsingZ)
        {
            baseHeight = transform.localPosition.z;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        StartElevator();
    }

    public void StartElevator()
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

            if (isUsingZ)
            {
                newPosition.z = Mathf.Lerp(baseHeight, alternateHeight, curvedPercentage);
            }
            else
            {
                newPosition.y = Mathf.Lerp(baseHeight, alternateHeight, curvedPercentage);
            }

            transform.localPosition = newPosition;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (isUsingZ)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, alternateHeight);
        }
        else
        {
            transform.localPosition = new Vector3(transform.localPosition.x, alternateHeight, transform.localPosition.z);
        }
    }

}
