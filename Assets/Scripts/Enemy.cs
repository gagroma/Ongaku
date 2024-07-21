using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private enum EnemyType { Attacking, Shooting, Mage }
    [SerializeField] private EnemyType enemyType;

    [SerializeField] private int enemyHealth = 3;
    public int enemyDmg;
    [SerializeField] private float enemySpeed;
    [Space]
    private float nextFireTime;
    [Space]
    [SerializeField] private GameObject deathEffect;
    [Space]
    public float enemyAttackRange = 2f;
    public float enemyAttackCooldown = 2f;
    private bool isAttacking = false;
    private bool isCooldown = false;
    private bool canDamagePlayer = true;
    private Transform playerTransform;
    [SerializeField] private bool canMove=true;
    private bool isFacingRight = true;
    private Animator animator;
    void Start()
    {
        canMove = true;
        animator = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void FixedUpdate()
    {
        if (canMove)
        {
            EnemyMovement();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && canDamagePlayer)
        {
            if (Player.instance.playerHealth != 0)
            {
                Player.instance.PlayerTakeDamage(1);
                canDamagePlayer = false;
                canMove = false;
                Invoke(nameof(ResetDamageFlag), 1f);

            }

        }
    }
    void Update()
    {
        switch (enemyType)
        {
            case EnemyType.Attacking:
                enemyAttackRange = 1f;
                break;
            case EnemyType.Shooting:
                enemyAttackRange = 10f;
                break;
            case EnemyType.Mage:
                enemyAttackRange = 10f;
                break;
        }
        if (!isCooldown)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= enemyAttackRange)
            {
                if (!isAttacking)
                {
                    StartCoroutine(AttackCooldown());
                    EnemyAttack();
                }
            }
        }
        if (enemyHealth <= 0)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    public void EnemyTakeDamage(int damage)
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        enemyHealth -= damage;
    }
    public void EnemyAttack()
    {
        switch (enemyType)
        {
            case EnemyType.Attacking:
                //StartCoroutine(Attack());
                break;
            case EnemyType.Shooting:
                break;
            case EnemyType.Mage:
                break;
        }
    }
    private void EnemyMovement()
    {
        switch (enemyType)
        {
            case EnemyType.Attacking:
                animator.SetBool("EnemyWalking", true);
                transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, enemySpeed * Time.deltaTime);
                break;
            case EnemyType.Shooting:
                animator.SetBool("EnemyWalking", false);
                break;
            case EnemyType.Mage:
                break;
        }
        if (transform.position.x < playerTransform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (transform.position.x > playerTransform.position.x && isFacingRight)
        {
            Flip();
        }
    }
    private IEnumerator AttackCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(enemyAttackCooldown);
        isCooldown = false;
    }

    private IEnumerator Attack()
    {
        isAttacking = true;

        Instantiate(deathEffect, playerTransform.position, Quaternion.identity);
        Player.instance.PlayerTakeDamage(enemyDmg);

        yield return new WaitForSeconds(1f);

        isAttacking = false;
    }
    private void ResetDamageFlag()
    {
        canDamagePlayer = true;
        canMove = true;
    }
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}
