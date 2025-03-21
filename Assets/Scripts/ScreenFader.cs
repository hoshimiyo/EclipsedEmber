using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private float fadeTime; // Time for the fade effect
    private Image fadeOutUIImage; // Reference to the Image component

    public enum FadeDirection
    {
        In,  // Fade to black
        Out // Fade to transparent
    }

    private void Awake()
    {
        // Get the Image component
        fadeOutUIImage = GetComponent<Image>();

        // Check if the Image component is assigned
        if (fadeOutUIImage == null)
        {
            Debug.LogError("No Image component found on " + gameObject.name);
        }
    }

    public IEnumerator Fade(FadeDirection fadeDirection)
    {
        // Check if fadeOutUIImage is assigned
        if (fadeOutUIImage == null)
        {
            Debug.LogError("fadeOutUIImage is not assigned!");
            yield break;
        }

        float alpha = fadeDirection == FadeDirection.Out ? 1 : 0;
        float fadeEndValue = fadeDirection == FadeDirection.Out ? 0 : 1;

        if (fadeDirection == FadeDirection.Out)
        {
            while (alpha >= fadeEndValue)
            {
                SetColorImage(ref alpha, fadeDirection);
                yield return null;
            }

            fadeOutUIImage.enabled = false; // Disable the Image after fading out
        }
        else
        {
            fadeOutUIImage.enabled = true; // Enable the Image before fading in
            while (alpha <= fadeEndValue)
            {
                SetColorImage(ref alpha, fadeDirection);
                yield return null;
            }
        }
    }

    private void SetColorImage(ref float _alpha, FadeDirection _fadeDirection)
    {
        // Check if fadeOutUIImage is assigned
        if (fadeOutUIImage == null)
        {
            Debug.LogError("fadeOutUIImage is not assigned!");
            return;
        }

        // Update the alpha value of the Image
        fadeOutUIImage.color = new Color(fadeOutUIImage.color.r, fadeOutUIImage.color.g, fadeOutUIImage.color.b, _alpha);

        // Calculate the new alpha value
        _alpha += Time.deltaTime * (1 / fadeTime) * (_fadeDirection == FadeDirection.Out ? -1 : 1);
    }

    public IEnumerator FadeSeconds(float seconds)
    {
        StartCoroutine(GameUI2.instance.sceneFader.Fade(FadeDirection.In));
        yield return new WaitForSeconds(seconds);
        StartCoroutine(GameUI2.instance.sceneFader.Fade(FadeDirection.Out));
    }
}