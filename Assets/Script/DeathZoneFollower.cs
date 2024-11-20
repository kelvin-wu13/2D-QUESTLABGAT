using UnityEngine;

public class DeathZoneFollower : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform playerTransform;    // Reference to the player
    [SerializeField] private float offsetX = 0f;          // X offset from player
    [SerializeField] private float offsetY = -5f;         // Y offset from player (how far below the player)
    [SerializeField] private float followSpeed = 5f;      // How fast the death zone follows the player

    [Header("Size Settings")]
    [SerializeField] private float zoneWidth = 100f;      // Width of the death zone
    [SerializeField] private float zoneHeight = 2f;       // Height of the death zone

    [Header("Death Scene Settings")]
    [SerializeField] private string deathSceneName = "DeathScreen"; // Name of your death screen scene

    private BoxCollider2D deathZoneCollider;
    private Vector3 targetPosition;

    private void Start()
    {
        // Get or add BoxCollider2D
        deathZoneCollider = GetComponent<BoxCollider2D>();
        if (deathZoneCollider == null)
        {
            deathZoneCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        // Configure the collider
        deathZoneCollider.isTrigger = true;  // Make it a trigger
        deathZoneCollider.size = new Vector2(zoneWidth, zoneHeight);

        // Validate player reference
        if (playerTransform == null)
        {
            Debug.LogWarning("Player Transform not assigned to Death Zone! Attempting to find player...");
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("Could not find player! Please assign player manually or ensure player has 'Player' tag.");
                enabled = false;
                return;
            }
        }

        // Set initial position
        UpdateTargetPosition();
        transform.position = targetPosition;
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            // Update target position
            UpdateTargetPosition();

            // Smoothly move to target position
            transform.position = Vector3.Lerp(
                transform.position, 
                targetPosition, 
                followSpeed * Time.deltaTime
            );
        }
    }

    private void UpdateTargetPosition()
    {
        // Calculate target position based on player position and offsets
        targetPosition = new Vector3(
            playerTransform.position.x + offsetX,
            playerTransform.position.y + offsetY,
            transform.position.z
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            // Get the PlayerController component
            PlayerController playerController = other.GetComponent<PlayerController>();
            
            if (playerController != null)
            {
                // Disable player movement
                playerController.enabled = false;

                // Get the Rigidbody2D if it exists
                Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                    rb.isKinematic = true;
                }

                // Get the Animator if it exists
                Animator animator = other.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetBool("IsDead", true);
                }

                // Load death scene immediately
                UnityEngine.SceneManagement.SceneManager.LoadScene(deathSceneName);
            }
            else
            {
                // If no PlayerController found, just load death scene
                UnityEngine.SceneManagement.SceneManager.LoadScene(deathSceneName);
            }
        }
    }

    // Optional: Visualize the death zone in Scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f); // Semi-transparent red
        Vector3 gizmoCenter = transform.position;
        Vector3 gizmoSize = new Vector3(zoneWidth, zoneHeight, 0.1f);
        Gizmos.DrawCube(gizmoCenter, gizmoSize);
    }
}