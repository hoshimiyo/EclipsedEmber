using UnityEngine;
using UnityEngine.UI;

public class SaveSystemUI : MonoBehaviour
{
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        // Set up button listeners
        saveButton.onClick.AddListener(Save);
        loadButton.onClick.AddListener(Load);
        closeButton.onClick.AddListener(Hide);

        // Hide by default
        Hide();
    }

    public void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void Save()
    {
        GameManager.instance.SaveGame();
        // Show feedback that game was saved
    }

    public void Load()
    {
        GameManager.instance.LoadGame();
    }

    private void OnDestroy()
    {
        // Clean up listeners
        saveButton.onClick.RemoveListener(Save);
        loadButton.onClick.RemoveListener(Load);
        closeButton.onClick.RemoveListener(Hide);
    }
}