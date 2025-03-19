using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform player;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = Vector3.Lerp(transform.position, player.transform.position + offset, followSpeed);
    }
}
