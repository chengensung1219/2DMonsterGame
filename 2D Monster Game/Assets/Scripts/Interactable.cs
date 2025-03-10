using UnityEngine;

// This script allows an object to become passable when the player presses 'S' while touching it.
public class Interactable : MonoBehaviour
{
    public BoxCollider2D boxCollider2D;
    private bool touched = false;

    void Start()
    {
    
    }

    void Update()
    {
        // If the player is touching the object and presses 'S', make the collider a trigger (passable)
        if (touched && Input.GetKey(KeyCode.S))
        {
            boxCollider2D.isTrigger = true;
        }
        // Otherwise, if the object is not being touched, ensure the collider is solid
        else if (!touched)
        {
            boxCollider2D.isTrigger = false;
        }
    }

    // Detect when the player collides with the object
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If the colliding object is tagged as "Player", set touched to true
        if (collision.gameObject.CompareTag("Player"))
        {
            touched = true;
        }
    }

    // Detect when the player exits the trigger area
    private void OnTriggerExit2D(Collider2D collision)
    {
        // If the object leaving the trigger is the player, set touched to false
        if (collision.gameObject.CompareTag("Player"))
        {
            touched = false;
        }
    }
}
