using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInput : MonoBehaviourPun
{
    string moveAxisH = "Horizontal";  // 키보드 좌우 입력 (A, D) 
    string moveAxisV = "Vertical";  // 키보드 상하 입력 (W, S)
    string rotAxisX = "Mouse X";  // 마우스 좌우 이동
    string rotAxisY = "Mouse Y";  // 마우스 상하 이동
    string fireBtn = "Fire1";  // 발사 입력 (마우스 왼쪽)
    string reloadBtn = "Reload";  // 장전 (R)
    string jumpBtn = "Jump";  // 점프 (Space)
    string walkBtn = "Walk";  // 걷기 (LShift)
    string sitBtn = "Sit";  // 앉기 (LCtrl)
    string throwGunBtn = "ThrowGun";  // 총 버리기 (G)
    string mainGunBtn = "MainGun";  // 주총 (1)
    string subGunBtn = "SubGun";  // 권총 (2)
    string knifeBtn = "Knife";  // 칼 (3)
    string grenadeBtn = "Grenade";  // 수류탄 (4)
    string aimBtn = "Aim";  // 에임 (마우스 오른쪽)
    string escBtn = "Cancel";  // 취소 (Esc) 

    public float h {  get; private set; }
    public float v { get; private set; }
    public float rotX { get; private set; }
    public float rotY { get; private set; }
    public bool fire { get; private set; }
    public bool fireDown { get; private set; }
    public bool fireUp { get; private set; }
    public bool reload { get; private set; }
    public bool jump { get; private set; }
    public bool walk { get; private set; }
    public bool sit { get; private set; }
    public bool throwGun { get; private set; }
    public bool mainGun { get; private set; }
    public bool subGun { get; private set; }
    public bool knife { get; private set; }
    public bool grenade { get; private set; }
    public bool aim { get; private set; }
    public bool esc { get; private set; }
    public int weapon { get; private set; }

    private void Update()
    {
        if(!photonView.IsMine)
        {
            return;
        }
        else
        {
            h = Input.GetAxisRaw(moveAxisH);
            v = Input.GetAxisRaw(moveAxisV);
            rotX = Input.GetAxisRaw(rotAxisX);
            rotY = Input.GetAxisRaw(rotAxisY);
            fire = Input.GetButton(fireBtn);
            fireDown = Input.GetButtonDown(fireBtn);
            fireUp = Input.GetButtonUp(fireBtn);
            reload = Input.GetButtonDown(reloadBtn);
            jump = Input.GetButtonDown(jumpBtn);
            walk = Input.GetButton(walkBtn);
            sit = Input.GetButton(sitBtn);
            throwGun = Input.GetButtonDown(throwGunBtn);
            mainGun = Input.GetButtonDown(mainGunBtn);
            subGun = Input.GetButtonDown(subGunBtn);
            knife = Input.GetButtonDown(knifeBtn);
            grenade = Input.GetButtonDown(grenadeBtn);
            aim = Input.GetButtonDown(aimBtn);
            esc = Input.GetButtonDown(escBtn);

            if (mainGun)
                weapon = 0;
            else if (subGun)
                weapon = 1;
            else if (knife)
                weapon = 2;
            else if (grenade)
                weapon = 3;
            else  //아무것도 아닐 때
                weapon = 10;
        }
    }
}
