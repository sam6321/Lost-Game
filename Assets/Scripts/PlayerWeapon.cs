using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public enum AttackDirectionSource
    {
        MOUSE,
        PLAYER
    }

    [SerializeField]
    private float attackDelay = 0.25f;

    [SerializeField]
    private AttackDirectionSource attackDirectionSource = AttackDirectionSource.MOUSE;

    private Transform weaponParent;
    private Transform sword;
    private Animator weaponAnimator;

    private PlayerMovement movement;
    private bool canAttack = true;

    private class AttackDirection
    {
        public Vector2 direction;
        public Vector2 offset;
        public float rotation;
        public float xScale;
    }

    private AttackDirection[] attackDirections =
    {
        new AttackDirection()
        {
            direction = new Vector2(-1, 0),
            offset = new Vector2(-0.42f, -0.57f),
            rotation = 0,
            xScale = -1
        },
        new AttackDirection()
        {
            direction = new Vector2(1, 0),
            offset = new Vector2(0.42f, -0.57f),
            rotation = 0,
            xScale = 1
        },
        new AttackDirection()
        {
            direction = new Vector2(0, 1),
            offset = new Vector2(0, 0.25f),
            rotation = 45,
            xScale = 1
        },
        new AttackDirection()
        {
            direction = new Vector2(0, -1),
            offset = new Vector2(0, -0.75f),
            rotation = -135,
            xScale = 1
        },
    };

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        weaponParent = transform.Find("WeaponParent");
        sword = weaponParent.transform.Find("Sword");
        weaponAnimator = weaponParent.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1") && canAttack)
        {
            // Do attack
            AttackDirection direction = GetAttackDirection();

            // Begin the swing animation and move the weapon onto the correct side of the player
            // The weapon itself will perform damage effects when it collides with an enemy.
            weaponAnimator.SetTrigger("Swing");
            weaponParent.localScale = new Vector3(direction.xScale, 1, 1);
            weaponParent.localEulerAngles = new Vector3(0, 0, direction.rotation);
            sword.position = (Vector2)transform.position + direction.offset;

            StartCoroutine(ResetNextAttackTime());
        }
    }

    IEnumerator ResetNextAttackTime()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }
    
    private AttackDirection GetAttackDirection()
    {
        Vector2 direction;
        switch(attackDirectionSource)
        {
            default:
            case AttackDirectionSource.MOUSE: // Get direction from screen
                Vector2 screenPoint = Input.mousePosition;
                direction = (Camera.main.ScreenToWorldPoint(screenPoint) - transform.position).normalized;
                break;

            case AttackDirectionSource.PLAYER: // Get direction from player
                direction = movement.LastMoveDirection;
                break;
        }

        float bestDot = Vector2.Dot(direction, attackDirections[0].direction);
        AttackDirection bestDirection = attackDirections[0];
        for(int i = 1; i < attackDirections.Length; i++)
        {
            float dot = Vector2.Dot(direction, attackDirections[i].direction);
            if (dot > bestDot)
            {
                bestDot = dot;
                bestDirection = attackDirections[i];
            }
        }
        return bestDirection;


    }
}
