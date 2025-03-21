using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    [SerializeField] private float animTime = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float animTime = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;

        Destroy(gameObject, animTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
