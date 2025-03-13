using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _anim;
    private void Awake()
    {
        if (_anim == null)
        _anim = GetComponent<Animator>();
    } 

    public void UpdateAnimation(float velocityX, float velocityY, bool isGrounded, bool isWallSliding, bool isJumping, bool isAttacking)
    {
        _anim.SetFloat("VelX", Mathf.Abs(velocityX));
        _anim.SetFloat("VelY", velocityY);
        _anim.SetBool("IsGrounded", isGrounded);
        _anim.SetBool("IsWallSliding", isWallSliding);
        _anim.SetBool("IsJumping", isJumping);
        _anim.SetBool("IsAttacking", isAttacking);
    }
}
