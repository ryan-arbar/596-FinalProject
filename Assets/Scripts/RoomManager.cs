using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class Door
{
    public GameObject doorObject;
    public DoorColor color;
}

[System.Serializable]
public class DoorColorGroup
{
    public DoorColor color;
    public List<GameObject> roomPrefabs; // List of prefabs for rooms of this color
}

public enum DoorColor
{
    Blue,
    Yellow,
    White_NoRoomSpawn,
}

public class RoomManager : MonoBehaviour
{
    [SerializeField] private List<DoorColorGroup> colorGroups;
    public GameObject currentRoom; 
    public Transform PlayerTransform;
    private GameObject previousRoom; // To store the previous room
    private int currentRoomIndex = 0;
    public AudioClip roomTransitionSound;

    public TMP_Text completionMessageText;
    private HashSet<GameObject> completedRooms = new HashSet<GameObject>();


    void Start()
    {
        if (completionMessageText != null)
        {
            completionMessageText.gameObject.SetActive(false);
        }

        // Initialize the current room at the start of the game
        if (!currentRoom)
        {
            currentRoom = GameObject.FindWithTag("InitialRoom");
            if (currentRoom == null)
            {
                Debug.LogError("Initial room not found. Make sure it's tagged 'InitialRoom'.");
            }
        }
    }

    /*void OnDrawGizmos()
    {
        // Visualize the current room in the editor with Gizmos
        if (currentRoom != null)
        {
            Gizmos.color = Color.green;
            Bounds bounds = CalculateRoomBounds(currentRoom);
            Gizmos.DrawWireCube(bounds.center, bounds.size);
            GUIStyle style = new GUIStyle() { normal = { textColor = Color.green } };
            UnityEditor.Handles.Label(bounds.center, "Current Room", style);
        }
    }*/


    Bounds CalculateRoomBounds(GameObject room)
    {
        // Calculate the bounds of the current room for visualization
        Bounds bounds = new Bounds(room.transform.position, Vector3.zero);
        foreach (Renderer renderer in room.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }

    public void DoorOpened(DoorInteraction openedDoor)
    {
        // Find all DoorInteraction components in the current room
        DoorInteraction[] doors = currentRoom.GetComponentsInChildren<DoorInteraction>();
        foreach (var door in doors)
        {
            if (door != openedDoor)
            {
                door.LockDoor(); // Immediately lock the door
            }
        }

    }

    public void UnlockAllDoors()
    {
        // Unlock all doors within the current room
        DoorInteraction[] doors = currentRoom.GetComponentsInChildren<DoorInteraction>(true);
        foreach (var door in doors)
        {
            door.UnlockDoor();
        }
        Debug.Log("All doors in the current room have been unlocked.");
    }

    public void LockAllOtherDoors(DoorInteraction unlockedDoor)
    {
        DoorInteraction[] doors = currentRoom.GetComponentsInChildren<DoorInteraction>();
        foreach (var door in doors)
        {
            if (door != unlockedDoor)
            {
                door.LockDoor();
            }
        }
    }


    public void SpawnNextRoom(DoorColor doorColor, Transform doorExitPoint)
    {
        GameObject nextRoomPrefab = DetermineNextRoomPrefab(doorColor);
        if (nextRoomPrefab == null)
        {
            Debug.LogError($"Next room prefab is not set for color: {doorColor}");
            return;
        }

        GameObject nextRoomInstance = Instantiate(nextRoomPrefab, Vector3.down * 50, Quaternion.identity);
        DisableRigidbodies(nextRoomInstance);
        nextRoomInstance.SetActive(true);
        StartCoroutine(AlignAndActivateRoom(nextRoomInstance, doorExitPoint));
    }

    private void CheckAllRoomsCompleted()
    {
        if (completedRooms.Count >= TotalRoomCount())
        {
            ShowCompletionMessage();
        }
    }

    private int TotalRoomCount()
    {
        int count = 0;
        foreach (var group in colorGroups)
        {
            count += group.roomPrefabs.Count;
        }
        return count;
    }

    private void ShowCompletionMessage()
    {
        if (completionMessageText != null)
        {
            completionMessageText.text = "Congratulations! All unique rooms completed!";
            completionMessageText.gameObject.SetActive(true);
        }
    }

    IEnumerator AlignAndActivateRoom(GameObject nextRoomInstance, Transform doorExitPoint)
    {
        yield return new WaitForEndOfFrame();

        Transform newRoomEntrance = FindDeepChild(nextRoomInstance.transform, "EntrancePoint");

        if (newRoomEntrance == null || doorExitPoint == null)
        {
            Debug.LogError("Entrance or specified exit points not found.", this);
            yield break;
        }

        Vector3 positionAdjustment = doorExitPoint.position - newRoomEntrance.position;
        nextRoomInstance.transform.position += new Vector3(positionAdjustment.x, 0, positionAdjustment.z);

        StartCoroutine(RaiseRoom(nextRoomInstance, positionAdjustment.y));
    }

    private void DisableRigidbodies(GameObject room)
    {
        Rigidbody[] rigidbodies = room.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true; // Make them kinematic to prevent them from falling
        }
    }

