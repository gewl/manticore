using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDoor : MonoBehaviour {

    float transitionTime = 0.5f;
    float destinationYOffset = 10.5f;
    bool hasDoorClosed = false;

    public void CloseDoor()
    {
        if (!hasDoorClosed)
        {
            hasDoorClosed = true;
            StartCoroutine(SlideDoorShut());
        }
    }

    IEnumerator SlideDoorShut()
    {
        float timeElapsed = 0.0f;
        float baseY = transform.position.y;
        float destinationY = baseY + destinationYOffset;

        while (timeElapsed < transitionTime)
        {
            float percentageComplete = timeElapsed / transitionTime;
            float curvedPercentageComplete = GameManager.BelovedSwingCurve.Evaluate(percentageComplete);
            float newYOffset = Mathf.Lerp(baseY, destinationY, curvedPercentageComplete);

            transform.position = new Vector3(transform.position.x, newYOffset, transform.position.z);

            timeElapsed += Time.deltaTime;

            yield return null;
        }
    }
}
