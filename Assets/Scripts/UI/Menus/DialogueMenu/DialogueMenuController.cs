using UnityEngine;

public class DialogueMenuController : MonoBehaviour {

    MenuManager menuManager;

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
    }

    void InitializeBubbleMatrix()
    {
        dialogueBubbleMatrix = new DialogueBubbleController[3,3];
        DialogueBubbleController[] bubbleList = GetComponentsInChildren<DialogueBubbleController>(true);

        for (int i = 0; i < 3; i++)
        {
            int startingPoint = i * 3;

            for (int j = 0; j < 3; j++)
            {
                dialogueBubbleMatrix[i, j] = bubbleList[startingPoint + j];
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
        if (IsPositionValid(xCoord, yCoord) && !dialogueBubbleMatrix[xCoord, yCoord].gameObject.activeSelf)
        {
            Debug.Log("Found free bubble at " + xCoord + ", " + yCoord);
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
        if (xCoord > 0 && yCoord > 0 && xCoord < 3 && yCoord < 3)
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
