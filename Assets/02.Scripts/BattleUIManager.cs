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

    }

    void Update()
    {
        if(fireCtrl != null)
        {
            UIUpdate();
        }
    }

    void UIUpdate()
    {
        HPTxt.text = "HP : " + player.health;
        if(fireCtrl.weapon != null)
        {
            ammoTxt.text = fireCtrl.weapon.magAmmo + " / " + fireCtrl.weapon.ammoRemain;
        }
        else
        {
            ammoTxt.text = "0 / 0";
        }
    }

    public void PlayerFind()
    {
        fireCtrl = GameObject.FindWithTag("PLAYER").GetComponent<FireCtrl>();
        player = fireCtrl.gameObject.GetComponent<TestPlayerMovement>();
    }
}
