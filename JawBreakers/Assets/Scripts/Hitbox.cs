using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField]
    GameObject player;

    [SerializeField]
    SpecialEffects effects;

    [SerializeField]
    float damagePercent, baseKnockback, knockbackGrowth, knockback;

    [SerializeField]
    float posXDirection, negXDirection, yDirection;

    [SerializeField]
    Vector3 angle;

    public SpecialEffects Effect { get => effects; }

    public float DamagePercent { get => damagePercent; }

    public Vector3 ApplyKnockback(float healthPercent, int weight)
    {
        knockback = (((((healthPercent / 10) + (healthPercent * damagePercent / 20)) * (200 / (weight + 100)) * 1.4f) + 18) * (knockbackGrowth / 100)) + baseKnockback;

        if (player.transform.localEulerAngles.y == 0)
        {
            angle.x = posXDirection;
        }
        if (player.transform.localEulerAngles.y == 180)
        {
            angle.x = negXDirection;
        }

        angle.y = yDirection;

        angle.y = angle.y * knockback;
        angle.x = angle.x * knockback;

        return angle;
    }
}

public enum SpecialEffects
{
    Normal,
    Caramal
}