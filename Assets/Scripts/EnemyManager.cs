using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    public IsometricPlayerController isoPlayerController;
    private bool canMove = true;
    GameObject player;
    NavMeshAgent agent;

    [SerializeField] LayerMask groundLayer, playerLayer;

    // Roaming
    Vector3 destPoint;
    bool walkpointSet;
    [SerializeField] float Walkrange;

    // States
    [SerializeField] float Viewrange;
    bool playerInView;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
    }

    void Chase()
    {
        agent.SetDestination(player.transform.position);

    }
    
    void Roam()
    {
        
        if (!walkpointSet) SearchForDest();
        if (walkpointSet) agent.SetDestination(destPoint);
        if (Vector3.Distance(transform.position, destPoint) < 5) walkpointSet = false;
    }

    void SearchForDest()
    {
        float z = Random.Range(-Walkrange, Walkrange);
        float x = Random.Range(-Walkrange, Walkrange);

        destPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        if (Physics.Raycast(destPoint, Vector3.down, groundLayer))
        {
            walkpointSet = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canMove = false;
            StartCoroutine(ResumeMovementAfterDelay());
        }
    }
    private IEnumerator ResumeMovementAfterDelay()
    {
        yield return new WaitForSeconds(0.6f);
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove)
        {
            return;
        }

        playerInView = Physics.CheckSphere(transform.position, Viewrange, playerLayer);
       

        if (!playerInView) Roam();
        if (playerInView) Chase();
        


        
    }
}
