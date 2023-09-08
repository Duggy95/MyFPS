using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;  //무기 이름
    public int weaponType;  //0: 주무기, 1: 보조무기, 2: 근접무기, 3: 수류탄
    public int ammoRemain;  //총 남은 총알
    public int magAmmo;  //현재 총알
    public int magCapacity;  //한 탄창에 들어가는 총알
    public float fireDistance;  //총알 사정거리
    public float reloadTime;  //재장전 시간
    public float timeBetFire;  //총알 발사간격
    public float damage;  //총알 데미지
    public Transform firePos;  //총알 나가는 위치
    public Animator anim;  //애니메이터
    public ParticleSystem muzzleFlash;  //총구 화염 효과.
    public Material fireMaterial;  //총알 발사 머터리얼

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void FindFirePos()  //총알 나가는 위치를 찾는 메서드.
    {
        //자기 자식중에서 태그가 FIREPOS인 것을 찾음.
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

    //스크립터블 데이터를 받아서 세팅. 
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
    }

    //총을 받아서 세팅. 쓰던 총을 받기 위해
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
    }
}
