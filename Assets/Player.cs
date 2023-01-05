using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float slowdown;

    public float jumpHeight;
    private int floorContacts = 0;
    private int jumpCooldown = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var rb = GetComponent<Rigidbody>();
        void Move(Vector3 direction)
        {
            rb.AddForce(Vector3.ProjectOnPlane(direction * speed, Vector3.up));
        }
        
        
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            Move(Camera.main.transform.forward);
        } else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            Move(-Camera.main.transform.forward);
        }
        
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            Move(-Camera.main.transform.right);
        } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            Move(Camera.main.transform.right);
        }
        
        if (Input.GetKey(KeyCode.Space) && floorContacts > 0 && jumpCooldown <= 0)
        {
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            jumpCooldown = 20;
            print("jump");
        }

        jumpCooldown--;

        floorContacts = 0;

        Vector2 xzVelocity = new Vector2(rb.velocity.x, rb.velocity.z);
        xzVelocity *= 1 - slowdown;
        rb.velocity = new Vector3(xzVelocity.x, rb.velocity.y, xzVelocity.y);
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.transform.position.y < gameObject.transform.position.y)
        {
            floorContacts++;
        }
    }
}
