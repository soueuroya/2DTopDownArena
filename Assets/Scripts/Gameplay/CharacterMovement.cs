using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;

    private Vector2 lookDir;
    private Vector2 movement;
    private Vector2 mousePosition;

    private bool isBusy = false;
    private bool IsLookingUp_C = false;
    private bool IsLookingDown_C = false;
    private bool IsLookingLeft_C = false;
    private bool IsLookingRight_C = false;
    private bool IsLookingUpLeft_C = false;
    private bool IsLookingUpRight_C = false;
    private bool IsLookingDownLeft_C = false;
    private bool IsLookingDownRight_C = false;

    private bool IsLookingUp_K = false;
    private bool IsLookingDown_K = false;
    private bool IsLookingLeft_K = false;
    private bool IsLookingRight_K = false;
    private bool IsLookingUpLeft_K = false;
    private bool IsLookingUpRight_K = false;
    private bool IsLookingDownLeft_K = false;
    private bool IsLookingDownRight_K = false;


    void Update()
    {
        // Get movement input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Calculate movement speed
        float currentSpeed = movement.sqrMagnitude;

        // Update the Animator with the current speed
        animator.SetFloat("Speed", currentSpeed);

        // Get mouse position relative to the player
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lookDir = mousePosition - rb.position;

        // Normalize the look direction for comparison
        lookDir.Normalize();

        // Reset all direction WASD booleans
        ResetDirectionWASDBools();

        // Reset all direction Cursor booleans
        ResetDirectionCursorBools();

        // Reads the direction from WASD
        ReadDirectionWithWASD();

        // Reads the direction of the cursor
        ReadDirectionWithCursor(lookDir);

        if (currentSpeed != 0.0f)
        {
            // Updates animator with cursor direction
            UpdateDirectionWithWASD();
        }
        else
        {
            // Updates animator with cursor direction
            UpdateDirectionWithCursor();
        }

        // Normalize the movement vector if the player is moving diagonally
        movement.Normalize();
    }

    private void ReadDirectionWithCursor(Vector2 lookDir)
    {
        // Determine which direction the player is facing - CURSOR
        if (lookDir.x > 0.5f && Mathf.Abs(lookDir.y) < 0.5f)
        {
            IsLookingRight_C = true;
        }
        else if (lookDir.x < -0.5f && Mathf.Abs(lookDir.y) < 0.5f)
        {
            IsLookingLeft_C = true;
        }
        else if (lookDir.y > 0.5f && Mathf.Abs(lookDir.x) < 0.5f)
        {
            IsLookingUp_C = true;
        }
        else if (lookDir.y < -0.5f && Mathf.Abs(lookDir.x) < 0.5f)
        {
            IsLookingDown_C = true;
        }
        else if (lookDir.x > 0.5f && lookDir.y > 0.5f)
        {
            IsLookingUpRight_C = true;
        }
        else if (lookDir.x < -0.5f && lookDir.y > 0.5f)
        {
            IsLookingUpLeft_C = true;
        }
        else if (lookDir.x > 0.5f && lookDir.y < -0.5f)
        {
            IsLookingDownRight_C = true;
        }
        else if (lookDir.x < -0.5f && lookDir.y < -0.5f)
        {
            IsLookingDownLeft_C = true;
        }
    }

    private void UpdateDirectionWithCursor()
    {
        animator.SetBool("IsLookingRight", IsLookingRight_C);
        animator.SetBool("IsLookingLeft", IsLookingLeft_C);
        animator.SetBool("IsLookingUp", IsLookingUp_C);
        animator.SetBool("IsLookingDown", IsLookingDown_C);
        animator.SetBool("IsLookingUpRight", IsLookingUpRight_C);
        animator.SetBool("IsLookingUpLeft", IsLookingUpLeft_C);
        animator.SetBool("IsLookingDownRight", IsLookingDownRight_C);
        animator.SetBool("IsLookingDownLeft", IsLookingDownLeft_C);
    }

    private void UpdateDirectionWithWASD()
    {
        animator.SetBool("IsLookingUp", IsLookingUp_K);
        animator.SetBool("IsLookingLeft", IsLookingLeft_K);
        animator.SetBool("IsLookingDown", IsLookingDown_K);
        animator.SetBool("IsLookingRight", IsLookingRight_K);
        animator.SetBool("IsLookingUpLeft", IsLookingUpLeft_K);
        animator.SetBool("IsLookingUpRight", IsLookingUpRight_K);
        animator.SetBool("IsLookingDownLeft", IsLookingDownLeft_K);
        animator.SetBool("IsLookingDownRight", IsLookingDownRight_K);
    }

    private void ReadDirectionWithWASD()
    {
        // Determine which direction the player is facing - WALK - WASD
        if (movement.x > 0.5f && Mathf.Abs(movement.y) < 0.5f)
        {
            IsLookingRight_K = true;
        }
        else if (movement.x < -0.5f && Mathf.Abs(movement.y) < 0.5f)
        {
            IsLookingLeft_K = true;
        }
        else if (movement.y > 0.5f && Mathf.Abs(movement.x) < 0.5f)
        {
            IsLookingUp_K = true;
        }
        else if (movement.y < -0.5f && Mathf.Abs(movement.x) < 0.5f)
        {
            IsLookingDown_K = true;
        }
        else if (movement.x > 0.5f && movement.y > 0.5f)
        {
            IsLookingUpRight_K = true;
        }
        else if (movement.x < -0.5f && movement.y > 0.5f)
        {
            IsLookingUpLeft_K = true;
        }
        else if (movement.x > 0.5f && movement.y < -0.5f)
        {
            IsLookingDownRight_K = true;
        }
        else if (movement.x < -0.5f && movement.y < -0.5f)
        {
            IsLookingDownLeft_K = true;
        }
    }

    void FixedUpdate()
    {
        // Move the character
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void ResetDirectionWASDBools()
    {
        IsLookingRight_K = false;
        IsLookingLeft_K = false;
        IsLookingUp_K = false;
        IsLookingDown_K = false;
        IsLookingUpRight_K = false;
        IsLookingUpLeft_K = false;
        IsLookingDownRight_K = false;
        IsLookingDownLeft_K = false;
    }

    private void ResetDirectionCursorBools()
    {
        IsLookingRight_C = false;
        IsLookingLeft_C = false;
        IsLookingUp_C = false;
        IsLookingDown_C = false;
        IsLookingUpRight_C = false;
        IsLookingUpLeft_C = false;
        IsLookingDownRight_C = false;
        IsLookingDownLeft_C = false;
    }
}