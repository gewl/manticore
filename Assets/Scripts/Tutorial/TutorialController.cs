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

    [HideInInspector]
    public bool InEquipActiveRoom = false;
    [HideInInspector]
    public bool InEquipPassiveRoom = false;
    [SerializeField]
    GameObject equipActiveRoomInventoryPane;
    [SerializeField]
    GameObject equipPassiveRoomInventoryPane;

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
        StopAllCoroutines();
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

    public void RegisterInventoryMenuOpen()
    {
        if (InEquipActiveRoom)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }

            equipActiveRoomInventoryPane.SetActive(true);
        }
        else if (InEquipPassiveRoom)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }

            equipPassiveRoomInventoryPane.SetActive(true);
        }
    }
    
    public void RegisterInventoryMenuClose()
    {
        if (InEquipActiveRoom)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
            primaryBubRectTransform.gameObject.SetActive(true);
            if (InventoryController.GetEquippedActiveHardware()[2] == HardwareType.Nullify)
            {
                primaryTutorialText.text = "Now you can RIGHT-CLICK to NULLIFY incoming enemy bullets!";
            }
            else
            {
                primaryTutorialText.text = "Talk to the robot to INSTALL your new HARDWARE!";
            }
        }
        else if (InEquipPassiveRoom)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
            primaryBubRectTransform.gameObject.SetActive(true);
            if (InventoryController.GetEquippedPassiveHardware()[0] == HardwareType.Nullify)
            {
                primaryTutorialText.text = "Now your parried bullets have a NULLIFY effect!";
            }
            else
            {
                primaryTutorialText.text = "Talk to the robot to INSTALL NULLIFY as PASSIVE HARDWARE!";
            }
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
