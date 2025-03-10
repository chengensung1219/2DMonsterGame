using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int maxHealth = 3;
    public Text health;
    public Animator animator;
    
    // Rigidbody2D component for physics interactions
    public Rigidbody2D rb;
    public float jumpForce = 5f;
    public bool isGrounded = true;
    private float movement;
    public float speed = 5f;
    private bool facingRight = true;
    public Transform attackPoint;
    public float attackRadius = 1f;
    
    // LayerMask to define attackable objects
    public LayerMask attackLayer;
    private bool isUpdating = true;
    
    // Reference to an enemy GameObject (used for victory condition check)
    public GameObject enemy;
    
    // Audio sources for background music and sound effects
    public AudioSource backgroundSound;
    public AudioSource attackSound;
    public AudioSource jumpSound;
    public AudioSource hurtSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backgroundSound.Play(); // Play background music at game start
    }

    // Update is called once per frame
    void Update()
    {
        if (!isUpdating)
            return; // Stop updating if the flag is set to false

        // Update health UI text
        health.text = maxHealth.ToString();

        // Get horizontal movement input (-1 to 1)
        movement = Input.GetAxis("Horizontal");

        // Flip the player's direction based on movement
        if (movement < 0f && facingRight)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
            facingRight = false;
        }
        else if (movement > 0f && !facingRight)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            facingRight = true;
        }

        // Handle jump input
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            Jump();
            isGrounded = false; // Prevent multiple jumps
            PlayJumpSound();
        }

        // Update run animation based on movement
        if (Mathf.Abs(movement) > 0.5f)
        {
            animator.SetFloat("Run", 1f);
        }
        else
        {
            animator.SetFloat("Run", 0f);
        }

        // Handle attack input
        if (Input.GetKeyDown(KeyCode.J))
        {
            animator.SetTrigger("Attack");
        }
    }

    // FixedUpdate is used for physics-based movement
    private void FixedUpdate()
    {
        transform.position += new Vector3(movement, 0f, 0f) * Time.fixedDeltaTime * speed;
    }

    // Function to handle jumping logic
    void Jump()
    {
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
    }

    // Detect collision with the ground
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // Attack function, detects and damages enemies in range
    public void Attack()
    {
        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);
        if (collInfo)
        {
            Enemy enemyScript = collInfo.gameObject.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(1); // Deal damage
                enemyScript.Hurt(); // Play hurt animation/effects
            }
        }
    }

    // Visualize attack range in Unity's Scene view
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    // Play attack sound effect
    public void PlayAttackSound()
    {
        attackSound.Play();
    }

    // Play jump sound effect
    public void PlayJumpSound()
    {
        jumpSound.Play();
    }

    // Play hurt sound effect
    public void PlayHurtSound()
    {
        hurtSound.Play();
    }

    // Reduce player's health when taking damage
    public void TakeDamage(int damage)
    {
        if (maxHealth <= 0)
        {
            return;
        }
        maxHealth -= damage;
    }

    // Handle hurt animation and check for player death
    public void Hurt()
    {
        if (maxHealth <= 0)
        {
            Die(); // Trigger death sequence
        }
        else
        {
            animator.SetTrigger("Hurt");
        }
    }

    // Handle collision with trigger objects (e.g., victory point)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (enemy == null && (other.gameObject.tag == "VictoryPoint"))
        {
            FindFirstObjectByType<SceneManagement>().LoadLevel(); // Load next level
        }
    }

    // Handle player death
    void Die()
    {
        animator.SetTrigger("Death"); // Play death animation
        FindFirstObjectByType<GameManager>().isGameActive = false; // Mark game as inactive
        Destroy(this.gameObject, 1.6f); // Destroy player object after animation
    }
}
