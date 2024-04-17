using UnityEngine;

public class DragRigidbody : MonoBehaviour
{
    public float force = 600;
    public float damping = 6;
    public float distance = 100;

    public LineRenderer lr;
    public LineRenderer depthLr;

    private Camera mainCamera;
    private Transform jointTrans;
    private Rigidbody attachedRigidbody;
    private Vector3 localAttachPoint;
    private float dragDepth;

    // Define a layer mask for the raycast to ignore
    // This should be set in the inspector to ignore the layers of the dragged object and the drag line
    public LayerMask ignoreLayers;

    void Start()
    {
        mainCamera = Camera.main;

        // Find and assign the LineRenderers by searching for GameObjects by name
        GameObject dragLineObject = GameObject.Find("DragLine");
        if (dragLineObject != null)
        {
            lr = dragLineObject.GetComponent<LineRenderer>();
        }
        else
        {
            Debug.LogError("DragLine GameObject not found in the scene.");
        }

        GameObject depthLineObject = GameObject.Find("DepthLine");
        if (depthLineObject != null)
        {
            depthLr = depthLineObject.GetComponent<LineRenderer>();
        }
        else
        {
            Debug.LogError("DepthLine GameObject not found in the scene.");
        }
    }

    void FixedUpdate()
    {
        if (jointTrans != null && attachedRigidbody != null)
        {
            UpdateDepthLine(attachedRigidbody.transform.TransformPoint(localAttachPoint));
        }
    }


    void OnMouseDown()
    {
        BeginDrag(Input.mousePosition);
    }

    void OnMouseUp()
    {
        EndDrag();
    }

    void OnMouseDrag()
    {
        ContinueDrag(Input.mousePosition);
        AdjustHeightWithScroll();
    }

    private void BeginDrag(Vector3 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance))
        {
            if (hit.rigidbody != null)
            {
                attachedRigidbody = hit.rigidbody;
                localAttachPoint = attachedRigidbody.transform.InverseTransformPoint(hit.point);
                jointTrans = AttachJoint(hit.rigidbody, hit.point);
                dragDepth = hit.distance; // Correctly initialize dragDepth

                if (lr != null)
                {
                    lr.positionCount = 2;
                }
                if (depthLr != null)
                {
                    depthLr.positionCount = 2; // Initialize depth LineRenderer
                }
            }
        }
    }

    private void ContinueDrag(Vector3 screenPosition)
    {
        if (jointTrans == null || attachedRigidbody == null) return;

        Vector3 mousePosition = new Vector3(screenPosition.x, screenPosition.y, dragDepth);
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        jointTrans.position = worldPosition;

        if (lr != null)
        {
            lr.SetPosition(0, mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, dragDepth)));
            lr.SetPosition(1, attachedRigidbody.transform.TransformPoint(localAttachPoint));
        }

        // Update depth line
        if (depthLr != null)
        {
            //UpdateDepthLine(attachedRigidbody.transform.TransformPoint(localAttachPoint));
        }
    }

    private void UpdateDepthLine(Vector3 startPoint)
    {
        Ray downRay = new Ray(startPoint, Vector3.down);
        RaycastHit hitInfo;
        // Use the inverse of ignoreLayers to only hit objects that are NOT in the specified layers
        if (Physics.Raycast(downRay, out hitInfo, Mathf.Infinity, ~ignoreLayers))
        {
            // Set the depth line from the startPoint downwards to the hit point
            depthLr.SetPosition(0, startPoint);
            depthLr.SetPosition(1, hitInfo.point);
        }
    }

    private void AdjustHeightWithScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        dragDepth += scroll * 5f; // Adjust the 5f multiplier to control the sensitivity
    }

    private void EndDrag()
    {
        if (lr != null)
        {
            lr.positionCount = 0;
        }
        if (depthLr != null)
        {
            depthLr.positionCount = 0; // Clear the depth gauge line
        }
        if (jointTrans != null)
        {
            Destroy(jointTrans.gameObject);
        }
    }

    private Transform AttachJoint(Rigidbody rb, Vector3 attachmentPosition)
    {
        GameObject go = new GameObject("Attachment Point");
        go.transform.position = attachmentPosition;
        Rigidbody newRb = go.AddComponent<Rigidbody>();
        newRb.isKinematic = true;

        ConfigurableJoint joint = go.AddComponent<ConfigurableJoint>();
        joint.connectedBody = rb;
        joint.configuredInWorldSpace = true;
        joint.xDrive = NewJointDrive(force, damping);
        joint.yDrive = NewJointDrive(force, damping);
        joint.zDrive = NewJointDrive(force, damping);
        joint.rotationDriveMode = RotationDriveMode.Slerp;

        return go.transform;
    }

    private JointDrive NewJointDrive(float force, float damping)
    {
        JointDrive drive = new JointDrive
        {
            positionSpring = force,
            positionDamper = damping,
            maximumForce = Mathf.Infinity
        };
        return drive;
    }
}
