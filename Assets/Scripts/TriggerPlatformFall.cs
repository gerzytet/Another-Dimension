using System;
using UnityEngine;

public class TriggerPlatformFall : MonoBehaviour
{
    public GameObject platformToFall;
    private bool triggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!triggered)
        {
            triggered = true;
            platformToFall.AddComponent<Rigidbody>();
        }
    }
}