    private void EnableRigidbodies(GameObject room)
    {
        Rigidbody[] rigidbodies = room.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false; // Re-enable physics by making them non-kinematic
        }
    }



    IEnumerator RaiseRoom(GameObject room, float yOffset)
    {
        float duration = 2.0f; // Adjust duration for the effect
        Vector3 start = room.transform.position;
        Vector3 end = new Vector3(start.x, start.y + Mathf.Abs(yOffset), start.z);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float progress = elapsed / duration;
            float overshoot = 1.70158f; // Overshoot amount, can be adjusted
            progress = progress - 1;
            room.transform.position = start + (end - start) * ((progress * progress * ((overshoot + 1) * progress + overshoot) + 1));
            elapsed += Time.deltaTime;
            yield return null;
        }

        room.transform.position = end;
        // After the room is raised and positioned, update the currentRoom reference
        currentRoom = room;
        AudioManager.Instance.PlaySound2D(roomTransitionSound, 1f);
        EnableRigidbodies(currentRoom); // Enable rigidbodies
    }


    private GameObject DetermineNextRoomPrefab(DoorColor doorColor)
    {
        foreach (DoorColorGroup group in colorGroups)
        {
            if (group.color == doorColor && group.roomPrefabs.Count > 0)
            {
                // Ensure the index is within bounds
                if (currentRoomIndex >= group.roomPrefabs.Count)
                {
                    currentRoomIndex = 0; // Reset to start
                }

                GameObject selectedPrefab = group.roomPrefabs[currentRoomIndex];

                // Move to the next room for the next call
                currentRoomIndex++;

                return selectedPrefab;
            }
        }
        return null; // Return null if no matching prefab is found
    }


    Transform FindDeepChild(Transform parent, string name)
    {
        // Recursively search for a child transform by name
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform foundChild = FindDeepChild(child, name);
            if (foundChild != null) return foundChild;
        }
        return null;
    }

    public void UpdateCurrentRoom(GameObject newRoom)
    {
        if (currentRoom != null && currentRoom != newRoom)
        {
            Debug.Log("Updating current room. Previous room: " + currentRoom.name);
            MarkRoomAsCompleted(currentRoom); // Mark the previous room as completed
            DespawnPreviousRoom(currentRoom);
        }
        else
        {
            Debug.LogWarning("Attempted to update current room with the same or null room.");
        }
        currentRoom = newRoom;

        CheckAllRoomsCompleted();
    }



    public void DespawnPreviousRoom(GameObject previousRoom)
    {
        // Find the "Block" object in the previous room
        GameObject block = previousRoom.transform.Find("Path/To/Block").gameObject;

        if (block != null)
        {
            // Start rescale animation back to original scale
            StartCoroutine(RescaleBlock(block, Vector3.one, () => {
                // After rescaling, start sinking the room
                StartCoroutine(SinkAndDeactivateRoom(previousRoom));
            }));
        }
        else
        {
            // If no block is found, just sink and deactivate the room
            StartCoroutine(SinkAndDeactivateRoom(previousRoom));
        }
    }

    public void PrepareForNewRoom(GameObject doorObject)
    {
        previousRoom = doorObject.transform.parent.parent.gameObject;

        StartCoroutine(SinkAndDeactivateRoom(previousRoom));
    }

    public GameObject GetCurrentRoom()
    {
        return currentRoom;
    }

    private IEnumerator RescaleBlock(GameObject block, Vector3 targetScale, System.Action onComplete)
    {
        float duration = 1f; // Duration of the scale animation
        Vector3 initialScale = block.transform.localScale;
        float timer = 0;

        while (timer < duration)
        {
            block.transform.localScale = Vector3.Lerp(initialScale, targetScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        block.transform.localScale = targetScale; // Ensure target scale is set
        onComplete?.Invoke(); // Call the completion callback
    }


    IEnumerator SinkAndDeactivateRoom(GameObject room)
    {
        float duration = 2.0f;
        Vector3 start = room.transform.position;
        Vector3 end = start + Vector3.down * 50; // Adjust depth
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float progress = elapsed / duration;
            
            room.transform.position = Vector3.Lerp(start, end, progress * progress);
            elapsed += Time.deltaTime;
            yield return null;
        }

        room.SetActive(false); // Deactivate after animation completes
    }

    public void MarkRoomAsCompleted(GameObject room)
    {
        completedRooms.Add(room);
        CheckAllRoomsCompleted();
    }
}
