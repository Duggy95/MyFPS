using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraCtrl : MonoBehaviour, IPunObservable
{
    PhotonView pv;

    [SerializeField]
    float mouseSensitivity;
    TestPlayerMovement player;
    PlayerInput playerInput;
    float eulerAngleX;
    float eulerAngleY;
    float damping = 10;
    Quaternion setAngle;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        player = GetComponentInParent<TestPlayerMovement>();
        playerInput = GetComponentInParent<PlayerInput>();
    }

    void Update()
    {

    }

    void LateUpdate()
    {
        eulerAngleY += playerInput.rotX * mouseSensitivity;  //���콺 �¿�
        eulerAngleX += playerInput.rotY * mouseSensitivity;  //���콺 ���Ʒ�
        eulerAngleX = Mathf.Clamp(eulerAngleX, -90, 90);  //���Ʒ� ���� 90���� ����

        if(pv.IsMine)
        {
            transform.localEulerAngles = Vector3.left * eulerAngleX;
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, setAngle, Time.deltaTime * damping);
        }

    }

    void FixedUpdate()
    {
        if(pv.IsMine)
        {
            player.Rotate(eulerAngleY);
        } 
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.rotation);
        }
        else if(stream.IsReading)
        {
            setAngle = (Quaternion)stream.ReceiveNext();
        }
    }
}
