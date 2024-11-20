using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float invincibilityTime = 2f;

    [Header("Death Settings")]
    [SerializeField] private string deathSceneName = "DeathScreen"; // Name of your death screen scene
    [SerializeField] private float deathSceneDelay = 1f;           // Delay before loading death scene

    [Header("Damage Settings")]
    [SerializeField] private Color invincibilityColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private float blinkInterval = 0.2f;

    [Header("UI References")]
    [SerializeField] private TMP_Text healthText;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    // Animation parameter names
    private const string ANIM_SPEED = "Speed";
    private const string ANIM_IS_GROUNDED = "IsGrounded";
    private const string ANIM_IS_DEAD = "IsDead";

    private Rigidbody2D rb;
    private PlayerAttack playerAttack;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded;
    private int currentHealth;
    private bool isInvincible = false;
    private bool isDead = false;  // Added this line

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAttack = GetComponent<PlayerAttack>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize health
        currentHealth = maxHealth;
        UpdateHealthDisplay();

        // Ensure animator is set
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Don't allow any updates if player is dead
        if (isDead) return;

        // Ground Check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Horizontal Movement
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Update animation parameters
        animator.SetFloat(ANIM_SPEED, Mathf.Abs(moveInput));
        animator.SetBool(ANIM_IS_GROUNDED, isGrounded);

        // Flip character based on movement direction
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        // Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetTrigger("Jump");
        }

        // Attack Input
        if (Input.GetMouseButtonDown(0))
        {
            playerAttack.Attack();
        }

        // Attack Type Switch Input
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (playerAttack.CanSwitchAttackType())
            {
                playerAttack.SwitchAttackType();
            }
        }
    }

    public void TakeDamage(Vector2 enemyPosition)
    {
        // Check if player is not invincible and not dead
        if (!isInvincible && !isDead)
        {
            // Reduce health
            currentHealth--;
            UpdateHealthDisplay();

            // Trigger hit/damage animation
            animator.SetTrigger("Hit");

            // Check if player is dead
            if (currentHealth <= 0)
            {
                Die();
                return;
            }

            // Apply knockback
            ApplyKnockback(enemyPosition);

            // Start invincibility
            StartCoroutine(HandleInvincibility());
        }
    }

    private void Die()
    {
        if (isDead) return; // Prevent multiple deaths
        isDead = true;

        // Set death animation
        animator.SetBool(ANIM_IS_DEAD, true);

        // Disable player controls
        enabled = false;

        // Disable rigidbody physics
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // Start coroutine to load death scene
        StartCoroutine(LoadDeathScene());
    }

    private IEnumerator LoadDeathScene()
    {
        // Wait for death animation/effects
        yield return new WaitForSeconds(deathSceneDelay);

        // Load the death scene
        SceneManager.LoadScene(deathSceneName);
    }

    private void ApplyKnockback(Vector2 enemyPosition)
    {
        // Calculate knockback direction
        Vector2 knockbackDirection = (Vector2)transform.position - enemyPosition;
        knockbackDirection.Normalize();

        // Apply knockback force
        rb.velocity = knockbackDirection * knockbackForce;
    }

    private IEnumerator HandleInvincibility()
    {
        // Set invincibility flag
        isInvincible = true;

        // Store original color
        Color originalColor = spriteRenderer.color;

        // Blinking effect
        float elapsedTime = 0f;
        while (elapsedTime < invincibilityTime)
        {
            // Toggle sprite visibility
            spriteRenderer.color = isInvincible 
                ? new Color(1f, 1f, 1f, 0.5f)  // Semi-transparent
                : originalColor;
            
            yield return new WaitForSeconds(blinkInterval);
            
            elapsedTime += blinkInterval;
        }

        // Reset sprite color and invincibility
        spriteRenderer.color = originalColor;
        isInvincible = false;
    }

    // Getter for current health (optional, for UI)
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public void UpdateHealthDisplay()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth + "/" + maxHealth;
        }
    }
}