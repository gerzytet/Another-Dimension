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
    private bool is3DMode;
    private Rigidbody rb;

    //Camera fields
    private Transform cameraPivot;
    private Quaternion newRotation;
    private Camera playerCamera;
    private Quaternion previous3DRotation;

    //Camera parameters
    public float rotationAmount;
    public float rotationTime;

    void Start()
    {
        this.is3DMode = true;
        this.rb = GetComponent<Rigidbody>();
        this.cameraPivot = transform.Find("CameraPivot");
        this.previous3DRotation = cameraPivot.rotation;
        this.newRotation = cameraPivot.rotation;
        this.playerCamera = cameraPivot.Find("PlayerCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        handleCamera();
    }

    void FixedUpdate() {
        handleAction();
    }

    private void handleAction() {
        void Move(Vector3 direction)
        {
            rb.AddForce(Vector3.ProjectOnPlane(direction * speed, Vector3.up));
        }
        
        
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            Move(playerCamera.transform.forward);
        } else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            Move(-playerCamera.transform.forward);
        }
        
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            Move(-playerCamera.transform.right);
        } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            Move(playerCamera.transform.right);  
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

    private void handleCamera() {
        if (Input.GetKey(KeyCode.Q) && is3DMode) {
            this.newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        if (Input.GetKey(KeyCode.E) && is3DMode) {
            this.newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }
        if (Input.GetKeyDown(KeyCode.Z)) {
            this.switchDimensionMode();
        }
        this.cameraPivot.rotation = Quaternion.Lerp(cameraPivot.rotation, newRotation, Time.deltaTime * rotationTime);
    }

    private void switchDimensionMode() {
        if (is3DMode) {
            print("3d->2d");
            //Save the current camera rotation
            this.previous3DRotation = this.cameraPivot.rotation;
            //We can change this to ask the level for a direction to snap to 2D later
            this.newRotation = Quaternion.AngleAxis(0, Vector3.up);
            this.playerCamera.orthographic = true;
            this.is3DMode = !is3DMode;
        }
        else {
            print("2d->3d");
            //Load back the original camera configuration
            this.newRotation = this.previous3DRotation;
            this.playerCamera.orthographic = false;
            this.is3DMode = !is3DMode;
        }

    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.transform.position.y < gameObject.transform.position.y)
        {
            floorContacts++;
        }
    }
}
