using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IDamageable
{
    void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal);

    void OnMelee(float damage);
}
