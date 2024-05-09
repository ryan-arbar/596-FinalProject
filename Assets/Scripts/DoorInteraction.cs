using UnityEngine;
using System.Collections;

public class DoorInteraction : MonoBehaviour
{
    //Right now a lot of stuff needs to be assigned in inspector to work, kinda annoying but it works

    public bool isLocked = true;
    private bool doorOpened = false; // Prevents re-opening once opened
    public Material unlockedMaterial; // Assign a green material in the Inspector to indicate unlocked state
    public Material lockedMaterial;
    private RoomManager roomManager;
    public GameObject doorBlock; // The door "Block" object to animate
    public DoorColor doorColor;
    private Transform playerTransform; // Now private, will be set automatically
    public float unlockDistance = 2f; // Distance within which the player can unlock/open the door
    public float animationDuration = 1f; // Duration of the block animation
    public Vector3 targetScale = Vector3.zero; // Target scale for the block animation

    public AudioClip doorOpeningSound;

    public Transform exitPoint; // Assign this in the Inspector to the specific exit point of this door
                                //This will probably need adjustments to fit the next room correctly


    void Start()
    {
        // Automatically find and assign the ExitPoint transform
        exitPoint = transform.parent.Find("ExitPoint");

        roomManager = FindObjectOfType<RoomManager>();
        doorBlock = transform.parent.Find("Block")?.gameObject; // Assumes "Block" is a sibling

        // Automatically assign the player transform using the "Player" tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found. Make sure your player is tagged correctly.");
        }

        if (roomManager == null)
        {
            Debug.LogError("RoomManager not found in the scene.", this);
        }
        if (doorBlock == null)
        {
            Debug.LogError("Door 'Block' not found.", this);
        }
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not assigned in DoorInteraction.", this);
        }
    }

    void Awake()
    {
        unlockedMaterial = Resources.Load<Material>("Green"); // Load the green material
        lockedMaterial = Resources.Load<Material>("Gray"); // Load the gray material
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && PlayerIsClose() && !doorOpened)
        {
            if (!isLocked)
            {
                Debug.Log("Opening door.", this);
                AudioManager.Instance.PlaySound2D(doorOpeningSound, 1f);
                StartCoroutine(AnimateBlock());
                roomManager.SpawnNextRoom(doorColor, exitPoint); // This triggers the next room's appearance
                doorOpened = true; // Ensure the door cannot be reopened
                roomManager.DoorOpened(this);

                // Lock all other doors in the room
                roomManager.LockAllOtherDoors(this);
            }
            else
            {
                Debug.Log("The door is locked.", this);
            }
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            roomManager.UnlockAllDoors();
        }
    }

    bool PlayerIsClose()
    {
        if (playerTransform == null)
        {
            return false;
        }
        return Vector3.Distance(transform.position, playerTransform.position) <= unlockDistance;
    }

    public void UnlockDoor()
    {
        if (!isLocked) return;

        isLocked = false;
        UpdateDoorBlockColor(unlockedMaterial);
        //Debug.Log("Door unlocked.", this);
    }


    public void LockDoor()
    {
        isLocked = true;
        UpdateDoorBlockColor(lockedMaterial); // Use the locked material
    }


    // Update the door "Block" object's color
    void UpdateDoorBlockColor(Material material)
    {
        if (doorBlock != null)
        {
            Renderer renderer = doorBlock.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = material;
            }
            else
            {
                Debug.LogError("Renderer not found on doorBlock", this);
            }
        }
        else
        {
            Debug.LogError("Door block object not assigned", this);
        }
    }


    IEnumerator AnimateBlock()
    {
        Vector3 originalScale = doorBlock.transform.localScale;
        Vector3 scaleDown = targetScale;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            doorBlock.transform.localScale = Vector3.Lerp(originalScale, scaleDown, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        doorBlock.transform.localScale = scaleDown; // Ensure it reaches the target scale
        doorBlock.SetActive(false); // Deactivate the block
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Get the current room from the RoomManager
            GameObject currentRoom = roomManager.GetCurrentRoom();

            if (currentRoom != null)
            {
                // Find walls within the current room context
                Transform wallFull3 = currentRoom.transform.Find("Wall3(Full)");
                Transform wallFull4 = currentRoom.transform.Find("Wall4(Full)");

                // Check if the walls were found and set them active
                if (wallFull3 != null && wallFull4 != null)
                {
                    wallFull3.gameObject.SetActive(true);
                    wallFull4.gameObject.SetActive(true);
                    Debug.Log("Wall3(Full) and Wall4(Full) have been activated in the current room.");
                }
                else
                {
                    Debug.LogError("One or both Wall3(Full) and Wall4(Full) objects not found in the current room.");
                }
            }
            else
            {
                Debug.LogError("Current room is null in RoomManager.");
            }

            roomManager.PrepareForNewRoom(gameObject);
        }
    }
}
