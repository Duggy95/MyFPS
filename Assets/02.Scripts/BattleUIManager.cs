using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    public Text ammoTxt;
    public Text HPTxt;
    FireCtrl fireCtrl;
    TestPlayerMovement player;
    WeaponManager weaponManager;

    void Start()
    {
        weaponManager = GameObject.Find("WeaponManager").GetComponent<WeaponManager>();
    }

    void Update()
    {
        if(player != null)
        {
            UIUpdate();
        }
    }

    void UIUpdate()
    {
        HPTxt.text = "HP : " + player.health;
        ammoTxt.text = weaponManager.currWeapon.magAmmo + " / " + weaponManager.currWeapon.ammoRemain;      
    }

    public void PlayerFind()
    {
        fireCtrl = GameObject.FindWithTag("PLAYER").GetComponent<FireCtrl>();
        player = fireCtrl.gameObject.GetComponent<TestPlayerMovement>();
    }
}
