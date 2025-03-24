using UnityEngine;

public class UnlockDoubleJump : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            PlayerMovement.instance.canDoubleJump = true;
        }
        Destroy(gameObject);
    }
}
