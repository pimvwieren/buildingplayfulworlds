using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class SpikeTriggerScript : MonoBehaviour
{
    public UnityEvent triggers;
    AudioSource audioData;
    // Start is called before the first frame update
    void Start()
    {
        audioData = GetComponent<AudioSource>();
        if (triggers == null)
            triggers = new UnityEvent();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (triggers != null)
        {
            audioData.PlayDelayed(0.5f);
            triggers.Invoke();
        }
    }
}
