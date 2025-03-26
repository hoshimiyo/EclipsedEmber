using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private ConfirmPanelScript confirmPanel;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private Button continueButton;
    [SerializeField] private VideoPlayer vid;
    private void Start()
    {
        optionPanel.SetActive(false);
        vid.gameObject.SetActive(false);
    }
    public void ShowOptionPanel()
    {
        if (optionPanel == null)
            return;
        optionPanel.SetActive(true);
    }
    public void HideOptionPanel()
    {
        if (optionPanel == null)
            return;
        optionPanel.SetActive(false);
    }

    
    #region StartNewGame
    public void StartNewGame()
    {
        if (!SaveSystem.SaveExists())
        {
            SceneManager.LoadScene("Tutorial");
        }
        else
        {
            confirmPanel.ShowConfirmPanel(
                "Delete Old Save And Start From The Beginning?",
                OnNewConfirmed,
                OnNewCancelled
            );
        }
    }

    private void OnNewConfirmed()
    {
        Debug.Log("Start New");
        SaveSystem.DeleteSave();
        vid.gameObject.SetActive(true);
        vid.loopPointReached += OnCutsceneFinished;
        vid.Play();
    }

    private void OnCutsceneFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene("Tutorial");
    }

    private void OnNewCancelled()
    {
        Debug.Log("Cancelled");
    }
    #endregion

    #region Continue
    public void Continue()
    {
        if(!SaveSystem.SaveExists())
            continueButton.interactable = false;
        GameManager.instance.LoadGame();
    }
    #endregion

    #region Quit
    public void QuitGame()
    {
        confirmPanel.ShowConfirmPanel(
            "Are you sure you want to quit?",
            OnQuitConfirmed,
            OnQuitCancelled
        );
    }

    private void OnQuitConfirmed()
    {
        Debug.Log("Quit confirmed. Exiting application...");
        Application.Quit();
    }

    private void OnQuitCancelled()
    {
        Debug.Log("Quit cancelled.");
    }
    #endregion
}
