using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsScript : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        other.transform.GetComponent<PlayerControllerScript>().restartGame = 1;
    }
}