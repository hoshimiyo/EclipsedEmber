using System.Collections;
using UnityEngine;

public class DummyScript : MonoBehaviour
{
    [SerializeField] private Animator anim;

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if(_other.tag == "PlayerDamage")
        {
            Debug.Log("GetHit");
            PlayHitAnim();
        }
    }
    public IEnumerator HitAnim()
    {
        anim.SetBool("IsHit", true);
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        anim.SetBool("IsHit", true);
    }

    public void PlayHitAnim()
    {
        StartCoroutine(HitAnim());
    }

}
