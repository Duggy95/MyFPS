using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class FireCtrl : MonoBehaviourPun
{
    public enum State  //상태
    {
        Ready = 0,
        Empty,
        Reloading
    }

    public State state;  //상태
    public PhotonView pv;  //포톤 뷰
    public Animator gunAnim;  //총 애니메이션
    public Image crossHair;  //크로스헤어

    public Transform[] poses = new Transform[4]; //무기를 들고있을 위치들
    public GameObject explodePrefab;  //폭발 프리팹
    public GameObject bulletEff;
    public Weapon weapon;  //현재 무기

    WeaponManager weaponManager;  //웨폰 매니저
    LineRenderer lineRenderer;  //라인렌더러
    //LineRenderer grenadeLine;
    PlayerInput playerInput;  //입력
    Color originColor;  //크로스헤어 색
    Vector3 throwDirection = new Vector3(0, 0.5f, 0);  //던지는 방향
    float lastFireTime;  //마지막 발사
    int ammoToFill;  //채워야 할 총알 수
    //Transform throwPosition;
    //던지는 힘
    float throwPower = 10;
    float maxPower = 20;
    float chargeTime = 0;
    bool isCharge = false;
    float explosionRadius = 5;  //수류탄 범위

    

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        lineRenderer = GetComponent<LineRenderer>();
        //grenadeLine = grenadeWeapon.GetComponentInChildren<LineRenderer>();
        //throwPosition = grenadeWeapon.GetComponentInChildren<Transform>();
        weapon = GetComponentInChildren<Weapon>();
        crossHair = GameObject.FindWithTag("CROSSHAIR").GetComponent<Image>();
        //weaponManager = GameObject.FindWithTag("MANAGER").GetComponent<WeaponManager>();
        weaponManager = WeaponManager.instance;
        originColor = crossHair.color;  //크로스헤어 원래 색 저장.
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    void OnEnable()  //생성되었을 때 초기화.
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
                if (weapon.weaponType <= 1)  //현재무기가 주 무기, 보조 무기일 때
                {
                    lastFireTime = Time.time;  //마지막 발사 시간 초기화
                    Fire();  //총 발사
                }
                else if (weapon.weaponType == 2)  //현재무기가 근접 무기일 때
                {
                    lastFireTime = Time.time;  //마지막 발사 시간 초기화
                    Melee();  //근접 공격
                }
            }
        }

        if (weapon.weaponType == 3)  //현재 무기가 수류탄일 때
        {
            if (playerInput.fireDown)
            {
                //수류탄 뽑기
                PullOutPin();
            }
            if (playerInput.fire)
            {
                //뽑고 대기
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

        if (playerInput.throwGun)  //총 던지기
        {
            //총 던지고, 들고 있는거 삭제.
            weaponManager.ThrowWeapon(weapon);
            weaponManager.RemoveWeapon(weapon);
        }
    }

    void Fire()  //발사
    {
        gunAnim.SetTrigger("Shot");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  //화면에서 마우스 위치로 레이 발사

        RaycastHit hitInfo;
        Vector3 hitPosition = Vector3.zero;

        if (Physics.Raycast(ray, out hitInfo, weapon.fireDistance))
        {
            IDamageable target = hitInfo.collider.GetComponent<IDamageable>();  //맞은 놈한테서 IDamageble 가져와서

            if (target != null)  //가져오는데 성공하면 데미지 받는 함수 호출하고, 크로스헤어 색깔 바꾸기.
            {
                target.OnDamage(weapon.damage, hitInfo.point, hitInfo.normal);
                StartCoroutine(ChangeColor());
            }
            hitPosition = hitInfo.point;  //맞은 위치 저장.
        }
        else  //레이에 아무것도 안 맞았으면, 사격거리 끝을 맞은 위치로.
        {
            hitPosition = weapon.firePos.position + weapon.firePos.forward * weapon.fireDistance;
        }

        weapon.magAmmo--;
        if (weapon.magAmmo <= 0)  //총알이 떨어지면 Empty로
        {
            state = State.Empty;
        }

        pv.RPC("ShotEff", RpcTarget.All, weapon.firePos.position, hitPosition);  //발사 효과
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, hitInfo.normal);
        GameObject bulletMark = Instantiate(bulletEff, hitPosition, rot);
    }

    [PunRPC]
    IEnumerator ShotEff(Vector3 APos, Vector3 BPos)  //발사 효과
    {
        weapon.muzzleFlash.Play();
        print(weapon.muzzleFlash);
        //두 인자 사이에 라인 그리고 지우기.
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
        ammoToFill = weapon.magCapacity - weapon.magAmmo;  //채워야 할 총알은 한 탄창 빼기 현재 총알.
        if (weapon.ammoRemain < ammoToFill)  //남은 총알보다 채워야 할 총알이 많으면 채워야할 총알은 남은 총알.
        {
            ammoToFill = weapon.ammoRemain;
        }

        weapon.magAmmo += ammoToFill;
        weapon.ammoRemain -= ammoToFill;

        state = State.Ready;  //발사 가능 상태.
    }

    IEnumerator ChangeColor()  //크로스헤어 색깔 바꾸기.
    {
        crossHair.color = Color.red;

        yield return new WaitForSeconds(0.1f);
        crossHair.color = originColor;
    }

    public void Melee()  //근접 공격.
    {
        gunAnim.SetTrigger("Melee");
    }

    void PullOutPin()  //수류탄 핀 뽑기.
    {
        //핀 뽑는 사운드 재생

        isCharge = true;
        chargeTime = 0;

        //궤적 그리기
        //grenadeLine.enabled = true;
    }

    void ChargeThrow()  //핀 뽑는 상태로 대기.
    {
        chargeTime += Time.deltaTime;

        //Vector3 grenadeVelocity = (Camera.main.transform.forward + throwDirection).normalized * Mathf.Min(chargeTime * throwPower, maxPower);
        //ShowThrowLine(throwPosition.position + throwPosition.forward, grenadeVelocity);
    }

    /*void ShowThrowLine(Vector3 origin, Vector3 speed)
    {
        Vector3[] points = new Vector3[100];
        grenadeLine.positionCount = points.Length;
        for (int i = 0; i < points.Length; i++)
        {
            float time = i * 0.1f;
            points[i] = origin + speed * time + 0.5f * Physics.gravity * time * time;
        }
        grenadeLine.SetPositions(points);
    }*/

    void ReleaseThrow()  //던지기.
    {
        weapon.magAmmo--;
        ThrowGrenade(Mathf.Min(throwPower, maxPower));
        isCharge = false;

        weaponManager.RemoveWeapon(weapon);

        //라인 숨기기.
        //grenadeLine.enabled = false;
    }

    void ThrowGrenade(float force)  //복사본 생성해서 앞으로 던지고 현재 무기 삭제.
    {
        Vector3 spawnPosition = weapon.firePos.position + Camera.main.transform.forward;

        GameObject grenade = PhotonNetwork.Instantiate("Grenade", spawnPosition, Camera.main.transform.rotation);

        Rigidbody rb = grenade.gameObject.GetComponent<Rigidbody>();
        Vector3 finalThrowDirection = (Camera.main.transform.forward + throwDirection).normalized;
        rb.AddForce(finalThrowDirection * force, ForceMode.VelocityChange);
        StartCoroutine(Explosion(grenade.transform, grenade.gameObject));
    }

    IEnumerator Explosion(Transform transform, GameObject grenade)  //폭발 효과. 2초뒤에 폭발효과 생성.
    {
        yield return new WaitForSeconds(2);
        GameObject explodeEff = Instantiate(explodePrefab, transform.position, Quaternion.identity);
        Destroy(explodeEff, 2);
        Destroy(grenade);
        NearByApply(grenade.transform);
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
