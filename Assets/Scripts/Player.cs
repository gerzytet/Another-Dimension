using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;

public class Player : MonoBehaviour
{
    //general constants
    public float zDimensionWidth = 100f;

    //Components
    private Rigidbody rb;
    private AudioSource playerAudioSource;
    private BoxCollider playerCollider;

    //movement
    public float speed;
    public float slowdown;

    //jump-related variables
    public float jumpHeight;
    private int jumpCooldown = 0;
    public int health { get; private set; } = 0;
    public int maxHealth = 3;
    public int jetpackMaxFuel;
    private int jetpackFuel;
    private RaycastHit groundedCheckRaycastHit;
    private int contacts = 0;

    //Camera fields
    private Transform cameraPivot;
    private Quaternion newRotation;
    private Camera playerCamera;
    private Quaternion previous3DRotation;
    private Quaternion CameraAngleConstant2D;
    public bool is3DMode;
    private Vector3 currentMousePosition;

    public bool forceZ;

    //Camera parameters
    public float rotationAmount;
    public float rotationTime;
    public float maxCameraYAngle;

    public static Player instance;
    public float cameraDistance = 5f;

    //Audio
    public AudioClip[] genericHitSounds;

    //death and respawning
    public Vector3 respawnPoint;
    public float fallThreshold = -10f;

    //flags
    public bool dimensionTransitionFlag;
    
    //animation
    private Animator animator;
    public float maxRunThreshold;

    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            //instance.transform.position = transform.position;
            instance.respawnPoint = transform.position;
            return;
        }
        
        //Get components
        this.rb = GetComponent<Rigidbody>();
        this.playerCollider = rb.gameObject.GetComponent<BoxCollider>();
        this.is3DMode = true;
        playerAudioSource = GetComponent<AudioSource>();
        
        //Find components
        this.cameraPivot = transform.Find("CameraPivot");
        this.playerCamera = cameraPivot.Find("PlayerCamera").GetComponent<Camera>();
        
        //Variable in itializations
        this.previous3DRotation = cameraPivot.rotation;
        this.newRotation = cameraPivot.rotation;
        health = maxHealth;
        respawnPoint = transform.position;
        this.CameraAngleConstant2D = Quaternion.AngleAxis(0, Vector3.up);
        this.currentMousePosition = new Vector3(Screen.width/2f, Screen.height/2f, 0f); //initial camera poisition is center of screen
        dimensionTransitionFlag = false;
        this.jetpackFuel = jetpackMaxFuel;
        
        instance = this;
        //Cursor.lockState = CursorLockMode.Locked;
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        handleCamera();
        Animate();
    }

    private void Animate()
    {
        float blend = Mathf.InverseLerp(0, maxRunThreshold, rb.velocity.magnitude);
        animator.SetFloat("Speed", blend);
    }

    private void checkDeath()
    {
        if (transform.position.y < fallThreshold || health <= 0)
        {
            Die(); //rip
        }
    }

    private void Die()
    {
        health = maxHealth;
        transform.position = respawnPoint;
        rb.velocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        handleAction();
        if (dimensionTransitionFlag)
        {
            if (!is3DMode)
            {
                this.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
                this.playerCollider.size = new Vector3(zDimensionWidth, 1, 1);
                rb.constraints |= RigidbodyConstraints.FreezeRotationY;
                this.cameraPivot.rotation = this.CameraAngleConstant2D;
            }
            else
            {
                this.playerCollider.size = new Vector3(1, 1, 1);
                rb.constraints &= ~RigidbodyConstraints.FreezeRotationY;
            }
            dimensionTransitionFlag = false;
        }


        checkDeath();
        jumpCooldown--;
        Vector2 xzVelocity = new Vector2(rb.velocity.x, rb.velocity.z);
        xzVelocity *= 1 - slowdown;
        rb.velocity = new Vector3(xzVelocity.x, rb.velocity.y, xzVelocity.y);
    }

    private void handleAction()
    {
        //WASD Movement
        void Move(Vector3 direction)
        {
            rb.AddForce(Vector3.ProjectOnPlane(direction * speed, Vector3.up));
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            Move(playerCamera.transform.forward);
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            Move(-playerCamera.transform.forward);
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            Move(-playerCamera.transform.right);
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            Move(playerCamera.transform.right);
        }

        //Jumping
        if (Input.GetKey(KeyCode.Space) && isGrounded() && jumpCooldown <= 0 && contacts > 0)
        {
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            jumpCooldown = 20;
        }

        //Jetpack

        //Switch Dimension Mode
        if (Input.GetKeyDown(KeyCode.Z))
        {
            this.switchDimensionMode();
        }
    }

    private void handleCamera()
    {
        if (is3DMode)
        {
            this.cameraPivot.rotation = Quaternion.Lerp(cameraPivot.rotation, newRotation, Time.deltaTime * rotationTime);

            this.cameraPivot.eulerAngles += (new Vector3(currentMousePosition.y - Input.mousePosition.y, Input.mousePosition.x - currentMousePosition.x, 0f) / 5f);

            this.newRotation = cameraPivot.rotation;

            this.currentMousePosition = Input.mousePosition;

            //from player to camera
            Ray ray = new Ray(transform.position, playerCamera.transform.position - transform.position);
            //ignore the 3d layer, which contains only the player
            bool found = Physics.Raycast(ray, out RaycastHit hit, cameraDistance, ~(1 << LayerMask.NameToLayer("3d")));
            if (found)
            {
                setCameraDistance(hit.distance);
            }
            else
            {
                setCameraDistance(cameraDistance);
            }
        }
    }

    private void switchDimensionMode()
    {
        if (is3DMode)
        {
            print("3d->2d");
            //Save the current camera rotation, in global context
            this.previous3DRotation = this.previous3DRotation = this.cameraPivot.localRotation * transform.rotation;
            this.newRotation = this.CameraAngleConstant2D;
            this.playerCamera.orthographic = true;
            //switch to layer "3d", looking up by name
            gameObject.layer = LayerMask.NameToLayer("2d");
            playerCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("3d only no render"));
            this.is3DMode = !is3DMode;
        }
        else
        {
            print("2d->3d");
            //Load back the original camera configuration, in global context
            this.newRotation = this.previous3DRotation * Quaternion.Inverse(this.transform.rotation);
            this.playerCamera.orthographic = false;
            gameObject.layer = LayerMask.NameToLayer("3d");
            playerCamera.cullingMask |= 1 << LayerMask.NameToLayer("3d only no render");
            if (forceZ)
            {
                setCameraDistance(cameraDistance);
            }
            this.is3DMode = !is3DMode;
        }
        this.dimensionTransitionFlag = true;
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

    private bool isGrounded() {
        if (Physics.BoxCast(playerCollider.bounds.center, playerCollider.size * 0.495f, Vector3.down, out groundedCheckRaycastHit, transform.rotation, playerCollider.size.y /2f)) {
            return true;
        }
        return false;
    }

    public void OnCollisionEnter(Collision collision)
    {
        contacts++;
    }
    
    public void OnCollisionExit(Collision collision)
    {
        contacts--;
    }
}
