using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float horizontalMove;
    public float verticalMove;

    private Vector3 playerInput;

    public CharacterController player;
    public float playerSpeed;
    public float gravity = 9.8f;
    public float fallVelocity;
    public float jumpForce;

    //Movimiento relativo a cámara
    public Camera mainCamera;
    private Vector3 cameraForward;
    private Vector3 cameraRight;
    private Vector3 movePlayer;

    //Deslizamiento en pendientes
    private bool isOnSlope = false;
    private Vector3 hitNormal;
    public float slideVelocity;
    public float slopeForceDown;

    //Animation controller
    public Animator playerAnimatorController;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<CharacterController>();
        playerAnimatorController = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxis("Horizontal");
        verticalMove = Input.GetAxis("Vertical");

        playerInput = new Vector3(horizontalMove, 0, verticalMove);
        playerInput = Vector3.ClampMagnitude(playerInput, 1);

        playerAnimatorController.SetFloat("PlayerWalkVelocity", playerInput.magnitude * playerSpeed);

        cameraDirection();

        movePlayer = playerInput.x * cameraRight + playerInput.z * cameraForward;
        movePlayer = movePlayer * playerSpeed;

        player.transform.LookAt(player.transform.position + movePlayer);

        setGravity();

        playerSkills();

        player.Move(movePlayer * Time.deltaTime);    
    }

    void cameraDirection()
    {
        cameraForward = mainCamera.transform.forward;
        cameraRight = mainCamera.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;
    }

    public void playerSkills()
    {
        if(player.isGrounded && Input.GetButtonDown("Jump"))
        {
            fallVelocity = jumpForce;
            movePlayer.y = fallVelocity;
            playerAnimatorController.SetTrigger("PlayerJump");
        }
    }

    public void setGravity()
    {
        if(player.isGrounded)
        {
            fallVelocity = -gravity * Time.deltaTime;
            movePlayer.y = fallVelocity;
        } 
        else
        {
            fallVelocity -= gravity * Time.deltaTime;
            movePlayer.y = fallVelocity;

            playerAnimatorController.SetFloat("PlayerVerticalVelocity", player.velocity.y);
        }

        playerAnimatorController.SetBool("IsGrounded", player.isGrounded);
        slideDown();
    }

    public void slideDown()
    {
        isOnSlope = Vector3.Angle(Vector3.up, hitNormal) >= player.slopeLimit;

        if(isOnSlope)
        {
            movePlayer.x += ((1f - hitNormal.y) * hitNormal.x) * slideVelocity;
            movePlayer.z += ((1f - hitNormal.y) * hitNormal.z) * slideVelocity;

            movePlayer.y += slopeForceDown;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitNormal = hit.normal;
    }

    private void OnAnimatorMove()
    {
    
    }

}
