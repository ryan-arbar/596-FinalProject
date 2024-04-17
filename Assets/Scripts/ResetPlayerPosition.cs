using UnityEngine;

public class ResetPlayerPosition : MonoBehaviour
{
    public GameObject player; // Assign your player GameObject in the Inspector

    void Update()
    {
        // Check if the 'R' key was pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPlayerToTriggerPosition();
        }
    }

    void ResetPlayerToTriggerPosition()
    {
        // Find the MechanicUnlockTrigger GameObject in the current room
        GameObject unlockTrigger = GameObject.Find("MechanicUnlockTrigger");
        if (unlockTrigger != null && player != null)
        {
            // Set the player's position to the MechanicUnlockTrigger's position
            player.transform.position = unlockTrigger.transform.position;
        }
        else
        {
            Debug.LogError("MechanicUnlockTrigger or Player not found.");
        }
    }
}
