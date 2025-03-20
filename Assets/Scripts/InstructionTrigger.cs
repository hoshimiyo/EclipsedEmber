using TMPro;
using UnityEngine;

public class InstructionTrigger : MonoBehaviour
{
    [SerializeField] private GameObject instructionText;
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player entered the trigger
        if (other.CompareTag("Player"))
        {
            // Show the instruction panel
            instructionText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the player exited the trigger
        if (other.CompareTag("Player"))
        {
            // Hide the instruction panel
            instructionText.SetActive(false);
        }
    }
}
