using System.Runtime.InteropServices;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 5;
    private bool facingLeft = true;
    public float speed = 2f;
    public Transform checkPoint;
    public float distance = 1f;

    // Layer mask to check for ground
    public LayerMask layerMask;
    public bool inRange = false;
    public Transform player;
    public float chaseRange = 10f;
    public float attackRange = 2.5f;
    public float chaseSpped = 4f;
    public Animator animator;
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask attackLayer;
    private bool isUpdating = true;
    
    // Called once before the first frame update
    void Start()
    {

    }

    // Called once per frame to update the enemy's behavior
    void Update()
    {   
        // If the enemy is not supposed to update, exit early
        if (!isUpdating)
            return;

        // Check if the game is active before processing behavior
        if (FindFirstObjectByType<GameManager>().isGameActive == false){
            return;
        }

        // Determine if the player is within chase range
        if (Vector2.Distance(transform.position, player.position) <= chaseRange){
            inRange = true;
        } else {
            inRange = false;
        }

        // Behavior when the player is within chase range
        if (inRange){
            // Flip enemy direction to face the player
            if (player.position.x > transform.position.x && facingLeft == true){
                transform.eulerAngles = new Vector3(0, -180, 0);
                facingLeft = false;
            } else if (player.position.x < transform.position.x && facingLeft == false){
                transform.eulerAngles = new Vector3(0, 0, 0);
                facingLeft = true;
            }
            
            // Move towards the player if outside attack range
            if (Vector2.Distance(transform.position, player.position) > attackRange){
                animator.SetBool("Attack1", false);
                transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpped * Time.deltaTime);
            } else {
                // Attack the player if within attack range
                animator.SetBool("Attack1", true);
            }
        
        } else {
            // Enemy patrol movement when not chasing the player
            transform.Translate(Vector2.left * Time.deltaTime * speed);

            // Check for ground ahead using raycast
            RaycastHit2D hit = Physics2D.Raycast(checkPoint.position, Vector2.down, distance, layerMask);

            // Flip direction if there is no ground ahead
            if (!hit && facingLeft){
                transform.eulerAngles = new Vector3(0, -180, 0);
                facingLeft = false;
            } else if (!hit && !facingLeft) {
                transform.eulerAngles = new Vector3(0, 0, 0);
                facingLeft = true;
            }
        }
    }

    // Handles attack logic
    public void Attack(){
        // Detects if the player is within attack range
        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);

        if (collInfo){
            // If the target is a player, apply damage and trigger hurt animation
            if (collInfo.gameObject.GetComponent<Player>() != null){
                collInfo.gameObject.GetComponent<Player>().TakeDamage(1);
                collInfo.gameObject.GetComponent<Player>().Hurt();
            }
        }
    }

    // Handles enemy taking damage and playing hurt animation
    public void Hurt(){
        if (maxHealth <= 0){
            Die();
        } else {
            animator.SetTrigger("Hurt");
        }
    }

    // Reduces enemy health when taking damage
    public void TakeDamage(int damage){
        if (maxHealth <= 0){
            return;
        }
        maxHealth -= damage;
    }

    // Visualizes detection and attack ranges in the Unity Editor
    private void OnDrawGizmosSelected()
    {
        if (checkPoint == null){
            return;
        }
        // Draw yellow ray for ground detection
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(checkPoint.position, Vector2.down * distance);

        // Draw blue wire sphere for chase range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        if (attackPoint == null) return;
        // Draw red wire sphere for attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    // Handles enemy death behavior
    void Die(){
        // Stop updating behavior after death
        isUpdating = false;

        // Play death animation
        animator.SetTrigger("Die");

        // Destroy the enemy game object after 1 second
        Destroy(this.gameObject, 1f);
    }
}
