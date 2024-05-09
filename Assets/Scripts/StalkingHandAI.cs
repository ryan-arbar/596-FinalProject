using UnityEngine;
using System.Collections;

public class StalkingHandAI : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 5f;
    public float rushDistance = 10f;
    //public float closeEnoughDistance = 3f;
    public float fieldOfView = 60f;
    public float rotationSpeed = 5f;
    public float zigzagFrequency = 0.5f;
    public float zigzagMagnitude = 2f;
    public float activationDelay = 2f;  // Delay before the script activates
    public float despawnDistance = 50f;  // Distance at which the object despawns
    public float fastApproachDistance = 20f;  // Distance for fast approach

    private bool isFrozen = false;
    private float zigzagPhase;
    private bool isActivated = false;  // Controls the activation of the script's logic

    void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object not found: Check that your player object is tagged correctly.");
        }

        zigzagPhase = Random.Range(0f, 2 * Mathf.PI);
        StartCoroutine(ActivateAfterDelay(activationDelay));
    }

    IEnumerator ActivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isActivated = true;
    }

    void Update()
    {
        if (!isActivated || player == null) return;

        if (Vector3.Distance(transform.position, player.position) > despawnDistance)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(player.forward, directionToPlayer);

        if (angleToPlayer < fieldOfView / 2 && !IsPlayerFacing(directionToPlayer))
        {
            isFrozen = false;
            MoveAndRotateTowardsPlayer(directionToPlayer);
        }
        else
        {
            Freeze();
        }
    }

    void OnDestroy()
    {
        GroundTile.DecrementHandCount();  // This will ensure that the count is decremented when the hand is destroyed
    }


    bool IsPlayerFacing(Vector3 directionToPlayer)
    {
        RaycastHit hit;
        if (Physics.Raycast(player.position, directionToPlayer, out hit, 100f))
        {
            return hit.transform == transform;
        }
        return false;
    }

    void MoveAndRotateTowardsPlayer(Vector3 directionToPlayer)
    {
        if (!isFrozen)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            Vector3 right = Quaternion.Euler(0, 90, 0) * directionToPlayer;
            float zigzagStep = Mathf.Sin(Time.time * zigzagFrequency + zigzagPhase) * zigzagMagnitude;
            Vector3 zigzagPosition = transform.position + (directionToPlayer + right * zigzagStep).normalized * CalculateSpeed() * Time.deltaTime;

            zigzagPosition.y = transform.position.y;
            transform.position = Vector3.MoveTowards(transform.position, zigzagPosition, CalculateSpeed() * Time.deltaTime);
        }
    }

    void Freeze()
    {
        isFrozen = true;
    }

    float CalculateSpeed()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > fastApproachDistance)
            return moveSpeed * 2; // Move faster when far from the player
        else if (distance <= rushDistance)
            return moveSpeed * 2; // Rush when close
        else
            return moveSpeed; // Normal movement speed otherwise
    }

    void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = Color.blue;
            Vector3 forward = player.forward * 10;
            Gizmos.DrawLine(player.position, player.position + forward);

            Vector3 fovLeft = Quaternion.Euler(0, fieldOfView / 2, 0) * player.forward * 10;
            Vector3 fovRight = Quaternion.Euler(0, -fieldOfView / 2, 0) * player.forward * 10;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(player.position, player.position + fovLeft);
            Gizmos.DrawLine(player.position, player.position + fovRight);
        }
    }
}
