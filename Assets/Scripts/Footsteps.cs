using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public AudioClip[] footstepSounds; // Footstep sounds array
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayFootstepSound();
    }

    void PlayFootstepSound()
    {
        if (footstepSounds.Length > 0)
        {
            int index = Random.Range(0, footstepSounds.Length); // Get a random index
            AudioClip clip = footstepSounds[index]; // Select a random footstep sound
            audioSource.PlayOneShot(clip); // Play the selected footstep sound
        }
    }
}
