using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance; // �ν��Ͻ�ȭ
    public bool isStart = true;  // ���� ���� ����
    public bool isGameStart = false;
    public GameObject unableStart;  // ���� �Ұ� �� ��µǴ� �ȳ��ؽ�Ʈ
    public GameObject excessivePeaple;  // ���� �Ұ� �� ��µǴ� �ȳ��ؽ�Ʈ
    public GameObject gameStart;  // ���� �Ұ� �� ��µǴ� �ȳ��ؽ�Ʈ
    public GameObject impossJoin;  // �� ���� �Ұ� �ؽ�Ʈ

    [SerializeField]
    TMP_InputField roomNameInputField;  // �� �̸��� �Է��ϴ� ��ǲ�ʵ�
    [SerializeField]
    TMP_Text errorText;  // ���� �� ��µǴ� �ؽ�Ʈ
    [SerializeField]
    Text roomNameTxt;  // �� �̸��� ��µǴ� �ؽ�Ʈ
    [SerializeField]
    Text myNameTxt;  // �� �̸��� ��µǴ� �ؽ�Ʈ
    [SerializeField]
    Transform roomListContent;  // �� ���
    [SerializeField]
    GameObject roomListPrefab;  // �� ������
    [SerializeField]
    Text noRoomTxt;  // ���� ���� ��� ��µǴ� �ؽ�Ʈ
    [SerializeField]
    GameObject userInfoPrefab;  // ���� ������
    [SerializeField]
    Transform[] userInfoListContents;  // ���� ���

    Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();

    Image[] blueTeamList;  // ����� ���
    Image[] redTeamList;  // ������ ���
    GameObject userInfo; // ���� �ٸ� ���� ������ ��� ������
    Text userStateTxt;  // ���� �غ����
    PhotonView pv;

    string userId;  // ���� ���̵�
    int randomTeamNum;  // �� ���� ��ȣ
    int changeCount;  // �غ� ���¸� �� �� �ٲٴ��� ī��Ʈ
    bool redChange = true;  // ���������� ���� ���� ����
    bool blueChange = true;  // ��������� ���� ���� ����

    private void Awake()
    {
        Instance = this;
        pv = GetComponent<PhotonView>();
        roomListPrefab = Resources.Load<GameObject>("RoomInfo");  // ���ҽ��������� �ִ� ��������
        userInfoPrefab = Resources.Load<GameObject>("UserInfo");  // ���ҽ��������� �ִ� ����������
    }

    void Start()
    {
        Debug.Log("Connecting to Master");
        // ���� ���� ��
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        // ���� �Ϸ�
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        // Ÿ��Ʋ ȭ�� ����
        MenuManager.Instance.OpenMenu("title");
    }

    // �游���
    public void CreateRoom()
    {
        // ��ǲ�ʵ忡 �Էµ� ���� ���� ���(���ڼ� 4~10 ����) ����
        if (string.IsNullOrEmpty(roomNameInputField.text) || (roomNameInputField.text.Length < 4 || roomNameInputField.text.Length > 11))
        {
            return;
        }

        // �� �ɼ� ����
        RoomOptions ro = new RoomOptions();
        ro.CustomRoomProperties = new Hashtable();
        ro.CustomRoomProperties.Add("MASTERNICK", userId); // ������ �г��� ����
        ro.CustomRoomPropertiesForLobby = new string[] { "MASTERNICK" }; // �κ񿡼��� ����� ���� ����
        ro.MaxPlayers = 6;  // �ִ� �÷��̾� ��
        ro.IsOpen = true;  // ���� ���ɿ���
        ro.IsVisible = true;  // ���� ����

        PhotonNetwork.CreateRoom(roomNameInputField.text, ro);
        // ��ǲ�ʵ忡 �Էµ� ������ �� ����
        MenuManager.Instance.OpenMenu("loading");
    }

    // �� ��� �ʱ�ȭ
    void PlayerListInit(Image[] playerListImg)
    {
        Debug.Log("�� �� : " + playerListImg.Length);
        for (int i = 3; i < playerListImg.Length; i++)
        {
            Destroy(playerListImg[i].gameObject);
        }
    }

    // �� ����
    public override void OnJoinedRoom()
    {
        blueTeamList = userInfoListContents[0].GetComponentsInChildren<Image>();
        redTeamList = userInfoListContents[1].GetComponentsInChildren<Image>();
        // �� ���� �� �ʱ�ȭ
        PlayerListInit(blueTeamList);
        PlayerListInit(redTeamList);

        MenuManager.Instance.OpenMenu("room");
        // �� �̸� ǥ��
        roomNameTxt.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;  // ���漭�� ���� �÷��̾� ���� ����
        Debug.Log("photonPlayer �� : " + players.Length);
        Debug.Log("�� id : " + userId);

        // PhotonPlayer�� ���� ŭ �ݺ�
        for (int i = 0; i < players.Count(); i++)
        {
            string userNick = null;

            // �÷��̾ �г��� �Ӽ����� ����� ���� �ִ��� Ȯ���Ͽ� ����
            if (players[i].CustomProperties.TryGetValue("NICKNAME", out object nickValue))
            {
                userNick = (string)nickValue;
            }
            Debug.Log("playerID : " + userNick + "�� ID : " + userId);
            if (userNick == userId)  // players[i]�� �г����� �� userId�� ���ٸ� >> ����
            {
                SetMyTeamList(players[i]);
            }
            else
            {
                SetOtherTeamList(players[i]);
            }
        }
    }

    void SetMyTeamList(Player pl)
    {
        Debug.Log("����");

        // �� ���� ����
        randomTeamNum = Random.Range(0, 2);

        Image[] _userInfo = userInfoListContents[randomTeamNum].GetComponentsInChildren<Image>();

        if (_userInfo.Length > 6)  // ���� ���� ���� �� ���¶�� �ٸ� �� ����
        {
            if (randomTeamNum == 0)
                randomTeamNum = 1;
            else
                randomTeamNum = 0;
        }

        // ���� �÷��̾��� �� ������ NUMBER�� ����
        Hashtable initialProps = new Hashtable();
        initialProps["NUMBER"] = randomTeamNum;
        pl.SetCustomProperties(initialProps);

        // ���� ��� ����
        userInfo = Instantiate(userInfoPrefab, userInfoListContents[randomTeamNum]);
        userInfo.GetComponent<PlayerListItem>().SetUp(pl);
        userInfo.GetComponent<Image>().color = Color.yellow;  // �������� Ȯ���ϱ� ���� �� ����
        userStateTxt = userInfo.GetComponentsInChildren<Text>()[1];

        // �÷��̾��� �غ� ���� ����
        Hashtable initialStateProps = new Hashtable();
        initialStateProps["STATE"] = userStateTxt.text;
        pl.SetCustomProperties(initialStateProps);
    }

    void SetOtherTeamList(Player pl)
    {
        Debug.Log("�޶�");

        int num = 0;
        // players[i]�� �� ������ �ҷ��� ����
        if (pl.CustomProperties.TryGetValue("NUMBER", out object numberValue))
        {
            num = (int)numberValue;
        }
        userInfo = Instantiate(userInfoPrefab, userInfoListContents[num]);
        userInfo.GetComponent<PlayerListItem>().SetUp(pl);
        userStateTxt = userInfo.GetComponentsInChildren<Text>()[1];
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("photonPlayer �� : " + PhotonNetwork.PlayerList.Length);

        // ����� �� ������ �ִ��� Ȯ���Ͽ� ����
        int num = 0;

        if (newPlayer.CustomProperties.TryGetValue("NUMBER", out object teamNum))
        {
            num = (int)teamNum;
            Debug.Log("����� �� ����" + num);
        }

        userInfo = Instantiate(userInfoPrefab, userInfoListContents[num]);
        userInfo.GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    // �غ� ��ư Ŭ�� (���� ����)
    public void OnClickReady(Player pl)
    {
        if (PhotonNetwork.IsMasterClient)
            return;

        // ī��Ʈ�� ���� ����
        changeCount++;

        string stateTxt = "error";

        if (changeCount % 2 != 0)
            stateTxt = "�غ� �Ϸ�";

        else
        {
            stateTxt = "�غ� ��";
            changeCount = 0;
        }

        Hashtable initialStateProps = new Hashtable();
        initialStateProps["STATE"] = stateTxt;
        pl.SetCustomProperties(initialStateProps);
    }

    // ���� ���� ���ɿ��θ� Ȯ���� �Լ�
    bool StateCheck(Image[] teamListImg, Text[] teamState)
    {
        isStart = true;
        Debug.Log("���ο� : " + (teamListImg.Length - 3).ToString());
        if (teamListImg.Length > 6)  // ���� �ο��� �ʰ��� ���
        {
            isStart = false;
            Debug.Log("���ο��ʰ�");
            StartCoroutine(ExcessivePeaple());
        }
        if (isStart)
        {
            // �ο��� �غ���¸� ã�Ƽ� ��� ����
            for (int i = 0; i < teamState.Length; i++)
            {
                if (teamState[i].text.Contains("��"))
                {
                    isStart = false;
                    Debug.Log("�� �غ� ��");
                    StartCoroutine(UnableStart());
                    break;
                }
            }
        }
        return isStart;
    }

    // ���� ���� ��ư Ŭ�� (���常)
    public void OnClickStartGame()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        blueTeamList = userInfoListContents[0].GetComponentsInChildren<Image>();
        redTeamList = userInfoListContents[1].GetComponentsInChildren<Image>();
        Text[] blueTeamTxt = userInfoListContents[0].GetComponentsInChildren<Text>();
        Text[] redTeamTxt = userInfoListContents[1].GetComponentsInChildren<Text>();

        // ��� �ο��� �غ�Ϸ�Ǹ� ���� ����
        if (StateCheck(blueTeamList, blueTeamTxt) && StateCheck(redTeamList, redTeamTxt))
        {
            isGameStart = true;
            Debug.Log("���� ����");
            StartCoroutine(GameStart());
        }
    }

    // �غ� ���� �����ʾ��� �� �ȳ�����
    IEnumerator UnableStart()
    {
        unableStart.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        unableStart.gameObject.SetActive(false);
    }

    // �� �ο� �ʰ��Ǿ��� �� �ȳ�����
    IEnumerator ExcessivePeaple()
    {
        excessivePeaple.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        excessivePeaple.gameObject.SetActive(false);
    }

    // ���� ������ �˸��� ����
    IEnumerator GameStart()
    {
        gameStart.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        gameStart.gameObject.SetActive(false);

        pv.RPC("TestSceneLoad", RpcTarget.All);
        // ���� �� ���� �߰�
    }

    [PunRPC]
    void TestSceneLoad()
    {
        PhotonNetwork.LoadLevel("TestScene");
    }


    // �� ���� �������� �� �������
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "�� ���� ����\n" + message;

        MenuManager.Instance.OpenMenu("error");
    }

    // �Խ�Ʈ ������ ��� ���� ���̵� �ο��ϰ�
    // ���̵� �� ����
    public void GuestId()
    {
        userId = "USER_" + Random.Range(1000, 10000);
        PhotonNetwork.NickName = userId;
        myNameTxt.text = userId;
        Hashtable nick = new Hashtable();
        nick["NICKNAME"] = userId;
        PhotonNetwork.LocalPlayer.SetCustomProperties(nick);

        MenuManager.Instance.OpenMenu("lobby");
    }

    public void Login(string nickName)
    {
        userId = nickName;
        PhotonNetwork.NickName = userId;
        myNameTxt.text = userId;
        Hashtable nick = new Hashtable();
        nick["NICKNAME"] = userId;
        PhotonNetwork.LocalPlayer.SetCustomProperties(nick);

        MenuManager.Instance.OpenMenu("lobby");
    }

    // �� ������
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    // �� ����
    public void JoinRoom(RoomInfo info)
    {
        if (info.PlayerCount < info.MaxPlayers)
        {
            PhotonNetwork.JoinRoom(info.Name);
            MenuManager.Instance.OpenMenu("loading");
        }
        else
           StartCoroutine(ImpossJoin());
    }

    // �� �� �ο� �ʰ� �� ���� �Ұ��ȳ� ����
    IEnumerator ImpossJoin()
    {
        impossJoin.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        impossJoin.gameObject.SetActive(false);
    }

    // ������ �� ����
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    // �� ������
    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("lobby");
    }

    // �� ü����
    public void OnclickChangeTeam(string color, Player pl)
    {
        blueTeamList = userInfoListContents[0].GetComponentsInChildren<Image>();
        redTeamList = userInfoListContents[1].GetComponentsInChildren<Image>();

        Debug.Log("blueTeamPlayerCount : " + (blueTeamList.Length - 3).ToString());
        Debug.Log("redTeamPlayerCount : " + (redTeamList.Length - 3).ToString());

        // ���� ���� ü���� �����ϴٸ� �� ü����
        if (color == "Blue" && ChangePossible(blueTeamList, blueChange))
        {
            TeamChange(0, pl);
        }
        else if (color == "Red" && ChangePossible(redTeamList, redChange))
        {
            TeamChange(1, pl);
        }
    }

    // �� ���� ���ɿ��θ� �������� �Լ�
    bool ChangePossible(Image[] list, bool teamChange)
    {
        if (list.Length > 6)
            teamChange = false;
        else
            teamChange = true;

        return teamChange;
    }

    // �� ���� �Լ�
    void TeamChange(int num, Player pl)
    {
        Hashtable initialProps = new Hashtable();
        initialProps["NUMBER"] = num;
        pl.SetCustomProperties(initialProps);
        Debug.Log("ToChangeTeam");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null; // ������ ������� �ӽ� ����� ����

        foreach (var room in roomList)
        {
            if (room.RemovedFromList) // ���� ������ ���
            {
                // ��ųʸ����� �� �̸����� �˻��Ͽ� �ӽ����� ������ ����
                rooms.TryGetValue(room.Name, out tempRoom);
                Destroy(tempRoom); // �� ������ ������ ����
                rooms.Remove(room.Name); // ��ųʸ����� ����
            }

            else  // �� ���� ����
            {
                // Contains >> �����ϸ� true
                // rooms��� ��ųʸ��� �ش� ���̸��� ����X = ���� �߰�
                if (!rooms.ContainsKey(room.Name))
                {
                    GameObject roomPrefab = Instantiate(roomListPrefab, roomListContent);
                    roomPrefab.GetComponent<RoomListItem>().SetUp(room);
                    rooms.Add(room.Name, roomPrefab);
                }
                // rooms��� ��ųʸ��� �ش� ���̸��� ����X = �� ���� ����
                else
                {
                    rooms.TryGetValue(room.Name, out tempRoom);
                    tempRoom.GetComponent<RoomListItem>().SetUp(room);
                }
            }
            print("Room = " + room.Name + "(" + room.PlayerCount + "/" + room.MaxPlayers + ")");
        }
    }
}
