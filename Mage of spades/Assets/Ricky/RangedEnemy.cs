using System.Collections;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    public Transform player;
    public GameObject arrowPrefab;
    public float detectionRange = 10f;
    public float attackCooldown = 2f;
    public float arrowSpeed = 20f;
    private float health = 1;
    private bool canAttack = true;
    private Rigidbody rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Check if the player is within detection range.
        if (Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            // Rotate to aim at the player.
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            // Check if it's time to attack.
            if (canAttack)
            {
                StartCoroutine(ShootArrow());
            }
        }
    }

    IEnumerator ShootArrow()
    {
        canAttack = false;
        yield return new WaitForSeconds(1.5f);

        // Calculate arrow travel time based on player distance and arrow speed.
        float playerDistance = Vector3.Distance(transform.position, player.position);
        float travelTime = playerDistance / arrowSpeed;

        // Calculate the predicted player position.
        Vector3 predictedPosition = player.position + player.GetComponent<Rigidbody>().velocity * travelTime;
        predictedPosition = predictedPosition + new Vector3(0, 2, 0);
        // Create and shoot the arrow.
        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        Vector3 arrowDirection = (predictedPosition - transform.position).normalized;
        arrow.GetComponent<Rigidbody>().velocity = arrowDirection * arrowSpeed;
        arrow.transform.LookAt(player.position);

        // Wait for the attack cooldown.
        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }
    public void TakeDamage(int damage)
    {
        Debug.Log("Hit");
        health -= damage;
        if (health <= 0)
        {
            Destroy(this.gameObject);
            rb.useGravity = true;
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerHurtBox")
        {
            TakeDamage(1);
        }
    }
}
