using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private float moveSpeed = 5f;
    private float dashSpeed = 5f;
    private float dashDuration = 0.2f; // How long the dash lasts
    private float dashCooldown = 1f; // Time before the player can dash again
    private float jumpCooldown = 1f; // Time before the player can jump again
    private float maxHealth = 5f;
    private float deadZone = 0.1f;
    [SerializeField]private Rigidbody2D rb;
    [SerializeField]private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField]private Transform directional;
    [SerializeField] private Transform spawnerAxis;
    [SerializeField] private Transform spawner;
    [SerializeField] private Transform meleeSpawner;
    [SerializeField] private Transform shieldSpawner;
    [SerializeField] private Transform shadow;

    [SerializeField] private TMP_InputField moveSpeedInput;
    [SerializeField] private TMP_InputField dashSpeedInput;
    [SerializeField] private TMP_InputField dashDurationInput;
    [SerializeField] private TMP_InputField dashCooldownInput;


    [SerializeField] List<PlayerStatus> playerStatusses;
    [SerializeField] int currentStatus = 0;
    [SerializeField] bool localPlayer = false;

    private bool isSlow = false;
    private bool isDead = false;
    private bool isFalling = false;
    private bool shouldFall = false;
    private bool isDashing = false; // To track if currently dashing
    private Vector2 lookDir;
    private Vector2 movement;
    private Vector2 aim;
    private Vector2 mousePosition;
    private Vector2 prevMousePosition;
    private Vector2 playerPosition;
    private Vector2 prevPlayerPosition;
    private Vector2 respawnPosition;

    private float generalCooldown = 0;
    private float stunnedCooldown = 0;
    private float attack1Cooldown = 0;
    private float attack2Cooldown = 0;
    private float shieldCooldown = 0;
    private float dashingCooldown = 0;

    private bool isAttacking = false;

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

    private void Awake()
    {
        UpdateValuesWithCurrentClass();
        SaveRespawnPosition();

        if (!localPlayer)
        {
            return;
        }

        moveSpeedInput.onValueChanged.AddListener(OnMoveSpeedChanged);
        dashSpeedInput.onValueChanged.AddListener(OnDashSpeedChanged);
        dashDurationInput.onValueChanged.AddListener(OnDashDurationChanged);
        dashCooldownInput.onValueChanged.AddListener(OnDashCooldownChanged);
    }

    void Update()
    {
        if (!localPlayer)
        {
            return;
        }

        if (isDead)
        {
            return;
        }

        // Get movement input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Calculate movement speed
        float currentSpeed = movement.sqrMagnitude;

        // Update the Animator with the current speed
        animator.SetFloat("Speed", currentSpeed);

        aim.x = Input.GetAxis("Horizontal_Aim");
        aim.y = Input.GetAxis("Vertical_Aim");

        if (prevMousePosition != mousePosition || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            Cursor.visible = true;
        }
        prevMousePosition = mousePosition;

        if (aim.x != 0 || aim.y != 0)
        {
            Cursor.visible = false;
        }

        if (!Cursor.visible)
        {
            if (aim != Vector2.zero)
            {
                lookDir = aim;
            }
        }
        else
        {
            // Get mouse position relative to the player
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lookDir = mousePosition - rb.position;
        }

        // Normalize the look direction for comparison
        lookDir.Normalize();

        // Handle dash input (e.g., pressing Space)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (dashingCooldown <= 0)
            {
                if (CanAct())
                {
                    StartCoroutine(Dash());
                }
            }
        }

        // Reset all direction Cursor booleans
        ResetDirectionCursorBools();

        // Reads the direction of the cursor
        ReadDirectionWithCursor(lookDir);

        // Reset all direction WASD booleans
        ResetDirectionWASDBools();

        // Reads the direction from WASD
        ReadDirectionWithWASD();

        // Reads the character switch
        HandleStatusSwitch();

        // Handles the angle of the directional
        HandleDirectional();

        HandleAttacks();

        if (currentSpeed != 0.0f && !isAttacking)
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

        HandleCooldowns();
    }

    private void HandleAttacks()
    {
        // Handle attack inputs
        if (Input.GetMouseButton(0) && MouseGameScreenTarget.Instance.mouseOver)
        {
            isSlow = true;

            if (attack1Cooldown > 0 || !CanAct()) return;
            StartCoroutine(PerformAttack1(playerStatusses[currentStatus].attack1));
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (attack2Cooldown > 0 || !CanAct()) return;
            StartCoroutine(PerformAttack2(playerStatusses[currentStatus].attack2));
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (shieldCooldown > 0 || !CanAct()) return;
            StartCoroutine(PerformShield(playerStatusses[currentStatus].shield));
        }
    }

    private void ReadDirectionWithCursor(Vector2 lookDir)
    {
        // Determine which direction the player is facing - CURSOR
        if (lookDir.x > deadZone && Mathf.Abs(lookDir.y) < deadZone)
        {
            IsLookingRight_C = true;
        }
        else if (lookDir.x < -deadZone && Mathf.Abs(lookDir.y) < deadZone)
        {
            IsLookingLeft_C = true;
        }
        else if (lookDir.y > deadZone && Mathf.Abs(lookDir.x) < deadZone)
        {
            IsLookingUp_C = true;
        }
        else if (lookDir.y < -deadZone && Mathf.Abs(lookDir.x) < deadZone)
        {
            IsLookingDown_C = true;
        }
        else if (lookDir.x > deadZone && lookDir.y > deadZone)
        {
            IsLookingUpRight_C = true;
        }
        else if (lookDir.x < -deadZone && lookDir.y > deadZone)
        {
            IsLookingUpLeft_C = true;
        }
        else if (lookDir.x > deadZone && lookDir.y < -deadZone)
        {
            IsLookingDownRight_C = true;
        }
        else if (lookDir.x < -deadZone && lookDir.y < -deadZone)
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
        if (movement.x > deadZone && Mathf.Abs(movement.y) < deadZone)
        {
            IsLookingRight_K = true;
            if (!Cursor.visible)
            {
                IsLookingRight_C = true;
            }
        }
        else if (movement.x < -deadZone && Mathf.Abs(movement.y) < deadZone)
        {
            IsLookingLeft_K = true;
            if (!Cursor.visible)
            {
                IsLookingLeft_C = true;
            }
        }
        else if (movement.y > deadZone && Mathf.Abs(movement.x) < deadZone)
        {
            IsLookingUp_K = true;
            if (!Cursor.visible)
            {
                IsLookingUp_C = true;
            }
        }
        else if (movement.y < -deadZone && Mathf.Abs(movement.x) < deadZone)
        {
            IsLookingDown_K = true;
            if (!Cursor.visible)
            {
                IsLookingDown_C = true;
            }
        }
        else if (movement.x > deadZone && movement.y > deadZone)
        {
            IsLookingUpRight_K = true;
            if (!Cursor.visible)
            {
                IsLookingUpRight_C = true;
            }
        }
        else if (movement.x < -deadZone && movement.y > deadZone)
        {
            IsLookingUpLeft_K = true;
            if (!Cursor.visible)
            {
                IsLookingUpLeft_C = true;
            }
        }
        else if (movement.x > deadZone && movement.y < -deadZone)
        {
            IsLookingDownRight_K = true;
            if (!Cursor.visible)
            {
                IsLookingDownRight_C = true;
            }
        }
        else if (movement.x < -deadZone && movement.y < -deadZone)
        {
            IsLookingDownLeft_K = true;
            if (!Cursor.visible)
            {
                IsLookingDownLeft_C = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (!localPlayer)
        {
            return;
        }

        // Move the character
        if (CanWalk())
        {
            if (isSlow)
            {
                movement *= 0.5f;
            }
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
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

    private void HandleStatusSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentStatus++;
            if (currentStatus >= playerStatusses.Count)
            {
                currentStatus = 0;
            }

            UpdateValuesWithCurrentClass();
        }
    }

    private void UpdateValuesWithCurrentClass()
    {
        moveSpeed = playerStatusses[currentStatus].speed;
        moveSpeedInput.text = moveSpeed.ToString();

        dashSpeed = playerStatusses[currentStatus].dashSpeed;
        dashSpeedInput.text = dashSpeed.ToString();

        dashDuration = playerStatusses[currentStatus].dashDuration;
        dashDurationInput.text = dashDuration.ToString();

        dashCooldown = playerStatusses[currentStatus].dashCooldown;
        dashCooldownInput.text = dashCooldown.ToString();

        maxHealth = playerStatusses[currentStatus].maxHealth;
        animator.runtimeAnimatorController = playerStatusses[currentStatus].anim;
    }

    private void OnMoveSpeedChanged(string value)
    {
        moveSpeed = float.Parse(value);
    }

    private void OnDashSpeedChanged(string value)
    {
        dashSpeed = float.Parse(value);
    }

    private void OnDashDurationChanged(string value)
    {
        dashDuration = float.Parse(value);
    }

    private void OnDashCooldownChanged(string value)
    {
        dashCooldown = float.Parse(value);
    }

    private IEnumerator Dash()
    {
        dashingCooldown = dashCooldown;
        generalCooldown = dashDuration;
        audioSource.PlayOneShot(playerStatusses[currentStatus].dashAudio);

        Debug.Log("dashing: " + lookDir);
        isDashing = true;

        Vector2 dashDirection = movement; // Dash in the direction of the movement
        if (dashDirection == Vector2.zero)
        {
            dashDirection = lookDir; // Dash in the direction of the cursor
        }

        // Temporarily disable normal movement while dashing
        float originalMoveSpeed = moveSpeed;
        moveSpeed = 0f; // Stop normal movement during dash

        rb.linearVelocity = dashDirection.normalized * dashSpeed; // Use velocity for dash

        yield return new WaitForSeconds(dashDuration);

        // Reset velocity and movement speed after dash
        rb.linearVelocity = Vector2.zero;
        moveSpeed = originalMoveSpeed;

        isDashing = false;
        if (shouldFall)
        {
            Fall();
        }
    }

    private void HandleDirectional()
    {
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        directional.rotation = Quaternion.Euler(0, 0, angle);
        spawnerAxis.rotation = Quaternion.Euler(0, 0, angle);
    }

    private bool CanAct()
    {
        return (generalCooldown <= 0 && stunnedCooldown <= 0 && jumpCooldown <= 0 && !isDashing && !isDead/* && !isAttacking*/);
    }

    private bool CanWalk()
    {
        return (stunnedCooldown <= 0 && !isDead && !isDashing);
    }

    private IEnumerator PerformAttack1(Cast cast)
    {
        attack1Cooldown = cast.cooldown;
        generalCooldown = cast.generalCooldown;
        GameObject castObject = null;

        isAttacking = true;
        animator.SetBool("Attacking", true);
        animator.SetTrigger("Attack1");

        if (cast.attackRate == 0 && cast.length == 0)
        {
            castObject = Instantiate(cast.prefab, spawner.position, Quaternion.identity);

            switch (cast.position)
            {
                case Cast.Positions.Center:castObject.transform.position = transform.position;
                    break;
                case Cast.Positions.Spawner: castObject.transform.position = spawner.position;
                    break;
                case Cast.Positions.Melee: castObject.transform.position = meleeSpawner.position;
                    break;
                default:
                    break;
            }

            castObject.transform.rotation = directional.rotation;
        }
        else
        {
            // For continuous attacks over the duration specified by cast.lenght
            float elapsedTime = 0f;

            while (elapsedTime < cast.length)
            {
                // Instantiate a new attack prefab at the specified position and rotation
                castObject = Instantiate(cast.prefab, spawner.position, Quaternion.identity);

                if (cast.position == Cast.Positions.Center)
                {
                    castObject.transform.position = transform.position;
                }
                else if (cast.position == Cast.Positions.Spawner)
                {
                    castObject.transform.position = spawner.position;
                }

                castObject.transform.rotation = directional.rotation;

                // Wait for the next attack based on attackRate
                yield return new WaitForSeconds(cast.attackRate);

                elapsedTime += cast.attackRate;
            }
        }

        if (castObject != null)
        {
            Projectile proj = castObject.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.Initialize(this);
            }
        }

        yield return new WaitForSeconds(generalCooldown);

        isAttacking = false;
        if (!Input.GetMouseButton(0))
        {
            animator.SetBool("Attacking", false);
        }

        yield return null;
    }

    private IEnumerator PerformAttack2(Cast cast)
    {
        attack2Cooldown = cast.cooldown;
        generalCooldown = cast.generalCooldown;
        GameObject castObject = null;

        isAttacking = true;
        //animator.SetBool("Attacking", true);
        animator.SetTrigger("Attack2");

        if (cast.attackRate == 0 || cast.length == 0)
        {
            castObject = Instantiate(cast.prefab, spawner.position, Quaternion.identity);

            if (cast.position == Cast.Positions.Center)
            {
                castObject.transform.position = transform.position;
            }
            else if (cast.position == Cast.Positions.Spawner)
            {
                castObject.transform.position = spawner.position;
            }

            castObject.transform.rotation = directional.rotation;
        }
        else
        {
            // For continuous attacks over the duration specified by cast.lenght
            float elapsedTime = 0f;

            while (elapsedTime < cast.length)
            {
                // Instantiate a new attack prefab at the specified position and rotation
                castObject = Instantiate(cast.prefab, spawner.position, Quaternion.identity);

                if (cast.position == Cast.Positions.Center)
                {
                    castObject.transform.position = transform.position;
                }
                else if (cast.position == Cast.Positions.Spawner)
                {
                    castObject.transform.position = spawner.position;
                }

                castObject.transform.rotation = directional.rotation;

                // Wait for the next attack based on attackRate
                yield return new WaitForSeconds(cast.attackRate);

                elapsedTime += cast.attackRate;
            }
        }

        if (castObject != null)
        {
            Projectile proj = castObject.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.Initialize(this);
            }
        }

        yield return new WaitForSeconds(generalCooldown);
        isAttacking = false;
        //animator.SetBool("Attacking", false);

        yield return null;
    }

    private IEnumerator PerformShield(Cast cast)
    {
        shieldCooldown = cast.cooldown;
        generalCooldown = cast.generalCooldown;
        GameObject castObject = Instantiate(cast.prefab, shieldSpawner.position, Quaternion.identity, transform);

        //isAttacking = true;
        //animator.SetBool("Attacking", true);
        animator.SetTrigger("Shield");

        if (castObject != null)
        {
            Projectile proj = castObject.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.Initialize(this);
            }
        }

        yield return new WaitForSeconds(generalCooldown);
        //isAttacking = false;
        //animator.SetBool("Attacking", false);

        yield return null;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Abyss"))
        {
            if (!isDashing && !isDead && !isFalling)
            {
                isFalling = true;
                CancelInvoke("Fall");
                Invoke("Fall", playerStatusses[currentStatus].coyoteTime);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Abyss"))
        {
            if (!isDead)
            {
                isFalling = false;
                shouldFall = false;
                CancelInvoke("Fall");
            }
        }
    }

    private void Fall()
    {
        if (isDashing)
        {
            shouldFall = true;
            return;
        }


        isDead = true;
        animator.SetFloat("Speed", 0);
        directional.gameObject.SetActive(false);
        ResetDirectionWASDBools();
        UpdateDirectionWithWASD();
        ResetDirectionCursorBools();
        UpdateDirectionWithCursor();
        animator.ResetTrigger("Reshow");
        animator.SetTrigger("Fall");
        Invoke("Reposition", Constants.respawnTime);
        Invoke("Reshow", Constants.respawnTime);
        rb.linearVelocity = Vector2.zero;
        isFalling = false;
    }

    private void Reposition()
    {
        transform.position = respawnPosition;
    }

    private void Reshow()
    {
        isDead = false;
        animator.SetTrigger("Reshow");
        directional.gameObject.SetActive(true);
    }

    private void HandleCooldowns()
    {
        if (generalCooldown > 0)
        {
            generalCooldown -= Time.deltaTime;
        }

        if (stunnedCooldown > 0)
        {
            stunnedCooldown -= Time.deltaTime;
        }

        if (shieldCooldown > 0)
        {
            shieldCooldown -= Time.deltaTime;
        }

        if (attack1Cooldown > 0)
        {
            attack1Cooldown -= Time.deltaTime;
        }
        else if (isSlow)
        {
            isSlow = false;
        }

        if (attack2Cooldown > 0)
        {
            attack2Cooldown -= Time.deltaTime;
        }

        if (dashingCooldown > 0)
        {
            dashingCooldown -= Time.deltaTime;
        }

        if (jumpCooldown > 0)
        {
            jumpCooldown -= Time.deltaTime;
        }
    }

    private void SaveRespawnPosition()
    {
        respawnPosition = transform.position;
    }
}