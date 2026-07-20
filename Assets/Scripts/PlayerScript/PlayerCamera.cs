using System.Collections;
using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{
    #region Player
    public static PlayerCamera instance;
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform player;
    #endregion 

    private Vector3 originalPosition;  // Store original camera position for shaking
    private float shakeDuration = 0f;  // How long the camera will shake
    private float shakeMagnitude = 0f;  // How intense the shake will be
    private Vector3 shakeOffset;

    #region Awake, Start
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SnapToPlayer(); // Try snapping first
        StartCoroutine(SnapAfterOneFrame()); // Fallback for delayed player spawn
    }
    #endregion

    #region Player
    private IEnumerator SnapAfterOneFrame()
    {
        yield return null; // Wait one frame
        SnapToPlayer(); // Final correction after everything initializes
    }

    void Update()
    {
        if (!isSwitched)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            transform.position = Vector3.Lerp(transform.position, player.transform.position + offset + shakeOffset, followSpeed);
        }
        else
        {
            // Countdown and switch back
            switchTimer -= Time.deltaTime;
            if (switchTimer <= 0f)
            {
                isSwitched = false;
            }
        }
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


    public void SnapToPlayer()
    {
        if (player != null)
        {
            transform.position = player.position + offset;
        }
    }
    #endregion

    #region Switch
    private bool isSwitched = false;
    private float switchTimer = 0f;
    public Transform alternateTarget; // The object to switch to on collision
    public float switchDuration = 3f; // How long (seconds) to stay on alternate target

    // Call this when collision happens
    public void SwitchToAlternateTarget()
    {
        if (alternateTarget != null)
        {
            isSwitched = true;
            switchTimer = switchDuration;
            transform.position = alternateTarget.position + offset;
            transform.LookAt(alternateTarget);
        }
    }
    #endregion
}
