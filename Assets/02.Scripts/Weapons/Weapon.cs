using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int weaponType;  //{0: main, 1: sub, 2: knife, 3: grenade}
    public int ammoRemain;  //총 남은 총알
    public int magAmmo;  //현재 총알
    public int magCapacity;  //한 탄창에 들어가는 총알
    public float fireDistance;  //총알 사정거리
    public float reloadTime;  //재장전 시간
    public float timeBetFire;  //총알 발사간격
    public int damage;  //총알 데미지
    public Transform firePos;  //총알 나가는 위치
    public Animator anim;  //애니메이터

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
