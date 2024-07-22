using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 2f;
    private bool canDamagePlayer = true;
    [SerializeField] private int damage = 1;
    [SerializeField] private bool enemyBullet;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !enemyBullet)
        {
            collision.gameObject.GetComponent<Enemy>().EnemyTakeDamage(damage);
            Destroy(gameObject);
        }
        if (collision.CompareTag("Player") && enemyBullet && canDamagePlayer)
        {
            if (collision.GetComponent<Player>().playerHealth != 0)
            {
                collision.gameObject.GetComponent<Player>().PlayerTakeDamage(damage);
                canDamagePlayer = false;
                Invoke(nameof(ResetDamageFlag), 2f);
                Destroy(gameObject);
            }
        }
    }
    private void ResetDamageFlag()
    {
        canDamagePlayer = true;
    }
}
