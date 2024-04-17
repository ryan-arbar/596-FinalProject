using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public GameObject triggerZone;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        { // Press U to unlock the door
            UnlockDoor();
        }
    }

    public void UnlockDoor()
    {
        Debug.Log("Door Unlocked!");

        // Activate the trigger zone when the door is unlocked
        if (triggerZone != null)
        {
            triggerZone.SetActive(true);
        }
    }
}
