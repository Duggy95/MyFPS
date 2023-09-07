using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement1 : MonoBehaviour
{

    PlayerInput1 playerInput;
    Rigidbody rb;
    Animator anim;

    float moveSpeed = 5f;
    float jumpPower = 8f;
    bool isJumping = false;

    readonly int hashSpeed = Animator.StringToHash("Speed");
    readonly int hashWalk = Animator.StringToHash("Walk");
    readonly int hashV = Animator.StringToHash("V");
    readonly int hashH = Animator.StringToHash("H");

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput1>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    private void Update()
    {
        Jump();
        anim.SetBool(hashWalk, playerInput.walk);

        //Debug.Log("hashspeed: " + hashSpeed);
        //Debug.Log("hashWalk: " + hashWalk);
        //Debug.Log("hashV: " + hashV);
        //Debug.Log("hashH: " + hashH);
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if (playerInput.h == 0 && playerInput.v == 0)
        {
            moveSpeed = 0;
            anim.SetBool("Walk1", false);
        }
        else
        {
            anim.SetBool("Walk1", true);
            moveSpeed = 5;
        }

        if (playerInput.walk)
        {
            anim.SetFloat(hashV, playerInput.v);
            anim.SetFloat(hashH, playerInput.h);
            moveSpeed = 1;
        }

        Vector3 moveDir = (transform.forward * playerInput.v) + (transform.right * playerInput.h);
        Vector3 moveDis = moveDir.normalized * moveSpeed;
        rb.MovePosition(transform.position + moveDis * Time.deltaTime);
        anim.SetFloat(hashSpeed, moveSpeed);
    }

    public void Rotate(float angle)
    {
        /*float turn = playerInput.rotX * playerInput.rotY * rotSpeed * Time.deltaTime;
        rb.rotation = rb.rotation * Quaternion.Euler(0f, turn, 0f);*/
        this.transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    void Jump()
    {
        if (isJumping && playerInput.jump)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isJumping = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("GROUND"))
        {
            isJumping = true;
        }
    }
}
