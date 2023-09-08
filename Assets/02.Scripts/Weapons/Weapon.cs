using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;  //���� �̸�
    public int weaponType;  //0: �ֹ���, 1: ��������, 2: ��������, 3: ����ź
    public int ammoRemain;  //�� ���� �Ѿ�
    public int magAmmo;  //���� �Ѿ�
    public int magCapacity;  //�� źâ�� ���� �Ѿ�
    public float fireDistance;  //�Ѿ� �����Ÿ�
    public float reloadTime;  //������ �ð�
    public float timeBetFire;  //�Ѿ� �߻簣��
    public float damage;  //�Ѿ� ������
    public float recoilY;  //�� �����ݵ�.
    public Transform firePos;  //�Ѿ� ������ ��ġ
    public Animator anim;  //�ִϸ�����
    public ParticleSystem muzzleFlash;  //�ѱ� ȭ�� ȿ��.
    public Material fireMaterial;  //�Ѿ� �߻� ���͸���

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void FindFirePos()  //�Ѿ� ������ ��ġ�� ã�� �޼���.
    {
        //�ڱ� �ڽ��߿��� �±װ� FIREPOS�� ���� ã��.
        Transform[] childs = transform.GetComponentsInChildren<Transform>();
        foreach (Transform child in childs)
        {
            if (child.CompareTag("FIREPOS"))
            {
                firePos = child;
                muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
            }
        }
    }

    //��ũ���ͺ� �����͸� �޾Ƽ� ����. 
    public void SetWeapon(GunData gunData)
    {
        this.weaponName = gunData.gunName;
        this.ammoRemain = gunData.ammoRemain;
        this.magCapacity = gunData.magCapacity;
        this.magAmmo = this.magCapacity;
        this.fireDistance = gunData.distance;
        this.reloadTime = gunData.reloadTime;
        this.timeBetFire = gunData.timeBetFire;
        this.damage = gunData.damage;
        this.weaponType = gunData.weaponType;
        this.fireMaterial = gunData.fireMaterial;
        this.recoilY = gunData.recoilY;
    }

    //���� �޾Ƽ� ����. ���� ���� �ޱ� ����
    public void ShareInfo(Weapon weapon)
    {
        this.weaponName = weapon.weaponName;
        this.ammoRemain = weapon.ammoRemain;
        this.magCapacity = weapon.magCapacity;
        this.magAmmo = weapon.magAmmo;
        this.magCapacity = weapon.magCapacity;
        this.fireDistance = weapon.fireDistance;
        this.reloadTime = weapon.reloadTime;
        this.timeBetFire = weapon.timeBetFire;
        this.damage = weapon.damage;
        this.weaponType = weapon.weaponType;
        this.fireMaterial = weapon.fireMaterial;
        this.recoilY = weapon.recoilY;
    }
}
