using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class watch : MonoBehaviour
{
    public GameObject player;

    public bool enabled = false;

    // Update is called once per frame
    void Update()
    {
        if (enabled)
        {
            transform.LookAt(player.transform, transform.up);
        }     
    }
}
