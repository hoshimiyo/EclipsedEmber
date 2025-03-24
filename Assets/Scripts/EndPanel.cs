using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndPanel : MonoBehaviour
{
    public void OnQuitConfirmed()
    {
        Debug.Log("Quit confirmed. Exiting application...");
        Application.Quit();
    }

    public void ToMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
