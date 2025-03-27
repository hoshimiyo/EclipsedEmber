using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PausePanelScript : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel; // Reference to the PausePanel GameObject
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Button resumeButton; // Reference to the Resume button
    [SerializeField] private Button optionButton; // Reference to the Restart button
    public PlayerMovement player;
    private bool isPaused = false; // Track whether the game is pauseds

    private void Start()
    {
        // Hide the pause panel initially
        pausePanel.SetActive(false);
        optionPanel.SetActive(false);
        // Add listeners to the buttons
        resumeButton.onClick.AddListener(ResumeGame);
        optionButton.onClick.AddListener(OptionPanel);
    }

    private void Update()
    {
        // Toggle pause when the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (isPaused)
            {
                if (optionPanel.activeInHierarchy)
                {
                    optionPanel.SetActive(false);
                    pausePanel.SetActive(true);
                }
                else ResumeGame();
            }
            else
            {
                if (mixer != null) mixer.SetFloat("Lowpass", 300f);
                player.active = false;
                TogglePause();
            }

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
        mixer.SetFloat("Lowpass", 22000f);
        player.active = true;
        TogglePause();
    }


    private void OptionPanel()
    {
        optionPanel.SetActive(true);
        pausePanel.SetActive(false);
    }
    public void CloseOption()
    {
        optionPanel.SetActive(false);
        pausePanel.SetActive(true);
    }
    #region Quit
    [SerializeField] private ConfirmPanelScript confirmPanel;
    public void QuitGame()
    {
        confirmPanel.ShowConfirmPanel(
            "Are you sure you want to quit?",
            OnQuitFromPauseConfirmed,
            OnQuitFromPauseCancelled
        );
    }

    private void OnQuitFromPauseConfirmed()
    {
        Debug.Log("Quit confirmed. Exiting application...");
        GameManager.instance.SaveGame();
        Application.Quit();
    }

    private void OnQuitFromPauseCancelled()
    {
        Debug.Log("Quit cancelled.");
    }
    #endregion

    #region ToMenu
    public void QuitToMenu()
    {
        confirmPanel.ShowConfirmPanel(
            "Save and Quit to Menu?",
            OnMenuConfirmed,
            OnMenuCancelled
        );
    }

    private void OnMenuConfirmed()
    {
        Debug.Log("Quit confirmed. Exiting application...");
        mixer.SetFloat("Lowpass", 22000f);
        GameManager.instance.SaveGame();
        SceneManager.LoadScene("MainMenuScene");
    }

    private void OnMenuCancelled()
    {
        Debug.Log("Quit cancelled.");
    }

    #endregion
}