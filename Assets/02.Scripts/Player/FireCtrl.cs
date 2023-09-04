using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class FireCtrl : MonoBehaviourPun
{
    public enum State  //����
    {
        Ready = 0,
        Empty,
        Reloading
    }

    public PhotonView pv;

    public Animator gunAnim;
    LineRenderer lineRenderer;  //���η�����
    LineRenderer trajectoryLine;
    PlayerInput playerInput;  //�Է�
    public State state;  //����
    public Image crossHair;  //ũ�ν����
    Color originColor;
    float lastFireTime;  //������ �߻�
    int ammoToFill;  //ä���� �� �Ѿ� ��
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
            if (state == State.Ready && Time.time >= lastFireTime + weapon.timeBetFire)  //�߻簡�� ������ ��
            {
                if (weapon.weaponType <= 1)
                {
                    lastFireTime = Time.time;  //������ �߻� �ð� �ʱ�ȭ
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
                //����ź �̱�
                PullOutPin();
            }
        }

        if(playerInput.fireUp)
        {
            if(weapon.weaponType == 3)
            {
                //����ź ������
                ReleaseThrow();
            }
        }

        if (state != State.Reloading && weapon.ammoRemain > 0 &&
            weapon.magAmmo != weapon.magCapacity)  //�������� �ƴ� ��
        {
            if (playerInput.reload)
                StartCoroutine(Reload());  //���� �ڷ�ƾ ȣ��
        }

        /*if (weaponManager.currWeapon != null)
        {
            weapon = weaponManager.currWeapon;
            gunAnim = weapon.anim;
        }*/
    }

    void Fire()  //�߻�
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
    IEnumerator ShotEff(Vector3 APos, Vector3 BPos)  //�߻� ȿ��
    {
        lineRenderer.SetPosition(0, APos);
        lineRenderer.SetPosition(1, BPos);
        lineRenderer.enabled = true;

        yield return new WaitForSeconds(0.016f);
        lineRenderer.enabled = false;
    }

    IEnumerator Reload()  //������
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
        //�� �̴� ���� ���

        isCharge = true;
        chargeTime = 0;

        //���� �׸���
        trajectoryLine.enabled = true;
    }

    void ChargeThrow()
    {
        chargeTime += Time.deltaTime;
    }

    void ReleaseThrow()
    {
        ThrowGrenade(Mathf.Min(throwPower, maxPower));

        //���� �����.
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
