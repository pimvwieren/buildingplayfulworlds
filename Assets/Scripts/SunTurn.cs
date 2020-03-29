using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunTurn : MonoBehaviour
{
    public GameObject playerController;
    PlayerControllerScript playerControllerObject;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerObject = playerController.GetComponent<PlayerControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerControllerObject.gravityDirection == 1)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(50, 50f, 0f), 180 * Time.deltaTime);
        }
        else
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(230, 50f, 0f), 180 * Time.deltaTime);
        }
    }
}
