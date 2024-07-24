using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public static Weapon instance;

    public enum GunType { Player, Enemy };
    public enum ShootingMode { Single, Shotgun, Auto};

    [Header("Gun Type")]
    public GunType gunType;

    [Header("Gun Parameters")]
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float shootCooldown = 0.5f;
    [SerializeField] private float reloadTime = 1f;
    [SerializeField] private int maxBulletCount = 6;
    [SerializeField] private ShootingMode shootingMode = ShootingMode.Single;
    public int currentBulletCount;
    public bool isReloading = false;

    [Header("Shotgun Mode Parameters")]
    [SerializeField] private float shotgunReloadTime = 3f;
    [SerializeField] private int shotgunPelletCount = 6;
    [SerializeField] private float shotgunSpreadAngle = 10f;

    [Header("Auto Mode Parameters")]
    //uhm maybe later :)

    [Header("Burst Mode Parameters")]
    [SerializeField] private int burstCount = 3;
    [SerializeField] private float burstDelay = 0.1f;

    private bool canShoot = true;

    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    private Transform playerTransform;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        currentBulletCount = maxBulletCount;
    }
    private void Update()
    {
        if (gunType == GunType.Player)
            HandlePlayerInput();

        else if (gunType == GunType.Enemy)
            HandleEnemyShooting();
    }
    private void HandlePlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
            ToggleShootingMode();

        if (Input.GetButtonDown("Fire1") && canShoot && !isReloading)
        {
            if (currentBulletCount > 0)
            {
                Shoot();
                StartCoroutine(ShootCooldown(shootCooldown));
            }
            else
            {
                StartCoroutine(Reload());
            }
        }
        if (Input.GetKeyDown(KeyCode.R) && !isReloading) StartCoroutine(Reload());
    }
    private void AimAtPlayer()
    {
        Vector2 direction = playerTransform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    private void HandleEnemyShooting()
    {
        if (canShoot && !isReloading)
        {
            Shoot();
            StartCoroutine(ShootCooldown(shootCooldown));
        }
    }
    private void Shoot()
    {
        switch (shootingMode)
        {
            case ShootingMode.Single:
                SingleMode();
                currentBulletCount--;
                break;
            case ShootingMode.Shotgun:
                ShotgunMode();
                currentBulletCount = Mathf.Max(0, currentBulletCount - shotgunPelletCount);
                break;
            case ShootingMode.Auto:
                StartCoroutine(AutoMode());
                break;
            //case ShootingMode.Burst:
            //    StartCoroutine(BurstMode());
            //    break;
        }
    }

    #region Shooting Modes
    private void SingleMode()
    {
        Vector2 direction = (gunType == GunType.Player) ? GetMouseDirection() : GetPlayerDirection();
        CreateBullet(direction);
    }
    private void ShotgunMode()
    {
        for (int i = 0; i < shotgunPelletCount; i++)
        {
            float angle = transform.eulerAngles.z - (shotgunSpreadAngle * 0.5f) + (i * (shotgunSpreadAngle / (shotgunPelletCount - 1)));
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            CreateBullet(direction);
        }
    }
    private IEnumerator AutoMode()
    {
        while (Input.GetButton("Fire1") && currentBulletCount > 0)
        {
            SingleMode();
            currentBulletCount--;
            yield return new WaitForSeconds(shootCooldown);
        }
    }
    //private IEnumerator BurstMode()
    //{
    //    canShoot = false;
    //    for (int i = 0; i < burstCount; i++)
    //    {
    //        if (currentBulletCount <= 0) break;

    //        SingleMode();
    //        currentBulletCount--;
    //        yield return new WaitForSeconds(burstDelay);
    //    }
    //    canShoot = true;
    //}
    #endregion
    private IEnumerator ShootCooldown(float cooldownTime)
    {
        canShoot = false;
        yield return new WaitForSeconds(cooldownTime);
        canShoot = true;
    }
    private IEnumerator Reload()
    {
        isReloading = true;
        canShoot = false;
        yield return new WaitForSeconds(shootingMode == ShootingMode.Shotgun ? shotgunReloadTime : reloadTime);
        currentBulletCount = maxBulletCount;
        canShoot = true;
        isReloading = false;
    }
    private Vector2 GetMouseDirection()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        return (mousePosition - transform.position).normalized;
    }
    private Vector2 GetPlayerDirection()
    {
        return (playerTransform.position - transform.position).normalized;
    }
    private void CreateBullet(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
        bulletRigidbody.linearVelocity = direction * bulletSpeed;
    }
    private void ToggleShootingMode()
    {
        shootingMode = (ShootingMode)(((int)shootingMode + 1) % System.Enum.GetValues(typeof(ShootingMode)).Length);
    }
}