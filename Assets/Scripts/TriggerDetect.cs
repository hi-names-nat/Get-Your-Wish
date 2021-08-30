using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerDetect : MonoBehaviour
{
    private Collider c;

    public UnityEvent onTriggered;

    void Start()
    {
        c = GetComponent<Collider>();
        if (!c.isTrigger) {
            Debug.LogWarning("Collider was not trigger! Fixing...");
            c.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            onTriggered.Invoke();
            print("triggered");
            Destroy(this.gameObject);
        }
    }
}
