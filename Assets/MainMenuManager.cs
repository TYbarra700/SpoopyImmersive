using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
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
}

