using UnityEngine;

public class UnlockMechanicTrigger : MonoBehaviour
{
    public GameObject instructionTextPrefab;
    public enum Ability { Run, Jump, RotateCamera }
    public Ability abilityToUnlock;

    private IsometricPlayerController playerController;
    private CameraRotateSnap cameraRotateSnap;

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<IsometricPlayerController>();
        cameraRotateSnap = FindObjectOfType<CameraRotateSnap>();

        if (instructionTextPrefab != null)
        {
            var instantiatedPrefab = Instantiate(instructionTextPrefab, transform.position, Quaternion.identity);
            instantiatedPrefab.SetActive(false);
            instructionTextPrefab = instantiatedPrefab;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (abilityToUnlock)
            {
                case Ability.Run:
                    playerController.EnableRunning();
                    Debug.Log("Running Unlocked");
                    break;
                case Ability.Jump:
                    playerController.EnableJumping();
                    Debug.Log("Jumping Unlocked");
                    break;
                case Ability.RotateCamera:
                    if (cameraRotateSnap != null)
                    {
                        cameraRotateSnap.UnlockRotation();
                        Debug.Log("Camera Rotation Unlocked");
                    }
                    else
                    {
                        Debug.LogError("CameraRotateSnap script not found in the scene.");
                    }
                    break;
            }

            if (instructionTextPrefab != null)
            {
                instructionTextPrefab.SetActive(true);
            }
        }
    }
}
