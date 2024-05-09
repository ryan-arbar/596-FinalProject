using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    public Interactable[] prerequisites;
    public DoorInteraction[] doorsToUnlock;

    void Awake()
    {
        // Automatically find all DoorInteraction components in the scene
        doorsToUnlock = FindObjectsOfType<DoorInteraction>();
    }

    void Update()
    {
        CheckPuzzleState();
    }

    public void CheckPuzzleState()
    {
        bool allActive = true;
        foreach (Interactable interactable in prerequisites)
        {
            if (!interactable.isActive)
            {
                allActive = false;
                break;
            }
        }

        if (allActive)
        {
            // If all prerequisites are active, unlock each door in the array
            foreach (DoorInteraction door in doorsToUnlock)
            {
                door.UnlockDoor();
            }
        }
        else
        {
            // If any prerequisite is not active, lock the doors
            foreach (DoorInteraction door in doorsToUnlock)
            {
                door.LockDoor();
            }
        }
    }
}
