using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviourPun
{
    Rigidbody rb;
    Weapon weapon;
    float explosionRadius = 5;  //수류탄 범위
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

    IEnumerator Explosion()  //폭발 효과. 2초뒤에 폭발효과 생성.
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
        //폭발 반경 범위 안에서 콜라이더 감지.
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            RaycastHit hit;
            //폭발 위치에서 콜라이더 까지의 방향.
            Vector3 direction = (collider.transform.position - transform.position).normalized;
            //폭발 위치에서 콜라이더 까지의 거리.
            float distance = Vector3.Distance(transform.position, collider.transform.position);

            if (Physics.Raycast(transform.position, direction, out hit, explosionRadius))
            {
                IDamageable target = hit.collider.GetComponent<IDamageable>();

                if (target != null)
                {
                    //거리가 가까운 비율에 따라 데미지가 차등.
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
