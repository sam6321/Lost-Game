using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWeapon : MonoBehaviour
{
    [SerializeField]
    private float puntForce = 100.0f;

    [SerializeField]
    private int damage = 1;

    new Collider2D collider;
    List<Collider2D> collidersHit = new List<Collider2D>();
    int enemyMask;

    void Start()
    {
        collider = GetComponent<Collider2D>();
        enemyMask = LayerMask.NameToLayer("Enemy");
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(!collidersHit.Contains(collider) && collider.gameObject.layer == enemyMask && collider.isTrigger)
        {
            EnemyAI ai = collider.GetComponent<EnemyAI>();
            if (ai)
            {
                ai.Punt((ai.transform.position - transform.position).normalized * puntForce);
                Debug.Log("Send On Take Damage");
                collider.gameObject.SendMessage("OnTakeDamage", new DamageInfo(gameObject, damage));
            }

            collidersHit.Add(collider);
        }
    }

    void OnSwingStart()
    {
        collider.enabled = true;
        collidersHit.Clear();
    }

    void OnSwingStop()
    {
        collider.enabled = false;
    }
}
