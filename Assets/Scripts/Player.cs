using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;

public class Player : MonoBehaviour
{
    //movement
    public float speed;
    public float slowdown;
    
    //jump-related variables
    public float jumpHeight;
    private int floorContacts = 0;
    private int jumpCooldown = 0;
    public int health { get; private set; } = 0;
    public int maxHealth = 3;
    private Rigidbody rb;

    //Camera fields
    private Transform cameraPivot;
    private Quaternion newRotation;
    private Camera playerCamera;
    private Quaternion previous3DRotation;
    public bool is3DMode;
    private bool pending2dColliderChange;

    public bool forceZ;
    public float forcedZ;

    //Camera parameters
    public float rotationAmount;
    public float rotationTime;

    public static Player instance;
    public float cameraDistance = 5f;

    //Audio
    private AudioSource playerAudioSource;
    public AudioClip[] genericHitSounds;
    
    //death and respawning
    public Vector3 respawnPoint;
    public float fallThreshold = -10f;
    

    void Start()
    {
        this.is3DMode = true;
        this.rb = GetComponent<Rigidbody>();
        this.cameraPivot = transform.Find("CameraPivot");
        this.previous3DRotation = cameraPivot.rotation;
        this.newRotation = cameraPivot.rotation;
        this.playerCamera = cameraPivot.Find("PlayerCamera").GetComponent<Camera>();
        health = maxHealth;
        instance = this;
        playerAudioSource = GetComponent<AudioSource>();
        respawnPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (pending2dColliderChange)
        {
            GetComponent<BoxCollider>().size = new Vector3(1, 1, 100);
            pending2dColliderChange = false;
        }
        handleCamera();
        checkDeath();
    }

    private void checkDeath()
    {
        if (transform.position.y < fallThreshold || health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        health = maxHealth;
        transform.position = respawnPoint;
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

        if (is3DMode)
        {
            //from player to camera
            Ray ray = new Ray(transform.position, playerCamera.transform.position - transform.position);
            //ignore the 3d layer, which contains only the player
            bool found = Physics.Raycast(ray, out RaycastHit hit, cameraDistance, ~(1 << LayerMask.NameToLayer("3d")));
            if (found)
            {
                print("HIT " + hit.distance);
                setCameraDistance(hit.distance);
            }
            else
            {
                setCameraDistance(cameraDistance);
            }
        }
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
            pending2dColliderChange = true;
            rb.constraints |= RigidbodyConstraints.FreezeRotationY;
            transform.rotation = Quaternion.identity;
            //switch to layer "3d", looking up by name
            gameObject.layer = LayerMask.NameToLayer("2d");
            playerCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("3d only no render"));
        }
        else {
            print("2d->3d");
            //Load back the original camera configuration
            this.newRotation = this.previous3DRotation;
            this.playerCamera.orthographic = false;
            this.is3DMode = !is3DMode;
            rb.constraints &= ~RigidbodyConstraints.FreezeRotationY;
            GetComponent<BoxCollider>().size = new Vector3(1, 1, 1);
            gameObject.layer = LayerMask.NameToLayer("3d");
            playerCamera.cullingMask |= 1 << LayerMask.NameToLayer("3d only no render");
            if (forceZ)
            {
                setCameraDistance(cameraDistance);
            }
        }

    }

    private void setCameraDistance(float dist)
    {
        playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x,
            playerCamera.transform.localPosition.y, -dist);
    }

    public void Damage(int amount)
    {
        playerAudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        playerAudioSource.PlayOneShot(genericHitSounds[UnityEngine.Random.Range(0, genericHitSounds.Length - 1)], 1.0f);
        health = Math.Clamp(health - amount, 0, maxHealth);
    }

    public void Heal(int amount)
    {
        health = Math.Clamp(health + amount, 0, maxHealth);
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.transform.position.y < transform.position.y)
        {
            floorContacts++;
        }
    }
}
