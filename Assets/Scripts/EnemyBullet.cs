using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private float bulletSpeed = 10f;
    private float lifeTime = 3f;
    private int damage = 1;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }
    private void Update()
    {
        // Move the bullet in the forward direction
        rb.linearVelocity = transform.right * bulletSpeed;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
            if (collision.GetComponent<Player>().playerHealth != 0)
            {
                collision.GetComponent<Player>().PlayerTakeDamage(damage);
            }

            // Destroy the bullet
            Destroy(gameObject);
        }
    }
}
