using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetect : MonoBehaviour
{
    private Collider c;
    public GameObject check;

    [System.NonSerialized]
    public bool triggered = false;

    // Start is called before the first frame update
    void Start()
    {
        c = GetComponent<Collider>();
        if (!c.isTrigger) {
            throw new System.Exception("Collider must be a trigger!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject  == check) {
            triggered = true;
        }
    }

    private void OnTriggerExit(Collider other) {

        if (other.gameObject == check) {
            triggered = false;
        }
    }
}
