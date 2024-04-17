using System;
using System.Collections;
using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    private Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OpenDoor()
    {
        StartCoroutine(ScaleDoor(Vector3.zero, deactivate: true));
    }

    // Updated to include an Action callback parameter
    public void CloseDoor(Action onFinished = null)
    {
        gameObject.SetActive(true); // Reactivate the block before starting to scale up
        StartCoroutine(ScaleDoor(originalScale, onFinished: onFinished));
    }

    private IEnumerator ScaleDoor(Vector3 targetScale, bool deactivate = false, Action onFinished = null)
    {
        while (transform.localScale != targetScale)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, Time.deltaTime);
            yield return null;
        }

        if (deactivate && targetScale == Vector3.zero)
        {
            gameObject.SetActive(false);
        }

        onFinished?.Invoke(); // Invoke the callback when the door finishes scaling
    }
}
