using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/GunData", fileName = "GunData")]
public class GunData : ScriptableObject
{
    public string gunName;  //�̸�
    public int weaponType;  //���� Ÿ��. {0: main, 1: sub, 2: knife, 3: grenade}
    public int damage;  //������
    public int price;  //����
    public float distance;   //���� �Ÿ�
    public float reloadTime;  //������ �ð�
    public int magCapacity;  //źâ
    public int ammoRemain;
    public float timeBetFire;  //�߻� ���� = ���� �ӵ�
    public GameObject gunPrefab;  //������ ��
    public Animator weaponAnim;
    public AudioClip shotClip;  //�ѼҸ� Ŭ��
    public AudioClip reloadClip;  //������ Ŭ��
}
