/**
 * 
 * USAGE: Place this script onto the main camera. Make the main camera the child of a capsule collider.
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
    private Vector2 currentRotation;
    [Tooltip("The angle the player can look up/down")]
    public float maxYAngle = 90f;
    [Tooltip("Height the player can jump")]
    public float jumpHeight = 10f;
    private GameObject playerObject;

    private void Start()
    {
        playerObject = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        moveCamera();
        movePlayer();
    }

    void moveCamera()
    {
        currentRotation.x += Input.GetAxis("Mouse X") * sensitivity;
        currentRotation.y -= Input.GetAxis("Mouse Y") * sensitivity;
        currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
        currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);
        transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
    }

    void movePlayer()
    {
        playerObject.transform.Translate(transform.forward.x * Input.GetAxisRaw("Player1UD") * sensitivity / 100, 0, transform.forward.z * Input.GetAxisRaw("Player1UD") * sensitivity / 100); //avoids getting y component of the vector so there's no weird floaty shit
        //playerObject.transform.Translate(Mathf.Abs(Mathf.Cos(transform.rotation.y))* transform.forward.x * Input.GetAxisRaw("Player1UD") * sensitivity / 100, 0, Mathf.Abs(Mathf.Cos(transform.rotation.y)) * transform.forward.z * Input.GetAxisRaw("Player1UD") * sensitivity / 100); //avoids getting y component of the vector so there's no weird floaty shit
        playerObject.transform.Translate(transform.right * Input.GetAxisRaw("Player1LR") * sensitivity / 100);
        if (Input.GetButtonDown("Player1Jump") && isGrounded())
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

}