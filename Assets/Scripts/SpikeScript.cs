using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    public float spikeDelay = 0.5f;
    public float spikeSpeed = 0.5f;
    public float spikeMaxHeight = 2f;
    public float spikeWaitDuration = 1f;
    public bool isTimingSpear = false;
    private bool isSpikeActive = false;
    Vector3 localSpikePosition;

    void Start()
    {
        localSpikePosition = transform.localPosition;
        if (isTimingSpear)
        {
            ActivateTrap();
        }
    }

    public void ActivateTrap()
    {
        if (isSpikeActive) { return; }
        StartCoroutine(SpikeMovement());
    }

    IEnumerator SpikeMovement()
    {
        isSpikeActive = true;
        yield return new WaitForSeconds(spikeDelay);

        float t = 0;
        
        while (t < 1)
        {
            t += Time.deltaTime * 1f / spikeSpeed;
            yield return null;
            transform.localPosition = Vector3.Lerp(localSpikePosition, new Vector3(localSpikePosition.x, localSpikePosition.y + spikeMaxHeight, localSpikePosition.z), t);
        }

        yield return new WaitForSeconds(spikeWaitDuration);

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 1f / spikeSpeed;
            yield return null;
            transform.localPosition = Vector3.Lerp(new Vector3(localSpikePosition.x, localSpikePosition.y + spikeMaxHeight, localSpikePosition.z), localSpikePosition, t);
        }
        isSpikeActive = false;

        if (isTimingSpear)
        {
            ActivateTrap();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isSpikeActive == true)
        {
            other.transform.GetComponent<PlayerControllerScript>().restartGame = 1;
        }
    }

}