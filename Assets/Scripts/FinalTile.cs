using System.Collections;
using UnityEngine;

public class FinalTile : MonoBehaviour
{
    public float loweringSpeed = 5f;
    public float lowerToY = -5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Make the player a child of this tile
            other.transform.SetParent(transform);

            // Start lowering the tile
            StartLowering();

            // If it's the final tile, you may want to call a specific function
            if (gameObject.CompareTag("FinalTile"))
            {
                EndGame();
            }
        }
    }

    void StartLowering()
    {
        StartCoroutine(LowerTile());
    }

    IEnumerator LowerTile()
    {
        Vector3 start = transform.position;
        Vector3 end = new Vector3(start.x, lowerToY, start.z);
        float duration = Mathf.Abs(start.y - lowerToY) / loweringSpeed;
        float elapsed = 0;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;

        /*// Optionally unparent the player when the lowering completes
        // This might be necessary if the tile despawns or you have other gameplay mechanics to consider.
        if (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                if (child.CompareTag("Player"))
                {
                    child.SetParent(null);
                }
            }
        }*/
    }

    void EndGame()
    {
        // Logic for ending the game or transitioning to another scene
        Debug.Log("Game Over! Tile has lowered.");
    }
}
