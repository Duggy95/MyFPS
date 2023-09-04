using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubWeapon : WeaponBase
{
    public void SetWeapon(GunData gunData)
    {
        ammoRemain = gunData.ammoRemain;
        magCapacity = gunData.magCapacity;
        magAmmo = magCapacity;
        fireDistance = gunData.distance;
        reloadTime = gunData.reloadTime;
        timeBetFire = gunData.timeBetFire;
        damage = gunData.damage;
        firePos = GameObject.FindWithTag("FIREPOS").GetComponent<Transform>();
    }
}
