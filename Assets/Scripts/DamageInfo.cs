using UnityEngine;

public struct DamageInfo
{
    public DamageInfo(GameObject attacker, int damage)
    {
        this.attacker = attacker;
        this.damage = damage;
    }

    public GameObject attacker;
    public int damage;
}
