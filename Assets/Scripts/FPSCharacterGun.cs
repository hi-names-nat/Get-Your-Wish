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

public class FPSCharacterGun : MonoBehaviour {
    [Tooltip("Amount of ammo the gun can hold")]
    public int maxAmmo = 30;

    [Tooltip("How much force the bullet exerts when it hits something")]
    public float power = 10f;

    [Tooltip("Time (in seconds) it takes to shoot another bullet after firing one")]
    public float fireRate = 0.1f; // in seconds

    [Tooltip("Whether or not if the gun will keep shooting when trigger is held")]
    public bool isAutomatic = true;

    private Camera cam;
    private int ammo;
    private float fireCooldown;

    // Start is called before the first frame update
    void Start() {
        cam = GetComponentInChildren<Camera>();
        ammo = maxAmmo;
        fireCooldown = 0f;
    }

    // Update is called once per frame
    void Update() {
        if (canFire() && Input.GetKey(KeyCode.F))
            FireGun();

        if (Input.GetKey(KeyCode.R))
            ReloadGun(); // Don't know if this should be its own function or not ¯\_(ツ)_/¯
    }

    // Returns whether or not the gun can fire a bullet
    bool canFire() {
        bool canFire = (fireCooldown <= 0f);
        if (!canFire) {
            fireCooldown -= Time.deltaTime;
            canFire = (fireCooldown <= 0f);
        }

        if (!isAutomatic)
            canFire = canFire && Input.GetKeyDown(KeyCode.F);

        return canFire;
    }

    void FireGun() {
        if (ammo != 0) {
            ammo--;
            fireCooldown = fireRate;

            Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0)); // ray to center
            Debug.DrawRay(ray.origin, ray.direction, Color.red, 5);

            // Just messing with/testing the rays
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                Transform objectHit = hit.transform;

                var rb = objectHit.GetComponent<Rigidbody>();
                if (rb != null) rb.AddForce(ray.direction * power);
            }
        }
    }

    void ReloadGun() {
        if (ammo != maxAmmo)
            ammo = maxAmmo;
    }
}