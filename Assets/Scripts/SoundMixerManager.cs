using UnityEngine;
using UnityEngine.Audio;
public class SoundMixerManager : MonoBehaviour
{
    /*
    để setup audio cứ kéo cái prefab AudioManager vào scene là có audio nhé
    còn add audio vào các obj khác thì cứ copy PlaySFXClip với PlayRandomSFXClip trong PlayerMovement vào, khai báo AudioClip r drag audio clip vào script là xài thôi
    */
    [SerializeField] private AudioMixer _audioMixer;

    public void SetMasterVolumeLevel(float level)
    {
        _audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20f);
    }

    public void SetSFXVolumeLevel(float level)
    {
        _audioMixer.SetFloat("sfxVolume", Mathf.Log10(level) * 20f);
    }

    public void SetMusicVolumeLevel(float level)
    {
        _audioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20f);
    }
}
