using UnityEngine;
using System.Collections;

public class AnimationTrigger : MonoBehaviour
{
    [System.Serializable]
    public struct MovableObject
    {
        public Animator animator;
        public float targetY;
        public float moveSpeed;
    }

    public MovableObject[] movableObjects;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (MovableObject movableObject in movableObjects)
            {
                if (movableObject.animator != null)
                {
                    movableObject.animator.enabled = true;  // Enable the animator
                    StartCoroutine(MoveObjectToY(movableObject.animator.transform, movableObject.targetY, movableObject.moveSpeed));
                }
            }
        }
    }

    // Just move the object downwards a certain amount over an amount of time
    IEnumerator MoveObjectToY(Transform obj, float targetY, float speed)
    {
        Vector3 startPosition = obj.position;
        Vector3 endPosition = new Vector3(startPosition.x, targetY, startPosition.z);
        float duration = Mathf.Abs(startPosition.y - targetY) / speed;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            obj.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        obj.position = endPosition;
    }

    /*void OnDrawGizmosSelected()
    {
        if (movableObjects != null)
        {
            foreach (var movableObject in movableObjects)
            {
                if (movableObject.animator != null)
                {
                    Vector3 startPosition = movableObject.animator.transform.position;
                    Vector3 endPosition = new Vector3(startPosition.x, movableObject.targetY, startPosition.z);

                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(endPosition, 0.1f);
                    Gizmos.DrawLine(startPosition, endPosition);
                }
            }
        }
    }*/
}
