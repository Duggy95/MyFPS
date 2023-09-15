using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TestPhoton : MonoBehaviourPunCallbacks
{
    readonly string version = "1.0";
    //string userId = "jin";

    BattleUIManager battleUIManager;
    public WeaponManager weaponManager;

    [HideInInspector]
    public GameObject player;

    void Awake()
    {
        Screen.SetResolution(1920, 1080, false);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = version;

        //서버 접속 시도. 접속중이 아닐 때
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();

        battleUIManager = GameObject.FindWithTag("MANAGER").GetComponent<BattleUIManager>();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 }, null);
    }

    public override void OnJoinedRoom()
    {
        player = PhotonNetwork.Instantiate("Player", Vector3.up, Quaternion.identity);
        battleUIManager.PlayerFind();
    }
}