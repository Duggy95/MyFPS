using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInput : MonoBehaviourPun
{
    string moveAxisH = "Horizontal";  // Ű���� �¿� �Է� (A, D) 
    string moveAxisV = "Vertical";  // Ű���� ���� �Է� (W, S)
    string rotAxisX = "Mouse X";  // ���콺 �¿� �̵�
    string rotAxisY = "Mouse Y";  // ���콺 ���� �̵�
    string fireBtn = "Fire1";  // �߻� �Է� (���콺 ����)
    string reloadBtn = "Reload";  // ���� (R)
    string jumpBtn = "Jump";  // ���� (Space)
    string walkBtn = "Walk";  // �ȱ� (LShift)
    string sitBtn = "Sit";  // �ɱ� (LCtrl)
    string throwGunBtn = "ThrowGun";  // �� ������ (G)
    string mainGunBtn = "MainGun";  // ���� (1)
    string subGunBtn = "SubGun";  // ���� (2)
    string knifeBtn = "Knife";  // Į (3)
    string grenadeBtn = "Grenade";  // ����ź (4)
    string aimBtn = "Aim";  // ���� (���콺 ������)
    string escBtn = "Cancel";  // ��� (Esc) 

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
            else  //�ƹ��͵� �ƴ� ��
                weapon = 10;
        }
    }
}
