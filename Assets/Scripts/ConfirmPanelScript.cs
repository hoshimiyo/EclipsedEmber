using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPanelScript : MonoBehaviour
{
    [SerializeField] private GameObject confirmPanel; // Reference to the ConfirmPanel GameObject
    [SerializeField] private TextMeshProUGUI messageText; // Reference to the Text component
    [SerializeField] private Button yesButton; // Reference to the Yes button
    [SerializeField] private Button noButton; // Reference to the No button

    private System.Action onYesAction; // Callback for Yes button
    private System.Action onNoAction; // Callback for No button

    private void Start()
    {
        // Hide the panel initially
        confirmPanel.SetActive(false);

        // Add listeners to the buttons
        yesButton.onClick.AddListener(OnYesClicked);
        noButton.onClick.AddListener(OnNoClicked);
    }

    public void ShowConfirmPanel(string message, System.Action onYes, System.Action onNo)
    {
        // Set the message text
        messageText.text = message;

        // Store the callbacks
        onYesAction = onYes;
        onNoAction = onNo;

        // Show the panel
        confirmPanel.SetActive(true);
    }
    
    private void OnYesClicked()
    {
        // Execute the Yes action
        onYesAction?.Invoke();

        // Hide the panel
        confirmPanel.SetActive(false);
    }
    
    private void OnNoClicked()
    {
        // Execute the No action
        onNoAction?.Invoke();

        // Hide the panel
        confirmPanel.SetActive(false);
    }
}
