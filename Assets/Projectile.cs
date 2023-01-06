using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float lifetime;
    void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.Damage(1);
        }
        Destroy(gameObject);
    }
}
