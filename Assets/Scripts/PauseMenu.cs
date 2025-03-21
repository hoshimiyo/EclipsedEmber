using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject exitConfirm;
    public GameObject optionsMenu;
    [SerializeField] private GameObject _backgroundMusic;
    private AudioLowPassFilter _lowPass;
    public PlayerMovement player;
    public bool isPaused;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseMenu.SetActive(false);
        if (_backgroundMusic != null) _lowPass = _backgroundMusic.GetComponent<AudioLowPassFilter>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    public void PauseGame()
    {

        pauseMenu.SetActive(true);
        exitConfirm.SetActive(false);
        optionsMenu.SetActive(false);
        if (_backgroundMusic != null) _lowPass.cutoffFrequency = 300f;
        Time.timeScale = 0f;
        player.active = false;
        isPaused = true;

    }
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        exitConfirm.SetActive(false);
        optionsMenu.SetActive(false);
        _lowPass.cutoffFrequency = 22000f;
        player.active = true;
        Time.timeScale = 1f;
        isPaused = false;
    }
    public void OpenOptions()
    {
        optionsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void QuitConfirm()
    {
        exitConfirm.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void OpenQuitConfirm()
    {
        pauseMenu.SetActive(false);
        exitConfirm.SetActive(true);
    }
    public void Return()
    {
        exitConfirm.SetActive(false);
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
    
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
