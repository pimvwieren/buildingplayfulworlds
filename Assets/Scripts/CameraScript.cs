using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    public Transform playerBody;

    float xRotation = 0f;

    public float gravityRotationSpeed = 360f;

    PlayerControllerScript playerControllerObject;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerControllerObject = gameObject.GetComponentInParent<PlayerControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * playerControllerObject.gravityDirection * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * playerControllerObject.gravityDirection * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerBody.Rotate(Vector3.up * mouseX);

        if (playerControllerObject.gravityDirection == 1)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(xRotation, 0f, 0f), 180 / gravityRotationSpeed * Time.deltaTime);
        }
        else {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(xRotation, 0f, 180f), 180 / gravityRotationSpeed * Time.deltaTime);
        }
    }
}