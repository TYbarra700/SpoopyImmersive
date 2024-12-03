using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToFirstScene : MonoBehaviour
{
    public void LoadFirstScene()
    {
        // Load the first scene in the build order
        SceneManager.LoadScene(0);
    }
}
