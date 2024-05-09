using System.Collections;
using UnityEngine;

public class FinalTile : MonoBehaviour
{
    public float loweringSpeed = 5f;
    public float lowerToY = -5f;
    public Animator[] animators;  // Array of animators to play animations
    public AudioClip gameOverSound;  // Sound effect to play at the end
    public AudioSource audioSource;  // Audio source component for playing sound
    public GameObject blackoutScreen;  // The object that will be toggled (e.g., a blackout or fade panel)

    private bool hasTriggered = false;  // Flag to check if the trigger has already been activated

    void Start()
    {
        // Attempt to find the blackout screen by tag and report status
        blackoutScreen = GameObject.FindGameObjectWithTag("BlackoutScreen");
        if (blackoutScreen == null)
        {
            Debug.LogError("Blackout screen not found. Make sure it's tagged 'BlackoutScreen'.");
        }
        else
        {
            Debug.Log("Blackout screen successfully found.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;  // Set the flag to true to prevent reactivation
            other.transform.SetParent(transform);  // Make the player a child of this tile
            StartLowering();
        }
    }

    void StartLowering()
    {
        StartCoroutine(LowerTile());
    }

    IEnumerator LowerTile()
    {
        Vector3 start = transform.position;
        Vector3 end = new Vector3(start.x, lowerToY, start.z);
        float duration = Mathf.Abs(start.y - lowerToY) / loweringSpeed;
        float elapsed = 0;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        ActivateEndGameEffects();
    }

    void ActivateEndGameEffects()
    {
        foreach (var animator in animators)
        {
            animator.enabled = true;  // Enable the animator
            animator.SetTrigger("PlayAnimation");  // Trigger the animation
        }

        if (audioSource && gameOverSound)
        {
            audioSource.PlayOneShot(gameOverSound);  // Play the sound effect
        }

        StartCoroutine(ActivateBlackoutScreenAfterDelay(6f));  // Activate the blackout screen after a delay
    }

    IEnumerator ActivateBlackoutScreenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // Wait for the specified delay
        if (blackoutScreen != null)
        {
            blackoutScreen.SetActive(true);  // Activate the blackout screen
            Debug.Log("Blackout screen has been activated.");
        }
        else
        {
            Debug.LogError("Failed to activate the blackout screen because it is not assigned.");
        }
    }
}
