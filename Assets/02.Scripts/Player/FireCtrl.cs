using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireCtrl : MonoBehaviourPun
{
    public enum State  //상태
    {
        Ready = 0,
        Empty,
        Reloading
    }

    public PhotonView pv;

    public Animator gunAnim;
    LineRenderer lineRenderer;  //라인렌더러
    LineRenderer grenadeLine;
    PlayerInput playerInput;  //입력
    public State state;  //상태
    public Image crossHair;  //크로스헤어
    Color originColor;
    float lastFireTime;  //마지막 발사
    int ammoToFill;  //채워야 할 총알 수
    public Transform meleePos;
    Transform throwPosition;
    float throwPower = 10;
    float maxPower = 20;
    float chargeTime = 0;
    bool isCharge = false;
    float explosionRadius = 5;

    Vector3 throwDirection = new Vector3(0, 0.5f, 0);
    public Transform[] poses = new Transform[4]; 

    public GameObject explodePrefab;

    public Weapon weapon;
    public GameObject grenadePrefab;
    WeaponManager weaponManager;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        lineRenderer = GetComponent<LineRenderer>();
        //grenadeLine = grenadeWeapon.GetComponentInChildren<LineRenderer>();
        //throwPosition = grenadeWeapon.GetComponentInChildren<Transform>();
        weapon = GetComponentInChildren<Weapon>();
        crossHair = GameObject.Find("Crosshair").GetComponent<Image>();
        //weaponManager = GameObject.FindWithTag("MANAGER").GetComponent<WeaponManager>();
        weaponManager = WeaponManager.instance;
        originColor = crossHair.color;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    void OnEnable()
    {
        state = State.Ready;
        lastFireTime = 0;
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (playerInput.fire)
        {
            if (state == State.Ready && Time.time >= lastFireTime + weapon.timeBetFire)  //발사가능 상태일 때
            {
                if (weapon.weaponType <= 1)
                {
                    lastFireTime = Time.time;  //마지막 발사 시간 초기화
                    //pv.RPC("Fire", RpcTarget.MasterClient);
                    Fire();
                }
                else if (weapon.weaponType == 2)
                {
                    lastFireTime = Time.time;
                    Melee();
                }
            }
        }

        if (weapon.weaponType == 3 && weapon.magAmmo >= 1)
        {
            if (playerInput.fireDown)
            {
                //수류탄 뽑기
                PullOutPin();
            }
            if (playerInput.fire)
            {
                ChargeThrow();
            }
            if (playerInput.fireUp)
            {
                //수류탄 던지기
                ReleaseThrow();
            }
        }

        if (state != State.Reloading && weapon.ammoRemain > 0 &&
            weapon.magAmmo != weapon.magCapacity)  //장전중이 아닐 때
        {
            if (playerInput.reload)
                StartCoroutine(Reload());  //장전 코루틴 호출
        }

        if(playerInput.throwGun)
        {
            weaponManager.ThrowWeapon(weapon);
            weaponManager.RemoveWeapon(weapon);
        }
    }

    void Fire()  //발사
    {
        gunAnim.SetTrigger("Shot");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;
        Vector3 hitPosition = Vector3.zero;

        if (Physics.Raycast(ray, out hitInfo, weapon.fireDistance))
        {
            IDamageable target = hitInfo.collider.GetComponent<IDamageable>();

            if (target != null)
            {
                target.OnDamage(weapon.damage, hitInfo.point, hitInfo.normal);
                StartCoroutine(HitEff());
            }
            hitPosition = hitInfo.point;
        }
        else
        {
            hitPosition = weapon.firePos.position + weapon.firePos.forward * weapon.fireDistance;
        }

        weapon.magAmmo--;
        if (weapon.magAmmo <= 0)
        {
            state = State.Empty;
        }

        pv.RPC("ShotEff", RpcTarget.All, weapon.firePos.position, hitPosition);
    }

    [PunRPC]
    IEnumerator ShotEff(Vector3 APos, Vector3 BPos)  //발사 효과
    {
        lineRenderer.SetPosition(0, APos);
        lineRenderer.SetPosition(1, BPos);
        lineRenderer.enabled = true;

        yield return new WaitForSeconds(0.016f);
        lineRenderer.enabled = false;
    }

    IEnumerator Reload()  //재장전
    {
        state = State.Reloading;
        gunAnim.SetTrigger("Reload");

        yield return new WaitForSeconds(weapon.reloadTime);
        ammoToFill = weapon.magCapacity - weapon.magAmmo;
        if (weapon.ammoRemain < ammoToFill)
        {
            ammoToFill = weapon.ammoRemain;
        }

        weapon.magAmmo += ammoToFill;
        weapon.ammoRemain -= ammoToFill;

        state = State.Ready;
    }

    IEnumerator HitEff()
    {
        crossHair.color = Color.red;

        yield return new WaitForSeconds(0.1f);
        crossHair.color = originColor;
    }

    public void Melee()
    {
        gunAnim.SetTrigger("Melee");
    }

    void PullOutPin()
    {
        //핀 뽑는 사운드 재생

        isCharge = true;
        chargeTime = 0;

        //궤적 그리기
        //grenadeLine.enabled = true;
    }

    void ChargeThrow()
    {
        chargeTime += Time.deltaTime;

        //Vector3 grenadeVelocity = (Camera.main.transform.forward + throwDirection).normalized * Mathf.Min(chargeTime * throwPower, maxPower);
        //ShowThrowLine(throwPosition.position + throwPosition.forward, grenadeVelocity);
    }

    void ShowThrowLine(Vector3 origin, Vector3 speed)
    {
        Vector3[] points = new Vector3[100];
        grenadeLine.positionCount = points.Length;
        for (int i = 0; i < points.Length; i++)
        {
            float time = i * 0.1f;
            points[i] = origin + speed * time + 0.5f * Physics.gravity * time * time;
        }
        //grenadeLine.SetPositions(points);
    }

    void ReleaseThrow()
    {
        weapon.magAmmo--;
        ThrowGrenade(Mathf.Min(throwPower, maxPower));
        isCharge = false;

        weaponManager.RemoveWeapon(weapon);

        //라인 숨기기.
        //grenadeLine.enabled = false;
    }

    void ThrowGrenade(float force)
    {
        Vector3 spawnPosition = weapon.firePos.position + Camera.main.transform.forward;

        GameObject grenade = PhotonNetwork.Instantiate("Grenade", spawnPosition, Camera.main.transform.rotation);

        Rigidbody rb = grenade.gameObject.GetComponent<Rigidbody>();
        Vector3 finalThrowDirection = (Camera.main.transform.forward + throwDirection).normalized;
        rb.AddForce(finalThrowDirection * force, ForceMode.VelocityChange);
        StartCoroutine(Explosion(grenade.transform, grenade.gameObject));
    }

    IEnumerator Explosion(Transform transform, GameObject grenade)
    {
        yield return new WaitForSeconds(2);
        GameObject explodeEff = Instantiate(explodePrefab, transform.position, Quaternion.identity);
        Destroy(explodeEff, 2);
        Destroy(grenade);
        NearByApply(grenade.transform);
    }

    void NearByApply(Transform transform)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            RaycastHit hit;
            Vector3 direction = (collider.transform.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, collider.transform.position);

            if (Physics.Raycast(transform.position, direction, out hit, explosionRadius))
            {
                IDamageable target = hit.collider.GetComponent<IDamageable>();
                if(distance < explosionRadius * 0.2)
                {
                    weapon.damage = 100;
                }
                else if(distance < explosionRadius * 0.4)
                {
                    weapon.damage = 40;
                }
                else if(distance < explosionRadius)
                {
                    weapon.damage = 20;
                }
                else
                {
                    weapon.damage = 0;
                }

                if (target != null)
                {
                    target.OnDamage(weapon.damage, hit.point, hit.normal);
                    print(distance + " 데미지 : " + weapon.damage);
                }
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(weapon.ammoRemain);
            stream.SendNext(weapon.magAmmo);
            stream.SendNext(state);
        }
        else
        {
            weapon.ammoRemain = (int)stream.ReceiveNext();
            weapon.magAmmo = (int)stream.ReceiveNext();
            state = (State)stream.ReceiveNext();
        }
    }
}
