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
    public void ApplyUpdateHealth(float newHealth, bool newDead)  //ü�� ����.
    {
        health = newHealth;
        dead = newDead;
    }

    protected virtual void OnEnable()  //�����Ǿ��� ��
    {
        dead = false;
        health = startingHealth;
    }

    [PunRPC]
    void IDamageable.OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)  //������ �Դ� �޼���.
    {
        Vector3 localHitPoint = transform.InverseTransformPoint(hitPoint);  //���� ��ġ�� �÷��̾��� ������ġ�� ��ȯ

        if (localHitPoint.y >= 0.6f)  //���̰� 0.6�̻��� �� ��弦���� ����.
        {
            print("��弦!!");
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
            print("���");
            photonView.RPC("Die", RpcTarget.All);
        }
    }

    [PunRPC]
    public void OnMelee(float damage)  //�������� �޴� �޼���.
    {
        health -= damage;

        photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health, dead);

        if (health <= 0)
        {
            print("���");
            StartCoroutine(PlayerDie());
        }
    }

    void OnTriggerEnter(Collider other)  //�浹ü �ν�.
    {
        if (other.gameObject.CompareTag("KNIFE"))  //���������̸�
        {
            OnMelee(meleeDamage);
        }

        if (other.gameObject.CompareTag("GUN"))  //�� ������Ʈ�� �ֿ� ��
        {
            //�ֿ� ������Ʈ���� �����̰�, ������Ʈ ����.
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
