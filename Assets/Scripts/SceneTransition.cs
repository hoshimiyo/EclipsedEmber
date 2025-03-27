using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition instance;
    public static bool isTrigger;
    [SerializeField] private string transitionTo;
    [SerializeField] private string nextSpawnPoint;
    [SerializeField] private Vector2 exitDirection;
    [SerializeField] private float exitTime;
    private void Start()
    {
        StartCoroutine(GameUI2.instance.sceneFader.Fade(ScreenFader.FadeDirection.Out, Color.black));
    }
    public void TransitionToScene(string sceneName, string spawnPointName)
    {
        nextSpawnPoint = spawnPointName;
        SceneManager.LoadScene(sceneName);
    }

    
    public Vector3 GetSpawnPosition()
    {
        GameObject spawnPoint = GameObject.Find(nextSpawnPoint);
        Debug.Log("method += " + spawnPoint);
        return spawnPoint != null ? spawnPoint.transform.position : Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Trigger: " + GetSpawnPosition());
            isTrigger = true;
            PlayerMovement.instance.transform.position = GetSpawnPosition();
            PlayerStat.InvincibilityFrame(exitTime);
            StartCoroutine(GameUI2.instance.sceneFader.Fade(ScreenFader.FadeDirection.In, Color.black));
            TransitionToScene(transitionTo, nextSpawnPoint);
        }
    }
}
