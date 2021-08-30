using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMoveTowards : MonoBehaviour
{

    public GameObject target;

    private Vector3 start;

    public Vector3 movedir = new Vector3();

    public Vector3 lastDir;
    // Start is called before the first frame update
    void Start()
    {
        start = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        lastDir = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, .1f);
        if (transform.position == target.transform.position)
        {
            transform.position = start;
        }
        movedir = transform.position - lastDir;
        
    }
}
