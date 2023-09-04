using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    PlayerMovement playerMovement;
    PlayerInput playerInput;
    Transform chestSpine;
    Transform upperchestSpine;
    Animator anim;

    float eulerAngleX;
    float eulerAngleY;

    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerInput = GetComponentInParent<PlayerInput>();
        anim = GetComponentInParent<Animator>();
        chestSpine = anim.GetBoneTransform(HumanBodyBones.Chest);
    }

    private void Update()
    {
    }

    void LateUpdate()
    {
        eulerAngleY += playerInput.rotX;  //마우스 좌/우 이동으로 카메라 y축 회전
        eulerAngleX -= playerInput.rotY;  //마우스 위/아래 이동으로 카메라 x축 회전

        eulerAngleX = Mathf.Clamp(eulerAngleX, -80, 80);
        transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);

        upperchestSpine.localRotation = Quaternion.Euler(eulerAngleX+30, 0, 0);
    }

    private void FixedUpdate()
    {
        playerMovement.Rotate(eulerAngleY);
    }
}
