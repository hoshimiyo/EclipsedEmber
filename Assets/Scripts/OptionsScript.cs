using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsScript : MonoBehaviour
{
    [SerializeField] private SoundMixerManager mixer;
    [SerializeField] private Slider mainSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    public void setMusicVolume()
    {
        float volume = musicSlider.value;
        mixer.SetMusicVolumeLevel(volume);
    }

    public void setMainVolume()
    {
        float volume = mainSlider.value;
        mixer.SetMasterVolumeLevel(volume);
    }

    public void setSfxVolume()
    {
        float volume = sfxSlider.value;
        mixer.SetSFXVolumeLevel(volume);
    }

}
