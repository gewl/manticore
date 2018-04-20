using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogueBubbleController : MonoBehaviour, IPointerClickHandler
{
    Image _dialogueBubble;
    Image DialogueBubble
    {
        get
        {
            if (_dialogueBubble == null)
            {
                _dialogueBubble = GetComponent<Image>();
            }

            return _dialogueBubble;
        }
    }
    DialogueMenuController dialogueMenu;

    // These values track which dialogue bubble lead to this bubble, which
    // DialogueMenuController will use to prevent patricide.
    public int bubbleParentX;
    public int bubbleParentY;

    string bubbleContents;
    Text _dialogueBubbleText;
    Text DialogueBubbleText
    {
        get
        {
            if (_dialogueBubbleText == null)
            {
                _dialogueBubbleText = GetComponentInChildren<Text>(true);
            }

            return _dialogueBubbleText;
        }
    }
    TextGenerator generator;

    List<string> _clickableTerms;
    List<bool> termsClicked;

    const float baseTimeBetweenCharacters = 0.02f;
    const float timeBetweenCharactersVariance = 0.01f;

    const float timeToExpandBubble = 0.4f;

    private void Awake()
    {
        generator = DialogueBubbleText.cachedTextGenerator;

        dialogueMenu = GetComponentInParent<DialogueMenuController>();
    }

    private void OnEnable()
    {
        DialogueBubbleText.text = "";
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void ActivateBubble(string text, List<string> clickableTerms, DialogueMenuController.BubbleExpandDirection bubbleExpandDirection = DialogueMenuController.BubbleExpandDirection.RIGHT, int _bubbleParentX = -1, int _bubbleParentY = -1)
    {
        StopAllCoroutines();
        StartCoroutine(ExpandBubble(bubbleExpandDirection));

        UpdateBubbleContents(text, clickableTerms);

        bubbleParentX = _bubbleParentX;
        bubbleParentY = _bubbleParentY;
    }

    void UpdateBubbleContents(string text, List<string> clickableTerms)
    {
        DialogueBubbleText.text = "";
        bubbleContents = text;

        _clickableTerms = clickableTerms.ToList<string>();
        termsClicked = new List<bool>();
        for (int i = 0; i < _clickableTerms.Count; i++)
        {
            termsClicked.Add(false);
        }

        StartCoroutine(FillTextIn());
    }

    IEnumerator ExpandBubble(DialogueMenuController.BubbleExpandDirection bubbleExpandDirection)
    {
        switch (bubbleExpandDirection)
        {
            case DialogueMenuController.BubbleExpandDirection.UP:
                DialogueBubble.fillMethod = Image.FillMethod.Vertical;
                DialogueBubble.fillOrigin = 0;
                break;
            case DialogueMenuController.BubbleExpandDirection.DOWN:
                DialogueBubble.fillMethod = Image.FillMethod.Vertical;
                DialogueBubble.fillOrigin = 1;
                break;
            case DialogueMenuController.BubbleExpandDirection.LEFT:
                DialogueBubble.fillMethod = Image.FillMethod.Horizontal;
                DialogueBubble.fillOrigin = 1;
                break;
            case DialogueMenuController.BubbleExpandDirection.RIGHT:
                DialogueBubble.fillMethod = Image.FillMethod.Horizontal;
                DialogueBubble.fillOrigin = 0;
                break;
        }

        DialogueBubble.fillAmount = 0f;

        float timeOfCompletion = Time.realtimeSinceStartup + timeToExpandBubble;

        while (Time.realtimeSinceStartup < timeOfCompletion)
        {
            float percentageComplete = 1 - (timeOfCompletion - Time.realtimeSinceStartup) / timeToExpandBubble;

            float curvedPercentage = GameManager.BelovedSwingCurve.Evaluate(percentageComplete);

            DialogueBubble.fillAmount = curvedPercentage;
            yield return null;
        }

        DialogueBubble.fillAmount = 1f;
    }

    IEnumerator FillTextIn()
    {
        int textIndex = 0;

        while (textIndex <= bubbleContents.Length)
        {
            DialogueBubbleText.text = bubbleContents.Substring(0, textIndex);
            textIndex++;

            float timingVariance = UnityEngine.Random.Range(-timeBetweenCharactersVariance, timeBetweenCharactersVariance);

            float timeUntilNextCharacter = baseTimeBetweenCharacters + timingVariance;

            yield return new WaitForSecondsRealtime(timeUntilNextCharacter);
        }

        UpdateClickableTermsHighlighting();
    }

    void UpdateClickableTermsHighlighting()
    { 
        string newDialogueBubbleText = bubbleContents;
        for (int i = 0; i < _clickableTerms.Count; i++)
        {
            string clickableTerm = _clickableTerms[i];
            if (!newDialogueBubbleText.Contains(clickableTerm))
            {
                Debug.LogError("clickable term not in string: " + clickableTerm);
                continue;
            }

            // Can't cache indices here because length/position will update as color code is added.
            int startOfTermIndex = newDialogueBubbleText.IndexOf(clickableTerm);

            if (dialogueMenu.IsTermSpecial(clickableTerm))
            {
                newDialogueBubbleText = newDialogueBubbleText.Insert(startOfTermIndex, "<color=blue>");
            }
            else if (termsClicked[i])
            {
                newDialogueBubbleText = newDialogueBubbleText.Insert(startOfTermIndex, "<color=green>");
            }
            else
            {
                newDialogueBubbleText = newDialogueBubbleText.Insert(startOfTermIndex, "<color=red>");
            }

            int endOfTermIndex = newDialogueBubbleText.IndexOf(clickableTerm) + clickableTerm.Length;

            newDialogueBubbleText = newDialogueBubbleText.Insert(endOfTermIndex, "</color>");
        }

        DialogueBubbleText.text = newDialogueBubbleText;
    }   

    public void OnPointerClick(PointerEventData eventData)
    {
        generator = DialogueBubbleText.cachedTextGenerator;
        Vector2 clickPosition = DialogueBubbleText.transform.worldToLocalMatrix.MultiplyPoint(eventData.position);
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(DialogueBubbleText.rectTransform, eventData.position, Camera.main, out localPoint);

        for (int i = 0; i < _clickableTerms.Count; i++)
        {
            string clickableTerm = _clickableTerms[i];
            if (string.IsNullOrEmpty(clickableTerm)) continue;
            if (!DialogueBubbleText.text.Contains(clickableTerm))
            {
                continue;
            }

            int termIndex = DialogueBubbleText.text.IndexOf(clickableTerm);

            for (int pos = termIndex; pos < termIndex + clickableTerm.Length; pos++)
            {
                Vector2 upperLeft = new Vector2(generator.verts[pos * 4].position.x, generator.verts[pos * 4 + 2].position.y);
                Vector2 bottomRight = new Vector2(generator.verts[pos * 4 + 2].position.x, generator.verts[pos * 4].position.y);

                bool clickInBounds = clickPosition.x >= upperLeft.x && clickPosition.y >= upperLeft.y && clickPosition.x <= bottomRight.x && clickPosition.y <= bottomRight.y;

                if (clickInBounds)
                {
                    dialogueMenu.RegisterTermClick(this, clickableTerm);
                    termsClicked[i] = true;
                    UpdateClickableTermsHighlighting();
                    return;
                }

            }
        }
    }

}
