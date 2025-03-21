using System.Collections;
using UnityEngine;

public class PlayerSpellCasting : MonoBehaviour
{
    public bool isCasting = false;
    [Header("Spell Settings")]
    //spell stats
    [SerializeField] float manaSpellCost = 0.3f;
    [SerializeField] float timeBetweenCast = 0.5f;
    float timeSinceCast;
    [SerializeField] float spellDamage; //upspellexplosion and downspellfireball
    [SerializeField] float downSpellForce; // desolate dive only
    //spell cast objects
    [SerializeField] GameObject sideSpellFireball;
    [SerializeField] GameObject upSpellExplosion;
    [SerializeField] GameObject downSpellFireball;
    [SerializeField] public AudioClip spellSFX;

    void CastSpell()
    {
        if (Input.GetButtonDown("CastSpell") && timeSinceCast >= timeBetweenCast && PlayerStat.instance.Mana >= manaSpellCost)
        {
            isCasting = true;
            timeSinceCast = 0;
            StartCoroutine(CastCoroutine());
            PlaySFXClip(spellSFX);
        }
        else
        {
            timeSinceCast += Time.deltaTime;
        }
    }

    IEnumerator CastCoroutine()
    {
        // anim.SetBool("Casting", true);
        yield return new WaitForSeconds(0.15f);

        //side cast
        if (PlayerMovement.instance.yRaw == 0 || (PlayerMovement.instance.yRaw < 0 && PlayerMovement.instance.IsGrounded()))
        {
            GameObject _fireBall = Instantiate(sideSpellFireball, PlayerStat.instance.sideAttackTransform.position, Quaternion.identity);

            //flip fireball
            if (PlayerMovement.instance._isFacingRight)
            {
                _fireBall.transform.eulerAngles = Vector3.zero; // if facing right, fireball continues as per normal
            }
            else
            {
                _fireBall.transform.eulerAngles = new Vector2(_fireBall.transform.eulerAngles.x, 180);
                //if not facing right, rotate the fireball 180 deg
            }
            // pState.recoilingX = true;
        }

        //up cast
        // else if (yAxis > 0)
        // {
        //     Instantiate(upSpellExplosion, transform);
        //     rb.velocity = Vector2.zero;
        // }

        //down cast
        // else if (yAxis < 0 && !Grounded())
        // {
        //     downSpellFireball.SetActive(true);
        // }

        PlayerStat.instance.Mana -= manaSpellCost;
        yield return new WaitForSeconds(0.35f);
        // anim.SetBool("Casting", false);
        isCasting = false;
    }

    private void Update()
    {
        CastSpell();
    }

    private void PlaySFXClip(AudioClip soundClip)
    {
        if (soundClip == null || SFXManager.instance == null) return;
        SFXManager.instance.PlaySFXClip(soundClip, transform, 1f);
    }
}
