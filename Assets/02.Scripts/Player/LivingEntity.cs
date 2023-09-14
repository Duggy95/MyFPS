using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class LivingEntity : MonoBehaviourPun, IDamageable
{
    Transform myTr;
    public float startingHealth = 100;
    public float health;
    public bool dead = false;
    int meleeDamage = 50;

    void Start()
    {
        myTr = transform;
    }

    [PunRPC]
    public void ApplyUpdateHealth(float newHealth, bool newDead)  //체력 적용.
    {
        health = newHealth;
        dead = newDead;
    }

    protected virtual void OnEnable()  //생성되었을 때
    {
        dead = false;
        health = startingHealth;
    }

    [PunRPC]
    void IDamageable.OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)  //데미지 입는 메서드.
    {
        Vector3 localHitPoint = transform.InverseTransformPoint(hitPoint);  //맞은 위치를 플레이어의 로컬위치로 변환

        if (localHitPoint.y >= 0.6f)  //높이가 0.6이상일 때 헤드샷으로 간주.
        {
            print("헤드샷!!");
            health -= damage * 2;
        }
        else
        {
            health -= damage;
        }

        photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health, dead);
        //photonView.RPC("OnDamage", RpcTarget.Others, damage, hitPoint, hitNormal);

        if (health <= 0)
        {
            print("사망");
            photonView.RPC("Die", RpcTarget.All);
        }
    }

    [PunRPC]
    public void OnMelee(float damage)  //근접공격 받는 메서드.
    {
        health -= damage;

        photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health, dead);

        if (health <= 0)
        {
            print("사망");
            StartCoroutine(PlayerDie());
        }
    }

    void OnTriggerEnter(Collider other)  //충돌체 인식.
    {
        if (other.gameObject.CompareTag("KNIFE"))  //근접공격이면
        {
            OnMelee(meleeDamage);
        }

        if (other.gameObject.CompareTag("GUN"))  //총 오브젝트를 주울 때
        {
            //주운 오브젝트에서 정보뽑고, 오브젝트 삭제.
            Weapon weapon = other.gameObject.GetComponent<Weapon>();
            WeaponManager.instance.GetWeapon(weapon);
            Destroy(other.gameObject, 0.1f);
        }
    }

    IEnumerator PlayerDie()
    {
        dead = true;
        yield return null;
    }
}
