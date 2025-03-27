using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform player;

    private Vector3 originalPosition;  // Store original camera position for shaking
    private float shakeDuration = 0f;  // How long the camera will shake
    private float shakeMagnitude = 0f;  // How intense the shake will be
    private Vector3 shakeOffset;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            player = GameObject.FindGameObjectWithTag("Player").transform;
            originalPosition = transform.position;
            transform.position = Vector3.Lerp(transform.position, player.transform.position + offset, followSpeed);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

    }

    void Update()
    {
        // Follow the player
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = Vector3.Lerp(transform.position, player.transform.position + offset + shakeOffset, followSpeed);

        // Handle camera shake (if needed)
        if (shakeDuration > 0)
        {
            shakeDuration -= Time.deltaTime;
            shakeOffset = new Vector3(Random.Range(-shakeMagnitude, shakeMagnitude), Random.Range(-shakeMagnitude, shakeMagnitude), 0f);
        }
        else
        {
            shakeDuration = 0f;
            shakeOffset = Vector3.zero;
        }
    }

    // Method to trigger camera shake
    public void ShakeCamera(float magnitude, float duration)
    {
        shakeMagnitude = magnitude;
        shakeDuration = duration;
    }
}
