/*
 * File:	    FPSCharacterGun.cs
 * Author:      Dakota Taylor
 * Date:	    02 March 2020
 * Desc:	    A gun for the player object! Can shoot up to max ammo and can reload whenever not at max ammo. For now, 'F' is to shoot and 'R' is reload
 */


using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FPSCharacterGun : MonoBehaviour
{
    [Tooltip("Amount of ammo the gun can hold")]
    public int maxAmmo = 64;

    [Tooltip("Time (in seconds) it takes to shoot one bullet")]
    public float fireRate = 0.1f; // in seconds for now, might make it int and have it in milliseconds at another point

    [Tooltip("The object the gun shoots")]
    public GameObject bullet = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Projectile.prefab", typeof(GameObject));

    private int ammo;
    private float fireCooldown;

    // Start is called before the first frame update
    void Start()
    {
        ammo = maxAmmo;
        fireCooldown = 0f;

        // bullet = Resources.Load("Prefabs/Projectile") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        bool canFire = (fireCooldown <= 0f);
        if (!canFire)
            fireCooldown -= Time.deltaTime;

        if (canFire && Input.GetKey(KeyCode.F))
            FireGun();

        if (Input.GetKey(KeyCode.R))
            ReloadGun(); // Don't know if this should be its own function or not ¯\_(ツ)_/¯
    }

    void FireGun()
    {
        if (ammo != 0)
        {
            ammo -= 1;
            fireCooldown = fireRate;
            Camera cam = GetComponentInChildren<Camera>();
            Vector3 pos = cam.transform.position + cam.transform.forward;
            Quaternion rot = transform.rotation;

            // TODO: despawn new bullets after a few seconds from being shoot
            // -or- have each gun use list of bullets and reuse the oldest bullet
            GameObject newBullet = Instantiate(bullet, pos, rot);
            newBullet.transform.GetComponent<Rigidbody>().AddForce(cam.transform.forward * 500);
        }
    }

    void ReloadGun()
    {
        if (ammo != maxAmmo)
            ammo = maxAmmo;
    }
}