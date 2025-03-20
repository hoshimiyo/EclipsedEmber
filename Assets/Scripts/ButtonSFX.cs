using UnityEngine;
using UnityEngine.UI;

public class ButtonSFX : MonoBehaviour
{
    public AudioClip buttonSFX;
    private Button button;

    void Start()
    {
        // Get the Button component
        button = GetComponent<Button>();

        // Add a listener to the button's onClick event
        if (button != null)
        {
            button.onClick.AddListener(()=> PlaySFXClip()) ;
        }
    }
    void PlaySFXClip()
    {
        if (buttonSFX == null || SFXManager.instance == null) return;
        SFXManager.instance.PlaySFXClip(buttonSFX, transform, 1f);
    }
}
