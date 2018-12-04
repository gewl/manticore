using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    [SerializeField]
    GameObject mainMenu;
    [SerializeField]
    GameObject settingsPane;
    [SerializeField]
    GameObject creditsPane;

    void CloseAllPanes()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void OpenMainMenu()
    {
        CloseAllPanes();

        mainMenu.SetActive(true);
    }

    public void OpenSettingsPane()
    {
        CloseAllPanes();
        settingsPane.SetActive(true);
    }

    public void OpenCreditsPane()
    {
        CloseAllPanes();
        creditsPane.SetActive(true);
    }

    public void OpenTutorialScene()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
