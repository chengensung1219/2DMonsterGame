using UnityEngine;

public class Interactable : MonoBehaviour
{
    public BoxCollider2D boxCollider2D;
    private bool touched = false;

    void Start()
    {

    }

    void Update()
    {
        if (touched && Input.GetKey(KeyCode.S))
        {
            boxCollider2D.isTrigger = true;
        }
        else if (!touched)
        {
            boxCollider2D.isTrigger = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            touched = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            touched = false;
        }
    }
}