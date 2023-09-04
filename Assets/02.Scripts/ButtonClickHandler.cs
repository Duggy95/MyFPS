using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ButtonClickHandler : MonoBehaviour
{
    // 버튼 클릭 이벤트 발생 시 다음 정보를 해당 함수에 전달

    public void OnTeamChangeButtonClick()
    {
        if (Launcher.Instance.isGameStart)
            return;

        if (gameObject.name.Contains("Blue"))
        {
            Launcher.Instance.OnclickChangeTeam("Blue", PhotonNetwork.LocalPlayer);
        }

        else
        {
            Launcher.Instance.OnclickChangeTeam("Red", PhotonNetwork.LocalPlayer);
        }
    }

    public void OnStateChangeButtonClick()
    {
        if (Launcher.Instance.isGameStart)
            return;

        Launcher.Instance.OnClickReady(PhotonNetwork.LocalPlayer);
        Launcher.Instance.OnClickStartGame();
    }
}
