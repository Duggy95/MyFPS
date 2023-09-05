using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class LivingEntity : MonoBehaviourPun, IDamageable
{
    public float startingHealth = 100;
    public float health;
    public bool dead = false;
    int meleeDamage = 50;

    void Start()
    {

    }

    [PunRPC]
    public void ApplyUpdateHealth(float newHealth, bool newDead)
    {
        health = newHealth;
        dead = newDead;
    }

    protected virtual void OnEnable()
    {
        dead = false;
        health = startingHealth;
    }

    [PunRPC]
    void IDamageable.OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        health -= damage;

        photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health, dead);
        //photonView.RPC("OnDamage", RpcTarget.Others, damage, hitPoint, hitNormal);

        if (health <= 0)
        {
            print("»ç¸Á");
            //photonView.TransferOwnership(PhotonNetwork.MasterClient);
            //photonView.RPC("Die", RpcTarget.All);
            StartCoroutine(PlayerDie());
        }
    }

    [PunRPC]
    public void OnMelee(float damage)
    {
        health -= damage;

        photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health, dead);

        if(health <= 0)
        {
            print("»ç¸Á");
            StartCoroutine(PlayerDie());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("KNIFE"))
        {
            OnMelee(meleeDamage);
        }

        if(other.gameObject.CompareTag("GUN"))
        {
            Weapon weapon = other.gameObject.GetComponent<Weapon>();
            WeaponManager.instance.AddWeapon(weapon);
            Destroy(other.gameObject);
        }
    }

    IEnumerator PlayerDie()
    {
        dead = true;
        yield return null;
    }
}
