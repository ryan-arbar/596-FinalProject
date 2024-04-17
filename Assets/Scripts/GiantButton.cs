using UnityEngine;

public class GiantButton : MonoBehaviour
{
    public bool isActivated = false;
    public AudioClip buttonActivationSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            isActivated = true;
            Debug.Log("Button activated!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isActivated = false;
            Debug.Log("Button deactivated.");
        }
    }
}
