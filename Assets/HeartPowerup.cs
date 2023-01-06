using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPowerup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            player.Heal(1);
            Destroy(gameObject);
        }
    }
}
