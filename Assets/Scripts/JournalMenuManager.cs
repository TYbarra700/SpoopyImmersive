using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class JournalMenuManager : MonoBehaviour
{
    [Header("Journal Setup")]
    public GameObject journal; // Reference to the journal object
    public AudioSource audioCue; // Audio source for button clicks

    [Header("Menu Pages")]
    public GameObject mainPage;
    public GameObject titleConfirmPage;
    public GameObject exitConfirmPage;
    public GameObject controlsPage;

    [Header("Buttons - Main Page")]
    public Button controlsButton;
    public Button titleButton;
    public Button exitButton;

    [Header("Buttons - Controls Page")]
    public Button backButton;

    [Header("Buttons - Title Confirm Page")]
    public Button titleYesButton;
    public Button titleNoButton;

    [Header("Buttons - Exit Confirm Page")]
    public Button exitYesButton;
    public Button exitNoButton;

    [Header("Input Actions")]
    public InputActionReference pauseAction; // Reference to the Pause Menu action (from Input System)

    void OnEnable()
    {
        // Enable the pause action and subscribe to its event
        if (pauseAction != null && pauseAction.action != null)
        {
            pauseAction.action.Enable();
            pauseAction.action.performed += OnPauseButtonPressed;
        }
        else
        {
            Debug.LogError("Pause Action is not assigned or invalid.");
        }
    }

    void OnDisable()
    {
        // Disable the pause action and unsubscribe from its event
        if (pauseAction != null && pauseAction.action != null)
        {
            pauseAction.action.performed -= OnPauseButtonPressed;
            pauseAction.action.Disable();
        }
    }

    void Start()
    {
        // Add listeners to main page buttons
        if (controlsButton != null) controlsButton.onClick.AddListener(() => NavigateToPage(controlsPage));
        if (titleButton != null) titleButton.onClick.AddListener(() => NavigateToPage(titleConfirmPage));
        if (exitButton != null) exitButton.onClick.AddListener(() => NavigateToPage(exitConfirmPage));

        // Add listeners to controls page buttons
        if (backButton != null) backButton.onClick.AddListener(() => NavigateToPage(mainPage));

        // Add listeners to title confirm page buttons
        if (titleYesButton != null) titleYesButton.onClick.AddListener(() => LoadScene("HubScene")); // Replace with your hub scene name
        if (titleNoButton != null) titleNoButton.onClick.AddListener(() => NavigateToPage(mainPage));

        // Add listeners to exit confirm page buttons
        if (exitYesButton != null) exitYesButton.onClick.AddListener(QuitGame);
        if (exitNoButton != null) exitNoButton.onClick.AddListener(() => NavigateToPage(mainPage));
    }

    void OnPauseButtonPressed(InputAction.CallbackContext context)
    {
        ToggleJournal();
    }

    public void ToggleJournal()
    {
        bool isActive = journal.activeSelf;
        journal.SetActive(!isActive);

        if (!isActive)
        {
            NavigateToPage(mainPage); // Default to main page when opening the journal
        }
    }

    public void NavigateToPage(GameObject targetPage)
    {
        // Play audio cue
        if (audioCue != null)
        {
            audioCue.Play();
        }

        // Deactivate all pages
        mainPage.SetActive(false);
        titleConfirmPage.SetActive(false);
        exitConfirmPage.SetActive(false);
        controlsPage.SetActive(false);

        // Activate the target page
        targetPage.SetActive(true);
    }

    public void LoadScene(string sceneName)
    {
        // Play audio cue
        if (audioCue != null)
        {
            audioCue.Play();
        }

        // Load the specified scene
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        // Play audio cue
        if (audioCue != null)
        {
            audioCue.Play();
        }

        // Exit the application
        Application.Quit();
#if UNITY_EDITOR
        // For testing in the editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
