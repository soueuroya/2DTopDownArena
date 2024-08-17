using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    public Animator animator;
    private Vector2 movement;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        float speed = movement.sqrMagnitude;

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", speed);

        // Set other animation states
        if (Input.GetButtonDown("Jump"))
        {
            animator.SetTrigger("IsJumping");
        }
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("IsAttacking");
        }
        if (Input.GetButtonDown("Fire2"))
        {
            animator.SetTrigger("IsRolling");
        }
        if (Input.GetButtonDown("Run"))
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
    }
}