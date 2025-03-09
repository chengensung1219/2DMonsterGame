using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int maxHealth = 3;
    public Text health;
    public Animator animator;
    public Rigidbody2D rb;
    public float jumpForce = 5f;
    public bool isGrounded = true;
    private float movement;
    public float speed = 5f;
    private bool facingRight = true;
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask attackLayer;
    private bool isUpdating = true;
    public GameObject enemy;
    public AudioSource backgroundSound;
    public AudioSource attackSound;
    public AudioSource jumpSound;
    public AudioSource hurtSound;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backgroundSound.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isUpdating)
            return;

        health.text = maxHealth.ToString();

        movement = Input.GetAxis("Horizontal");

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

        if (Input.GetKey(KeyCode.Space) && isGrounded){
            Jump();
            isGrounded = false;
            PlayJumpSound();
        }

        if (Mathf.Abs(movement) > 0.5f){
            animator.SetFloat("Run", 1f);

        } else {
            animator.SetFloat("Run", 0f);
        }

        if (Input.GetKeyDown(KeyCode.J)){
            animator.SetTrigger("Attack");

        }
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(movement, 0f, 0f) * Time.fixedDeltaTime * speed;
    }

    void Jump(){
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    public void Attack(){
        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);
        if (collInfo){
            if (collInfo.gameObject.GetComponent<Enemy>() != null){
                collInfo.gameObject.GetComponent<Enemy>().TakeDamage(1);
                collInfo.gameObject.GetComponent<Enemy>().Hurt();
            }
        }
    }
    private void OnDrawGizmosSelected(){

        if (attackPoint == null){
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
    public void PlayAttackSound()
    {
        attackSound.Play();
    }

    public void PlayJumpSound(){
        jumpSound.Play();
    }

    public void PlayHurtSound(){
        hurtSound.Play();
    }

    public void TakeDamage(int damage){
        if (maxHealth <= 0){
            return;
        }
        maxHealth -= damage;
    }

    public void Hurt(){
         if (maxHealth <= 0){
            Die();
        } else {
           animator.SetTrigger("Hurt");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (enemy == null && (other.gameObject.tag == "VictoryPoint"))
        {
            FindFirstObjectByType<SceneManagement>().LoadLevel();
        }
    }

    void Die(){
        animator.SetTrigger("Death");
        FindFirstObjectByType<GameManager>().isGameActive = false;
        Destroy(this.gameObject, 1.6f);

    }
}
