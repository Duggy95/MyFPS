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

    public int ammoRemain;  //총 남은 총알
    public int magAmmo;  //현재 총알
    public int magCapacity;  //한 탄창에 들어가는 총알
    public float fireDistance;  //총알 사정거리
    public float reloadTime;  //재장전 시간
    public float timeBetFire;  //총알 발사간격
    public int damage;  //총알 데미지
    public Transform firePos;
}
