using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Prefab to Spawn")]
    [Tooltip("Drag the Collectible PREFAB from your Project folder here")]
    public GameObject collectiblePrefab; 

    [Header("Possible Spawn Locations")]
    public Transform[] spawnPoints; 

    [Header("Collection Zone Link")]
    [Tooltip("Drag your Trigger_Zone object here so we can tell it what we spawned")]
    public CollectionZone targetCollectionZone;

    void Start()
    {
        if (spawnPoints.Length > 0 && collectiblePrefab != null)
        {
            // 1. Pick a random location
            int randomIndex = Random.Range(0, spawnPoints.Length);
            
            // 2. Instantiate (Spawn) the prefab at that location
            GameObject spawnedItem = Instantiate(
                collectiblePrefab, 
                spawnPoints[randomIndex].position, 
                spawnPoints[randomIndex].rotation
            );
            
            // 3. Tell the Collection Zone to look for THIS specific spawned clone
            if (targetCollectionZone != null)
            {
                targetCollectionZone.specificCollectible = spawnedItem;
            }
            
            Debug.Log("Instantiated new prefab at: " + spawnPoints[randomIndex].name);
        }
        else
        {
            Debug.LogWarning("Spawner is missing its Prefab or Spawn Points!");
        }
    }
}