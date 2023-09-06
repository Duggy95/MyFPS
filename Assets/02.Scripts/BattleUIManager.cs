using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    public Text ammoTxt;
    public Text HPTxt;
    FireCtrl fireCtrl;
    PlayerMovement player;
    public TestPhoton testPhoton;

    void Update()
    {
        if(fireCtrl != null)
        {
            UIUpdate();
        }
    }

    void UIUpdate()  //UI 업데이트
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

    public void PlayerFind()  //플레이어 생성될때 정보 받아오기.
    {
        fireCtrl = testPhoton.player.GetComponent<FireCtrl>();
        player = testPhoton.player.GetComponent<PlayerMovement>();
    }
}
