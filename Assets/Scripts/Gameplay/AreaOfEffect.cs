using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfEffect : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr; // The SpriteRenderer to control transparency
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip initialAudio;
    [SerializeField] private AudioClip finalAudio;
    [SerializeField] private float startSpeed;
    [SerializeField] private float moveTime = 1f; // Duration for how long the object rb will be moving around with the start speed, after this duration, it will stop where it is and trigger the sequence
    [SerializeField] private float startTime = 0.5f; // Duration for scaling up and fading in
    [SerializeField] private float holdTime = 4f; // Duration to hold at full scale and alpha
    [SerializeField] private float endTime = 1f; // Duration for scaling down and fading out
    [SerializeField] private float startSize = 0.2f; // Starting Size
    [SerializeField] private float startFade = 0.2f; // Starting Opacity

    
    [SerializeField] public Constants.StatusEffects effect;
    
    private Vector3 originalScale; // To store the initial scale of the object
    private Color originalColor;   // To store the initial color of the sprite

    private void Awake()
    {
        originalScale = transform.localScale;
        originalColor = sr.color;

        // Start invisible: scale 0 and fully transparent
        transform.localScale = Vector3.zero;
        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
    }

    public void SetupSpeed(float _initialSpeed)
    {
        startSpeed = _initialSpeed;
    }

    private void Start()
    {
        // start rb with start speed
        rb.velocity = startSpeed * transform.right;
        transform.localScale = Vector2.one * startSize;
        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, startFade);
        if (moveTime != 0)
        {
            Invoke("StartSequence", moveTime);
        }
        else
        {
            StartSequence();
        }

        audioSource.PlayOneShot(initialAudio);
    }

    private void StartSequence()
    {
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        StartCoroutine(AOESequence());
    }

    private IEnumerator AOESequence()
    {
        // Phase 1: Scale up and fade in (run both coroutines in parallel)
        StartCoroutine(ScaleObject(transform.localScale, originalScale, startTime));
        StartCoroutine(FadeObject(0, 1, startTime));
        yield return new WaitForSeconds(startTime);

        // Phase 2: Hold at full scale and alpha for holdTime
        yield return new WaitForSeconds(holdTime);

        // Phase 3: Scale down and fade out
        StartCoroutine(ScaleObject(transform.localScale, Vector3.zero, endTime));
        StartCoroutine(FadeObject(1, 0, endTime));
        yield return new WaitForSeconds(endTime);

        // Optionally, destroy the object after the effect ends
        audioSource.PlayOneShot(finalAudio); // TODO wait for audio
        Destroy(gameObject);
    }

    private IEnumerator ScaleObject(Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
    }

    private IEnumerator FadeObject(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, t);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Apply specific effects based on the effect type
            if (effect == Constants.StatusEffects.Ice)
            {
                Debug.Log("Applying Ice effect to the player.");
                // Add slippery movement here
            }
            else if (effect == Constants.StatusEffects.Fire)
            {
                Debug.Log("Applying Fire effect to the player.");
                // Add damage-over-time effect here
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Remove the effect when the player leaves the area
            if (effect == Constants.StatusEffects.Ice)
            {
                Debug.Log("Removing Ice effect from the player.");
                // Remove slippery movement here
            }
            else if (effect == Constants.StatusEffects.Fire)
            {
                Debug.Log("Removing Fire effect from the player.");
                // Remove damage-over-time effect here
            }
        }
    }
}
