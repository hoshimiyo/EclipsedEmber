using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PausePanelScript : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel; // Reference to the PausePanel GameObject
    [SerializeField] private Button resumeButton; // Reference to the Resume button
    [SerializeField] private Button optionButton; // Reference to the Restart button
    [SerializeField] private Button quitButton; // Reference to the Quit button

    private bool isPaused = false; // Track whether the game is paused

    private void Start()
    {
        // Hide the pause panel initially
        pausePanel.SetActive(false);

        // Add listeners to the buttons
        resumeButton.onClick.AddListener(ResumeGame);
        optionButton.onClick.AddListener(OptionPanel);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void Update()
    {
        // Toggle pause when the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    /// <summary>
    /// Toggle the pause state of the game.
    /// </summary>
    private void TogglePause()
    {
        isPaused = !isPaused;

        // Show/hide the pause panel
        pausePanel.SetActive(isPaused);

        // Pause/unpause the game
        Time.timeScale = isPaused ? 0 : 1;
    }

    /// <summary>
    /// Resume the game.
    /// </summary>
    private void ResumeGame()
    {
        TogglePause();
    }

    /// <summary>
    /// Restart the current scene.
    /// </summary>
    private void OptionPanel()
    {
        Debug.Log("OptionTriggered");
    }

    #region Quit
    [SerializeField] private ConfirmPanelScript confirmPanel;
    public void QuitGame()
    {
        confirmPanel.ShowConfirmPanel(
            "Are you sure you want to quit?",
            OnQuitConfirmed,
            OnQuitCancelled
        );
    }

    /// <summary>
    /// Called when the player confirms they want to quit.
    /// </summary>
    private void OnQuitConfirmed()
    {
        Debug.Log("Quit confirmed. Exiting application...");
        Application.Quit();
    }

    /// <summary>
    /// Called when the player cancels the quit action.
    /// </summary>
    private void OnQuitCancelled()
    {
        Debug.Log("Quit cancelled.");
    }
    #endregion
}