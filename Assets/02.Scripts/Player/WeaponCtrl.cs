using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCtrl : MonoBehaviourPun
{
    public GunData[] datas = new GunData[4];  //���� �����͵�
    public Weapon[] weapons = new Weapon[4];  //���� ������ �ִ� �����. 0 = �ֹ���, 1 = ��������, 2 = ������, 3 = ����ź.
    public Transform[] poses = new Transform[4];  //���� ���⸦ ������ �� ��ġ��. 0 = �ֹ���, 1 = ��������, 2 = ������, 3 = ����ź
    PlayerInput playerInput;  //�÷��̾� �Է�
    FireCtrl fireCtrl;  //�÷��̾ ������ �ִ� �߻���
    public TestPhoton testPhoton;
    public Weapon currWeapon;  //���� ����
    Vector3 throwDirection = new Vector3(0, 1, 0);  //������ ����

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        fireCtrl = GetComponent<FireCtrl>();
        for (int i = 0; i < poses.Length; i++)
        {
            //���� ��ġ���� �ڽĿ� ����κ��� ���Ⱑ���ͼ� �迭�� �־���. ���⸦ �ȵ�� ������ null�� �� ����.
            weapons[i] = poses[i].GetComponentInChildren<Weapon>();
        }

        Setting();  //�ʱ� ���Ⱚ ����.
        SwitchWeapon(0);  //�������� Ȱ��ȭ.
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput.mainGun)
        {
            Switch(0);
            SwichRPC(0);
            Debug.Log("1�� ����");
        }
        else if (playerInput.subGun)
        {
            Switch(1);
            SwichRPC(1);
            Debug.Log("2�� ����");
        }
        else if (playerInput.knife)
        {
            Switch(2);
            SwichRPC(2);
            Debug.Log("3�� ����");
        }
        else if (playerInput.grenade)
        {
            Switch(3);
            SwichRPC(3);
            Debug.Log("4�� ����");
        }
       
    }

    public void Switch(int i)
    {
        photonView.RPC("SwichRPC", RpcTarget.All, i);
        /*if (!photonView.IsMine)
        {
            return;
        }

        switch (i)  //�÷��̾� �Է��� �޾Ƽ� 
        {
            //���� �Է¿� ���� ���⸦ �ٲ�.
            case 0:
                SwitchWeapon(0);
                //pv.RPC("SwitchWeapon", RpcTarget.Others, 0);
                break;
            case 1:
                SwitchWeapon(1);
                //pv.RPC("SwitchWeapon", RpcTarget.Others, 1);
                break;
            case 2:
                SwitchWeapon(2);
                //pv.RPC("SwitchWeapon", RpcTarget.Others, 2);
                break;
            case 3:
                SwitchWeapon(3);
                //pv.RPC("SwitchWeapon", RpcTarget.Others, 3);
                break;
        }*/
    }

    [PunRPC]
    void SwichRPC(int i)
    {
        switch (i)  //�÷��̾� �Է��� �޾Ƽ� 
        {
            //���� �Է¿� ���� ���⸦ �ٲ�.
            case 0:
                SwitchWeapon(0);
                //pv.RPC("SwitchWeapon", RpcTarget.Others, 0);
                break;
            case 1:
                SwitchWeapon(1);
                //pv.RPC("SwitchWeapon", RpcTarget.Others, 1);
                break;
            case 2:
                SwitchWeapon(2);
                //pv.RPC("SwitchWeapon", RpcTarget.Others, 2);
                break;
            case 3:
                SwitchWeapon(3);
                //pv.RPC("SwitchWeapon", RpcTarget.Others, 3);
                break;
        }

        StateCheck(currWeapon);
    }

    void Setting()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == null)  //���Ⱑ null�� �� ����
            {
                continue;
            }
            else  //������ ������ �ް� �߻� ��ġ ã��.
            {
                weapons[i].SetWeapon(datas[i]);
                weapons[i].FindFirePos();
            }
        }
    }

    void SwitchWeapon(int weaponIndex)  //���� ���� �ٲٴ� �޼���.
    {
        if (weapons[weaponIndex] == null)
        {
            return;
        }

        for (int i = 0; i < weapons.Length; i++)
        {
            //�μ��� ���� ����ܿ��� ���� ��Ȱ��ȭ. ���� ���Ⱑ null�̸� ��Ȱ��ȭ.
            if (weapons[i] != null)
            {
                bool isActive = (i == weaponIndex) ? true : false;
                poses[i].gameObject.SetActive(isActive);
            }
            else
            {
                poses[i].gameObject.SetActive(false);
            }
        }

        //���繫�� ����. fireCtrl�� ����.
        currWeapon = weapons[weaponIndex];
        fireCtrl.weapon = currWeapon;
        fireCtrl.gunAnim = currWeapon.anim;
        fireCtrl.lineRenderer.material = currWeapon.fireMaterial;
        StateCheck(currWeapon);
    }

    public void StateCheck(Weapon currWeapon)  //���� ���� üũ �޼���.
    {
        if (currWeapon.weaponType <= 1)  //�� ������ ��
        {
            if (currWeapon.magAmmo <= 0)  //�Ѿ��� ������ Empty. => �߻� �Ұ���
            {
                fireCtrl.state = FireCtrl.State.Empty;
            }
            else if (currWeapon.magAmmo >= 1 && fireCtrl.state != FireCtrl.State.Reloading)  //�Ѿ��� �ְ�, �������� �ƴ϶�� ���ݰ��� ����.
            {
                fireCtrl.state = FireCtrl.State.Ready;
            }
        }
        else  //�� ���� �ƴϸ� �׻� ���ݰ��� ����
        {
            fireCtrl.state = FireCtrl.State.Ready;
        }
    }

    public void GetWeapon(Weapon weapon)  //���⸦ �ݴ� �޼���.
    {
        //���� �����ؼ� pos�� �ڽ����� �־���.
        GameObject gun = PhotonNetwork.Instantiate(weapon.weaponName, Vector3.zero, Quaternion.identity);
        gun.transform.SetParent(poses[weapon.weaponType]);
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localEulerAngles = Vector3.zero;

        //���� ������ �ѿ��ٰ� �ֿ� ���� �������� �־���.
        Weapon _weapon = gun.GetComponent<Weapon>();
        _weapon.ShareInfo(weapon);
        _weapon.FindFirePos();
        weapons[weapon.weaponType] = _weapon;  //���� �迭�� ���� �־���.

        SwitchWeapon(weapon.weaponType);  //�ֿ� ���� ���� ������.
    }

    public void BuyWeapon(GunData gunData)
    {
        GameObject gun = PhotonNetwork.Instantiate(gunData.gunName, Vector3.zero, Quaternion.identity);

        gun.transform.SetParent(poses[gunData.weaponType]);
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localEulerAngles = Vector3.zero;

        Weapon _weapon = gun.GetComponent<Weapon>();
        _weapon.SetWeapon(gunData);
        _weapon.FindFirePos();
        weapons[_weapon.weaponType] = _weapon;

        SwitchWeapon(_weapon.weaponType);
    }

    public void RemoveWeapon(Weapon weapon)  //���⸦ ����� �޼���.
    {
        if (weapon.weaponType == 0 || weapon.weaponType == 3)  //�� ����, ����ź���� ��.
        {
            //���� ������Ʈ �����ϰ�, ���� ���⸦ ��������
            Destroy(weapons[weapon.weaponType].gameObject);
            weapons[weapon.weaponType] = null;

            SwitchWeapon(1);
        }
        else
            return;
    }

    public void ThrowWeapon(Weapon weapon)  //���� ������.(������)
    {
        if (weapon.weaponType != 0)  //�� ���⸸ ����.
            return;

        //���� ��ġ�� ȸ����
        Vector3 spawnPosition = weapon.transform.position + Camera.main.transform.forward;
        Vector3 spawnRotation = fireCtrl.transform.rotation.eulerAngles + new Vector3(0, -90, -90);

        //������ ���� ������Ʈ ����. 
        GameObject _weaponDrop = PhotonNetwork.Instantiate(weapon.weaponName + "_Drop", spawnPosition, Quaternion.Euler(spawnRotation));
        //���� ������ ������ ���� ���⿡ ������.
        Weapon _weapon = _weaponDrop.GetComponent<Weapon>();
        _weapon.anim.enabled = false;
        _weapon.ShareInfo(weapon);

        //������ٵ� �߰��ؼ� ������ ����.
        Rigidbody rb = _weapon.gameObject.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        Vector3 ThrowDirection = (Camera.main.transform.forward + throwDirection).normalized;
        rb.AddForce(ThrowDirection * 3, ForceMode.VelocityChange);
    }
}