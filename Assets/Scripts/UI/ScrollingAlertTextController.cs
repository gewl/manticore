using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingAlertTextController : MonoBehaviour {

    CoroutineQueue scrollCoroutineQueue;

    [SerializeField]
    Text primaryText;
    [SerializeField]
    Text secondaryText;

    [SerializeField]
    AnimationCurve transitionCurve;
    [SerializeField]
    float timeToTransition;
    [SerializeField]
    float primaryTextHangTime;
    [SerializeField]
    float secondaryTextHangTime;
    [SerializeField]
    float secondaryTextDelay;

    RectTransform rectTransform;

    Vector3 maximumPrimaryPosition, centerPrimaryPosition, minimumPrimaryPosition;
    Vector3 maximumSecondaryPosition, centerSecondaryPosition, minimumSecondaryPosition;

	void Start () {
        scrollCoroutineQueue = new CoroutineQueue(this);
        rectTransform = GetComponent<RectTransform>();

        float maxRightX = rectTransform.TransformPoint(rectTransform.rect.max).x;
        float minRightX = rectTransform.TransformPoint(rectTransform.rect.min).x;
        float centerX = rectTransform.TransformPoint(rectTransform.rect.center).x;

        float primaryY = primaryText.transform.position.y;
        float secondaryY = secondaryText.transform.position.y;

        maximumPrimaryPosition = new Vector3(maxRightX, primaryY, 0f);
        centerPrimaryPosition = new Vector3(centerX, primaryY, 0f);
        minimumPrimaryPosition = new Vector3(minRightX, primaryY, 0f);

        maximumSecondaryPosition = new Vector3(maxRightX, secondaryY, 0f);
        centerSecondaryPosition = new Vector3(centerX, secondaryY, 0f);
        minimumSecondaryPosition = new Vector3(minRightX, secondaryY, 0f);

        primaryText.gameObject.SetActive(false);
        secondaryText.gameObject.SetActive(false);
	}

    private void OnEnable()
    {
        GlobalEventEmitter.OnGameStateEvent += GameStateEventHandler;
    }

    private void OnDisable()
    {
        GlobalEventEmitter.OnGameStateEvent -= GameStateEventHandler;
    }

    void GameStateEventHandler(GlobalConstants.GameStateEvents gameStateEvent, string eventInformation)
    {
        string primaryTextContents = GetPrimaryTextContents(gameStateEvent);
        string secondaryTextContents = GetSecondaryTextContents(gameStateEvent, eventInformation);

        if (primaryTextContents.Length > 0 && secondaryTextContents.Length > 0)
        {
            scrollCoroutineQueue.EnqueueAction(ScrollText(primaryTextContents, secondaryTextContents));
        }
    }

    IEnumerator ScrollText(string primaryTextContents, string secondaryTextContents = "")
    {
        if (secondaryTextContents != "")
        {
            StartCoroutine(ScrollPrimaryText(primaryTextContents));
            yield return StartCoroutine(ScrollSecondaryText(secondaryTextContents));
        }
        else
        {
            yield return StartCoroutine(ScrollPrimaryText(primaryTextContents));
        }
    }
	
    IEnumerator ScrollPrimaryText(string primaryTextContents)
    {
        primaryText.transform.position = maximumPrimaryPosition;
        primaryText.gameObject.SetActive(true);
        primaryText.text = primaryTextContents;

        Vector3 initialPrimaryPosition = primaryText.transform.position;
        Vector3 targetPrimaryPosition = centerPrimaryPosition;

        yield return StartCoroutine(MovePrimaryText(initialPrimaryPosition, targetPrimaryPosition));

        yield return new WaitForSeconds(primaryTextHangTime);

        initialPrimaryPosition = primaryText.transform.position;
        targetPrimaryPosition = minimumPrimaryPosition;

        yield return StartCoroutine(MovePrimaryText(initialPrimaryPosition, targetPrimaryPosition));

        primaryText.gameObject.SetActive(false);
    }

    IEnumerator MovePrimaryText(Vector3 initialPosition, Vector3 destinationPosition)
    {
        float timeElapsed = 0.0f;

        while (timeElapsed < timeToTransition)
        {
            timeElapsed += Time.deltaTime;

            float percentageComplete = timeElapsed / timeToTransition;
            float curvePosition = transitionCurve.Evaluate(percentageComplete);

            primaryText.transform.position = Vector3.Lerp(initialPosition, destinationPosition, curvePosition);

            yield return null;
        }
    }

    IEnumerator ScrollSecondaryText(string secondaryTextContents)
    {
        secondaryText.transform.position = maximumSecondaryPosition;
        yield return new WaitForSeconds(secondaryTextDelay);
        secondaryText.gameObject.SetActive(true);
        secondaryText.text = secondaryTextContents;

        Vector3 initialSecondaryPosition = secondaryText.transform.position;
        Vector3 targetSecondaryPosition = centerSecondaryPosition;

        yield return StartCoroutine(MoveSecondaryText(initialSecondaryPosition, targetSecondaryPosition));

        yield return new WaitForSeconds(secondaryTextHangTime);

        initialSecondaryPosition = secondaryText.transform.position;
        targetSecondaryPosition = minimumSecondaryPosition;

        yield return StartCoroutine(MoveSecondaryText(initialSecondaryPosition, targetSecondaryPosition));

        secondaryText.gameObject.SetActive(false);
    }

    IEnumerator MoveSecondaryText(Vector3 initialPosition, Vector3 destinationPosition)
    {
        float timeElapsed = 0.0f;

        while (timeElapsed < timeToTransition)
        {
            timeElapsed += Time.deltaTime;

            float percentageComplete = timeElapsed / timeToTransition;
            float curvePosition = transitionCurve.Evaluate(percentageComplete);

            secondaryText.transform.position = Vector3.Lerp(initialPosition, destinationPosition, curvePosition);

            yield return null;
        }
    }

    string GetPrimaryTextContents(GlobalConstants.GameStateEvents gameStateEvent)
    {
        switch (gameStateEvent)
        {
            case GlobalConstants.GameStateEvents.PlayerDied:
                return "ya dead";
            case GlobalConstants.GameStateEvents.NewMomentumPoint:
                return "New Momentum Point Earned";
            case GlobalConstants.GameStateEvents.HardwareDiscovered:
                return "New Hardware Discovered";
            default:
                return "";
        }
    }

    string GetSecondaryTextContents(GlobalConstants.GameStateEvents gameStateEvent, string eventInformation)
    {
        switch (gameStateEvent)
        {
            case GlobalConstants.GameStateEvents.PlayerDied:
                return "by gil";
            case GlobalConstants.GameStateEvents.NewMomentumPoint:
                return "Spend It and Accelerate";
            case GlobalConstants.GameStateEvents.HardwareDiscovered:
                return "Now You Can " + eventInformation;
            default:
                return "";
        }
    }
}
