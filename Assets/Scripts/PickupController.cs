using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public KeyCode pickUpKey = KeyCode.E;
    public float pickUpRange = 2f;
    public float sphereCastRadius = 0.5f; // Radius of the sphere used for casting
    public float dropDistance = 1.5f; // Distance in front of the player to drop the cube
    public float verticalOffset = 0.5f; // Vertical offset to place the cube above the ground
    public Transform holdPoint;
    public GameObject greenIndicatorPrefab;
    public GameObject redIndicatorPrefab;
    private GameObject heldObject;
    private GameObject currentIndicator;
    private GameObject lastHoveredObject;

    private LayerMask carriableLayer; // Layer mask for carriable objects (set object to be carried with this layer)
    private LayerMask obstructionLayer; // Layer mask for obstructions (set objects that need to be indicated as "obstructions" to placement)

    private void Awake()
    {
        carriableLayer = LayerMask.GetMask("Carriable");
        obstructionLayer = LayerMask.GetMask("Default");
    }

    private void Update()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.6f;
        Vector3 rayDirection = transform.forward;

        Debug.DrawLine(rayOrigin, rayOrigin + rayDirection * pickUpRange, Color.blue);

        if (Input.GetKeyDown(pickUpKey))
        {
            if (heldObject == null)
            {
                TryPickUpObject(rayOrigin, rayDirection);
            }
            else
            {
                TryDropObject();
            }
        }

        if (heldObject == null)
        {
            CheckForObjectHighlight(rayOrigin, rayDirection);
        }

        if (heldObject != null)
        {
            UpdatePlacementIndicator();
            heldObject.transform.rotation = Quaternion.LookRotation(transform.forward); // Keep the cube facing forward
        }
    }

    private void CheckForObjectHighlight(Vector3 rayOrigin, Vector3 rayDirection)
    {
        RaycastHit hit;
        if (Physics.SphereCast(rayOrigin, sphereCastRadius, rayDirection, out hit, pickUpRange, carriableLayer))
        {
            if (hit.collider.CompareTag("Carriable"))
            {
                if (lastHoveredObject != hit.collider.gameObject)
                {
                    if (lastHoveredObject != null)
                    {
                        ToggleObjectEmission(lastHoveredObject, false);
                    }
                    lastHoveredObject = hit.collider.gameObject;
                    ToggleObjectEmission(lastHoveredObject, true);
                }
            }
        }
        else if (lastHoveredObject != null)
        {
            ToggleObjectEmission(lastHoveredObject, false);
            lastHoveredObject = null;
        }
    }

    private void TryPickUpObject(Vector3 rayOrigin, Vector3 rayDirection)
    {
        RaycastHit hit;
        if (Physics.SphereCast(rayOrigin, sphereCastRadius, rayDirection, out hit, pickUpRange, carriableLayer))
        {
            if (hit.collider.CompareTag("Carriable"))
            {
                Debug.Log($"Hit {hit.collider.name} at {hit.point}");
                ToggleObjectEmission(hit.collider.gameObject, true); // Turn on emission when in range
                PickUpObject(hit.collider.gameObject);
            }
        }
        else
        {
            Debug.Log("No carriable objects were hit by the SphereCast.");
        }
    }

    private void PickUpObject(GameObject pickUpObject)
    {
        if (lastHoveredObject)
        {
            ToggleObjectEmission(lastHoveredObject, false);
            lastHoveredObject = null;
        }

        heldObject = pickUpObject;
        heldObject.transform.position = holdPoint.position;
        heldObject.transform.parent = holdPoint;
        Rigidbody objectRb = heldObject.GetComponent<Rigidbody>();
        objectRb.isKinematic = true;

        if (currentIndicator)
        {
            Destroy(currentIndicator);
        }
    }

    private void TryDropObject()
    {
        Vector3 dropPosition = transform.position + transform.forward * dropDistance + Vector3.up * verticalOffset;
        if (IsPositionClear(dropPosition) && IsPathClear(dropPosition))
        {
            DropObject(dropPosition);
        }
    }

    private void DropObject(Vector3 position)
    {
        heldObject.transform.parent = null;
        heldObject.transform.position = position;
        Rigidbody objectRb = heldObject.GetComponent<Rigidbody>();
        objectRb.isKinematic = false;
        heldObject = null;

        if (currentIndicator)
        {
            Destroy(currentIndicator);
        }
    }

    private void UpdatePlacementIndicator()
    {
        Vector3 indicatorPosition = transform.position + transform.forward * dropDistance + Vector3.up * verticalOffset;
        bool clear = IsPositionClear(indicatorPosition) && IsPathClear(indicatorPosition);

        if (!currentIndicator || (clear && currentIndicator != greenIndicatorPrefab) || (!clear && currentIndicator != redIndicatorPrefab))
        {
            if (currentIndicator)
            {
                Destroy(currentIndicator);
            }

            GameObject indicatorPrefab = clear ? greenIndicatorPrefab : redIndicatorPrefab;
            currentIndicator = Instantiate(indicatorPrefab, indicatorPosition, Quaternion.LookRotation(transform.forward));
            currentIndicator.layer = LayerMask.NameToLayer("Indicator");
        }

        currentIndicator.transform.position = indicatorPosition;
    }

    private bool IsPositionClear(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 0.5f, obstructionLayer);
        foreach (Collider collider in colliders)
        {
            if (!collider.isTrigger)
                return false; // Found an obstruction
        }
        return true; // Clear if no colliders found
    }

    private bool IsPathClear(Vector3 indicatorPosition)
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 1.0f; // Slightly higher to avoid hitting the ground
        Vector3 direction = (indicatorPosition - rayOrigin).normalized;
        float distance = Vector3.Distance(rayOrigin, indicatorPosition);
        if (Physics.Raycast(rayOrigin, direction, out hit, distance, obstructionLayer))
        {
            if (!hit.collider.isTrigger)
            {
                Debug.DrawLine(rayOrigin, hit.point, Color.red); // Show red line if blocked
                return false; // Path is not clear
            }
        }

        Debug.DrawLine(rayOrigin, indicatorPosition, Color.green); // Show green line if clear
        return true; // Path is clear
    }

    private void ToggleObjectEmission(GameObject obj, bool state)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = renderer.material;
            if (mat != null)
            {
                if (state)
                {
                    mat.EnableKeyword("_EMISSION");
                }
                else
                {
                    mat.DisableKeyword("_EMISSION");
                }
            }
        }
    }
}
