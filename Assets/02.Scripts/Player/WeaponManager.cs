using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.Cockpit;
using Unity.VisualScripting;

public class WeaponManager : MonoBehaviourPun
{
    private static WeaponManager _instance;
    public GunData[] datas = new GunData[4];  //무기 데이터들
    public Weapon[] weapons = new Weapon[4];  //무기들
    public Transform[] poses = new Transform[4];  //내가 가지고 있는 무기들
    PlayerInput playerInput;
    FireCtrl fireCtrl;
    public TestPhoton testPhoton;
    public Weapon currWeapon;
    Vector3 throwDirection = new Vector3(0, 1, 0);

    public static WeaponManager instance        //WeaponManager타입의 변수 instance는 아래 코드대로 _instance를 반환하는 스태틱 매서드 역할.
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                _instance = FindObjectOfType<WeaponManager>();
            }
            // 싱글톤 오브젝트를 반환
            return _instance;
        }
    }


    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)   //주소값으로 비교하는것
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (playerInput != null && playerInput.weapon != 10)
        {
            switch (playerInput.weapon)
            {
                case 0:
                    SwitchWeapon(0);
                    photonView.RPC("SwitchWeapon", RpcTarget.All, 0);
                    break;
                case 1:
                    SwitchWeapon(1);
                    photonView.RPC("SwitchWeapon", RpcTarget.All, 1);
                    break;
                case 2:
                    SwitchWeapon(2);
                    photonView.RPC("SwitchWeapon", RpcTarget.All, 2);
                    break;
                case 3:
                    SwitchWeapon(3);
                    photonView.RPC("SwitchWeapon", RpcTarget.All, 3);
                    break;
            }

            StateCheck(currWeapon);
        }
    }

    public void PlayerFind()
    {
        playerInput = testPhoton.player.GetComponent<PlayerInput>();
        fireCtrl = testPhoton.player.GetComponent<FireCtrl>();
        //fireCtrl = playerInput.gameObject.GetComponent<FireCtrl>();
        //weapons = playerInput.gameObject.GetComponentsInChildren<Weapon>();
        poses = fireCtrl.poses;
        for (int i = 0; i < poses.Length; i++)
        {
            weapons[i] = poses[i].GetComponentInChildren<Weapon>();
        }

        for (int i = 0; i < 4; i++)
        {
            Setting(i);
        }

        SwitchWeapon(1);
        photonView.RPC("SwitchWeapon", RpcTarget.Others, 1);
    }

    void Setting(int i)
    {
        weapons[i].SetWeapon(datas[i]);
        weapons[i].FindFirePos();
    }

    [PunRPC]
    void SwitchWeapon(int weaponIndex)
    {
        if (weapons[weaponIndex] == null)
        {
            return;
        }

        for (int i = 0; i < weapons.Length; i++)
        {
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
        currWeapon = weapons[weaponIndex];
        fireCtrl.weapon = currWeapon;
        fireCtrl.gunAnim = currWeapon.anim;
    }

    public void StateCheck(Weapon currWeapon)
    {
        if (currWeapon.weaponType <= 1)
        {
            if (currWeapon.magAmmo <= 0)
            {
                fireCtrl.state = FireCtrl.State.Empty;
            }
            else if (currWeapon.magAmmo >= 1 && fireCtrl.state != FireCtrl.State.Reloading)
            {
                fireCtrl.state = FireCtrl.State.Ready;
            }
        }
        else
        {
            fireCtrl.state = FireCtrl.State.Ready;
        }
    }

    public void AddWeapon(Weapon weapon)
    {
        if (weapon == null)
        {
            foreach (GunData gunData in datas)
            {
                weapons[weapon.weaponType].SetWeapon(gunData);
            }
        }
        else
        {
            GameObject gun = PhotonNetwork.Instantiate("HeavyRifle", Vector3.zero, 
                                                                                                         Quaternion.identity);
            Weapon _weapon = gun.GetComponent<Weapon>();
            gun.transform.SetParent(poses[weapon.weaponType]);
            weapons[weapon.weaponType] = weapon;
            fireCtrl.gunAnim = _weapon.anim;
            SwitchWeapon(weapon.weaponType);
        }
    }

    public void RemoveWeapon(Weapon weapon)
    {
        if (weapon.weaponType == 0 || weapon.weaponType == 3)
        {
            Destroy(weapons[weapon.weaponType].gameObject);
            weapons[weapon.weaponType] = null;

            SwitchWeapon(1);
        }
        else
            return;
    }

    public void ThrowWeapon(Weapon weapon)
    {
        if (weapon.weaponType != 0)
            return;

        Vector3 spawnPosition = weapon.transform.position + Camera.main.transform.forward;
        Vector3 spawnRotation = fireCtrl.transform.rotation.eulerAngles + new Vector3(0, -90, -90);

        print(spawnPosition);
        GameObject _weaponDrop = PhotonNetwork.Instantiate("HeavyRifle_Drop", spawnPosition, Quaternion.Euler(spawnRotation));
        Weapon _weapon = _weaponDrop.GetComponent<Weapon>();
        _weapon.anim.enabled = false;
        _weapon.ShareAmmo(weapon);

        Rigidbody rb = _weapon.gameObject.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        Vector3 ThrowDirection = (Camera.main.transform.forward + throwDirection).normalized;
        rb.AddForce(ThrowDirection * 3, ForceMode.VelocityChange);
    }
}
