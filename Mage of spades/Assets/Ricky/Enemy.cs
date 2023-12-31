using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

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
    public float attackwindUp = 0.1f;
    public CharacterController cc;
    public GameObject hurtBox;
    private Transform player;
    private bool isDormant = false;
    private float lastAttackTime;
    private bool attacking = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Hit");
        health -= damage; 
        if(health <= 0)
        {
            Destroy(this.gameObject);
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
            transform.LookAt(new Vector3(player.position.x, this.transform.position.y, player.position.z));
            if (Vector3.Distance(transform.position, player.position) >= attackRange && !attacking)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                Vector3 movement = new Vector3(direction.x, -0.5f, direction.z);
                cc.Move(movement * moveSpeed);
            }
            // Check if it's time to attack.
            if (Time.time - lastAttackTime >= attackCooldown && !attacking  )
            {
                // Calculate the time when the player will enter the attack range.
                if (Vector3.Distance(transform.position, player.position) > attackRange)
                {
                    Debug.Log("Not in range");
                    float timeToAttack = Vector3.Distance(transform.position, player.position) / moveSpeed;
                    if (timeToAttack <= attackwindUp)
                    {
                        Debug.Log("winding up!");
                        StartCoroutine(Attack());

                    }
                }
                else
                {
                    Debug.Log("In range");
                    StartCoroutine(Attack());

                }
                }
            }
    }

    // Function to make the enemy leave dormant mode.
    public void LeaveDormantMode()
    {
        isDormant = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlayerHurtBox")
        {
            TakeDamage(1);
        }
    }
    public void Blocked()
    {
        hurtBox.gameObject.SetActive(false);
        cc.Move(-Vector3.forward * 3);
        lastAttackTime = Time.time;
        attacking = false;
    }
    // Function to perform an attack.
    IEnumerator Attack()
    {
        attacking = true;
        yield return new WaitForSecondsRealtime(0.1f);
        // Perform the attack action here (e.g., damage the player).
        hurtBox.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(0.2f);
        Debug.Log("Attack");
        hurtBox.gameObject.SetActive(false);
        attacking = false;
        // You can implement this according to your game's mechanics.

        // Set the last attack time to the current time.
        lastAttackTime = Time.time;
    }
}
