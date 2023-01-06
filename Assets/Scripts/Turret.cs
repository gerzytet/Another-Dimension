using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject projectile;
    public float shootCooldown;
    private float untilNextShot = 0;
    
    void FixedUpdate()
    {
        if (untilNextShot <= 0)
        {
            Shoot();
            untilNextShot = shootCooldown;
        }
        untilNextShot -= Time.deltaTime;
    }

    void Shoot()
    {
        Instantiate(projectile, transform.position + transform.up * 0.75f + transform.forward * 0.5f, transform.rotation);
    }
}
