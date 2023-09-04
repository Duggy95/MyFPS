using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Net;
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
    LineRenderer trajectoryLine;
    PlayerInput playerInput;  //입력
    public State state;  //상태
    public Image crossHair;  //크로스헤어
    Color originColor;
    float lastFireTime;  //마지막 발사
    int ammoToFill;  //채워야 할 총알 수
    public Transform meleePos;
    //Rigidbody rb;
    float throwPower = 10;
    float maxPower = 20;
    float chargeTime = 0; 
    bool isCharge = false;

    Vector3 throwDirection = new Vector3(0, 1, 0);

    public GameObject mainWeapon;
    public GameObject subWeapon;
    public GameObject knifeWeapon;
    public GameObject grenadeWeapon;

    public Weapon weapon;
    public GameObject grenadePrefab;
    WeaponManager weaponManager;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        lineRenderer = GetComponent<LineRenderer>();
        //gunAnim = GetComponentInChildren<Animator>();
        weapon = GetComponentInChildren<Weapon>();
        crossHair = GameObject.Find("Crosshair").GetComponent<Image>();
        weaponManager = GameObject.Find("WeaponManager").GetComponent<WeaponManager>();
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
                /*else if (weapon.weaponType == 3)
                {
                    lastFireTime = Time.time;
                    ReleaseGrenade();
                }*/
            }
        }

        if(playerInput.fireDown)
        {
            if(weapon.weaponType == 3)
            {
                //수류탄 뽑기
                PullOutPin();
            }
        }

        if(playerInput.fireUp)
        {
            if(weapon.weaponType == 3)
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

        /*if (weaponManager.currWeapon != null)
        {
            weapon = weaponManager.currWeapon;
            gunAnim = weapon.anim;
        }*/
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
        trajectoryLine.enabled = true;
    }

    void ChargeThrow()
    {
        chargeTime += Time.deltaTime;
    }

    void ReleaseThrow()
    {
        ThrowGrenade(Mathf.Min(throwPower, maxPower));

        //라인 숨기기.
    }

    void ThrowGrenade(float force)
    {
        Vector3 spawnPosition = weapon.firePos.position + Camera.main.transform.forward;

        GameObject grenade = PhotonNetwork.Instantiate("grenadePrefab", spawnPosition, Camera.main.transform.rotation);

        Rigidbody rb = weapon.gameObject.GetComponent<Rigidbody>();
        Vector3 finalThrowDirection = (Camera.main.transform.forward + throwDirection).normalized;
        rb.isKinematic = false;
        rb.AddForce(finalThrowDirection * force, ForceMode.VelocityChange);
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
