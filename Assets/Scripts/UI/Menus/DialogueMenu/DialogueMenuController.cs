using UnityEngine;
using System.Collections.Generic;

public class DialogueMenuController : MonoBehaviour {

    MenuManager menuManager;

    const string ENTRY_NAME = "_entry";
    const string TEXT_NAME = "TEXT";
    const string CLICKABLE_TERMS_NAME = "CLICKABLE_TERMS";
    string _conversationalPartnerID = "";
    JSONObject currentDialogueObject;

    DialogueBubbleController[,] dialogueBubbleMatrix;

    public enum BubbleExpandDirection
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    private void Awake()
    {
        menuManager = GetComponentInParent<MenuManager>();

        InitializeBubbleMatrix();
    }

    private void OnEnable()
    {
        // Turn first bubble on and all others off.
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (x == 0 && y == 0)
                {
                    dialogueBubbleMatrix[x, y].gameObject.SetActive(true);
                }
                else
                {
                    dialogueBubbleMatrix[x, y].gameObject.SetActive(false);
                }
            }
        }
    }

    void InitializeBubbleMatrix()
    {
        dialogueBubbleMatrix = new DialogueBubbleController[3,3];
        DialogueBubbleController[] bubbleList = GetComponentsInChildren<DialogueBubbleController>(true);

        for (int yCoord = 0; yCoord < 3; yCoord++)
        {
            int startingPoint = yCoord * 3;

            for (int xCoord = 0; xCoord < 3; xCoord++)
            {
                dialogueBubbleMatrix[xCoord, yCoord] = bubbleList[startingPoint + xCoord];
            }
        }
    }

    public void RegisterConversationalPartner(string conversationalPartnerID)
    {
        if (conversationalPartnerID == "")
        {
            Debug.LogError("Conversational partner ID not provided");
        }

        if (_conversationalPartnerID != conversationalPartnerID)
        {
            _conversationalPartnerID = conversationalPartnerID;

            currentDialogueObject = MasterSerializer.RetrieveDialogueObject(conversationalPartnerID);
        }

        string textContents = currentDialogueObject[ENTRY_NAME][TEXT_NAME].str;

        JSONObject clickableTermsObject = currentDialogueObject[ENTRY_NAME][CLICKABLE_TERMS_NAME];
        List<string> clickableTerms = new List<string>();

        foreach (JSONObject j in clickableTermsObject.list)
        {
            clickableTerms.Add(j.str);
        }

        dialogueBubbleMatrix[0, 0].ActivateBubble(textContents, clickableTerms);
    }

    public void CloseDialogueMenu()
    {
        menuManager.ToggleDialogueMenu();
    }

    public void RegisterTermClick(DialogueBubbleController dialogueBubble, string clickedTerm)
    {
        int bubbleXCoordinate = -1, bubbleYCoordinate = -1;
        int newBubbleXCoordinate = -1, newBubbleYCoordinate = -1;
        BubbleExpandDirection bubbleExpandDirection;
        string bubbleName = dialogueBubble.name;

        ParseBubbleID(bubbleName, out bubbleXCoordinate, out bubbleYCoordinate);

        FindOpenSpace(bubbleXCoordinate, bubbleYCoordinate, out newBubbleXCoordinate, out newBubbleYCoordinate, out bubbleExpandDirection);

        if (newBubbleXCoordinate == -1 || newBubbleYCoordinate == -1)
        {
            Debug.LogError("Error finding a space for new dialogue bubble.");
        }

        DialogueBubbleController activatingBubble = dialogueBubbleMatrix[newBubbleXCoordinate, newBubbleYCoordinate];
        activatingBubble.gameObject.SetActive(true);

        string textContents = currentDialogueObject[clickedTerm][TEXT_NAME].str;

        JSONObject clickableTermsObject = currentDialogueObject[clickedTerm][CLICKABLE_TERMS_NAME];
        List<string> clickableTerms = new List<string>();

        foreach (JSONObject j in clickableTermsObject.list)
        {
            clickableTerms.Add(j.str);
        }
        activatingBubble.ActivateBubble(textContents, clickableTerms, bubbleExpandDirection);
    }

    void FindOpenSpace(int originalBubbleXCoordinate, int originalBubbleYCoordinate, out int newBubbleXCoordinate, out int newBubbleYCoordinate, out BubbleExpandDirection bubbleExpandDirection)
    {
        int[] firstPosition = new int[2] {originalBubbleXCoordinate + 1, originalBubbleYCoordinate};
        int[] secondPosition = new int[2] {originalBubbleXCoordinate, originalBubbleYCoordinate + 1};
        int[] thirdPosition = new int[2] {originalBubbleXCoordinate - 1, originalBubbleYCoordinate};
        int[] fourthPosition = new int[2] {originalBubbleXCoordinate, originalBubbleYCoordinate - 1};

        // Just doing this manually — iterating is a pain and I want to check them clockwise so I can weight propagation directions.
        if (IsBubbleFree(firstPosition))
        {
            newBubbleXCoordinate = originalBubbleXCoordinate + 1;
            newBubbleYCoordinate = originalBubbleYCoordinate;
            bubbleExpandDirection = BubbleExpandDirection.RIGHT;
        }
        else if (IsBubbleFree(secondPosition))
        {
            newBubbleXCoordinate = originalBubbleXCoordinate;
            newBubbleYCoordinate = originalBubbleYCoordinate + 1;
            bubbleExpandDirection = BubbleExpandDirection.DOWN;
        }
        else if (IsBubbleFree(thirdPosition))
        {
            newBubbleXCoordinate = originalBubbleXCoordinate - 1;
            newBubbleYCoordinate = originalBubbleYCoordinate;
            bubbleExpandDirection = BubbleExpandDirection.LEFT;
        }
        else if (IsBubbleFree(fourthPosition))
        {
            newBubbleXCoordinate = originalBubbleXCoordinate;
            newBubbleYCoordinate = originalBubbleYCoordinate - 1;
            bubbleExpandDirection = BubbleExpandDirection.UP;
        }
        else if (IsPositionValid(firstPosition))
        {
            newBubbleXCoordinate = originalBubbleXCoordinate + 1;
            newBubbleYCoordinate = originalBubbleYCoordinate;
            bubbleExpandDirection = BubbleExpandDirection.RIGHT;
        }
        else if (IsPositionValid(secondPosition))
        {
            newBubbleXCoordinate = originalBubbleXCoordinate;
            newBubbleYCoordinate = originalBubbleYCoordinate + 1;
            bubbleExpandDirection = BubbleExpandDirection.DOWN;
        }
        else if (IsPositionValid(thirdPosition))
        {
            newBubbleXCoordinate = originalBubbleXCoordinate - 1;
            newBubbleYCoordinate = originalBubbleYCoordinate;
            bubbleExpandDirection = BubbleExpandDirection.LEFT;
        }
        else if (IsPositionValid(fourthPosition))
        {
            newBubbleXCoordinate = originalBubbleXCoordinate;
            newBubbleYCoordinate = originalBubbleYCoordinate - 1;
            bubbleExpandDirection = BubbleExpandDirection.UP;
        }
        else
        {
            newBubbleXCoordinate = -1;
            newBubbleYCoordinate = -1;
            bubbleExpandDirection = BubbleExpandDirection.RIGHT;
        }
    }

    bool IsBubbleFree(int[] coords)
    {
        return IsBubbleFree(coords[0], coords[1]);
    }

    bool IsBubbleFree(int xCoord, int yCoord)
    {
        if (IsPositionValid(xCoord, yCoord) && !dialogueBubbleMatrix[xCoord, yCoord].gameObject.activeInHierarchy)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool IsPositionValid(int[] coords)
    {
        return IsPositionValid(coords[0], coords[1]);
    }

    bool IsPositionValid(int xCoord, int yCoord)
    {
        if (xCoord >= 0 && yCoord >= 0 && xCoord < 3 && yCoord < 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void ParseBubbleID(string bubbleName, out int bubbleXCoordinate, out int bubbleYCoordinate)
    {
        string splitChar = "_";
        int splitIndex = bubbleName.IndexOf(splitChar);

        string bubbleID = bubbleName.Substring(splitIndex + 1);

        string[] bubbleCoordinates = bubbleID.Split('-');

        bubbleXCoordinate = int.Parse(bubbleCoordinates[0]);
        bubbleYCoordinate = int.Parse(bubbleCoordinates[1]);
    }
}
