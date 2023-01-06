using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Color activeColor;
    public Color inactiveColor;
    void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            player.respawnPoint = transform.position;
        }
    }

    void Update()
    {
        GetComponent<Renderer>().material.color = Player.instance.respawnPoint == transform.position ? activeColor : inactiveColor;
    }
}