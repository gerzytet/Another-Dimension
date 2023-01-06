using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject projectile;
    public float shootCooldown;
    private float untilNextShot = 0;
    private AudioSource turretAudioSource;
    public AudioClip turretShoot;
    public AudioClip turretDestroy;

    void Start() 
    {
        turretAudioSource = GetComponent<AudioSource>();
    }
    
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
        turretAudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        turretAudioSource.PlayOneShot(turretShoot);
        Instantiate(projectile, transform.position + transform.up * 0.75f + transform.forward * 0.5f, transform.rotation);
    }
}
