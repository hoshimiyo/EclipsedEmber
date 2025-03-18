using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string transitionFromScene;
    public static GameManager instance;
    public Vector2 respawnPoint;
    [SerializeField] private FirePitManager _firePitManager;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetRespawnPoint(FirePitManager firePit)
    {
        if (_firePitManager == null)
        {
            _firePitManager = firePit;
        }
        else if (_firePitManager != null && _firePitManager != firePit)
        {
            _firePitManager.Disable();
            _firePitManager = firePit;
        }
        respawnPoint = firePit.transform.position;
    }
}
