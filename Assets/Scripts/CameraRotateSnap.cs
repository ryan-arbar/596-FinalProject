using UnityEngine;

public class CameraRotateSnap : MonoBehaviour
{
    public bool canRotate = false; 
    public Transform cameraTransform; 
    private float rotationSpeed = 5f; // Speed at which the camera rotates
    private float initialYOffset = 45f; // Initial Y rotation offset, accounting for the default orientation
    private bool isRotating = false; // Flag to check if rotation is in progress
    private const float fixedXAngle = 30f; // Fixed X-axis angle

    private void Update()
    {
        if (canRotate && Input.GetMouseButtonDown(1)) // Right mouse button pressed
        {
            isRotating = true;
        }

        if (Input.GetMouseButtonUp(1)) // Right mouse button released
        {
            isRotating = false;
            SnapRotation();
        }

        if (isRotating)
        {
            float mouseX = Input.GetAxis("Mouse X");
            RotateCamera(mouseX);
        }
    }

    void RotateCamera(float mouseX)
    {
        // Apply rotation only around the Y-axis, keeping the X-axis fixed
        cameraTransform.Rotate(Vector3.up, mouseX * rotationSpeed, Space.World);
    }

    void SnapRotation()
    {
        float currentAngleY = NormalizeAngle(cameraTransform.eulerAngles.y - initialYOffset);
        float targetAngleY = Mathf.Round(currentAngleY / 90f) * 90f + initialYOffset;

        // Smoothly rotate the camera to the target angle, keeping the X-axis fixed at 30 degrees
        Vector3 targetEulerAngles = new Vector3(fixedXAngle, targetAngleY, 0f); // Ensure the X-axis remains at 30 degrees
        StartCoroutine(SmoothRotation(cameraTransform.eulerAngles, targetEulerAngles, 0.2f));
    }

    System.Collections.IEnumerator SmoothRotation(Vector3 fromAngle, Vector3 toAngle, float duration)
    {
        float timeElapsed = 0f;
        float startY = fromAngle.y;
        float endY = toAngle.y;

        // Calculate the shortest path for the rotation to snap
        float difference = Mathf.DeltaAngle(startY, endY);
        float targetY = startY + difference;

        while (timeElapsed < duration)
        {
            float newY = Mathf.LerpAngle(startY, targetY, timeElapsed / duration);
            cameraTransform.eulerAngles = new Vector3(fixedXAngle, newY, 0);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        cameraTransform.eulerAngles = new Vector3(fixedXAngle, targetY, 0);
    }


    float NormalizeAngle(float angle)
    {
        while (angle < 0f)
        {
            angle += 360f;
        }
        while (angle > 360f)
        {
            angle -= 360f;
        }
        return angle;
    }

    public void UnlockRotation()
    {
        canRotate = true; // Unlock the rotation ability
    }
}
