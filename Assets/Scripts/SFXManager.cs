using System.Collections;
using UnityEngine;

public class SFXManager : MonoBehaviour
{

    [SerializeField] private AudioSource sfxObject;
    public static SFXManager instance;
    private Coroutine sfxCoroutine;
    public bool isPlayingSFX = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlaySFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(sfxObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;

        audioSource.volume = volume;
        audioSource.Play();

        float length = audioSource.clip.length;
        Destroy(audioSource.gameObject, length);
    }

    public void PlayRandomSFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {
        int random = Random.Range(0, audioClip.Length);
        AudioSource audioSource = Instantiate(sfxObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip[random];

        audioSource.volume = volume;
        audioSource.Play();

        float length = audioSource.clip.length;
        Destroy(audioSource.gameObject, length);
    }
    public void PlaySFXClipRepeat(AudioClip audioClip, Transform spawnTransform, float volume, float repeatRate)
    {
        if (isPlayingSFX) return; // Prevents multiple calls

        isPlayingSFX = true;
        sfxCoroutine = StartCoroutine(PlaySFXRepeatedly(audioClip, spawnTransform, volume, repeatRate));
    }

    private IEnumerator PlaySFXRepeatedly(AudioClip audioClip, Transform spawnTransform, float volume, float repeatRate)
    {
        while (true)
        {
            AudioSource audioSource = new GameObject("SFX").AddComponent<AudioSource>();
            audioSource.transform.position = spawnTransform.position;
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.Play();

            Destroy(audioSource.gameObject, audioSource.clip.length);

            yield return new WaitForSeconds(repeatRate);
        }
    }

    public void StopSFXClipRepeat()
    {
        if (sfxCoroutine != null)
        {
            StopCoroutine(sfxCoroutine);
            isPlayingSFX = false;
        }
    }
}
