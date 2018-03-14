using UnityEngine;

public class DialogueMenuController : MonoBehaviour {

    MenuManager menuManager;

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
    }

    void Start () {
		
	}
	
	void Update () {
		
	}

    public void CloseDialogueMenu()
    {
        menuManager.ToggleDialogueMenu();
    }
}
