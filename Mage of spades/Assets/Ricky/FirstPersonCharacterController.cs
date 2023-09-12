using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class FirstPersonCharacterController : MonoBehaviour
{
    //standard movement stuff
    public float mouseSensitivity = 2.0f;
    public float walkSpeed = 5.0f;
    public float runSpeed = 10.0f;
    public float jumpForce = 5.0f;

    private CharacterController characterController;
    private bool isRunning = false;
    private Rigidbody rb;
    private Rigidbody crb;

    //Variables for Camera movements
    private Camera playerCamera;
    private float verticalRotation = 0;
    private float verticalVelocity = 0;
    public float bobFrequency = 2.0f;
    public float strafeTiltAngle = 10.0f;
    public float bobAmplitude = 0.1f;
    private float bobTimer = 0;
    private float strafeSpeed;

    //Variables for wallRunning
    private bool wallRunning, wallLeft, wallRight, canWallRun;
    private Coroutine wallRunRoutine;
    public float wallRunDuration = 5f;
    public float wallRunTransition = 0;
    private Quaternion wallRunRotation;
    private Quaternion preRunRotation;
    private Vector3 pushForce;
    private bool canAbortWallRun;

    //Variables for attacking
    public GameObject swordHitbox;
    public float swordSwingCooldown = 1.0f;
    public float swordDamage = 1.0f;
    private bool canSwingSword = true;
    private bool blocking;
    private bool canBlock = true;

    //Variables for death (Spoiler alert, its only a bool which tells the script to stop working when set to true, cause yknow, dead)
    private bool dead = false;


    void Start()
    {
        //Hide Cursor
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        //Set camera, Rigidbody and characterController
        playerCamera = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        crb = playerCamera.gameObject.GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        //All of this only happens when not dead (obviously)
        if (!dead)
        {
            // Mouse Look, move camera and body
            float horizontalRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
            verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            verticalRotation = Mathf.Clamp(verticalRotation, -90, 90);
            transform.Rotate(0, horizontalRotation, 0);
            playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

            // Movement, move character controller.
            float moveSpeed = isRunning ? runSpeed : walkSpeed;
            float forwardSpeed = Input.GetAxis("Vertical") * moveSpeed;
            Vector3 speed = Vector3.zero;

            // consumes the impact energy each cycle:

            if (characterController.isGrounded)
            {
                strafeSpeed = Input.GetAxis("Horizontal") * moveSpeed;
            }
            else
            {
                strafeSpeed = (Input.GetAxis("Horizontal") * moveSpeed) / 2;
            }

            if (!wallRunning)
            {
                speed = new Vector3(strafeSpeed, verticalVelocity, forwardSpeed);
                if(wallRunTransition < 1)
                {
                    gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, preRunRotation, wallRunTransition);
                    wallRunTransition += Time.deltaTime * 2;
                }
            }
            //WallRunning Code. Check if velocity is high enough, whether there are walls on either side. And if one is not true, abort the wallrun
            else if(canWallRun)
            {
                if (canAbortWallRun)
                {
                    if (rb.velocity.x < 5 && rb.velocity.z < 5 && rb.velocity.x > -5 && rb.velocity.z > -5)
                    {
                        AbortWallRun(false);
                    }
                    else if (!wallLeft && !wallRight)
                    {
                        AbortWallRun(false);
                    }
                }
                    //If there are walls, then attract to these walls.
                    if (wallLeft)
                {
                    speed = new Vector3(Physics.gravity.y * Time.deltaTime * 50, verticalVelocity, forwardSpeed);
                }
                else if (wallRight)
                {
                    speed = new Vector3(-Physics.gravity.y * Time.deltaTime * 50, verticalVelocity, forwardSpeed);
                }
                if (Input.GetButtonDown("Jump"))
                {
                    AbortWallRun(true);
                }
                gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, wallRunRotation, wallRunTransition);
                wallRunTransition += Time.deltaTime * 2;

            }
            //If there is a wall on either side, then check if the player can wallrun, is in the air, and not already wallrunning, then enable wallrunning.
            if (wallLeft || wallRight)
            {
                if (canWallRun && !characterController.isGrounded && !wallRunning)
                {
                    StartWallRun();
                }
            }
            //rest of the movement code.
            speed = transform.rotation * speed;
            if (pushForce.magnitude > 0.2)
            {
                speed += pushForce;
            }
            characterController.Move(speed * Time.deltaTime);


            //Attack code. Check if the player can swing their sword, and if they can and press M1. Then start the swordSwinging.
            if (canSwingSword)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    SwingSword();
                }

            }
            //if the player can block, and the player presses the M2 button, then start the block. If the button is unpressed, then stop the block.
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

            // Jumping. If on the ground and pressing spacebar, jump. And if not on the ground and not wallrunning, apply gravity.
            if (characterController.isGrounded)
            {
                canWallRun = true; 
                if (Input.GetButtonDown("Jump"))
                {
                    verticalVelocity = jumpForce;
                }
            }
            else if (!wallRunning)
            {
                verticalVelocity += Physics.gravity.y * Time.deltaTime;
            }

            // Running, with Lctrl, toggle between running and not running. Might get deleted.
            if (Input.GetKeyDown(KeyCode.LeftControl) && !isRunning)
            {
                isRunning = true;
            }
            else if (Input.GetKeyDown(KeyCode.LeftControl) && isRunning)
            {
                isRunning = false;
            }

            //Headbob code. If on the ground and moving it will move the camera up and down.
            if (characterController.isGrounded && Mathf.Abs(forwardSpeed) != 0 || characterController.isGrounded && Mathf.Abs(strafeSpeed) != 0 || wallRunning)
            {

                float bobY = Mathf.Cos(bobTimer * 2) * bobAmplitude * 4;

                Vector3 cameraPos = playerCamera.transform.localPosition;
                cameraPos.y = 0.76f + bobY; // Adjust the vertical position as needed
                playerCamera.transform.localPosition = cameraPos;

                // Update the bob timer based on the player's movement speed
                if (Mathf.Abs(forwardSpeed) > 0.1f || Mathf.Abs(strafeSpeed) > 0.1f)
                {
                    bobTimer += Time.deltaTime * bobFrequency;
                }
                float tiltAngle = 0;
                //When straving. The camera will tilt in the direction the player is straving. Giving extra visual flair.
                if (Mathf.Abs(strafeSpeed) > 0.1f)
                {
                    tiltAngle = strafeTiltAngle * Mathf.Sign(strafeSpeed);

                    playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, -tiltAngle);
                }


            }

            //If in the air, the bobbing stops, and the camera returns to its original position.
            else
            {
                // Reset camera position when jumping
                bobTimer = 0;
                Vector3 cameraPos = playerCamera.transform.localPosition;
                cameraPos.y = 0.76f;
                playerCamera.transform.localPosition = cameraPos;
            }
            pushForce = Vector3.Lerp(pushForce, Vector3.zero, 5 * Time.deltaTime);
        }
    }

    private void StartWallRun()
    {
        wallRunTransition = 0;
        Quaternion currenRotation = transform.localRotation;
        wallRunRotation = currenRotation * Quaternion.AngleAxis(wallLeft ? -15 : 15, Vector3.forward);
        //If falling, stop falling.
        if (verticalVelocity < 0)
        {
            verticalVelocity = 0;
        }
       wallRunRoutine = StartCoroutine(WallRunning());
    }

    private IEnumerator WallRunning()
    {
        //start wallrunning, rotate the player away from the wall for extra visual flair.
        wallRunning = true;
        Quaternion currenRotation = transform.localRotation;
        preRunRotation = Quaternion.Euler(0, 0, preRunRotation.z);
        preRunRotation = currenRotation * Quaternion.AngleAxis(0, Vector3.forward);
        canAbortWallRun = false;
        yield return new WaitForSecondsRealtime(0.2f);
        canAbortWallRun = true;
        yield return new WaitForSeconds(wallRunDuration);
        //Turn camera back, and disable wallrunning untill player comes in contact with the ground.
        wallRunTransition = 0;
        wallRunRotation = new Quaternion(0,0,0,0);
        wallRunning = false;
        canWallRun = false;
    }

    private void AbortWallRun(bool jump)
    {
        //If the player doesnt move fast enough, or the wall ends. Abort the coroutine, and stop the wallrun immediately.
        //Otherwise, if its a jump, do the same, but also propell the player up and away from the wall.
        StopCoroutine(wallRunRoutine);
        wallRunRotation = new Quaternion(0, 0, 0, 0);
        wallRunning = false;
        canWallRun = false;
        wallRunTransition = 0;

        if (jump)
        {
            Debug.Log("Jumping from wall");
            verticalVelocity = jumpForce;
            Push(wallLeft ? rb.transform.right : -  rb.transform.right, jumpForce * 25) ;
        }
        else
        {
            Debug.Log("Aborting wallrun");
        }
    }
    private void Push(Vector3 dir, float force)
    {
        Debug.Log("Push");
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        pushForce += dir.normalized * force / rb.mass;
    }

    public void SetWalls(bool wall, bool isLeft)
    {
        if(isLeft)
        {
            wallLeft = wall;
        }
        else
        {
            wallRight = wall;
        }
    }
    void SwingSword()
    {
        //Start the sword swing delay. after the delay, allow swinging of the sword again
        StartCoroutine(SwordCooldown(swordSwingCooldown));
        // Activate and then deactivate the sword hitbox after a short delay
        StartCoroutine(DeactivateSwordHitbox());
    }

    IEnumerator SwordCooldown(float cooldownTime)
    {
        canSwingSword = false;
        yield return new WaitForSeconds(cooldownTime);
        canSwingSword = true;
    }

    IEnumerator DeactivateSwordHitbox()
    {
        swordHitbox.SetActive(true);
        yield return new WaitForSeconds(0.2f); // Adjust as needed
        swordHitbox.SetActive(false);
    }

    private IEnumerator Blocking(bool enter, bool hit)
    {
        //If entering the block, blocking will be set to true. If exiting without being hit, Blocking will be set to false.
        //If exiting with being hit, then disable blocking for a moment and propell the player and the attacker backwards.
        if (enter)
        {
            blocking = true;
        }
        else if (!enter && !hit)
        {
            blocking = false;
        }
        else if (!enter && hit)
        {
            blocking = false;
            canBlock = false;
            StartCoroutine(SwordCooldown(2f));
            characterController.Move(-Vector3.forward * 3);
            yield return new WaitForSeconds(2);
            canBlock = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //If the enemy attacks and the player is not blocking, the player dies.
        if (other.gameObject.tag == "EnemyHurtBox" && !blocking)
        {
            Death();
        }
        //If however the player is blocking, the player will exit the block and be propelled backwards
        else if (blocking)
        {
            other.gameObject.transform.GetComponentInParent<Enemy>().Blocked();
            StartCoroutine(Blocking(false, true));
        }
    }
    public void Death()
    {
        //disable access to movement
        dead = true;

        //Make camera tumble to the ground, a nice visual indicator of death
        playerCamera.gameObject.transform.parent = null;
        crb.isKinematic = false;
        crb.AddTorque(new Vector3(-200, 100));
        Destroy(this.gameObject);
    }
}
