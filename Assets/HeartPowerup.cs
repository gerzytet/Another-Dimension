using UnityEngine;

public class HeartPowerup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            if (!player.is3DMode)
            {
                player.Heal(1);
                Destroy(gameObject);
            }
        }
    }
}
