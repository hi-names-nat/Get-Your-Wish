using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    //VARIABLES
    [Tooltip("The Sensitivity of the camera")]
    public float sensitivity = 10f;
    [Tooltip("The angle the player can look up/down")]
    public float maxYAngle = 90f;
    private Vector2 currentRotation;

    // Update is called once per frame
    void Update()
    {
        currentRotation.x -= Input.GetAxis("Mouse Y") * sensitivity;

        //clamps the y rotation of the camera from -maxYangle to maxYangle to prevent dumb stuff
        currentRotation.x = Mathf.Clamp(currentRotation.x, -maxYAngle, maxYAngle);

        transform.localRotation = Quaternion.Euler(currentRotation.x, 0, 0);
    }
}
