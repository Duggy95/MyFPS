using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;

public class FireCtrl : MonoBehaviourPun, IPunObservable
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
    public GameObject explodePrefab;  //폭발 프리팹
    public GameObject bulletEff;  //총알 자국
    public GameObject bloodEff;  //피 효과
    public Weapon weapon;  //현재 무기
    public CameraCtrl cameraCtrl;

    Ray ray;
    WeaponCtrl weaponCtrl;  //웨폰컨트롤러
    public LineRenderer lineRenderer;  //라인렌더러
    LineRenderer grenadeLine;
    PlayerInput playerInput;  //입력
    Color originColor;  //크로스헤어 색
    Vector3 throwDirection = new Vector3(0, 0.5f, 0);  //던지는 방향
    Vector3 grenadePosition;
    float lastFireTime;  //마지막 발사
    int ammoToFill;  //채워야 할 총알 수
    //Transform throwPosition;
    //던지는 힘
    float throwPower = 10;
    float maxPower = 20;
    float chargeTime = 0;
    bool isCharge = false;

    float headHeight = 2;


    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        lineRenderer = GetComponent<LineRenderer>();
        //grenadeLine = grenadeWeapon.GetComponentInChildren<LineRenderer>();
        //throwPosition = grenadeWeapon.GetComponentInChildren<Transform>();
        weapon = GetComponentInChildren<Weapon>();
        crossHair = GameObject.FindWithTag("CROSSHAIR").GetComponent<Image>();
        weaponCtrl = GetComponent<WeaponCtrl>();
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

        if (state == State.Ready && Time.time >= lastFireTime + weapon.timeBetFire)
        {
            if (weapon.weaponType <= 1)
            {

                if (playerInput.fire)
                {
                    lastFireTime = Time.time;  //마지막 발사 시간 초기화
                    pv.RPC("FireRPC", RpcTarget.All);  //총 발사
                }
            }

            if (weapon.weaponType == 2)  //현재 무기가 근접무기일 때
            {
                lastFireTime = Time.time;
                Melee();  //근접 공격
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
        }

        if (playerInput.fireUp)
        {
            OriginCrossHair();  //크로스헤어 색깔 복구
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
            weaponCtrl.ThrowWeapon(weapon);
            weaponCtrl.RemoveWeapon(weapon);
        }
    }

    [PunRPC]
    void FireRPC()  //발사
    {
        Fire();
    }

    void Fire()
    {
        gunAnim.SetTrigger("Shot");
        if (pv.IsMine)
        {
            ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);  //화면에서 마우스 위치로 레이 발사
        }
        else
        {
            ray = new Ray(transform.position, transform.forward);
        }

        RaycastHit hitInfo;
        Vector3 hitPosition = Vector3.zero;

        if (Physics.Raycast(ray, out hitInfo, weapon.fireDistance))
        {
            IDamageable target = hitInfo.collider.GetComponent<IDamageable>();  //맞은 놈한테서 IDamageble 가져와서

            if (target != null)  //가져오는데 성공하면 데미지 받는 함수 호출하고, 크로스헤어 색깔 바꾸기.
            {
                target.OnDamage(weapon.damage, hitInfo.point, hitInfo.normal);
                Quaternion rot = Quaternion.FromToRotation(Vector3.forward, hitInfo.normal);
                GameObject blood = Instantiate(bloodEff, hitInfo.point, rot);  //피 효과 생성.
                RedCrossHair();  //크로스헤어 빨간색으로 바꾸기
            }
            else  //플레이어가 아닌 것을 쐈을 때
            {
                OriginCrossHair();  //크로스헤어 색 복구
                Quaternion rot = Quaternion.FromToRotation(Vector3.forward, hitInfo.normal);
                GameObject bulletMark = Instantiate(bulletEff, hitInfo.point, rot);  //총알 자국 생성.
            }
            hitPosition = hitInfo.point;  //맞은 위치 저장.
        }
        else  //레이에 아무것도 안 맞았으면, 사격거리 끝을 맞은 위치로.
        {
            hitPosition = weapon.firePos.position + weapon.firePos.forward * weapon.fireDistance;
            OriginCrossHair();  //크로스헤어 색 복구
        }

        weapon.magAmmo--;
        if (weapon.magAmmo <= 0)  //총알이 떨어지면 Empty로
        {
            state = State.Empty;
        }

        StartCoroutine(ShotEff(hitPosition));
        //photonView.RPC("ShotEffRPC", RpcTarget.All, hitPosition);  //발사 효과
        Recoil();
    }

    [PunRPC]
    void ShotEffRPC(Vector3 point)
    {
        StartCoroutine(ShotEff(point));
    }

    IEnumerator ShotEff(Vector3 point)  //발사 효과
    {
        weapon.muzzleFlash.Play();

        //두 인자 사이에 라인 그리고 지우기.
        lineRenderer.SetPosition(0, weapon.firePos.position);
        lineRenderer.SetPosition(1, point);
        lineRenderer.enabled = true;

        yield return new WaitForSeconds(0.016f);
        lineRenderer.enabled = false;
    }

    void Recoil()
    {
        float randomX = Random.Range(-0.5f, 0.5f);
        float randomY = Random.Range(1f, weapon.recoilY);
        cameraCtrl.SetCameraRotation(new Vector3(randomX, randomY, 0));
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

    void RedCrossHair()  //크로스헤어 색깔 바꾸기.
    {
        crossHair.color = Color.red;

        /*yield return new WaitForSeconds(0.1f);
        crossHair.color = originColor;*/
    }

    void OriginCrossHair()
    {
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
        //weapon.magAmmo--;
        ThrowGrenade(Mathf.Min(throwPower, maxPower));
        isCharge = false;

        //weaponCtrl.RemoveWeapon(weapon);

        //라인 숨기기.
        //grenadeLine.enabled = false;
    }

    void ThrowGrenade(float force)  //복사본 생성해서 앞으로 던지고 현재 무기 삭제.
    {
        Vector3 spawnPosition = weapon.firePos.position + Camera.main.transform.forward;

        GameObject grenade = PhotonNetwork.Instantiate("Grenade", spawnPosition, Camera.main.transform.rotation);
        Weapon grenadeWeapon = grenade.GetComponent<Weapon>();
        Rigidbody rb = grenade.gameObject.GetComponent<Rigidbody>();
        grenadeWeapon.ShareInfo(weapon);
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
