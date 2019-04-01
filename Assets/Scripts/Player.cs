using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed = 10;

    private Animator playerAnimator;
    private Rigidbody2D playerRigidBody;
    private List<ContactPoint2D> contacts = new List<ContactPoint2D>();
    private Vector2 direction = new Vector2();

    private int walkParameter;

    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        direction.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        direction.Normalize();

        playerAnimator.SetBool("walking", direction.sqrMagnitude > 0.0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerRigidBody.velocity = direction * speed * Time.fixedDeltaTime;
    }
}
