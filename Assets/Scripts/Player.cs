using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player instance;
    public event Action OnPlayerTakedDamage;

    [Header("Player Parameters")]
    public float moveSpeed = 10;
    public int playerHealth;
    public int playerHealthMax = 60;
    private Vector2 moveInput;
    private Vector2 moveVelocity;
    public bool isFacingRight;
    
    [Header("References")]
    [SerializeField] private GameObject effect;
    private Rigidbody2D rb;
    private Animator animator;
    private void Awake()
    {
        if (instance is null) instance = this;
    }
    void Start()
    {
        playerHealth = playerHealthMax;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveVelocity * Time.deltaTime);
    }
    private void Update()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveVelocity = moveInput.normalized * moveSpeed;

        if(moveInput.x == 0) animator.SetBool("Walking", false);
        else animator.SetBool("Walking", true);

        if (moveInput.x == 0 && moveInput.y == 0) animator.SetBool("Walking", false);
        else animator.SetBool("Walking", true);
        Flip(moveInput.x);
    }
    public void PlayerTakeDamage(int damage)
    {
        playerHealth -= damage;
        OnPlayerTakedDamage?.Invoke();
        Instantiate(effect, transform.position, Quaternion.identity);

        if (playerHealth <= 0) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        else StartCoroutine(IFrames());
    }
    IEnumerator IFrames()
    {
        Physics2D.IgnoreLayerCollision(3, 7);
        yield return new WaitForSeconds(1);
        Physics2D.IgnoreLayerCollision(3, 7, false);
    }
    public void Flip(float direction)
    {
        if (direction > 0f && !isFacingRight)
        {
            isFacingRight = true;
            Vector3 scaler = transform.localScale;
            scaler.x = Mathf.Abs(scaler.x);
            transform.localScale = scaler;
        }

        else if (direction < 0f && isFacingRight)
        {
            isFacingRight = false;
            Vector3 scaler = transform.localScale;
            scaler.x = -Mathf.Abs(scaler.x);
            transform.localScale = scaler;
        }
    }
}
