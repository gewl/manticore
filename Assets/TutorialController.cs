using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour {

    float targetPositionOffset = 0f;

    [SerializeField]
    float transitionTime = 0.5f;

    float resetPositionOffset;
    RectTransform elementRectTransform;

    float x, height, width;

    private void Awake()
    {
        elementRectTransform = transform.GetChild(0).GetComponent<RectTransform>();

        x = elementRectTransform.anchoredPosition.x;
        height = elementRectTransform.rect.height;
        width = elementRectTransform.rect.width;

        resetPositionOffset = height * 1.5f;

        StartCoroutine(TransitionIn());
    }

    IEnumerator TransitionIn()
    {
        UpdateOffset(resetPositionOffset);

        float timeElapsed = 0.0f;

        while (timeElapsed < transitionTime)
        {
            float percentageComplete = timeElapsed / transitionTime;
            float curvedPercentageComplete = GameManager.BelovedSwingCurve.Evaluate(percentageComplete);

            float newYOffset = Mathf.Lerp(resetPositionOffset, targetPositionOffset, curvedPercentageComplete);

            UpdateOffset(newYOffset);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        UpdateOffset(targetPositionOffset);
    }

    void UpdateOffset(float newOffset)
    {
        elementRectTransform.anchoredPosition = new Vector3(x, newOffset, 0f);
    }
}
