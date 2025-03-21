using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _anim;
    private void Awake()
    {
        if (_anim == null)
            _anim = GetComponent<Animator>();
    }

    public void UpdateAnimation(float velocityX, float velocityY, bool isGrounded, bool isWallSliding, bool isJumping)
    {
        _anim.SetFloat("VelX", Mathf.Abs(velocityX));
        _anim.SetFloat("VelY", velocityY);
        _anim.SetBool("IsGrounded", isGrounded);
        _anim.SetBool("IsJumping", isJumping);
    }

    public void TriggerAttack()
    {
        _anim.SetTrigger("IsAttack");
        _anim.SetLayerWeight(1, 1);
    }

    public void TriggerDeathAnim(bool isDead)
    {
        _anim.SetBool("IsDead", isDead);
        _anim.SetLayerWeight(1, 1);
    }

    public void Healing(bool isHealing)
    {
        _anim.SetBool("IsHealing", isHealing);
    }

    public void Dashing()
    {
        _anim.SetTrigger("IsDashing");
        _anim.SetLayerWeight(1, 1);
    }
}
