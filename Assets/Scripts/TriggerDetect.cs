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
        StartCoroutine("waitandDestroy");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject  == check) {
            triggered = true;
            print("triggered");
        }
    }

    IEnumerator waitandDestroy()
    {
        while (triggered != true)
        {
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
