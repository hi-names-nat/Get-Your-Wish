/**
 * 
 * USAGE: Place this script onto the player object with the camera childed to it. Ensure there is a capsule collider on the parent gameobject.
 * Desc: basic FPS controller.
 * Author: Dani/Natalie Soltis, whatever I go by at the time :P
 * Other: Sean sux lol
 * 
 **/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCharacterMovement : MonoBehaviour
{
    //VARIABLES
    [Tooltip("The Sensitivity of the camera")]
    public float sensitivity = 10f;
    [Tooltip("The player's speed")]
    public float playerSpeed = 15f;
    [Tooltip("The angle the player can look up/down")]
    public float maxYAngle = 90f;
    [Tooltip("Height the player can jump")]
    public float jumpHeight = 10f;


    //private.
    private Vector2 currentRotation;
    private Vector2 cameraRot;
    private GameObject playerObject;
    private GameObject cameraObject;

    private void Start()
    {
        playerObject = transform.gameObject;
        cameraObject = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
        MovePlayer();
    }

    void MovePlayer()
    {
        //moves the player 
        currentRotation.y += Input.GetAxis("Mouse X") * sensitivity;
        currentRotation.y = Mathf.Repeat(currentRotation.y, 360);
        transform.rotation = Quaternion.Euler(0, currentRotation.y, 0);
        playerObject.transform.Translate(transform.worldToLocalMatrix.MultiplyVector(transform.forward) * Input.GetAxisRaw("Player1UD") * playerSpeed / 500);
        playerObject.transform.Translate(transform.worldToLocalMatrix.MultiplyVector(transform.right) * Input.GetAxisRaw("Player1LR") * playerSpeed / 500);
        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            playerObject.transform.GetComponent<Rigidbody>().AddForce(0, jumpHeight * 25, 0);
        }
    }

    bool isGrounded()
    {
        //Debug.DrawRay(playerObject.transform.localPosition, -playerObject.transform.up);
        if (Physics.Raycast(playerObject.transform.localPosition, -playerObject.transform.up, 1.5f))
        {
            return (true);
        }
        return (false);
    }

    void MoveCamera()
    {
        //manipulates the child camera to sync with the y value of the parent.
        cameraRot.x -= Input.GetAxis("Mouse Y") * sensitivity;

        //clamps the y rotation of the camera from -maxYangle to maxYangle to prevent dumb stuff
        cameraRot.x = Mathf.Clamp(cameraRot.x, -maxYAngle, maxYAngle);

        cameraObject.transform.localRotation = Quaternion.Euler(cameraRot.x, 0, 0);
    }

}