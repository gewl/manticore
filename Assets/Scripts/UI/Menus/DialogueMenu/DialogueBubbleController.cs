using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogueBubbleController : MonoBehaviour, IPointerClickHandler
{
    DialogueMenuController dialogueMenu;

    // These values track which dialogue bubble lead to this bubble, which
    // DialogueMenuController will use to prevent patricide.
    public int bubbleParentX;
    public int bubbleParentY;

    string bubbleContents;
    Text _dialogueBubbleText;
    Text dialogueBubbleText
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

    const float baseTimeBetweenCharacters = 0.02f;
    const float timeBetweenCharactersVariance = 0.005f;


    private void Awake()
    {
        generator = dialogueBubbleText.cachedTextGenerator;

        dialogueMenu = GetComponentInParent<DialogueMenuController>();
    }

    private void OnEnable()
    {
        dialogueBubbleText.text = "";
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void UpdateBubbleContents(string text, List<string> clickableTerms)
    {
        StopAllCoroutines();
        dialogueBubbleText.text = "";
        bubbleContents = text;

        _clickableTerms = clickableTerms.ToList<string>();

        StartCoroutine(FillTextIn());
    }

    IEnumerator FillTextIn()
    {
        int textIndex = 0;

        while (textIndex <= bubbleContents.Length)
        {
            dialogueBubbleText.text = bubbleContents.Substring(0, textIndex);
            textIndex++;

            float timingVariance = UnityEngine.Random.Range(-timeBetweenCharactersVariance, timeBetweenCharactersVariance);

            float timeUntilNextCharacter = baseTimeBetweenCharacters + timingVariance;

            yield return new WaitForSecondsRealtime(timeUntilNextCharacter);
        }

        HighlightClickableTerms();
    }

    void HighlightClickableTerms()
    { 
        string newDialogueBubbleText = dialogueBubbleText.text;
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

            newDialogueBubbleText = newDialogueBubbleText.Insert(startOfTermIndex, "<color=red>");

            int endOfTermIndex = newDialogueBubbleText.IndexOf(clickableTerm) + clickableTerm.Length;

            newDialogueBubbleText = newDialogueBubbleText.Insert(endOfTermIndex, "</color>");
        }

        dialogueBubbleText.text = newDialogueBubbleText;
    }   

    public void OnPointerClick(PointerEventData eventData)
    {
        generator = dialogueBubbleText.cachedTextGenerator;
        Vector2 clickPosition = dialogueBubbleText.transform.worldToLocalMatrix.MultiplyPoint(eventData.position);

        for (int i = 0; i < _clickableTerms.Count; i++)
        {
            string clickableTerm = _clickableTerms[i];
            if (string.IsNullOrEmpty(clickableTerm)) continue;
            if (!dialogueBubbleText.text.Contains(clickableTerm))
            {
                Debug.Log("clickable term not in string");
                continue;
            }

            int termIndex = dialogueBubbleText.text.IndexOf(clickableTerm);

            for (int pos = termIndex; pos < termIndex + clickableTerm.Length; pos++)
            {
                Vector2 upperLeft = new Vector2(generator.verts[pos * 4].position.x, generator.verts[pos * 4 + 2].position.y);
                Vector2 bottomRight = new Vector2(generator.verts[pos * 4 + 2].position.x, generator.verts[pos * 4].position.y);

                bool clickInBounds = clickPosition.x >= upperLeft.x && clickPosition.y >= upperLeft.y && clickPosition.x <= bottomRight.x && clickPosition.y <= bottomRight.y;

                if (clickInBounds)
                {
                    dialogueMenu.RegisterTermClick(this, clickableTerm);
                    return;
                }

            }
        }
    }

}
