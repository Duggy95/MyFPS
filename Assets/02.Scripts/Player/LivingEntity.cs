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

    [PunRPC]
    public void ApplyUpdateHealth(float newHealth, bool newDead)  //ü�� ����.
    {
        health = newHealth;
        dead = newDead;

        if (health <= 0)
        {
            print("���");
            Die();
        }
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

    protected virtual void OnTriggerEnter(Collider other)  //�浹ü �ν�.
    {
        if (other.gameObject.CompareTag("KNIFE"))  //���������̸�
        {
            OnMelee(meleeDamage);
        }
    }

    void Die()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    IEnumerator PlayerDie()
    {
        dead = true;
        yield return null;
    }
}
