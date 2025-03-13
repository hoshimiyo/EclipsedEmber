using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Animator anim;
    bool attack = false;
    float timeBetweenAttack, timeSinceAttack;

    void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            // Attack logic
            timeSinceAttack = 0;
            anim.SetTrigger("Attack");
        }
    }
}
