using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class Bilboard : MonoBehaviour
{

    public GameObject player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y, 0);
    }
}
