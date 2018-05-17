using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour {

    float primaryBubVisibleOffset = 0f;

    [SerializeField]
    float transitionTime = 0.5f;

    float resetPositionOffset;
    RectTransform primaryBubRectTransform;
    Text primaryTutorialText;

    float x, defaultHeight;

    private void Awake()
    {
        primaryBubRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        primaryTutorialText = primaryBubRectTransform.GetChild(0).GetComponent<Text>();

        x = primaryBubRectTransform.anchoredPosition.x;
        defaultHeight = primaryBubRectTransform.rect.height;

        resetPositionOffset = defaultHeight * 1.5f;

        StartCoroutine(TransitionPrimaryIn());
    }

    public void ChangeTutorialBub(string newText, GameObject secondaryBubPane = null)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        primaryBubRectTransform.gameObject.SetActive(true);
        StartCoroutine(TransitionPrimaryOutAndIn(newText));

        if (secondaryBubPane != null)
        {
            StartCoroutine(ShowSecondaryBubs(secondaryBubPane));
        }
    }

    IEnumerator TransitionPrimaryIn()
    {
        primaryTutorialText.text = "Use WASD to move.";

        UpdatePrimaryOffset(resetPositionOffset);

        float timeElapsed = 0.0f;

        while (timeElapsed < transitionTime)
        {
            float percentageComplete = timeElapsed / transitionTime;
            float curvedPercentageComplete = GameManager.BelovedSwingCurve.Evaluate(percentageComplete);

            float newYOffset = Mathf.Lerp(resetPositionOffset, primaryBubVisibleOffset, curvedPercentageComplete);

            UpdatePrimaryOffset(newYOffset);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        UpdatePrimaryOffset(primaryBubVisibleOffset);
    }

    IEnumerator TransitionPrimaryOutAndIn(string newText)
    {
        float timeElapsed = 0.0f;

        while (timeElapsed < transitionTime)
        {
            float percentageComplete = timeElapsed / transitionTime;
            float curvedPercentageComplete = GameManager.BelovedSwingCurve.Evaluate(percentageComplete);

            float newYOffset = Mathf.Lerp(primaryBubVisibleOffset, resetPositionOffset, curvedPercentageComplete);

            UpdatePrimaryOffset(newYOffset);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        UpdatePrimaryOffset(resetPositionOffset);

        primaryTutorialText.text = newText;

        timeElapsed = 0.0f;

        while (timeElapsed < transitionTime)
        {
            float percentageComplete = timeElapsed / transitionTime;
            float curvedPercentageComplete = GameManager.BelovedSwingCurve.Evaluate(percentageComplete);

            float newYOffset = Mathf.Lerp(resetPositionOffset, primaryBubVisibleOffset, curvedPercentageComplete);

            UpdatePrimaryOffset(newYOffset);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        UpdatePrimaryOffset(primaryBubVisibleOffset);

    }

    IEnumerator ShowSecondaryBubs(GameObject secondaryBubPane)
    {
        yield return new WaitForSeconds(transitionTime * 2f);

        secondaryBubPane.SetActive(true);
    }

    void UpdatePrimaryOffset(float newOffset)
    {
        primaryBubRectTransform.anchoredPosition = new Vector3(x, newOffset, 0f);
    }
}
