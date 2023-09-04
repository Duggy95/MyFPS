using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.Cockpit;

public class WeaponManager : MonoBehaviourPun
{
    public GunData[] datas = new GunData[4];  //¹«±â 
    public Weapon[] weapons = new Weapon[4];
    PlayerInput playerInput;
    FireCtrl fireCtrl;
    public TestPhoton testPhoton;
    public Weapon currWeapon;

    private void Awake()
    {

    }

    void Update()
    {
        if(playerInput != null)
        {
            switch(playerInput.weapon)
            {
                case 0:
                    SwitchWeapon(0);
                    //photonView.RPC("SwitchWeapon", RpcTarget.All, weapons[1]);
                    photonView.RPC("SwitchWeapon", RpcTarget.All, 0);
                    break;
                case 1:
                    SwitchWeapon(1);
                    //photonView.RPC("SwitchWeapon", RpcTarget.All, weapons[1]);
                    photonView.RPC("SwitchWeapon", RpcTarget.All, 1);
                    break;
                case 2:
                    SwitchWeapon(2);
                    //photonView.RPC("SwitchWeapon", RpcTarget.All, weapons[1]);
                    photonView.RPC("SwitchWeapon", RpcTarget.All, 2);
                    break;
                case 3:
                    SwitchWeapon(3);
                    //photonView.RPC("SwitchWeapon", RpcTarget.All, weapons[1]);
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
        weapons = playerInput.gameObject.GetComponentsInChildren<Weapon>();
        weapons[0].SetWeapon(datas[0]);
        weapons[0].FindFirePos();
        weapons[1].SetWeapon(datas[1]);
        weapons[1].FindFirePos();
        weapons[2].SetWeapon(datas[2]);
        weapons[3].SetWeapon(datas[3]);
        weapons[3].FindFirePos();
        SwitchWeapon(1);
        photonView.RPC("SwitchWeapon", RpcTarget.Others, 1);
    }

    [PunRPC]
    void SwitchWeapon(int weaponIndex)
    {
        for(int i = 0; i < weapons.Length; i++)
        {
            bool isActive = (i == weaponIndex) ? true : false;
            weapons[i].gameObject.SetActive(isActive);
        }

        currWeapon = weapons[weaponIndex];
        fireCtrl.weapon = currWeapon;
        fireCtrl.gunAnim = currWeapon.anim;
    }

    public void StateCheck(Weapon currWeapon)
    {
        if(currWeapon.weaponType <= 1)
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
}
