using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int health;
    private Rigidbody rb;
    public float detectionRange = 10f;
    public float moveSpeed = 3f;
    public float attackCooldown = 2f;
    public float attackRange = 2f;
    public float damage = 10f;

    private Transform player;
    private bool isDormant = false;
    private float lastAttackTime;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Hit");
        health -= damage; 
        if(health <= 0)
        {
            rb.useGravity = true;
        }

    }
    void Update()
    {
        if (isDormant)
        {
            // Do nothing in dormant mode.
            return;
        }

        // Check if the player is within detection range.
        if (Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            // Move towards the player.
            Vector3 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);

            // Check if it's time to attack.
            if (Time.time - lastAttackTime >= attackCooldown &&
                Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                Attack();
            }
        }
    }

    // Function to make the enemy leave dormant mode.
    public void LeaveDormantMode()
    {
        isDormant = false;
    }

    // Function to perform an attack.
    void Attack()
    {
        // Perform the attack action here (e.g., damage the player).
        // You can implement this according to your game's mechanics.

        // Set the last attack time to the current time.
        lastAttackTime = Time.time;
    }
}
