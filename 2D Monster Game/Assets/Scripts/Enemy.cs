using System.Runtime.InteropServices;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public int maxHealth = 5;
    private bool facingLeft = true;
    public float speed = 2f;
    public Transform checkPoint;
    public float distance = 1f;
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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        if (!isUpdating)
            return;

        if (FindFirstObjectByType<GameManager>().isGameActive == false){
            return;
        }

        if (Vector2.Distance(transform.position, player.position) <= chaseRange){
            inRange = true;
        } else {
            inRange = false;
        }

        if (inRange){
            if (player.position.x > transform.position.x && facingLeft == true){
                transform.eulerAngles = new Vector3(0, -180, 0);
                facingLeft = false;
            } else if (player.position.x < transform.position.x && facingLeft == false){
                transform.eulerAngles = new Vector3(0, 0, 0);
                facingLeft = true;
            }
            
            if (Vector2.Distance(transform.position, player.position) > attackRange){

                animator.SetBool("Attack1", false);
                transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpped * Time.deltaTime);

            } else {

                animator.SetBool("Attack1", true);

            }
        
        } else {
            transform.Translate(Vector2.left * Time.deltaTime * speed);

            RaycastHit2D hit = Physics2D.Raycast(checkPoint.position, Vector2.down, distance, layerMask);

            if (!hit && facingLeft){

                transform.eulerAngles = new Vector3(0, -180, 0);
                facingLeft = false;

            } else if (!hit && !facingLeft) {

                transform.eulerAngles = new Vector3(0, 0, 0);
                facingLeft = true;
            }
        }
        
    }

    public void Attack(){
        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);

        if (collInfo){
            if (collInfo.gameObject.GetComponent<Player>() != null){
                collInfo.gameObject.GetComponent<Player>().TakeDamage(1);
                collInfo.gameObject.GetComponent<Player>().Hurt();
            }
        }
    }

    public void Hurt(){
         if (maxHealth <= 0){
            Die();
        } else {
           animator.SetTrigger("Hurt");
        }
    }

    public void TakeDamage(int damage){
        if (maxHealth <= 0){
            return;
        }
        maxHealth -= damage;
    }

    private void OnDrawGizmosSelected()
    {
        if (checkPoint == null){
            return;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(checkPoint.position, Vector2.down * distance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    void Die(){
        isUpdating = false;
        animator.SetTrigger("Die");
        Destroy(this.gameObject, 1f);
    }
}
