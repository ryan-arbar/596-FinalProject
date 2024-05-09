using System.Collections;
using UnityEngine;

public class FinalTile : MonoBehaviour
{
    public float loweringSpeed = 5f;
    public float lowerToY = -5f;
    public Animator[] animators;
    public AudioClip gameOverSound;
    public AudioSource audioSource;
    public GameObject blackoutScreenPrefab;

    private bool hasTriggered = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            other.transform.SetParent(transform);
            StartLowering();
        }
    }

    void StartLowering()
    {
        StartCoroutine(LowerTile());
    }

    // The final tile will lower and then end game effects play
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

    // Play the animations and black screen for the end of game
    void ActivateEndGameEffects()
    {
        foreach (var animator in animators)
        {
            animator.enabled = true;
            animator.SetTrigger("PlayAnimation");
        }

        if (audioSource && gameOverSound)
            audioSource.PlayOneShot(gameOverSound);

        StartCoroutine(ActivateBlackoutScreenAfterDelay(6f));
    }

    IEnumerator ActivateBlackoutScreenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnBlackoutScreen();
    }

    void SpawnBlackoutScreen()
    {
        if (blackoutScreenPrefab && mainCamera != null)
        {
            // Create the blackout screen at a fixed distance in front of the camera
            GameObject blackoutScreen = Instantiate(blackoutScreenPrefab, mainCamera.transform.position + mainCamera.transform.forward * 1f, Quaternion.identity);
            blackoutScreen.transform.LookAt(mainCamera.transform.position);
            blackoutScreen.transform.Rotate(90f, 0f, 0f);  // Adjust rotation if needed

            // Set the scale of the blackout screen to 100 for all axes to cover whole screen for sure
            blackoutScreen.transform.localScale = new Vector3(100f, 100f, 100f);

            Debug.Log("Blackout screen has been activated and positioned with scale set to 100.");
        }
        else
        {
            Debug.LogError("Blackout screen prefab is not assigned or main camera is not available.");
        }
    }

}
