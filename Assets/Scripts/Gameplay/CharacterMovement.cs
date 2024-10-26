using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private float directionRange = 0.3f;
    private float deadZone = 0.05f;
    private float maxDistance = 1f;
    private bool readingWASD;
    [SerializeField]private Rigidbody2D rb;
    [SerializeField]private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField]private Transform directional;
    [SerializeField] private Transform spawnerAxis;
    [SerializeField] private Transform spawner;
    [SerializeField] private Transform meleeSpawner;
    [SerializeField] private Transform shieldSpawner;
    [SerializeField] private Transform shadow;
    [SerializeField] private GameObject target;

    [SerializeField] private TMP_InputField moveSpeedInput;
    [SerializeField] private TMP_InputField dashSpeedInput;
    [SerializeField] private TMP_InputField dashDurationInput;
    [SerializeField] private TMP_InputField dashCooldownInput;


    [SerializeField] List<PlayerStatus> playerStatusses;
    [SerializeField] int currentStatus = 0;
    [SerializeField] bool localPlayer = false;

    private CharacterMovement targetedPlayer;

    private bool directionUpdated = false;
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

    private float castingCooldown = 0;
    private float generalCooldown = 0;
    private float stunnedCooldown = 0;
    private float attack1Cooldown = 0;
    private float attack2Cooldown = 0;
    private float attack3Cooldown = 0;
    private float shieldCooldown = 0;
    private float dashingCooldown = 0;

    private bool isAttacking = false;
    private bool isInPlace = false;

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

        // Get movement input (from controller directional and WASD)
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        aim.x = Input.GetAxis("Horizontal_Aim");
        aim.y = Input.GetAxis("Vertical_Aim");

        directionUpdated = false;

        // Detect Mouse vs Directional
        if (prevMousePosition != mousePosition || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            Cursor.visible = true;
        }
        prevMousePosition = mousePosition;

        // Calculate movement speed
        float currentSpeed = movement.magnitude;

        // Normalize the movement vector if the player is moving diagonally
        movement.Normalize();

        // Normalize the look direction for comparison
        lookDir.Normalize();

        // Reset all direction WASD booleans
        ResetDirectionWASDBools();                                          // IsLookingRight_K = false;

        // Reset all directions with cursor
        ResetDirectionCursorBools();                                        // IsLookingRight_C = false;

        // Reads the direction of the cursor
        ReadDirectionWithCursor(lookDir);                                   // if (lookDir.x > deadZone && Mathf.Abs(lookDir.y) < deadZone)  { IsLookingRight_C = true; }

        // Reads the direction from WASD
        ReadDirectionWithWASD();

        // Reads cast inputs
        HandleAttacks();

        // Handle movement and velocity
        HandleMovement(currentSpeed);

        // reads and Handles the dash
        HandleDash();

        // Reads the character switch
        HandleStatusSwitch();

        // Handles the angle of the directional
        HandleDirectional();

        // Handles the time of cooldowns
        HandleCooldowns();
    }

    private void HandleMovement(float currentSpeed)
    {
        if (currentSpeed != 0.0f && !isAttacking /* && attack1Cooldown <= 0*/ /*&& attack2Cooldown <= 0*/ /*&& dashingCooldown <= 0*/ /*&& generalCooldown <= 0*/ /*&& !isInPlace*/ && castingCooldown <= 0)
        {
            // Updates animator with cursor direction
            UpdateDirectionWithWASD();                                      // animator.SetBool("IsLookingUp", IsLookingUp_K);
            readingWASD = true;
        }
        else
        {
            // Updates animator with cursor direction
            UpdateDirectionWithCursor();                                    // animator.SetBool("IsLookingUp", IsLookingUp_C);
            readingWASD = false;
        }

        // If controller directional is being used
        if (aim.magnitude >= deadZone)
        {
            Cursor.visible = false;
        }

        // if mouse not being used
        if (!Cursor.visible)
        {
            // if directional is being used
            if (aim != Vector2.zero)
            {
                lookDir = aim; // overwrite direction with directional
                directionUpdated = true;
            }
            else if (movement != Vector2.zero)
            {
                lookDir = movement; // use movement direction
                directionUpdated = true;
            }
        }
        else // if mouse is being used
        {
            // Get mouse position relative to the player
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lookDir = mousePosition - rb.position;
            directionUpdated = true;
        }
    }

    private void HandleDash()
    {
        // Handle dash input (e.g., pressing Space)
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetButtonDown("AButton"))
        {
            if (dashingCooldown <= 0)
            {
                if (CanAct())
                {
                    StartCoroutine(Dash());
                }
            }
        }
    }

    private void HandleJump()
    {
        // Handle jump input (e.g., pressing ctrl)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("YButton"))
        {
            if (dashingCooldown <= 0)
            {
                if (CanAct())
                {
                    StartCoroutine(Dash());
                }
            }
        }
    }

    private void HandleAttacks()
    {
        if (!CanAct())
        {
            return;
        }

        // Handle attack inputs
        if ((Input.GetMouseButton(0) && MouseGameScreenTarget.Instance.mouseOver) || Input.GetAxis("LT") > 0.5f)
        {
            if (attack1Cooldown > 0) return;
            StartCoroutine(PerformAttack1(playerStatusses[currentStatus].attack1));
        }

        if (Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("BButton"))
        {
            if (attack2Cooldown > 0) return;
            StartCoroutine(PerformAttack2(playerStatusses[currentStatus].attack2));
        }

        if ((Input.GetMouseButton(1) && MouseGameScreenTarget.Instance.mouseOver) || Input.GetAxis("RT") > 0.5f)
        {
            if (attack3Cooldown > 0) return;
            StartCoroutine(PerformAttack3(playerStatusses[currentStatus].attack3));
        }

        if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("XButton"))
        {
            if (shieldCooldown > 0) return;
            StartCoroutine(PerformShield(playerStatusses[currentStatus].shield));
        }
    }

    private void ReadDirectionWithCursor(Vector2 lookDir)
    {
        // Determine which direction the player is facing - CURSOR
        if (lookDir.x > directionRange && Mathf.Abs(lookDir.y) < directionRange)
        {
            IsLookingRight_C = true;
        }
        else if (lookDir.x < -directionRange && Mathf.Abs(lookDir.y) < directionRange)
        {
            IsLookingLeft_C = true;
        }
        else if (lookDir.y > directionRange && Mathf.Abs(lookDir.x) < directionRange)
        {
            IsLookingUp_C = true;
        }
        else if (lookDir.y < -directionRange && Mathf.Abs(lookDir.x) < directionRange)
        {
            IsLookingDown_C = true;
        }
        else if (lookDir.x > directionRange && lookDir.y > directionRange)
        {
            IsLookingUpRight_C = true;
        }
        else if (lookDir.x < -directionRange && lookDir.y > directionRange)
        {
            IsLookingUpLeft_C = true;
        }
        else if (lookDir.x > directionRange && lookDir.y < -directionRange)
        {
            IsLookingDownRight_C = true;
        }
        else if (lookDir.x < -directionRange && lookDir.y < -directionRange)
        {
            IsLookingDownLeft_C = true;
        }
    }

    private void UpdateDirectionWithCursor()
    {
        animator.SetBool("IsLookingUp", IsLookingUp_C);
        animator.SetBool("IsLookingLeft", IsLookingLeft_C);
        animator.SetBool("IsLookingDown", IsLookingDown_C);
        animator.SetBool("IsLookingRight", IsLookingRight_C);
        animator.SetBool("IsLookingUpLeft", IsLookingUpLeft_C);
        animator.SetBool("IsLookingUpRight", IsLookingUpRight_C);
        animator.SetBool("IsLookingDownLeft", IsLookingDownLeft_C);
        animator.SetBool("IsLookingDownRight", IsLookingDownRight_C);
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
        if (movement.x > directionRange && Mathf.Abs(movement.y) < directionRange)
        {
            IsLookingRight_K = true;
            if (!Cursor.visible)
            {
                IsLookingRight_C = true;
            }
        }
        else if (movement.x < -directionRange && Mathf.Abs(movement.y) < directionRange)
        {
            IsLookingLeft_K = true;
            if (!Cursor.visible)
            {
                IsLookingLeft_C = true;
            }
        }
        else if (movement.y > directionRange && Mathf.Abs(movement.x) < directionRange)
        {
            IsLookingUp_K = true;
            if (!Cursor.visible)
            {
                IsLookingUp_C = true;
            }
        }
        else if (movement.y < -directionRange && Mathf.Abs(movement.x) < directionRange)
        {
            IsLookingDown_K = true;
            if (!Cursor.visible)
            {
                IsLookingDown_C = true;
            }
        }
        else if (movement.x > directionRange && movement.y > directionRange)
        {
            IsLookingUpRight_K = true;
            if (!Cursor.visible)
            {
                IsLookingUpRight_C = true;
            }
        }
        else if (movement.x < -directionRange && movement.y > directionRange)
        {
            IsLookingUpLeft_K = true;
            if (!Cursor.visible)
            {
                IsLookingUpLeft_C = true;
            }
        }
        else if (movement.x > directionRange && movement.y < -directionRange)
        {
            IsLookingDownRight_K = true;
            if (!Cursor.visible)
            {
                IsLookingDownRight_C = true;
            }
        }
        else if (movement.x < -directionRange && movement.y < -directionRange)
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
            if (isInPlace)
            {
                movement *= 0;
            }
            else
            if (isSlow)
            {
                movement *= 0.5f;
            }
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

            // Update the Animator with the current speed
            animator.SetFloat("Speed", movement.magnitude);
        }

        HandlePlayerClose();
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
        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetButtonDown("Switch"))
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

        AbilitiesManager.Instance.SetImageAbility1(playerStatusses[currentStatus].attack1.sprite);
        AbilitiesManager.Instance.SetImageAbility2(playerStatusses[currentStatus].attack2.sprite);
        AbilitiesManager.Instance.SetImageAbility3(playerStatusses[currentStatus].attack3.sprite);
        AbilitiesManager.Instance.SetImageAbility4(playerStatusses[currentStatus].shield.sprite);
        AbilitiesManager.Instance.SetImageAbility5(playerStatusses[currentStatus].dashIcon);
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

        float elapsedTime = 0f;

        // Keep applying velocity while the dash is happening
        while (elapsedTime < dashDuration)
        {
            rb.linearVelocity = dashDirection.normalized * dashSpeed; // Apply velocity every frame
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

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
        if (directionUpdated)
        {
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            directional.rotation = Quaternion.Euler(0, 0, angle);
            spawnerAxis.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private bool CanAct()
    {
        return (generalCooldown <= 0 && stunnedCooldown <= 0 && jumpCooldown <= 0 && !isDashing && !isDead && !isAttacking);
    }

    private bool CanWalk()
    {
        return (stunnedCooldown <= 0 && !isDead && !isDashing);
    }

    private IEnumerator PerformAttack1(Cast cast)
    {
        attack1Cooldown = cast.cooldown;
        generalCooldown = cast.generalCooldown;
        castingCooldown = cast.castingCooldown;
        GameObject castObject = null;

        isSlow = true;
        isAttacking = true;
        animator.SetBool("Attacking", true);
        animator.SetTrigger("Attack1");

        if (cast.attackRate == 0 && cast.length == 0) // if this is a one time cast
        {
            castObject = Instantiate(cast.prefab, spawner.position, Quaternion.identity);
            castObject.transform.rotation = directional.rotation;

            switch (cast.position)
            {
                case Cast.Positions.Center:castObject.transform.position = transform.position;
                    break;
                case Cast.Positions.Spawner: castObject.transform.position = spawner.position;
                    break;
                case Cast.Positions.Melee: 
                    castObject.transform.position = meleeSpawner.position;
                    isInPlace = true;
                    break;
                default:
                    break;
            }

        }
        else // if this cast has to repeat multiple times
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

        yield return new WaitForSeconds(cast.castingCooldown);
        isInPlace = false;

        if (cast.generalCooldown != cast.castingCooldown)
        {
            isAttacking = false;
            animator.SetBool("Attacking", false);
        }

        float elapsedTime2 = 0f;

        // Keep applying velocity while the dash is happening
        while (elapsedTime2 < cast.generalCooldown - cast.castingCooldown)
        {
            if (!Input.GetMouseButton(0) && !(Input.GetAxis("LT") > 0.5f))
            {
                isAttacking = false;
                animator.SetBool("Attacking", false);
            }
            elapsedTime2 += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        if (!Input.GetMouseButton(0) && !(Input.GetAxis("LT") > 0.5f))
        {
            isAttacking = false;
            animator.SetBool("Attacking", false);
        }
    }

    private IEnumerator PerformAttack2(Cast cast)
    {
        attack2Cooldown = cast.cooldown;
        generalCooldown = cast.generalCooldown;
        castingCooldown = cast.generalCooldown;
        GameObject castObject = null;

        //isAttacking = true;
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
    }

    private IEnumerator PerformAttack3(Cast cast)
    {
        attack3Cooldown = cast.cooldown;
        generalCooldown = cast.generalCooldown;
        castingCooldown = cast.generalCooldown;
        GameObject castObject = null;

        //isAttacking = true;
        //animator.SetBool("Attacking", true);
        animator.SetTrigger("Attack3");

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
    }

    private IEnumerator PerformShield(Cast cast)
    {
        shieldCooldown = cast.cooldown;
        generalCooldown = cast.generalCooldown;
        //castingCooldown = cast.castingCooldown;
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
        //else if (collision.gameObject.CompareTag("Player"))
        //{
        //    CharacterMovement cm = collision.gameObject.GetComponent<CharacterMovement>();
        //    if (!otherPlayers.Contains(cm))
        //    {
        //        otherPlayers.Add(cm);
        //        HandlePlayerClose();
        //    }
        //}
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
        //else if (collision.gameObject.CompareTag("Player"))
        //{
        //    CharacterMovement cm = collision.gameObject.GetComponent<CharacterMovement>();
        //    if (otherPlayers.Contains(cm))
        //    {
        //        otherPlayers.Remove(cm);
        //        HandlePlayerClose();
        //    }
        //}
    }

    private void HandlePlayerClose()
    {
        CharacterMovement closest = GetClosestPlayerToCursor();
        if (closest == null)
        {
            return;
        }

        if (targetedPlayer != null)
        {
            if (targetedPlayer != closest)
            {
                targetedPlayer.UntargetPlayer();
                targetedPlayer = closest;
                targetedPlayer.TargetPlayer();
            }
        }
        else
        {
            targetedPlayer = closest;
            targetedPlayer.TargetPlayer();
        }
    }


    private CharacterMovement GetClosestPlayerToCursor()
    {
        if (GameManager.Instance.allPlayers.Count <= 0)
        {
            Debug.Log("No other players to target");
            return null; // No other players to target
        }

        // Find the closest player to the cursor
        CharacterMovement closestPlayer = GameManager.Instance.allPlayers
            .Where(player => player != this) // Exclude "this" player
            .OrderBy(player => Vector2.Distance(player.transform.position, mousePosition))
            .FirstOrDefault(); // Get the closest player

        if (closestPlayer != null)
        {
            // Calculate the distance from the cursor to the closest player
            float distanceToCursor = Vector2.Distance(closestPlayer.transform.position, transform.position);

            // Return null if the distance exceeds maxDistance
            if (distanceToCursor > maxDistance)
            {
                if (closestPlayer == targetedPlayer)
                {
                    targetedPlayer.UntargetPlayer();
                    targetedPlayer = null;
                }
                return null;
            }
        }

        return closestPlayer;
    }

    private void Fall()
    {
        if (isDashing)
        {
            shouldFall = true;
            return;
        }


        isDead = true;
        directional.gameObject.SetActive(false);
        animator.SetFloat("Speed", 0);
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
            AbilitiesManager.Instance.HighlightAbility4();
            AbilitiesManager.Instance.SetCoolDownAbility4(shieldCooldown / playerStatusses[currentStatus].shield.cooldown, shieldCooldown);
        }
        else
        {
            AbilitiesManager.Instance.UnhighlightAbility4();
        }

        if (attack1Cooldown > 0)
        {
            attack1Cooldown -= Time.deltaTime;
            AbilitiesManager.Instance.HighlightAbility1();
            AbilitiesManager.Instance.SetCoolDownAbility1(attack1Cooldown / playerStatusses[currentStatus].attack1.cooldown, attack1Cooldown);
        }
        else {
            AbilitiesManager.Instance.UnhighlightAbility1();
            if (isSlow)
            { 
                isSlow = false;        
            }
        }

        if (attack2Cooldown > 0)
        {
            attack2Cooldown -= Time.deltaTime;
            AbilitiesManager.Instance.HighlightAbility2();
            AbilitiesManager.Instance.SetCoolDownAbility2(attack2Cooldown / playerStatusses[currentStatus].attack2.cooldown, attack2Cooldown);
        }
        else
        {
            AbilitiesManager.Instance.UnhighlightAbility2();
        }

        if (attack3Cooldown > 0)
        {
            attack3Cooldown -= Time.deltaTime;
            AbilitiesManager.Instance.HighlightAbility3();
            AbilitiesManager.Instance.SetCoolDownAbility3(attack3Cooldown / playerStatusses[currentStatus].attack3.cooldown, attack3Cooldown);
        }
        else
        {
            AbilitiesManager.Instance.UnhighlightAbility3();
        }

        if (dashingCooldown > 0)
        {
            dashingCooldown -= Time.deltaTime;
            AbilitiesManager.Instance.HighlightAbility5();
            AbilitiesManager.Instance.SetCoolDownAbility5(dashingCooldown / dashCooldown, dashingCooldown);
        }
        else
        {
            AbilitiesManager.Instance.UnhighlightAbility5();
        }

        if (jumpCooldown > 0)
        {
            jumpCooldown -= Time.deltaTime;
        }

        if (castingCooldown > 0)
        {
            castingCooldown -= Time.deltaTime;
        }
    }

    private void SaveRespawnPosition()
    {
        respawnPosition = transform.position;
    }

    private void TargetPlayer()
    {
        target.SetActive(true);
    }

    private void UntargetPlayer()
    {
        target.SetActive(false);
    }
}
