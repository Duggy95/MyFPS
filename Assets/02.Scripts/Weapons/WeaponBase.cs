using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    main = 0,
    sub,
    knife,
    grenade
}

public class WeaponBase : MonoBehaviour
{
    [SerializeField]    
    WeaponType weaponType;

    public int ammoRemain;  //�� ���� �Ѿ�
    public int magAmmo;  //���� �Ѿ�
    public int magCapacity;  //�� źâ�� ���� �Ѿ�
    public float fireDistance;  //�Ѿ� �����Ÿ�
    public float reloadTime;  //������ �ð�
    public float timeBetFire;  //�Ѿ� �߻簣��
    public int damage;  //�Ѿ� ������
    public Transform firePos;
}
