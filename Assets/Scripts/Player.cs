using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player instance;
    public event Action OnPlayerTakedDamage;

    [Header("Player Parameters")]
    public float moveSpeed = 10f;
    public int playerHealth;
    public int playerHealthMax = 60;
    private Vector2 moveInput;
    private Vector2 moveVelocity;
    public bool isFacingRight = true;

    [Header("References")]
    [SerializeField] private GameObject effect;
    private Rigidbody2D rb;
    private Animator animator;

    private void Awake()
    {
        if (instance is null)
            instance = this;
    }
    private void Start()
    {
        playerHealth = playerHealthMax;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        HandleMovement();
        HandleAnimation();
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveVelocity * Time.deltaTime);
    }
    private void HandleMovement()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveVelocity = moveInput.normalized * moveSpeed;
        //Implement dash SOMETIME;))))
        Flip(moveInput.x);
    }
    private void HandleAnimation()
    {
        bool isWalking = moveInput != Vector2.zero;
        animator.SetBool("Walking", isWalking);
    }
    public void PlayerTakeDamage(int damage)
    {
        playerHealth -= damage;
        OnPlayerTakedDamage?.Invoke();
        Instantiate(effect, transform.position, Quaternion.identity);

        if (playerHealth <= 0) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        else StartCoroutine(IFrames(1f));
    }
    private IEnumerator IFrames(float iFrameTime)
    {
        Physics2D.IgnoreLayerCollision(3, 7, true);
        yield return new WaitForSeconds(iFrameTime);
        Physics2D.IgnoreLayerCollision(3, 7, false);
    }
    private void Flip(float direction)
    {
        if ((direction > 0f && !isFacingRight) || (direction < 0f && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 scaler = transform.localScale;
            scaler.x *= -1;
            transform.localScale = scaler;
        }
    }
}