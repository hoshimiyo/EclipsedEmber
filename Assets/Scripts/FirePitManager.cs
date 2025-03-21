using UnityEngine;

public class FirePitManager : MonoBehaviour
{
    public bool interacted;
    [SerializeField] private Animator _anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            interacted = true;
            GameManager.instance.SetRespawnPoint(this);
            _anim.SetBool("IsInteracted", interacted);
        }
    }

    public void Disable()
    {
        interacted = false;
        _anim.SetBool("IsInteracted", interacted);
    }
}
