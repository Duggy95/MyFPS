using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ButtonClickHandler : MonoBehaviour
{
    // ��ư Ŭ�� �̺�Ʈ �߻� �� ���� ������ �ش� �Լ��� ����

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
