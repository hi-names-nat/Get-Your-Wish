using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class Bilboard : MonoBehaviour
{

    [TextArea]
    public string text = "FIX IF YOU HAVE THE TIME!";

    public GameObject player;

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y, 0);
    }
}
