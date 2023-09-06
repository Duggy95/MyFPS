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

    void UIUpdate()  //UI ������Ʈ
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

    public void PlayerFind()  //�÷��̾� �����ɶ� ���� �޾ƿ���.
    {
        fireCtrl = testPhoton.player.GetComponent<FireCtrl>();
        player = testPhoton.player.GetComponent<PlayerMovement>();
    }
}
