using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explode : MonoBehaviour
{
    public GameObject blip;
    public GameObject monitor2;

    private void Update()
    {
        if (blip.GetComponent<TriggerDetect>().triggered)
        {
            print("running this code");
            monitor2.SetActive(true);
            gameObject.SetActive(false);
        }
    }   
}
