using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This manager is what controls the particle system, sound, and teleportation of a player when they fall into a death volume. Does a small amount of damage.
/// </summary>
public class FallManager : MonoBehaviour
{
    public ParticleSystem part;
    public AudioSource a;
    public Vector3 originalPosition;
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
