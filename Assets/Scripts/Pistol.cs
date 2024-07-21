using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Pistol : MonoBehaviour
{
    public static Pistol instance;
    public GunType gunType;
    public enum GunType {Default, Enemy};
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float shootCooldown;
    private bool canShoot = true;
    private bool enemyCanShoot = true;
    public bool isReloading = false;
    private bool isShotgunMode = false;
    [SerializeField] private float reloadTime = 1f;
    [SerializeField] private int maxBulletCount = 6;  
    public int currentBulletCount;
    private Animator animator;
    private Transform playerTransform;

    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }
        
    }
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        currentBulletCount = maxBulletCount;
    }
    private void Update()
    {
        if (gunType == GunType.Default)
        {
            Vector2 cursorPosition = Input.mousePosition;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(cursorPosition);
            mousePosition.z = 0f;
            Vector2 direction = mousePosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

        }
        else if (gunType == GunType.Enemy)
        {
            Vector2 direction = playerTransform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }
        if (Input.GetKeyDown(KeyCode.E) && gunType == GunType.Default)
        {
            isShotgunMode = !isShotgunMode;
        }
        
        if (Input.GetButtonDown("Fire1") && canShoot && currentBulletCount > 0 && gunType == GunType.Default || gunType == GunType.Enemy && enemyCanShoot)
        {
            StartCoroutine(ShootCooldown(shootCooldown));

            if (gunType == GunType.Default)
            {
                Vector2 cursorPosition = Input.mousePosition;
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(cursorPosition);
                mousePosition.z = 0f;
                Vector2 direction = mousePosition - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);

                if (isShotgunMode)
                {
                    ShotgunShoot();
                    currentBulletCount -= currentBulletCount;
                }
                else
                {
                    Shoot(direction);
                    --currentBulletCount;
                }
            }
            if (gunType == GunType.Enemy)
            {
                Vector2 direction = playerTransform.position - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Shoot(direction);
            }
        }
        if (Input.GetKeyDown(KeyCode.R) && currentBulletCount == 0 && !isReloading && gunType == GunType.Default)
        {
            StartCoroutine(Reload());
        }
  
       
    }

    private void Shoot(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
        bulletRigidbody.linearVelocity = direction.normalized * bulletSpeed;

    }
    private void ShotgunShoot()
    {
        float angleOffset = 10f;

        for (int i = 0; i < currentBulletCount; i++)
        {
            float angle = transform.eulerAngles.z - 2 * angleOffset + (i * angleOffset);
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Shoot(direction);
        }
    }
    IEnumerator ShootCooldown(float shootCooldown)
    {
        if (gunType == GunType.Default)
        {
            canShoot = false;
            yield return new WaitForSeconds(shootCooldown);
            canShoot = true;
        }
        else if (gunType == GunType.Enemy)
        {
            enemyCanShoot = false;
            yield return new WaitForSeconds(shootCooldown);
            enemyCanShoot = true;
        }
    }
    public IEnumerator Reload()
    {
        isReloading = true;
        canShoot = false;
        yield return new WaitForSeconds(reloadTime);
        currentBulletCount = maxBulletCount;
        canShoot = true;
        isReloading = false;
    }
   
}
