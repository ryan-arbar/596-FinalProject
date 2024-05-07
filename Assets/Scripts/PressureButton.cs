using UnityEngine;
using System.Collections;

public class PressureButton : Interactable
{
    public enum ButtonType { Standard, Timed, Continuous, ContinuousDraggable }
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
        StartCoroutine(CaptureOriginalPositionAfterDelay(initialPositionCaptureDelay));
    }

    IEnumerator CaptureOriginalPositionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        originalPosition = transform.position; // Store the position after the delay
    }

    void Update()
    {
        if (isPressed && (buttonType == ButtonType.Continuous || buttonType == ButtonType.ContinuousDraggable))
        {
            if (!OtherObjectsPresent())
            {
                DeactivateButton();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPressed && other.attachedRigidbody && other.attachedRigidbody.mass >= activationMass)
        {
            if ((buttonType == ButtonType.Continuous && other.CompareTag("Carriable")) ||
                (buttonType == ButtonType.ContinuousDraggable && other.CompareTag("Draggable")))
            {
                ActivateButton();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // No further action needed here since Update() will handle the deactivation
    }

    void ActivateButton()
    {
        isPressed = true;
        ToggleState();
        transform.localPosition -= new Vector3(0, sinkDepth, 0);
        if (buttonType == ButtonType.Timed)
        {
            StartCoroutine(ResetButtonAfterDelay(timerDuration));
        }
    }

    void DeactivateButton()
    {
        ToggleState();
        ResetButtonPosition();
        isPressed = false;
    }

    bool OtherObjectsPresent()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity);
        foreach (Collider collider in colliders)
        {
            if ((collider.CompareTag("Carriable") || collider.CompareTag("Draggable")) && collider.enabled)
            {
                return true; // Found another qualifying object
            }
        }
        return false; // No qualifying objects found
    }

    public override void ToggleState()
    {
        base.ToggleState(); // Ensure isActive toggle and visual updates
        AudioManager.Instance.PlaySound2D(buttonActivationSound, 1f);
    }

    IEnumerator ResetButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (isActive && (buttonType == ButtonType.Continuous || buttonType == ButtonType.ContinuousDraggable))
        {
            DeactivateButton();
        }
    }

    void ResetButtonPosition()
    {
        transform.position = originalPosition; // Reset to the original position
    }
}
