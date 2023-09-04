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
    public int damage;  //�Ѿ� ������
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
        this.magAmmo = this.magCapacity;
        this.fireDistance = gunData.distance;
        this.reloadTime = gunData.reloadTime;
        this.timeBetFire = gunData.timeBetFire;
        this.damage = gunData.damage;
        this.weaponType = gunData.weaponType;
    }
}
