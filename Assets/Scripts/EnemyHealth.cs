using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private int health = 2;

    private AIDestinationSetter setter;
    private BoardCreator creator;
    private Rigidbody2D rb2d;
    private Collider2D[] colliders;

    void Start()
    {
        setter = GetComponent<AIDestinationSetter>();
        rb2d = GetComponent<Rigidbody2D>();
        colliders = GetComponents<Collider2D>();
        creator = GameObject.Find("GameManager").GetComponent<BoardCreator>();
    }

    void OnTakeDamage(DamageInfo info)
    {
        Debug.Log("Enemy hit " + health);
        health -= info.damage;
        if(health <= 0)
        {
            setter.target = null;
            Destroy(GetComponent<EnemyAI>());
            Destroy(GetComponent<AIPath>());
            Destroy(GetComponent<Seeker>());
            foreach(Collider2D collider in colliders)
            {
                Destroy(collider);
            }
            Destroy(setter);

            rb2d.AddForce((transform.position - info.attacker.transform.position).normalized * 1000);

            creator.OnEnemyKilled();

            FadeDestroy destroy = gameObject.AddComponent<FadeDestroy>();
            destroy.FadeTime = Time.time + 1;
        }
    }
}
