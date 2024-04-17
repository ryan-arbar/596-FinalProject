using UnityEngine;

//REMOVED AT LAST MINUTE BECAUSE BROKEN

public class TriggerZoneScript : MonoBehaviour
{
    public GameObject completionMessage; // This will be assigned automatically

    private void Start()
    {
        // Automatically find and assign the completion message by tag
        completionMessage = GameObject.FindGameObjectWithTag("CompletionMessage");
        if (completionMessage == null)
        {
            Debug.LogWarning("Completion message GameObject not found. Make sure it's tagged 'CompletionMessage'.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.ShowCompletionMessage();
        }

    }

}
