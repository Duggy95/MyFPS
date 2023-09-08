using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour, IPunObservable
{
    PhotonView pv;

    [SerializeField]
    float mouseSensitivity;  //���콺 ����.
    PlayerMovement player;
    PlayerInput playerInput;
    float eulerAngleX;  //���콺 �¿�
    float eulerAngleY;  //���콺 ���Ʒ�
    float damping = 10;
    Quaternion setAngle;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        player = GetComponentInParent<PlayerMovement>();
        playerInput = GetComponentInParent<PlayerInput>();
    }

    void LateUpdate()
    {
        eulerAngleY += playerInput.rotX * mouseSensitivity;  //���콺 �¿�
        eulerAngleX += playerInput.rotY * mouseSensitivity;  //���콺 ���Ʒ�
        eulerAngleX = Mathf.Clamp(eulerAngleX, -90, 90);  //���Ʒ� ���� 90���� ����

        if(pv.IsMine)
        {
            transform.localEulerAngles = Vector3.left * eulerAngleX;  //X������ ī�޶� ���� ����.
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
            player.Rotate(eulerAngleY);  //ī�޶��� ȸ������ �÷��̾� ���� ����
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

    public void SetCameraRotation(Vector3 newRotation)
    {
        eulerAngleX += newRotation.y;
        eulerAngleY += newRotation.x;
    }
}
