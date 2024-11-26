using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // References to the menus
    public GameObject mainMenu;
    public GameObject controlsMenu;

    // This function loads the next scene based on the current scene index
    public void StartGame()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }

    // This function quits the application
    public void ExitGame()
    {
        Debug.Log("Exiting Game"); // This will only show in the Unity Editor
        Application.Quit();
    }

    // This function hides the main menu and shows the controls menu
    public void ShowControlsMenu()
    {
        mainMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }

    // This function hides the controls menu and shows the main menu
    public void BackToMainMenu()
    {
        controlsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
}
