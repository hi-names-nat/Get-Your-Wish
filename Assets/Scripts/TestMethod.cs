using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMethod : MonoBehaviour
{
    public TriggerDetect td;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        print(td.triggered);
    }
    
}
