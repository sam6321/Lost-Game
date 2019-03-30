using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed = 10;

    private BoxCollider2D collider;
    private Rigidbody2D characterRigidBody;
    private List<ContactPoint2D> contacts = new List<ContactPoint2D>();

    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        characterRigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        UnpenetrateContacts();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        characterRigidBody.velocity = direction * speed * Time.fixedDeltaTime;
    }

    void UnpenetrateContacts()
    {
        contacts.Clear();
        Vector2 movement = new Vector2(0, 0);
        characterRigidBody.GetContacts(contacts);
        foreach (ContactPoint2D contact in contacts)
        {
            bool hasMovedX = movement.x != 0;
            bool hasMovedY = movement.y != 0;

            if (hasMovedX && contact.normal.x != 0)
            {
                movement.x = contact.normal.x; ;
            }

            if (hasMovedY && contact.normal.y != 0)
            {
                movement.y = contact.normal.y;
            }

            if (hasMovedX && hasMovedY)
            {
                break;
            }
        }

        if(movement.sqrMagnitude > 0)
        {
            Debug.Log("Moving");
        }

        characterRigidBody.position += movement;
    }
}
