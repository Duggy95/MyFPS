using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Text playerNameTxt;
    [SerializeField]
    Text playerStateTxt;

    Player player;
    GameObject teamBlue;
    GameObject teamRed;
    int teamNum;

    private void Awake()
    {
        teamBlue = GameObject.FindGameObjectWithTag("TEAMBLUE");
        teamRed = GameObject.FindGameObjectWithTag("TEAMRED");
    }

    private void Update()
    {
        // 방장이 누구인지 확인하여 방장 방 이탈 시 다음 방장으로 최신화
        if (PhotonNetwork.MasterClient.NickName == playerNameTxt.text)
        {
            playerStateTxt.text = "방장";
        }

        // 방장이 아니라면 STATE에 저장된 값을 토대로 정보 최신화
        else if (player.CustomProperties.TryGetValue("STATE", out object stateValue))
        {
            playerStateTxt.text = (string)stateValue;
        }

        // 플레이어의 팀 넘버에 따라 팀 리스트 최신화
        if (player.CustomProperties.TryGetValue("NUMBER", out object numValue))
        {
            teamNum = (int)numValue;

            if (teamNum == 0)
            {
                transform.SetParent(teamBlue.transform);
            }
            else
            {
                transform.SetParent(teamRed.transform);
            }
        }
    }

    // 플레이어 정보
    public void SetUp(Player _player)
    {
        player = _player;
        playerNameTxt.text = player.NickName;
        playerStateTxt.text = "준비 중";
    }

    // 플레이어가 방을 나갈 때 그 플레이어가 자신인지 확인
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    // 방을 나갔을 때
    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
