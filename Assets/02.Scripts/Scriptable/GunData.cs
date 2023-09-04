using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/GunData", fileName = "GunData")]
public class GunData : ScriptableObject
{
    public string gunName;  //이름
    public int weaponType;  //무기 타입. {0: main, 1: sub, 2: knife, 3: grenade}
    public int damage;  //데미지
    public float distance;   //사정 거리
    public float reloadTime;  //재장전 시간
    public int magCapacity;  //탄창
    public int ammoRemain;
    public float timeBetFire;  //발사 간격 = 연사 속도
    public GameObject gunPrefab;  //프리팹 모델
    public Animator weaponAnim;
    public AudioClip shotClip;  //총소리 클립
    public AudioClip reloadClip;  //재장전 클립
}
