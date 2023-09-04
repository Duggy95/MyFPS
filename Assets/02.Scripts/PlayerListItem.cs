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
        // ������ �������� Ȯ���Ͽ� ���� �� ��Ż �� ���� �������� �ֽ�ȭ
        if (PhotonNetwork.MasterClient.NickName == playerNameTxt.text)
        {
            playerStateTxt.text = "����";
        }

        // ������ �ƴ϶�� STATE�� ����� ���� ���� ���� �ֽ�ȭ
        else if (player.CustomProperties.TryGetValue("STATE", out object stateValue))
        {
            playerStateTxt.text = (string)stateValue;
        }

        // �÷��̾��� �� �ѹ��� ���� �� ����Ʈ �ֽ�ȭ
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

    // �÷��̾� ����
    public void SetUp(Player _player)
    {
        player = _player;
        playerNameTxt.text = player.NickName;
        playerStateTxt.text = "�غ� ��";
    }

    // �÷��̾ ���� ���� �� �� �÷��̾ �ڽ����� Ȯ��
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    // ���� ������ ��
    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
