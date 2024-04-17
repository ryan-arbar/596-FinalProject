using UnityEngine;
using System.Collections;

public class PressureButton : Interactable
{
    public enum ButtonType { Standard, Timed }
    public ButtonType buttonType = ButtonType.Standard; // Select the button type in inspector
    public float activationMass = 2f;
    private Vector3 originalPosition;
    public float sinkDepth = 0.8f;
    private bool isPressed = false;
    public float timerDuration = 5f; // Duration for timed buttons

    public AudioClip buttonActivationSound;

    // Delay for capturing the original position to account for initial setup
    public float initialPositionCaptureDelay = 2f;

    void Start()
    {
        // Delay capturing the original position
        StartCoroutine(CaptureOriginalPositionAfterDelay(initialPositionCaptureDelay));
    }

    IEnumerator CaptureOriginalPositionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        originalPosition = transform.position; // Store the position after the delay
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPressed && other.attachedRigidbody && other.attachedRigidbody.mass >= activationMass)
        {
            isPressed = true;
            ToggleState();
            transform.localPosition -= new Vector3(0, sinkDepth, 0);

            if (buttonType == ButtonType.Timed)
            {
                // Start a coroutine to reset the button after a delay
                StartCoroutine(ResetButtonAfterDelay(timerDuration));
            }
        }
    }

    public override void ToggleState()
    {
        base.ToggleState(); // Ensure isActive toggle and visual updates
        AudioManager.Instance.PlaySound2D(buttonActivationSound, 1f);
        if (buttonType == ButtonType.Standard && !isActive)
        {
            // If the button is a standard type and gets deactivated, reset its position
            ResetButtonPosition();
        }
    }

    IEnumerator ResetButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Reset the button state and position if it's still active
        if (isActive)
        {
            ToggleState(); // This will deactivate and update visuals
            ResetButtonPosition();
        }
    }

    void ResetButtonPosition()
    {
        transform.position = originalPosition; // Reset to the original position
        isPressed = false; // Allow reactivation
    }
}
