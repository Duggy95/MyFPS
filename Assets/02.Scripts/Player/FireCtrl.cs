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
    public enum State  //����
    {
        Ready = 0,
        Empty,
        Reloading
    }

    public State state;  //����
    public PhotonView pv;  //���� ��
    public Animator gunAnim;  //�� �ִϸ��̼�
    public Image crossHair;  //ũ�ν����
    public GameObject explodePrefab;  //���� ������
    public GameObject bulletEff;  //�Ѿ� �ڱ�
    public GameObject bloodEff;  //�� ȿ��
    public Weapon weapon;  //���� ����
    public CameraCtrl cameraCtrl;

    Ray ray;
    public LineRenderer lineRenderer;  //���η�����
    LineRenderer grenadeLine;
    PlayerInput playerInput;  //�Է�
    Color originColor;  //ũ�ν���� ��
    Vector3 grenadePosition;
    float lastFireTime;  //������ �߻�
    int ammoToFill;  //ä���� �� �Ѿ� ��
    //Transform throwPosition;
    //������ ��
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
        originColor = crossHair.color;  //ũ�ν���� ���� �� ����.
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    void OnEnable()  //�����Ǿ��� �� �ʱ�ȭ.
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
                    lastFireTime = Time.time;  //������ �߻� �ð� �ʱ�ȭ
                    pv.RPC("FireRPC", RpcTarget.All);  //�� �߻�
                }
            }

            if (weapon.weaponType == 2)  //���� ���Ⱑ ���������� ��
            {
                lastFireTime = Time.time;
                Melee();  //���� ����
            }

            if (weapon.weaponType == 3)  //���� ���Ⱑ ����ź�� ��
            {
                if (playerInput.fireDown)
                {
                    //����ź �̱�
                    PullOutPin();
                }
                if (playerInput.fire)
                {
                    //�̰� ���
                    ChargeThrow();
                }
                if (playerInput.fireUp)
                {
                    //����ź ������
                    ReleaseThrow();
                }
            }
        }

        if (playerInput.fireUp)
        {
            OriginCrossHair();  //ũ�ν���� ���� ����
        }

        if (state != State.Reloading && weapon.ammoRemain > 0 &&
            weapon.magAmmo != weapon.magCapacity)  //�������� �ƴ� ��
        {
            if (playerInput.reload)
                StartCoroutine(Reload());  //���� �ڷ�ƾ ȣ��
        }
    }

    [PunRPC]
    void FireRPC()  //�߻�
    {
        Fire();
    }

    void Fire()
    {
        gunAnim.SetTrigger("Shot");
        if (pv.IsMine)
        {
            ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);  //ȭ�鿡�� ���콺 ��ġ�� ���� �߻�
        }
        else
        {
            ray = new Ray(transform.position, transform.forward);
        }

        RaycastHit hitInfo;
        Vector3 hitPosition = Vector3.zero;

        if (Physics.Raycast(ray, out hitInfo, weapon.fireDistance))
        {
            IDamageable target = hitInfo.collider.GetComponent<IDamageable>();  //���� �����׼� IDamageble �����ͼ�

            if (target != null)  //�������µ� �����ϸ� ������ �޴� �Լ� ȣ���ϰ�, ũ�ν���� ���� �ٲٱ�.
            {
                target.OnDamage(weapon.damage, hitInfo.point, hitInfo.normal);
                Quaternion rot = Quaternion.FromToRotation(Vector3.forward, hitInfo.normal);
                GameObject blood = Instantiate(bloodEff, hitInfo.point, rot);  //�� ȿ�� ����.
                RedCrossHair();  //ũ�ν���� ���������� �ٲٱ�
            }
            else  //�÷��̾ �ƴ� ���� ���� ��
            {
                OriginCrossHair();  //ũ�ν���� �� ����
                Quaternion rot = Quaternion.FromToRotation(Vector3.forward, hitInfo.normal);
                GameObject bulletMark = Instantiate(bulletEff, hitInfo.point, rot);  //�Ѿ� �ڱ� ����.
            }
            hitPosition = hitInfo.point;  //���� ��ġ ����.
        }
        else  //���̿� �ƹ��͵� �� �¾�����, ��ݰŸ� ���� ���� ��ġ��.
        {
            hitPosition = weapon.firePos.position + weapon.firePos.forward * weapon.fireDistance;
            OriginCrossHair();  //ũ�ν���� �� ����
        }

        weapon.magAmmo--;
        if (weapon.magAmmo <= 0)  //�Ѿ��� �������� Empty��
        {
            state = State.Empty;
        }

        StartCoroutine(ShotEff(hitPosition));
        //photonView.RPC("ShotEffRPC", RpcTarget.All, hitPosition);  //�߻� ȿ��
        Recoil();
    }

    [PunRPC]
    void ShotEffRPC(Vector3 point)
    {
        StartCoroutine(ShotEff(point));
    }

    IEnumerator ShotEff(Vector3 point)  //�߻� ȿ��
    {
        weapon.muzzleFlash.Play();

        //�� ���� ���̿� ���� �׸��� �����.
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

    IEnumerator Reload()  //������
    {
        state = State.Reloading;
        gunAnim.SetTrigger("Reload");

        yield return new WaitForSeconds(weapon.reloadTime);
        ammoToFill = weapon.magCapacity - weapon.magAmmo;  //ä���� �� �Ѿ��� �� źâ ���� ���� �Ѿ�.
        if (weapon.ammoRemain < ammoToFill)  //���� �Ѿ˺��� ä���� �� �Ѿ��� ������ ä������ �Ѿ��� ���� �Ѿ�.
        {
            ammoToFill = weapon.ammoRemain;
        }

        weapon.magAmmo += ammoToFill;
        weapon.ammoRemain -= ammoToFill;

        state = State.Ready;  //�߻� ���� ����.
    }

    void RedCrossHair()  //ũ�ν���� ���� �ٲٱ�.
    {
        crossHair.color = Color.red;

        /*yield return new WaitForSeconds(0.1f);
        crossHair.color = originColor;*/
    }

    void OriginCrossHair()
    {
        crossHair.color = originColor;
    }

    public void Melee()  //���� ����.
    {
        gunAnim.SetTrigger("Melee");
    }

    void PullOutPin()  //����ź �� �̱�.
    {
        //�� �̴� ���� ���

        isCharge = true;
        chargeTime = 0;

        //���� �׸���
        //grenadeLine.enabled = true;
    }

    void ChargeThrow()  //�� �̴� ���·� ���.
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

    void ReleaseThrow()  //������.
    {
        //weapon.magAmmo--;
        ThrowGrenade(Mathf.Min(throwPower, maxPower));
        isCharge = false;

        //weaponCtrl.RemoveWeapon(weapon);

        //���� �����.
        //grenadeLine.enabled = false;
    }

    void ThrowGrenade(float force)  //���纻 �����ؼ� ������ ������ ���� ���� ����.
    {
        Vector3 spawnPosition = weapon.firePos.position + Camera.main.transform.forward;

        GameObject grenade = PhotonNetwork.Instantiate("Grenade", spawnPosition, Camera.main.transform.rotation);
        Weapon grenadeWeapon = grenade.GetComponent<Weapon>();
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
