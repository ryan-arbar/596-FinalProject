using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject completionMessage;

    void Awake()
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
    }

    //Dont use because broken
    public void ShowCompletionMessage()
    {
        if (completionMessage != null)
        {
            completionMessage.SetActive(true);
        }
    }
}
