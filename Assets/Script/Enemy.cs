using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float maxHealth = 3f;

    private float currentHealth;
    private ScoreManager scoreManager;

    private void Start()
    {
        currentHealth = maxHealth;
        // Optional: Find score manager to award points on death
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            // Award points for killing enemy
            if (scoreManager != null)
            {
                scoreManager.AddEnemyKillPoints();
            }
            
            // Destroy enemy
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            // Calculate knockback direction from enemy to player
            Vector2 knockbackDirection = (Vector2)player.transform.position - (Vector2)transform.position;
            knockbackDirection.Normalize();

            // Call player's TakeDamage method with the enemy's position
            player.TakeDamage(transform.position);
        }
    }
}