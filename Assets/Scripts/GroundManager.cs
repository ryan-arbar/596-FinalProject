using UnityEngine;
using System.Collections;
using Unity.AI.Navigation;
using System.Collections.Generic;

public class GroundTile : MonoBehaviour
{
    [System.Serializable]
    public struct PrefabProbability
    {
        public GameObject prefab;
        public float probability; // Probability this prefab is chosen
    }

    public static int currentHandCount;  // Static variable to keep track of the current number of hands
    public int maxHandCount = 5;  // Maximum number of hands allowed at one time

    public List<PrefabProbability> prefabOptions; // List of prefabs and their probabilities
    public float spawnOffset = 10f;
    public float riseHeight = -5f;
    public float riseTime = 1f;
    public float overshootAmount = 0.5f;
    public float overshootDuration = 0.1f;

    private bool spawned = false;

    void Awake()
    {
        if (currentHandCount == 0) // Ensure it's set to zero only initially
            currentHandCount = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !spawned)
        {
            SpawnAdjacentTiles();
            spawned = true;
        }
    }

    private void SpawnAdjacentTiles()
    {
        NavMeshSurface navMeshSurface = GetComponentInParent<NavMeshSurface>();
        bool spawnedHand = false;

        foreach (Vector3 direction in new Vector3[] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right })
        {
            Vector3 newPosition = transform.position + direction * spawnOffset;
            if (!TileExists(newPosition) && !spawnedHand)
            {
                GameObject selectedPrefab = ChoosePrefabBasedOnProbability();
                if (selectedPrefab.GetComponentInChildren<StalkingHandAI>() != null && currentHandCount >= maxHandCount)
                    continue; // Skip spawning new hands if the max count is reached

                GameObject newTile = Instantiate(selectedPrefab, newPosition + Vector3.up * riseHeight, Quaternion.identity);
                newTile.transform.SetParent(transform.parent);
                StartCoroutine(RiseToPosition(newTile, newPosition, navMeshSurface));

                if (newTile.GetComponentInChildren<StalkingHandAI>() != null)
                {
                    currentHandCount++; // Increment hand count
                    spawnedHand = true;
                }
            }
        }
    }

    public static void OnHandDestroyed() // Call this statically from StalkingHandAI
    {
        currentHandCount--;
    }

    void OnDestroy()
    {
        GroundTile.DecrementHandCount();  // This will ensure that the count is decremented when the hand is destroyed
    }

    public static void DecrementHandCount()
    {
        if (currentHandCount > 0)
            currentHandCount--;
    }


    GameObject ChoosePrefabBasedOnProbability()
    {
        float totalProbability = 0;
        foreach (var item in prefabOptions)
            totalProbability += item.probability;

        float randomPoint = Random.value * totalProbability;
        foreach (var item in prefabOptions)
        {
            if (randomPoint < item.probability)
                return item.prefab;
            randomPoint -= item.probability;
        }

        return prefabOptions[prefabOptions.Count - 1].prefab; // Return last item as default if no other is selected
    }

    IEnumerator RiseToPosition(GameObject tile, Vector3 targetPosition, NavMeshSurface navMeshSurface)
    {
        float elapsedTime = 0;
        Vector3 startingPosition = tile.transform.position;
        Vector3 overshootPosition = targetPosition + Vector3.up * overshootAmount;

        // Rise to overshoot
        while (elapsedTime < riseTime)
        {
            tile.transform.position = Vector3.Lerp(startingPosition, overshootPosition, elapsedTime / riseTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        tile.transform.position = overshootPosition;
        yield return new WaitForSeconds(overshootDuration);

        // Settle to final position
        elapsedTime = 0;
        while (elapsedTime < overshootDuration)
        {
            tile.transform.position = Vector3.Lerp(overshootPosition, targetPosition, elapsedTime / overshootDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        tile.transform.position = targetPosition;
        yield return new WaitForSeconds(1f);  // Additional delay to ensure everything is settled

        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();  // Update the NavMesh with the new tile
        }
    }

    bool TileExists(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapBox(position, new Vector3(5, 0.1f, 5), Quaternion.identity);
        foreach (Collider col in colliders)
        {
            if (col.gameObject != gameObject && col.CompareTag("GroundTile"))
            {
                return true;
            }
        }
        return false;
    }
}
