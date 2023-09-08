using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FirstPersonCharacterController : MonoBehaviour
{
    public float mouseSensitivity = 2.0f;
    public float walkSpeed = 5.0f;
    public float runSpeed = 10.0f;
    public float jumpForce = 5.0f;
    public float wallRunDuration = 1f;

    private Camera playerCamera;
    private CharacterController characterController;
    private float verticalRotation = 0;
    private float verticalVelocity = 0;
    public float bobFrequency = 2.0f;
    public float strafeTiltAngle = 10.0f;


    public float bobAmplitude = 0.1f;
    private bool wallRunning, wallLeft, wallRight, canWallRun;
    private Coroutine wallRunRoutine;
    public float wallRunThreshold = 15f;

    private float bobTimer = 0;
    private bool isRunning = false;
    private Rigidbody rb;
    public GameObject swordHitbox;
    public float swordSwingCooldown = 1.0f;
    public float heavyAttackCooldown = 2.0f;
    public float swordDamage = 1.0f;
    public float heavyAttackDamage = 20.0f;
    private bool dead = false;
    private bool canSwingSword = true;
    private bool blocking;
    private bool canBlock = true;

    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        playerCamera = GetComponentInChildren<Camera>();
        rb = playerCamera.gameObject.GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!dead)
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
            Vector3 speed = Vector3.zero;
            if (!wallRunning)
            {
                speed = new Vector3(strafeSpeed, verticalVelocity, forwardSpeed);

            }
            else
            {

                if(rb.velocity.x < 15 && rb.velocity.z < 15 && rb.velocity.x > -15 && rb.velocity.z > -15)
                {
                    AbortWallRun(false);
                }
                if (wallLeft)
                {
                    speed = new Vector3(strafeSpeed + Physics.gravity.y * Time.deltaTime, verticalVelocity, forwardSpeed);
                }
                else if (wallRight)
                {
                    speed = new Vector3(strafeSpeed - Physics.gravity.y * Time.deltaTime, verticalVelocity, forwardSpeed);
                }
            }
            if (canWallRun && !characterController.isGrounded && !wallRunning)
            {
                    StartWallRun();
            }

            speed = transform.rotation * speed;
            characterController.Move(speed * Time.deltaTime);

            if (canSwingSword)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    SwingSword();
                }
                if (canBlock)
                {
                    if (Input.GetMouseButton(1))
                    {
                        StartCoroutine(Blocking(true, false));
                    }
                    if (Input.GetMouseButtonUp(1))
                    {
                        StartCoroutine(Blocking(false, false));
                    }
                }
            }


            // Jumping
            if (characterController.isGrounded)
            {

                if (Input.GetButtonDown("Jump"))
                {
                    verticalVelocity = jumpForce;
                }
            }
            else if (!wallRunning)
            {
                verticalVelocity += Physics.gravity.y * Time.deltaTime;
            }

            // Running
            if (Input.GetKeyDown(KeyCode.LeftControl) && !isRunning)
            {
                isRunning = true;
            }
            else if (Input.GetKeyDown(KeyCode.LeftControl) && isRunning)
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
                float tiltAngle = 0;

                if (Mathf.Abs(strafeSpeed) > 0.1f)
                {
                    tiltAngle = strafeTiltAngle * Mathf.Sign(strafeSpeed);

                    playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, -tiltAngle);
                }


            }


            else
            {
                // Reset camera position when jumping
                bobTimer = 0;
                Vector3 cameraPos = playerCamera.transform.localPosition;
                cameraPos.y = 1.7f;
                playerCamera.transform.localPosition = cameraPos;
            }
        }

        // Camera Movements
        // UpdateCameraPosition();
    }

    private void StartWallRun()
    {
        if (verticalVelocity < 0)
        {
            verticalVelocity = 0;
        }
       wallRunRoutine = StartCoroutine(WallRunning());
    }

    private IEnumerator WallRunning()
    {
        wallRunning = true;
        transform.rotation = Quaternion.Slerp(transform.rotation, wallLeft ? new Quaternion(transform.rotation.x, transform.rotation.y, -15, transform.rotation.w) : new Quaternion(transform.rotation.x, transform.rotation.y, 15, transform.rotation.w), 1);
        yield return new WaitForSeconds(wallRunDuration);
        transform.rotation = Quaternion.Slerp(transform.rotation, new Quaternion(transform.rotation.x, transform.rotation.y, 0, transform.rotation.w), 1);

        wallRunning = false;
        canWallRun = false;
    }

    private void AbortWallRun(bool jump)
    {
        StopCoroutine(wallRunRoutine);
        transform.rotation = Quaternion.Slerp(transform.rotation, new Quaternion(transform.rotation.x, transform.rotation.y, 0, transform.rotation.w), 1);
        wallRunning = false;
        canWallRun = false;
         
    }
    void SwingSword()
    {
        canSwingSword = false;
        StartCoroutine(SwordCooldown(swordSwingCooldown));

        // Activate the sword hitbox (make sure it's a trigger collider)
        swordHitbox.SetActive(true);

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

    private IEnumerator Blocking(bool enter, bool hit)
    {
        if (enter)
        {
            blocking = true;
            Debug.Log("Blocking");
        }
        else if (!enter && !hit)
        {
            blocking = false;
        }
        else if (!enter && hit)
        {
            blocking = false;
            canBlock = false;
            StartCoroutine(SwordCooldown(swordSwingCooldown));
            characterController.Move(-Vector3.forward * 3);
            yield return new WaitForSeconds(2);
            canBlock = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "EnemyHurtBox" && !blocking)
        {
            Death();
        }
        else if (blocking)
        {
            other.gameObject.transform.GetComponentInParent<Enemy>().Blocked();
            StartCoroutine(Blocking(false, true));
        }
    }
    public void Death()
    {
        dead = true;

        playerCamera.gameObject.transform.parent = null;
        rb.isKinematic = false;
        rb.AddTorque(new Vector3(-200, 100));
        Destroy(this.gameObject);
    }
}
