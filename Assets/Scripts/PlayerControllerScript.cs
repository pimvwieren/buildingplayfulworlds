using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour
{
    public CharacterController playerController;
    public LayerMask groundMask;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float fallMultiplier = 10f;
    public float lowJumpMultiplier = 5f;
    public float walkSpeed = 12f;
    public float runSpeed = 24f;
    Vector3 moveVector;
    Vector3 moveVectorFreeze;
    Vector3 lookVector;
    Vector3 reflectLookVector;
    Vector3 wallNormal;
    RaycastHit groundInfo;
    Vector3 platformPos;
    Vector3 oldPlatformPos;
    Vector3 platformVector;
    public Vector3 checkpointPosition;
    public int restartGravityDirection = 1;
    public int restartGame;


    public int gravityDirection = 1;

    Collider playerCollider;
    Vector3 velocity;
    float speed;
    float switchedVelocity; //Current y velocity times gravityDirection
    float distToGround; //Half of the y bound size
    bool hasTurned = false; //if the player has jumped
    bool jumpReady = false; //if the player is ready to jump when landed
    bool canWallJump = true;
    bool ignoreMove = true;

    void Start()
    {
        playerCollider = GetComponentInChildren<Collider>();
        distToGround = playerCollider.bounds.extents.y;
    }

    bool IsGrounded()
    {
        if (gravityDirection == 1)
        {
            return Physics.Raycast(transform.position, -Vector3.up, out groundInfo, distToGround + 0.1f, groundMask);
        }
        else
        {
            return Physics.Raycast(transform.position, Vector3.up, out groundInfo, distToGround + 0.1f, groundMask);
        }
    }



    bool JumpCheck()
    {
        if (gravityDirection == 1)
        {
            return Physics.Raycast(transform.position, -Vector3.up, distToGround + 1f, groundMask);
        }
        else
        {
            return Physics.Raycast(transform.position, Vector3.up, distToGround + 1f, groundMask);
        }
    }

    IEnumerator WallTurning()
    {
        for (float ft = 5f; ft >= 0; ft -= 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(reflectLookVector), 720f * Time.deltaTime);
            yield return null;
        }
    }
    public IEnumerator RestartFromCheckpoint()
    {
        restartGame = 0;
        playerController.enabled = false;
        transform.position = checkpointPosition;
        playerController.enabled = true;
        hasTurned = false; //if the player has jumped
        jumpReady = false; //if the player is ready to jump when landed
        canWallJump = true;
        ignoreMove = true;
        gravityDirection = restartGravityDirection;
        gravity = Mathf.Abs(gravity) * -restartGravityDirection;
        
        
        
        velocity = new Vector3(0, 0, 0);
        
        yield return null;
    }

    //Test

    // this script pushes all rigidbodies that the character touches
    float pushPower = 1f;
    void OnControllerColliderHit(ControllerColliderHit hit)
    {

        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
        {
            return;
        }

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3)
        {
            return;
        }

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        body.velocity = pushDir * (pushPower / body.mass);
    }

    void Update()
    {

        //Update input

        float xAxis = Input.GetAxis("Horizontal");
        float zAxis = Input.GetAxis("Vertical");

        //Run

        if (Input.GetButton("Run"))
        {
            speed = runSpeed;
        }
        else
        {
            speed = walkSpeed;
        }

        //Move vector

        moveVector = transform.right * (xAxis * gravityDirection) + transform.forward * zAxis;

        //Standard gravity

        velocity.y += gravity * Time.deltaTime;

        //Reset velocity and platform movement

        if (IsGrounded() && groundInfo.transform.gameObject.tag == "Platform")
        {
            platformPos = groundInfo.transform.position;
            platformVector = (platformPos - oldPlatformPos);
            if (ignoreMove == false)
            {
                playerController.Move(platformVector);
            }
            ignoreMove = false;
            oldPlatformPos = platformPos;
        }

        if (IsGrounded())
        {
            velocity.y = 0f;
            hasTurned = false;
            canWallJump = true;
            StopCoroutine(WallTurning());
        }
        else
        {
            ignoreMove = true;
        }

        //

        switchedVelocity = velocity.y * gravityDirection;

        //Faster falling

        if (switchedVelocity < 0)
        {
            velocity += Vector3.up * gravity * (fallMultiplier - 1) * Time.deltaTime;
        }

        //Not pressing jump multiplier

        if (switchedVelocity > 0 && !Input.GetButton("Jump"))
        {
            velocity += Vector3.up * gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        //Double jump gravity shift

        if (switchedVelocity >= 0 && Input.GetButtonDown("Switch") && hasTurned == false)
        {
            gravity = -gravity;
            gravityDirection = -gravityDirection;
            hasTurned = true;
        }

        //Add jump velocity

        if (Input.GetButtonDown("Jump") && JumpCheck() && switchedVelocity < 0)
        {
            jumpReady = true;
        }

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            jumpReady = true;
        }

        if (jumpReady == true && IsGrounded())
        {
            jumpReady = false;
            velocity.y = Mathf.Sqrt(jumpHeight * (-2f * gravityDirection) * gravity) * gravityDirection;
        }

        //Wall jump

        if (Input.GetButtonDown("Jump") && !IsGrounded() && moveVector.z != 0 && canWallJump)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 2f))
            {

                lookVector = Vector3.Scale(new Vector3(1, 0, 1), ray.direction);

                wallNormal = hit.normal;

                if (wallNormal.y == 0)
                {
                    velocity.y = Mathf.Sqrt(jumpHeight * (-2f * gravityDirection) * gravity) * gravityDirection;
                    reflectLookVector = Vector3.Reflect(lookVector, wallNormal);
                    canWallJump = false;
                    StartCoroutine(WallTurning());
                }
            }
        }

        //Restart from checkpoint debug button

        if (Input.GetButtonDown("Reset") || restartGame == 1)
        {
            StartCoroutine(RestartFromCheckpoint());
        }

      

        //Apply velocity

        playerController.Move(velocity * Time.deltaTime + moveVector * speed * Time.deltaTime);

    }
}