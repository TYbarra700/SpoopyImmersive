using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string sceneToLoad; // Set this in the Inspector to specify the scene name

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure the player has the "Player" tag
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
