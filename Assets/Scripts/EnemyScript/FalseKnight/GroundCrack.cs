using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCrack : AttackHitbox
{
    private Animator anim;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
    }

    protected override void ApplyDamage(Collider2D player)
    {
        base.ApplyDamage(player);
    }

    protected override void DestroyHitbox()
    {
        base.DestroyHitbox();
    }

    protected override void ResetHitbox()
    {
        base.ResetHitbox();
    }
}