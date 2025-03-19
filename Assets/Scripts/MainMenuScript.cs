using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private ConfirmPanelScript confirmPanel;
    [SerializeField] private Button continueButton;
    public void ShowPanel(GameObject panel)
    {
        if (panel = null)
            return;
        panel.SetActive(true);
    }
    public void HidePanel(GameObject panel)
    {
        if (panel = null)
            return;
        panel.SetActive(false);
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
