using UnityEngine;
using System.Collections;

public enum AttackType
{
    Projectile,
    Slash
}

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Type Selection")]
    public AttackType currentAttackType = AttackType.Projectile;

    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private float projectileAttackCooldown = 0.2f;
    [SerializeField] private int maxAmmo = 3;
    [SerializeField] private float reloadTimePerAmmo = 1f;

    [Header("Slash Attack Settings")]
    [SerializeField] private GameObject slashColliderPrefab;
    [SerializeField] private GameObject slashEffectPrefab;
    [SerializeField] private float slashEffectDuration = 0.5f;  // Duration before the effect is destroyed
    [SerializeField] private Vector2 slashAreaSize = new Vector2(1.5f, 2f);
    [SerializeField] private float slashAttackCooldown = 0.4f;
    [SerializeField] private float slashDamage = 1f;

    [Header("Attack Type Switching")]
    [SerializeField] private float switchCooldown = 2f; // Cooldown between attack type switches

    private int currentAmmo;
    private bool isReloading;
    private float lastAttackTime;
    private float lastSwitchTime;
    private Vector2 attackDirection;

    private void Start()
    {
        // Initialize ammo for projectile attack
        currentAmmo = maxAmmo;
    }

    private void Update()
    {
        // Determine attack direction based on player's facing direction
        attackDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // Switch attack type when 'C' key is pressed
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchAttackType();
        }

        // Perform attack when attack button is pressed
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
    }

    public bool CanAttack()
    {
        switch (currentAttackType)
        {
            case AttackType.Projectile:
                return !isReloading && currentAmmo > 0 && Time.time >= lastAttackTime + projectileAttackCooldown;
            case AttackType.Slash:
                return Time.time >= lastAttackTime + slashAttackCooldown;
            default:
                return false;
        }
    }

    public void Attack()
    {
        if (!CanAttack()) return;

        switch (currentAttackType)
        {
            case AttackType.Projectile:
                PerformProjectileAttack();
                break;
            case AttackType.Slash:
                PerformSlashAttack();
                break;
        }
    }

    public bool CanSwitchAttackType()
    {
        return Time.time >= lastSwitchTime + switchCooldown;
    }

    public void SwitchAttackType()
    {
        // Check if switching is allowed
        if (!CanSwitchAttackType()) return;

        // Toggle between Projectile and Slash
        currentAttackType = (currentAttackType == AttackType.Projectile) 
            ? AttackType.Slash 
            : AttackType.Projectile;

        // Reset relevant attack parameters
        lastSwitchTime = Time.time;
        currentAmmo = maxAmmo; // Reset ammo when switching
        isReloading = false;
    }

    private void PerformProjectileAttack()
    {
        // Create and setup projectile
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        projectileRb.velocity = attackDirection * projectileSpeed;
        
        currentAmmo--;
        lastAttackTime = Time.time;
        
        if (currentAmmo <= 0)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        
        while (currentAmmo < maxAmmo)
        {
            yield return new WaitForSeconds(reloadTimePerAmmo);
            currentAmmo++;
        }
        
        isReloading = false;
    }

    private void PerformSlashAttack()
    {
        // Calculate attack position and rotation
        Vector2 attackCenter = (Vector2)transform.position + attackDirection * (slashAreaSize.x / 2);
        float rotation = attackDirection.x > 0 ? 0f : 180f;
        Quaternion slashRotation = Quaternion.Euler(0, rotation, 0);

        // Spawn slash effect
        if (slashEffectPrefab != null)
        {
            GameObject slashEffect = Instantiate(slashEffectPrefab, attackCenter, slashRotation);
            Destroy(slashEffect, slashEffectDuration);
        }

        // Spawn slash collider
        if (slashColliderPrefab != null)
        {
            GameObject slashCollider = Instantiate(slashColliderPrefab, attackCenter, slashRotation);
            SlashCollider slashColliderComponent = slashCollider.GetComponent<SlashCollider>();
            
            if (slashColliderComponent != null)
            {
                slashColliderComponent.Initialize(slashDamage, slashAreaSize);
            }
            else
            {
                // If no SlashCollider component, use basic collider detection
                BoxCollider2D boxCollider = slashCollider.GetComponent<BoxCollider2D>();
                if (boxCollider != null)
                {
                    boxCollider.size = slashAreaSize;
                    StartCoroutine(ProcessSlashCollider(slashCollider));
                }
            }
        }
        
        lastAttackTime = Time.time;
    }

    private IEnumerator ProcessSlashCollider(GameObject slashCollider)
    {
        // Wait for a frame to allow collisions to be detected
        yield return new WaitForFixedUpdate();

        // Get all colliders in the slash area
        BoxCollider2D boxCollider = slashCollider.GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Collider2D[] hitColliders = Physics2D.OverlapBoxAll(slashCollider.transform.position, slashAreaSize, 0f);
            foreach (Collider2D hit in hitColliders)
            {
                if (hit.CompareTag("Enemy"))
                {
                    Enemy enemy = hit.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(slashDamage);
                    }
                }
            }
        }

        // Destroy the collider object
        Destroy(slashCollider);
    }

    // Getters for external access
    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;
    public bool IsReloading() => isReloading;
    public float GetSwitchCooldownRemaining() => Mathf.Max(0, switchCooldown - (Time.time - lastSwitchTime));

    private void OnDrawGizmos()
    {
        // Visualize slash area in editor
        if (currentAttackType == AttackType.Slash)
        {
            Gizmos.color = Color.red;
            Vector2 attackCenter = (Vector2)transform.position + 
                (transform.localScale.x > 0 ? Vector2.right : Vector2.left) * (slashAreaSize.x / 2);
            Gizmos.DrawWireCube(attackCenter, slashAreaSize);
        }
    }
}

// Helper class for slash collider if needed
public class SlashCollider : MonoBehaviour
{
    private float damage;
    private Vector2 size;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void Initialize(float damage, Vector2 size)
    {
        this.damage = damage;
        this.size = size;
        
        if (boxCollider != null)
        {
            boxCollider.size = size;
            boxCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}