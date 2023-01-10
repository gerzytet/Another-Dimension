using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float maxSpeed;
    private Rigidbody rb;

    void Start() {
        this.rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        transform.LookAt(Player.instance.transform);
        rb.AddForce(transform.forward * (speed * Time.deltaTime));
        rb.velocity = Vector3.ClampMagnitude(GetComponent<Rigidbody>().velocity, speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.Damage(1);
            Destroy(gameObject);
        }
    }
}
