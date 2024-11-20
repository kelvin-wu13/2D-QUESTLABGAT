using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [Header("Platform Spawning Settings")]
    [SerializeField] private GameObject[] platformPrefabs;  // Array of platform prefabs to spawn
    [SerializeField] private float platformWidth = 3f;      // Width of platforms
    [SerializeField] private int initialPlatformCount = 5;  // Number of platforms to spawn initially

    [Header("Enemy Spawning Settings")]
    [SerializeField] private GameObject enemyPrefab;        // Enemy prefab to spawn
    [SerializeField] private float enemySpawnChance = 0.3f; // 30% chance to spawn enemy
    [SerializeField] private float enemyHeightOffset = 1f;  // Height above platform to spawn enemy

    [Header("Spawn Variations")]
    [SerializeField] private float gapChance = 0.3f;        // Chance of spawning a gap instead of a platform
    [SerializeField] private float minGapWidth = 1f;        // Minimum width of gaps
    [SerializeField] private float maxGapWidth = 3f;        // Maximum width of gaps

    [Header("Platform Management")]
    [SerializeField] private Transform playerTransform;     // Reference to player's transform
    [SerializeField] private float platformDeleteDistance = 15f; // Distance behind player to delete platforms

    private Vector3 lastPlatformPosition;

    private void Start()
    {
        // Reset last platform position to spawner's position
        lastPlatformPosition = transform.position;

        // Spawn initial set of platforms
        for (int i = 0; i < initialPlatformCount; i++)
        {
            SpawnPlatform();
        }
    }

    private void Update()
    {
        // Check if player is far enough to spawn new platform
        if (playerTransform.position.x - lastPlatformPosition.x > platformWidth)
        {
            SpawnPlatform();
        }

        // Remove platforms that are far behind the player
        RemoveOldPlatforms();
    }

    private void SpawnPlatform()
    {
        // Decide whether to spawn a platform or a gap
        if (Random.value > gapChance)
        {
            // Spawn a platform
            GameObject platformPrefab = platformPrefabs[Random.Range(0, platformPrefabs.Length)];

            // Spawn at the exact last platform position (or spawner's position initially)
            Vector3 spawnPosition = lastPlatformPosition;

            // Instantiate the platform
            GameObject newPlatform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
            
            // Try to spawn an enemy on this platform
            TrySpawnEnemy(newPlatform);
            
            // Update last platform position
            lastPlatformPosition = spawnPosition + new Vector3(platformWidth, 0, 0);
        }
        else
        {
            // Spawn a gap
            float gapWidth = Random.Range(minGapWidth, maxGapWidth);

            // Update last platform position to create a gap
            lastPlatformPosition += new Vector3(gapWidth, 0, 0);
        }
    }

    private void TrySpawnEnemy(GameObject platform)
    {
        // Check if we should spawn an enemy (30% chance) and if we have a valid platform and enemy prefab
        if (enemyPrefab != null && platform != null && Random.value < enemySpawnChance)
        {
            // Get the platform's collider to determine its bounds
            Collider2D platformCollider = platform.GetComponent<Collider2D>();
            if (platformCollider != null)
            {
                // Get the bounds of the platform
                Bounds platformBounds = platformCollider.bounds;
                
                // Calculate spawn position:
                // X: Random position within the platform bounds
                // Y: Top of the platform + offset
                Vector3 enemyPosition = new Vector3(
                    Random.Range(platformBounds.min.x, platformBounds.max.x),
                    platformBounds.max.y + enemyHeightOffset,
                    platform.transform.position.z
                );
                
                // Spawn the enemy
                Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
            }
            else
            {
                // Fallback if no collider: use platform's transform
                Vector3 enemyPosition = platform.transform.position + new Vector3(
                    platformWidth / 2f,  // Center of platform
                    platform.transform.localScale.y / 2f + enemyHeightOffset,  // Top of platform + offset
                    0
                );
                
                Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
            }
        }
    }

    private void RemoveOldPlatforms()
    {
        // Find all platforms in the scene
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");

        foreach (GameObject platform in platforms)
        {
            // Check if platform is too far behind the player
            if (playerTransform.position.x - platform.transform.position.x > platformDeleteDistance)
            {
                Destroy(platform);
            }
        }
    }

    // Optional: Visualize spawn areas in Scene view
    private void OnDrawGizmosSelected()
    {
        // Draw spawn position
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(platformWidth, 0.5f, 1));
    }
}