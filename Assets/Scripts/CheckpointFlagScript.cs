using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointFlagScript : MonoBehaviour
{
    public int restartGravityDirection = 1;
    AudioSource audioData;
    private void Start()
    {
        audioData = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {

        PlayerControllerScript player = other.GetComponent<PlayerControllerScript>();
        if(player.checkpointPosition != new Vector3(transform.position.x, transform.position.y + (5 * restartGravityDirection), transform.position.z))
        {
            audioData.Play(0);
        }
        player.checkpointPosition = new Vector3(transform.position.x, transform.position.y + (5 * restartGravityDirection), transform.position.z);
        player.restartGravityDirection = restartGravityDirection;
    }
}