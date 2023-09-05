using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int weaponType;  //{0: main, 1: sub, 2: knife, 3: grenade}
    public int ammoRemain;  //�� ���� �Ѿ�
    public int magAmmo;  //���� �Ѿ�
    public int magCapacity;  //�� źâ�� ���� �Ѿ�
    public float fireDistance;  //�Ѿ� �����Ÿ�
    public float reloadTime;  //������ �ð�
    public float timeBetFire;  //�Ѿ� �߻簣��
    public float damage;  //�Ѿ� ������
    public Transform firePos;  //�Ѿ� ������ ��ġ
    public Animator anim;  //�ִϸ�����

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void FindFirePos()
    {
        Transform[] childs = transform.GetComponentsInChildren<Transform>();
        foreach(Transform child in childs)
        {
            if(child.CompareTag("FIREPOS"))
            {
                firePos = child;
            }
        }
    }

    public void SetWeapon(GunData gunData)
    {
        this.ammoRemain = gunData.ammoRemain;
        this.magCapacity = gunData.magCapacity;
        if (weaponType != 3)
            this.magAmmo = this.magCapacity;
        else
        {
            this.magAmmo = this.magCapacity;
            this.magCapacity = 0;
        }
        this.fireDistance = gunData.distance;
        this.reloadTime = gunData.reloadTime;
        this.timeBetFire = gunData.timeBetFire;
        this.damage = gunData.damage;
        this.weaponType = gunData.weaponType;
    }

    public void ShareAmmo(Weapon weapon)
    {
        this.ammoRemain = weapon.ammoRemain;
        this.magCapacity = weapon.magCapacity;
        this.magAmmo = weapon.magAmmo;
        this.magCapacity = weapon.magCapacity;
        this.fireDistance = weapon.fireDistance;
        this.reloadTime = weapon.reloadTime;
        this.timeBetFire = weapon.timeBetFire;
        this.damage = weapon.damage;
        this.weaponType = weapon.weaponType;
    }
}
