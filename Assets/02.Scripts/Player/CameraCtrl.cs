using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour, IPunObservable
{
    PhotonView pv;

    [SerializeField]
    float mouseSensitivity;  //마우스 감도.
    PlayerMovement player;
    PlayerInput playerInput;
    float eulerAngleX;  //마우스 좌우
    float eulerAngleY;  //마우스 위아래
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
        eulerAngleY += playerInput.rotX * mouseSensitivity;  //마우스 좌우
        eulerAngleX += playerInput.rotY * mouseSensitivity;  //마우스 위아래
        eulerAngleX = Mathf.Clamp(eulerAngleX, -90, 90);  //위아래 각도 90도씩 제한

        if(pv.IsMine)
        {
            transform.localEulerAngles = Vector3.left * eulerAngleX;  //X축으로 카메라 각도 조정.
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
            player.Rotate(eulerAngleY);  //카메라의 회전값을 플레이어 한테 적용
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
