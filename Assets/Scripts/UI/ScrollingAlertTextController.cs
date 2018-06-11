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

    Vector3 initialPosition, centerPosition, destinationPosition;

	void Start () {
        scrollCoroutineQueue = new CoroutineQueue(this);
        rectTransform = GetComponent<RectTransform>();

        float maxRightX = rectTransform.TransformPoint(rectTransform.rect.max).x;
        float minRightX = rectTransform.TransformPoint(rectTransform.rect.min).x;
        float centerX = rectTransform.TransformPoint(rectTransform.rect.center).x;

        float topY = rectTransform.TransformPoint(rectTransform.rect.max).y;
        float bottomY = rectTransform.TransformPoint(rectTransform.rect.min).y;
        float centerY = rectTransform.TransformPoint(rectTransform.rect.center).y;

        initialPosition = new Vector3(maxRightX, bottomY, transform.position.z);
        centerPosition = new Vector3(centerX, centerY, transform.position.z);
        destinationPosition = new Vector3(minRightX, topY, transform.position.z);

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
        StopAllCoroutines();
        primaryText.gameObject.SetActive(false);
        secondaryText.gameObject.SetActive(false);
    }

    void GameStateEventHandler(GlobalConstants.GameStateEvents gameStateEvent, string eventInformation)
    {
        string primaryTextContents = GetPrimaryTextContents(gameStateEvent, eventInformation);
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
        primaryText.transform.position = initialPosition;
        primaryText.gameObject.SetActive(true);
        primaryText.text = primaryTextContents;

        Vector3 initialPrimaryPosition = primaryText.transform.position;
        Vector3 targetPrimaryPosition = centerPosition;

        yield return StartCoroutine(MovePrimaryText(initialPrimaryPosition, targetPrimaryPosition));

        yield return new WaitForSeconds(primaryTextHangTime);

        initialPrimaryPosition = primaryText.transform.position;
        targetPrimaryPosition = destinationPosition;

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
        secondaryText.transform.position = initialPosition;
        yield return new WaitForSeconds(secondaryTextDelay);
        secondaryText.gameObject.SetActive(true);
        secondaryText.text = secondaryTextContents;

        Vector3 initialSecondaryPosition = secondaryText.transform.position;
        Vector3 targetSecondaryPosition = centerPosition;

        yield return StartCoroutine(MoveSecondaryText(initialSecondaryPosition, targetSecondaryPosition));

        yield return new WaitForSeconds(secondaryTextHangTime);

        initialSecondaryPosition = secondaryText.transform.position;
        targetSecondaryPosition = destinationPosition;

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

    string GetPrimaryTextContents(GlobalConstants.GameStateEvents gameStateEvent, string eventInformation)
    {
        switch (gameStateEvent)
        {
            case GlobalConstants.GameStateEvents.PlayerDied:
                return "ya";
            case GlobalConstants.GameStateEvents.NewMomentumPoint:
                return "New Momentum Point Earned";
            case GlobalConstants.GameStateEvents.HardwareDiscovered:
                return "New Hardware Discovered: " + eventInformation;
            case GlobalConstants.GameStateEvents.MomentumLost:
                return "A Little Momentum Lost, No Big Deal";
            default:
                return "";
        }
    }

    string GetSecondaryTextContents(GlobalConstants.GameStateEvents gameStateEvent, string eventInformation)
    {
        switch (gameStateEvent)
        {
            case GlobalConstants.GameStateEvents.PlayerDied:
                return "dead";
            case GlobalConstants.GameStateEvents.NewMomentumPoint:
                return "Spend It and Accelerate";
            case GlobalConstants.GameStateEvents.HardwareDiscovered:
                return "Now You Can " + GetDiscoveredHardwareFlavorText(eventInformation);
            case GlobalConstants.GameStateEvents.MomentumLost:
                return "Try It Again";
            default:
                return "";
        }
    }

    string GetDiscoveredHardwareFlavorText(string hardwareName)
    {
        switch (hardwareName)
        {
            case "Nullify":
                return "Cancel and Eliminate";
            case "Fracture":
                return "Smash and Splinter";
            case "Yank":
                return "Twist and Shoot";
            default:
                return "";
        }
    }
}
