using System.Collections;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    #region Player
    public static PlayerCamera instance;
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform player;
    #endregion 

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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    // Update is called once per frame
    void Update()
    {
        if (!isSwitched)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            transform.position = Vector3.Lerp(transform.position, player.transform.position + offset, followSpeed);
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
