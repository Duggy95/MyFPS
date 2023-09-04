using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] Text roomNameTxt;
    [SerializeField] Text roomcurrPeapleTxt;
    [SerializeField] Text roomModeTxt;
    [SerializeField] Text roomManagerNickTxt;

    RoomInfo info;
    string masterName;

    // �� ���� �ֽ�ȭ
    public void SetUp(RoomInfo _info)
    {
        // �� ���� ���� �г���
        masterName = (string)_info.CustomProperties["MASTERNICK"];

        info = _info; // �� ����
        roomNameTxt.text = _info.Name;  // �� �̸�
        roomcurrPeapleTxt.text = _info.PlayerCount + "/" + _info.MaxPlayers; // �� �ο�
        roomManagerNickTxt.text = masterName;
    }

    // �� Ŭ���� ����
    public void OnClick()
    {
        Launcher.Instance.JoinRoom(info);
    }
}
