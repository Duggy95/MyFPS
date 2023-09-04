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

    // 방 정보 최신화
    public void SetUp(RoomInfo _info)
    {
        // 방 만든 유저 닉네임
        masterName = (string)_info.CustomProperties["MASTERNICK"];

        info = _info; // 방 정보
        roomNameTxt.text = _info.Name;  // 방 이름
        roomcurrPeapleTxt.text = _info.PlayerCount + "/" + _info.MaxPlayers; // 방 인원
        roomManagerNickTxt.text = masterName;
    }

    // 방 클릭시 입장
    public void OnClick()
    {
        Launcher.Instance.JoinRoom(info);
    }
}
