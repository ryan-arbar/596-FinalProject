using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0; // Force AudioSource to be 2D
        }
    }

    public void PlaySound2D(AudioClip clip, float volume = 1f)
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}
