using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    // Will need to assign the materials manually 

    public bool isActive = false;
    public GameObject lineIndicator; // Assign in the Inspector
    public Material activeMaterial; // Assign a green material in the Inspector
    public Material inactiveMaterial; // Assign a red material in the Inspector

    void Awake()
    {
        // For Interactable or its derived classes
        activeMaterial = Resources.Load<Material>("Green");
        inactiveMaterial = Resources.Load<Material>("Gray");
    }

    public virtual void ToggleState()
    {
        isActive = !isActive;
        UpdateLineIndicator();
        UpdateConnectedDoorBlock();

        FindObjectOfType<PuzzleController>()?.CheckPuzzleState();
    }

    protected void UpdateLineIndicator()
    {
        if (lineIndicator != null)
        {
            foreach (Transform child in lineIndicator.transform)
            {
                Renderer childRenderer = child.GetComponent<Renderer>();
                if (childRenderer != null)
                {
                    // Use the assigned materials
                    childRenderer.material = isActive ? activeMaterial : inactiveMaterial;
                }
            }
        }
        else
        {
            Debug.LogError("lineIndicator not assigned", this);
        }
    }


    protected void UpdateConnectedDoorBlock()
    {
        DoorInteraction doorInteraction = GetComponentInParent<DoorInteraction>();
        if (doorInteraction != null && doorInteraction.doorBlock != null)
        {
            Renderer blockRenderer = doorInteraction.doorBlock.GetComponent<Renderer>();
            if (blockRenderer != null)
            {
                blockRenderer.material.color = isActive ? Color.green : Color.red; // Direct color change for simplicity
                Debug.Log("Door Block color changed.", this);
            }
            else
            {
                Debug.LogError("Renderer not found on doorBlock", this);
            }
        }
        else
        {
            //Debug.LogError("DoorInteraction or doorBlock not found", this);
        }
    }


}
