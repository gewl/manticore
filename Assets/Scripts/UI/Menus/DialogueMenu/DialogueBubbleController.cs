using System;
using System.Collections.Generic;
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

    Text dialogueBubbleText;
    TextGenerator generator;
    float xDisplacement;

    List<string> clickableTerms;

    const string textContents = "The world is filled half with evil and half with good. We can tilt it forward so that more good runs into our minds, or back, so that more runs into this. But the quantities are the same, we change only their proportion here or there.";

    private void Awake()
    {
        dialogueBubbleText = GetComponentInChildren<Text>();
        xDisplacement = dialogueBubbleText.rectTransform.localPosition.x;
        generator = dialogueBubbleText.cachedTextGenerator;

        dialogueMenu = GetComponentInParent<DialogueMenuController>();

        clickableTerms = new List<string>()
        {
            "minds",
            "evil"
        };
    }

    private void OnEnable()
    {
        dialogueBubbleText.text = textContents;
        HighlightClickableTerms();
        
    }

    void HighlightClickableTerms()
   { 
        string newDialogueBubbleText = dialogueBubbleText.text;
        for (int i = 0; i < clickableTerms.Count; i++)
        {
            string clickableTerm = clickableTerms[i];
            if (!newDialogueBubbleText.Contains(clickableTerm))
            {
                Debug.LogError("clickable term not in string: " + clickableTerm);
                continue;
            }

            // Can't cache indices here because length/position will update as color code is added.
            int startOfTermIndex = newDialogueBubbleText.IndexOf(clickableTerm) - 1;

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

        for (int i = 0; i < clickableTerms.Count; i++)
        {
            string clickableTerm = clickableTerms[i];
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
