using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed = 10;

    private Animator playerAnimator;
    private Rigidbody2D playerRigidBody;
    private List<ContactPoint2D> contacts = new List<ContactPoint2D>();
    private Vector2 direction = new Vector2();
    private Vector2 lastMoveDirection = new Vector2();

    private class FacingDirection
    {
        public Vector2 direction;
        public int index;
    }

    private FacingDirection[] facingDirections =
    {
        new FacingDirection() { direction = new Vector2(0, -1), index = 0 },
        new FacingDirection() { direction = new Vector2(1, 0), index = 1 },
        new FacingDirection() { direction = new Vector2(-1, 0), index = 2 },
        new FacingDirection() { direction = new Vector2(0, 1), index = 3 }
    };

    public Vector2 MoveDirection
    {
        get
        {
            return direction;
        }
    }

    public Vector2 LastMoveDirection
    {
        get
        {
            return lastMoveDirection;
        }
    }

    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
    }

    void OnDisable()
    {
        playerAnimator.SetBool("walking", false);
    }

    void Update()
    {
        direction.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        direction.Normalize();

        bool isMoving = direction.sqrMagnitude > 0.0f;
        if(isMoving)
        {
            int index = GetFacingIndex(direction);
            playerAnimator.SetInteger("direction", index);
            lastMoveDirection = direction;
        }

        playerAnimator.SetBool("walking", isMoving);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerRigidBody.AddForce(direction * speed * Time.fixedDeltaTime);
    }

    public void Punt(Vector2 velocity)
    {
        playerRigidBody.AddForce(velocity);
    }

    public int GetFacingIndex(Vector2 direction)
    {
        float bestDot = Vector2.Dot(direction, facingDirections[0].direction);
        FacingDirection bestDirection = facingDirections[0];
        for (int i = 1; i < facingDirections.Length; i++)
        {
            float dot = Vector2.Dot(direction, facingDirections[i].direction);
            if (dot > bestDot)
            {
                bestDot = dot;
                bestDirection = facingDirections[i];
            }
        }
        return bestDirection.index;
    }
}
