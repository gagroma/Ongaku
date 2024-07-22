using System.Collections;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    public static Pistol instance;

    [Header("Gun Type")]
    public GunType gunType;
    public enum GunType { Player, Enemy };

    [Header("Gun Parameters")]
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float shootCooldown;
    [SerializeField] private float reloadTime = 1f;
    [SerializeField] private float shotgunReloadTime = 3f;
    [SerializeField] private int maxBulletCount = 6;
    public bool isShotgunMode = false;
    public bool isReloading = false;
    public int currentBulletCount;
    private bool canShoot = true;
    private bool enemyCanShoot = true;

    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    private Animator animator;
    private Transform playerTransform;

    private void Awake()
    {
        if (instance is null) instance = this;

    }
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        currentBulletCount = maxBulletCount;
    }
    private void Update()
    {
        if (gunType == GunType.Enemy)
        {
            Vector2 direction = playerTransform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }
        //Switch Mode
        if (Input.GetKeyDown(KeyCode.E) && gunType == GunType.Player) isShotgunMode = !isShotgunMode;
        //Fire
        if (Input.GetButtonDown("Fire1") && !isReloading && canShoot && currentBulletCount > 0 && gunType == GunType.Player || gunType == GunType.Enemy && enemyCanShoot)
        {
            if (isShotgunMode) StartCoroutine(ShootCooldown(shotgunReloadTime));
            else StartCoroutine(ShootCooldown(shootCooldown));


            if (gunType == GunType.Player)
            {
                Vector2 cursorPosition = Input.mousePosition;
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(cursorPosition);
                mousePosition.z = 0f;
                Vector2 direction = mousePosition - transform.position;

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
        //Reload
        if ((currentBulletCount == 0 && Input.GetButtonDown("Fire1") && !isReloading) ||
    (Input.GetKeyDown(KeyCode.R) && currentBulletCount >= 0 && currentBulletCount < 6 && !isReloading && gunType == GunType.Player))
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
        if (gunType == GunType.Player)
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
        if(isShotgunMode) yield return new WaitForSeconds(shotgunReloadTime);
        else yield return new WaitForSeconds(reloadTime);
        currentBulletCount = maxBulletCount;
        canShoot = true;
        isReloading = false;
    }
}
