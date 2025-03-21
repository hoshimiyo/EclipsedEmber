using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition instance;
    [SerializeField] private string transitionTo;
    [SerializeField] private string nextSpawnPoint;
    [SerializeField] private Vector2 exitDirection;
    [SerializeField] private float exitTime;
    private void Start()
    {
        StartCoroutine(GameUI2.instance.sceneFader.Fade(ScreenFader.FadeDirection.Out));
    }   
    public void TransitionToScene(string sceneName, string spawnPointName)
    {
        nextSpawnPoint = spawnPointName;
        SceneManager.LoadScene(sceneName);
    }   

    public Vector3 GetSpawnPosition()
    {
        GameObject spawnPoint = GameObject.Find(nextSpawnPoint);
        return spawnPoint != null ? spawnPoint.transform.position : Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement.instance.transform.position = GetSpawnPosition();
            PlayerStat.InvincibilityFrame(exitTime);
            StartCoroutine(GameUI2.instance.sceneFader.Fade(ScreenFader.FadeDirection.In));
            TransitionToScene(transitionTo, nextSpawnPoint);
        }
    }
}
