using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCharacterController : MonoBehaviour
{
    public float mouseSensitivity = 2.0f;
    public float walkSpeed = 5.0f;
    public float runSpeed = 10.0f;
    public float jumpForce = 5.0f;

    private Camera playerCamera;
    private CharacterController characterController;
    private float verticalRotation = 0;
    private float verticalVelocity = 0;
    public float bobFrequency = 2.0f;
    public float strafeTiltAngle = 10.0f;
    public float bobAmplitude = 0.1f;

    private float bobTimer = 0;
    private bool isRunning = false;
    private Rigidbody rb;
    public GameObject swordHitbox;
    public float swordSwingCooldown = 1.0f;
    public float heavyAttackCooldown = 2.0f;
    public float swordDamage = 1.0f;
    public float heavyAttackDamage = 20.0f;

    private bool canSwingSword = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerCamera = GetComponentInChildren<Camera>();
        rb = playerCamera.gameObject.GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Mouse Look
        float horizontalRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -90, 90);
        transform.Rotate(0, horizontalRotation, 0);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        // Movement
        float moveSpeed = isRunning ? runSpeed : walkSpeed;
        float forwardSpeed = Input.GetAxis("Vertical") * moveSpeed;
        float strafeSpeed = Input.GetAxis("Horizontal") * moveSpeed;

        Vector3 speed = new Vector3(strafeSpeed, verticalVelocity, forwardSpeed);
        speed = transform.rotation * speed;
        characterController.Move(speed * Time.deltaTime);

        if (canSwingSword)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SwingSword();
            }

        }


        // Jumping
        if (characterController.isGrounded)
        {
            
            verticalVelocity = -0.5f;
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        // Running
        if (Input.GetKeyDown(KeyCode.LeftControl) && !isRunning)
        {
            isRunning = true;
        }
        else if(Input.GetKeyDown(KeyCode.LeftControl) && isRunning)
        {
            isRunning = false;
        }
        if (characterController.isGrounded && Mathf.Abs(forwardSpeed) != 0 || characterController.isGrounded && Mathf.Abs(strafeSpeed) != 0)
        {

            float bobY = Mathf.Cos(bobTimer * 2) * bobAmplitude * 4;

            Vector3 cameraPos = playerCamera.transform.localPosition;
            cameraPos.y = 1.7f + bobY; // Adjust the vertical position as needed
            playerCamera.transform.localPosition = cameraPos;

            // Update the bob timer based on the player's movement speed
            if (Mathf.Abs(forwardSpeed) > 0.1f || Mathf.Abs(strafeSpeed) > 0.1f)
            {
                bobTimer += Time.deltaTime * bobFrequency;
            }
            if (characterController.isGrounded)
            {
                float tiltAngle = 0;

                if (Mathf.Abs(strafeSpeed) > 0.1f)
                {
                    tiltAngle = strafeTiltAngle * Mathf.Sign(strafeSpeed);

                    playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, -tiltAngle);
                }

            }
            else
            {
                bobTimer = 0;
            }
        }


        else
        {
            // Reset camera position when jumping
            Vector3 cameraPos = playerCamera.transform.localPosition;
            cameraPos.y = 1.7f;
            playerCamera.transform.localPosition = cameraPos;
        }


        // Camera Movements
        // UpdateCameraPosition();
    }


    void SwingSword()
    {
        canSwingSword = false;
        StartCoroutine(SwordCooldown(swordSwingCooldown));

        // Activate the sword hitbox (make sure it's a trigger collider)
        swordHitbox.SetActive(true);

        // Send a message to the target to convey the attack type
        SendMessageToTarget((int)(swordDamage));

        // Deactivate the sword hitbox after a short delay
        StartCoroutine(DeactivateSwordHitbox());
    }

    IEnumerator SwordCooldown(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        canSwingSword = true;
    }

    IEnumerator DeactivateSwordHitbox()
    {
        yield return new WaitForSeconds(0.2f); // Adjust as needed
        swordHitbox.SetActive(false);
    }

    void SendMessageToTarget(int damage)
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 2.0f))
        {
            // Check if the object hit has a script that can take damage
            Enemy target = hit.collider.GetComponent<Enemy>();
            if (target != null)
            {
                target.TakeDamage(damage);
            } 
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "EnemyHurtBox")
        {
            Death();
        }
    }
    public void Death()
    {
        playerCamera.gameObject.transform.parent = null;
        rb.isKinematic = false;
    }
    /*  void UpdateCameraPosition()
      {
          if (isRunning)
          {
              // Adjust camera position while running
              Vector3 cameraPos = playerCamera.transform.localPosition;
              cameraPos.y = 1.5f;
              playerCamera.transform.localPosition = cameraPos;
          }
          else
          {
              // Reset camera position while walking
              Vector3 cameraPos = playerCamera.transform.localPosition;
              cameraPos.y = 1.7f;
              playerCamera.transform.localPosition = cameraPos;
          }
      } */
}
