using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class load1 : MonoBehaviour
{

    public GameObject triggeroBj;
    public GameObject destroyOnEnable;

    private void Start()
    {
        StartCoroutine("waitforEnable");
    }



    IEnumerator waitforEnable()
    {
        while (true)
        {
            print("working");
            if (triggeroBj.GetComponent<TriggerDetect>().triggered == true)
            {
                gameObject.SetActive(false);
                break;
            } else
            {
                yield return null;
            }
        }
    }
}
