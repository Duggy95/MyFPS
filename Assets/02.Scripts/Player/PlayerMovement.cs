using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
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

    Vector3 setPos;
    Quaternion setRot;

    Animator animator;

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

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(playerInput.h == 0 && playerInput.v == 0)
        {
            animator.SetBool("Walk1", false);
        }
        else
        {
            animator.SetBool("Walk1", true);
        }


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
