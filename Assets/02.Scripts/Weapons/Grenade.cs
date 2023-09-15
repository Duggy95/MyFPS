using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviourPun
{
    Rigidbody rb;
    Weapon weapon;
    float explosionRadius = 5;  //����ź ����
    int explosionTime = 2;
    float force = 10;
    Vector3 throwDirection = new Vector3(0, 0.5f, 0);

    private void Awake()
    {
        weapon = GetComponent<Weapon>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("ThrowAdd", RpcTarget.All, force);
        }
    }

    [PunRPC]
    void ThrowAdd(float force)
    {
        Vector3 finalThrowDirection = (transform.forward + throwDirection).normalized;
        rb.AddForce(finalThrowDirection * force, ForceMode.VelocityChange);

        if (photonView.IsMine)
            photonView.RPC("ExplosionRPC", RpcTarget.MasterClient);
    }

    [PunRPC]
    void ExplosionRPC()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()  //���� ȿ��. 2�ʵڿ� ����ȿ�� ����.
    {
        yield return new WaitForSeconds(explosionTime);
        GameObject explodeEff = PhotonNetwork.Instantiate("explodePrefab", transform.position, Quaternion.identity);
        NearByApply(this.transform);
        yield return new WaitForSeconds(1);
        PhotonNetwork.Destroy(explodeEff);
        PhotonNetwork.Destroy(this.gameObject);
    }

    void NearByApply(Transform transform)
    {
        //���� �ݰ� ���� �ȿ��� �ݶ��̴� ����.
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            RaycastHit hit;
            //���� ��ġ���� �ݶ��̴� ������ ����.
            Vector3 direction = (collider.transform.position - transform.position).normalized;
            //���� ��ġ���� �ݶ��̴� ������ �Ÿ�.
            float distance = Vector3.Distance(transform.position, collider.transform.position);

            if (Physics.Raycast(transform.position, direction, out hit, explosionRadius))
            {
                IDamageable target = hit.collider.GetComponent<IDamageable>();

                if (target != null)
                {
                    //�Ÿ��� ����� ������ ���� �������� ����.
                    if (distance < explosionRadius * 0.2)
                    {
                        weapon.damage = 100;
                    }
                    else if (distance < explosionRadius * 0.4)
                    {
                        weapon.damage = 40;
                    }
                    else if (distance < explosionRadius)
                    {
                        weapon.damage = 20;
                    }
                    else
                    {
                        weapon.damage = 0;
                    }

                    target.OnDamage(weapon.damage, hit.point, hit.normal);
                }
            }
        }
    }
}
