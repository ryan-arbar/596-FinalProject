using UnityEngine;

public class ObjectDragger : MonoBehaviour
{
    public float dragSpeed = 10f;
    private Transform draggedObject;
    private Vector3 offset;
    private Camera cam;
    public LayerMask draggableLayer;

    void Start()
    {
        cam = Camera.main; // Ensure this script is attached to a camera or has a reference to the main camera.
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Mouse button pressed
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, draggableLayer))
            {
                if (hit.collider != null && hit.collider.GetComponent<Rigidbody>() != null)
                {
                    draggedObject = hit.collider.transform;
                    // Calculate offset not in direct point but in projected plane
                    offset = draggedObject.position - cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.WorldToScreenPoint(draggedObject.position).z));
                }
            }
        }

        if (draggedObject != null && Input.GetMouseButton(0)) // Mouse button held down
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.WorldToScreenPoint(draggedObject.position).z);
            Vector3 targetPosition = cam.ScreenToWorldPoint(mousePosition) + offset;

            // Move directly without using velocity to avoid physical interactions causing issues
            draggedObject.position = Vector3.Lerp(draggedObject.position, targetPosition, Time.deltaTime * dragSpeed);
        }

        if (Input.GetMouseButtonUp(0)) // Mouse button released
        {
            draggedObject = null;
        }
    }
}
