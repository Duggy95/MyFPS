using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : LivingEntity, IPunObservable
{
    [SerializeField]
    float walkSpeed;
    [SerializeField]
    float runSpeed;
    [SerializeField]
    float jumpForce;
    [SerializeField]
    float smoothTime;

    float damping = 10;
    bool isGround;
    PlayerInput playerInput;
    Rigidbody rb;
    WeaponCtrl weaponCtrl;

    Vector3 setPos;
    Quaternion setRot;

    public PhotonView pv;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        if (!pv.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
        }
            playerInput = GetComponent<PlayerInput>();
            rb = GetComponent<Rigidbody>();
            weaponCtrl = GetComponent<WeaponCtrl>();
    }

    void Update()
    {
        if (pv.IsMine)
            Jump();
    }

    private void FixedUpdate()
    {
        if (pv.IsMine)
        {
            Move();
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, setPos, Time.deltaTime * damping);
        }
    }

    void Move()
    {
        Vector3 moveDir = (transform.forward * playerInput.v) + (transform.right * playerInput.h);
        Vector3 moveDis = moveDir.normalized * (playerInput.walk ? walkSpeed : runSpeed);
        rb.MovePosition(transform.position + moveDis * Time.deltaTime);
    }

    public void Rotate(float angle)
    {
        this.transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    void Jump()
    {
        if (playerInput.jump && isGround)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void SetGround(bool ground)
    {
        isGround = ground;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.gameObject.CompareTag("GUN"))  //총 오브젝트를 주울 때
        {
            //주운 오브젝트에서 정보뽑고, 오브젝트 삭제.
            Weapon weapon = other.gameObject.GetComponent<Weapon>();
            weaponCtrl.GetWeapon(weapon);
            Destroy(other.gameObject, 0.01f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else if (stream.IsReading)
        {
            setPos = (Vector3)stream.ReceiveNext();
            setRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
