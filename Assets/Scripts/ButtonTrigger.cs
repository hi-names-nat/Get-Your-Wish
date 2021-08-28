using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//make sure to set those assets to disabled
public class ButtonTrigger : MonoBehaviour
{
    public bool isEnabled;

    private void Update()
    {
        if (isEnabled)
        {
            GetComponent<Renderer>().enabled = true;
            GetComponent<Button>().enabled = true;
        }
    }



}
