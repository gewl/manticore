using UnityEngine;
using System.Collections.Generic;

public class DialogueMenuController : MonoBehaviour {

    MenuManager menuManager;
    const string textContents = "The world is filled half with evil and half with good. We can tilt it forward so that more good runs into our minds, or back, so that more runs into this. But the quantities are the same, we change only their proportion here or there.";
    List<string> clickableTerms;

    DialogueBubbleController[,] dialogueBubbleMatrix;

    public enum BubbleSpawnDirections
    {
        Up,
        Down,
        Left,
        Right 
    }

    private void Awake()
    {
        menuManager = GetComponentInParent<MenuManager>();

        InitializeBubbleMatrix();
        clickableTerms = new List<string>()
        {
            "minds",
            "evil"
        };
    }

    private void OnEnable()
    {
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

        dialogueBubbleMatrix[0, 0].UpdateBubbleContents(textContents, clickableTerms);
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

    public void CloseDialogueMenu()
    {
        menuManager.ToggleDialogueMenu();
    }

    public void RegisterTermClick(DialogueBubbleController dialogueBubble, string clickedTerm)
    {
        int bubbleXCoordinate = -1, bubbleYCoordinate = -1;
        int newBubbleXCoordinate = -1, newBubbleYCoordinate = -1;
        string bubbleName = dialogueBubble.name;

        ParseBubbleID(bubbleName, out bubbleXCoordinate, out bubbleYCoordinate);

        FindOpenSpace(bubbleXCoordinate, bubbleYCoordinate, out newBubbleXCoordinate, out newBubbleYCoordinate);

        if (newBubbleXCoordinate == -1 || newBubbleYCoordinate == -1)
        {
            Debug.LogError("Error finding a space for new dialogue bubble.");
        }

        DialogueBubbleController activatingBubble = dialogueBubbleMatrix[newBubbleXCoordinate, newBubbleYCoordinate];
        activatingBubble.gameObject.SetActive(true);
        activatingBubble.UpdateBubbleContents(textContents, clickableTerms);
    }

    void FindOpenSpace(int originalBubbleXCoordinate, int originalBubbleYCoordinate, out int newBubbleXCoordinate, out int newBubbleYCoordinate)
    {
        int[] firstPosition = new int[2] {originalBubbleXCoordinate + 1, originalBubbleYCoordinate};
        int[] secondPosition = new int[2]{originalBubbleXCoordinate, originalBubbleYCoordinate + 1};
        int[] thirdPosition = new int[2]{originalBubbleXCoordinate - 1, originalBubbleYCoordinate};
        int[] fourthPosition = new int[2]{originalBubbleXCoordinate, originalBubbleYCoordinate - 1};

        // Just doing this manually — iterating is a pain and I want to weight them so they check in a clockwise fashion.
        if (IsBubbleFree(firstPosition))
        {
            newBubbleXCoordinate = originalBubbleXCoordinate + 1;
            newBubbleYCoordinate = originalBubbleYCoordinate;
        }
        else if (IsBubbleFree(secondPosition))
        {
            newBubbleXCoordinate = originalBubbleXCoordinate;
            newBubbleYCoordinate = originalBubbleYCoordinate + 1;
        }
        else if (IsBubbleFree(thirdPosition))
        {
            newBubbleXCoordinate = originalBubbleXCoordinate - 1;
            newBubbleYCoordinate = originalBubbleYCoordinate;
        }
        else if (IsBubbleFree(fourthPosition))
        {
            newBubbleXCoordinate = originalBubbleXCoordinate;
            newBubbleYCoordinate = originalBubbleYCoordinate - 1;
        }
        else if (IsPositionValid(firstPosition))
        {
            newBubbleXCoordinate = originalBubbleXCoordinate + 1;
            newBubbleYCoordinate = originalBubbleYCoordinate;
        }
        else if (IsPositionValid(secondPosition))
        {
            newBubbleXCoordinate = originalBubbleXCoordinate;
            newBubbleYCoordinate = originalBubbleYCoordinate + 1;
        }
        else if (IsPositionValid(thirdPosition))
        {
            newBubbleXCoordinate = originalBubbleXCoordinate - 1;
            newBubbleYCoordinate = originalBubbleYCoordinate;
        }
        else if (IsPositionValid(fourthPosition))
        {
            newBubbleXCoordinate = originalBubbleXCoordinate;
            newBubbleYCoordinate = originalBubbleYCoordinate - 1;
        }
        else
        {
            newBubbleXCoordinate = -1;
            newBubbleYCoordinate = -1;
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
