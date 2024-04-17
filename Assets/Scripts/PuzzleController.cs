using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    public Interactable[] prerequisites; // Assign in the inspector
    public DoorInteraction[] doorsToUnlock; // Will be automatically assigned

    void Awake()
    {
        // Automatically find all DoorInteraction components in the scene
        doorsToUnlock = FindObjectsOfType<DoorInteraction>();
    }

    public void CheckPuzzleState()
    {
        foreach (Interactable interactable in prerequisites)
        {
            if (!interactable.isActive)
            {
                // If any prerequisite is not active, exit early
                return;
            }
        }

        // If all prerequisites are active, unlock each door in the array
        foreach (DoorInteraction door in doorsToUnlock)
        {
            door.UnlockDoor();
        }
    }
}
