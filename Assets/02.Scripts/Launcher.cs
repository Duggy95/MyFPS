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
    public static Launcher Instance; // 인스턴스화

    public bool isStart = true;  // 게임 가능 여부
    public bool isGameStart = false;
    public GameObject unableStart;  // 게임 불가 시 출력되는 안내텍스트
    public GameObject excessivePeaple;  // 게임 불가 시 출력되는 안내텍스트
    public GameObject gameStart;  // 게임 불가 시 출력되는 안내텍스트
    public GameObject impossJoin;  // 방 입장 불가 텍스트

    [SerializeField]
    TMP_InputField roomNameInputField;  // 룸 이름을 입력하는 인풋필드
    [SerializeField]
    TMP_Text errorText;  // 에러 시 출력되는 텍스트
    [SerializeField]
    Text roomNameTxt;  // 룸 이름이 출력되는 텍스트
    [SerializeField]
    Text myNameTxt;  // 룸 이름이 출력되는 텍스트
    [SerializeField]
    Transform roomListContent;  // 룸 목록
    [SerializeField]
    GameObject roomListPrefab;  // 룸 프리팹
    [SerializeField]
    Text noRoomTxt;  // 방이 없을 경우 출력되는 텍스트
    [SerializeField]
    GameObject userInfoPrefab;  // 유저 프리팹
    [SerializeField]
    Transform[] userInfoListContents;  // 유저 목록

    Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();  // 방 목록 업데이트하기 위한 딕셔너리

    Image[] blueTeamList;  // 블루팀 목록
    Image[] redTeamList;  // 레드팀 목록
    GameObject userInfo; // 현재 다른 유저 정보를 담는 프리팹
    Text userStateTxt;  // 유저 준비상태
    PhotonView pv;

    string userId;  // 유저 아이디
    int randomTeamNum;  // 팀 랜덤 번호
    int changeCount;  // 준비 상태를 몇 번 바꾸는지 카운트
    bool redChange = true;  // 레드팀으로 변경 가능 여부
    bool blueChange = true;  // 블루팀으로 변경 가능 여부

    private void Awake()
    {
        Instance = this;
        pv = GetComponent<PhotonView>();
        roomListPrefab = Resources.Load<GameObject>("RoomInfo");  // 리소시즈폴더에 있는 룸프리팹
        userInfoPrefab = Resources.Load<GameObject>("UserInfo");  // 리소시즈폴더에 있는 유저프리팹
    }

    void Start()
    {
        Debug.Log("Connecting to Master");
        // 포톤 접속 중
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        // 접속 완료
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        // 타이틀 화면 열기
        MenuManager.Instance.OpenMenu("title");
    }

    // 방만들기
    public void CreateRoom()
    {
        // 인풋필드에 입력된 값이 없는 경우(글자수 4~10 제한) 리턴
        if (string.IsNullOrEmpty(roomNameInputField.text) || (roomNameInputField.text.Length < 4 || roomNameInputField.text.Length > 11))
        {
            return;
        }

        // 룸 옵션 정보
        RoomOptions ro = new RoomOptions();
        ro.CustomRoomProperties = new Hashtable();
        ro.CustomRoomProperties.Add("MASTERNICK", userId); // 방장의 닉네임 저장
        ro.CustomRoomPropertiesForLobby = new string[] { "MASTERNICK" }; // 로비에서도 사용할 정보 지정
        ro.MaxPlayers = 6;  // 최대 플레이어 수
        ro.IsOpen = true;  // 입장 가능여부
        ro.IsVisible = true;  // 공개 여부

        PhotonNetwork.CreateRoom(roomNameInputField.text, ro);
        // 인풋필드에 입력된 값으로 방 생성
        MenuManager.Instance.OpenMenu("loading");
    }

    // 팀 목록 초기화
    void PlayerListInit(Image[] playerListImg)
    {
        Debug.Log("팀 수 : " + playerListImg.Length);
        for (int i = 3; i < playerListImg.Length; i++)
        {
            Destroy(playerListImg[i].gameObject);
        }
    }

    // 방 들어가기
    public override void OnJoinedRoom()
    {
        blueTeamList = userInfoListContents[0].GetComponentsInChildren<Image>();
        redTeamList = userInfoListContents[1].GetComponentsInChildren<Image>();
        // 방 입장 시 초기화
        PlayerListInit(blueTeamList);
        PlayerListInit(redTeamList);

        MenuManager.Instance.OpenMenu("room");
        // 방 이름 표시
        roomNameTxt.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;  // 포톤서버 내에 플레이어 정보 저장
        Debug.Log("photonPlayer 수 : " + players.Length);
        Debug.Log("내 id : " + userId);

        // PhotonPlayer의 수만 큼 반복
        for (int i = 0; i < players.Count(); i++)
        {
            string userNick = null;

            // 플레이어에 닉네임 속성으로 저장된 값이 있는지 확인하여 저장
            if (players[i].CustomProperties.TryGetValue("NICKNAME", out object nickValue))
            {
                userNick = (string)nickValue;
            }
            Debug.Log("playerID : " + userNick + "내 ID : " + userId);
            if (userNick == userId)  // players[i]의 닉네임이 내 userId와 같다면 >> 본인
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
        Debug.Log("같아");

        // 팀 랜덤 설정
        randomTeamNum = Random.Range(0, 2);

        Image[] _userInfo = userInfoListContents[randomTeamNum].GetComponentsInChildren<Image>();

        if (_userInfo.Length > 6)  // 현재 팀이 가득 찬 상태라면 다른 팀 배정
        {
            if (randomTeamNum == 0)
                randomTeamNum = 1;
            else
                randomTeamNum = 0;
        }

        // 현재 플레이어의 팀 정보값 NUMBER에 저장
        Hashtable initialProps = new Hashtable();
        initialProps["NUMBER"] = randomTeamNum;
        pl.SetCustomProperties(initialProps);

        // 유저 목록 생성
        userInfo = Instantiate(userInfoPrefab, userInfoListContents[randomTeamNum]);
        userInfo.GetComponent<PlayerListItem>().SetUp(pl);
        userInfo.GetComponent<Image>().color = Color.yellow;  // 본인인지 확인하기 위해 색 지정
        userStateTxt = userInfo.GetComponentsInChildren<Text>()[1];

        // 플레이어의 준비 상태 저장
        Hashtable initialStateProps = new Hashtable();
        initialStateProps["STATE"] = userStateTxt.text;
        pl.SetCustomProperties(initialStateProps);
    }

    void SetOtherTeamList(Player pl)
    {
        Debug.Log("달라");

        int num = 0;
        // players[i]의 팀 정보를 불러와 생성
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
        Debug.Log("photonPlayer 수 : " + PhotonNetwork.PlayerList.Length);

        // 저장된 팀 정보가 있는지 확인하여 생성
        int num = 0;

        if (newPlayer.CustomProperties.TryGetValue("NUMBER", out object teamNum))
        {
            num = (int)teamNum;
            Debug.Log("저장된 팀 정보" + num);
        }

        userInfo = Instantiate(userInfoPrefab, userInfoListContents[num]);
        userInfo.GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    // 준비 버튼 클릭 (방장 제외)
    public void OnClickReady(Player pl)
    {
        if (PhotonNetwork.IsMasterClient)
            return;

        // 카운트에 따라 동작
        changeCount++;

        string stateTxt = "error";

        if (changeCount % 2 != 0)
            stateTxt = "준비 완료";

        else
        {
            stateTxt = "준비 중";
            changeCount = 0;
        }

        Hashtable initialStateProps = new Hashtable();
        initialStateProps["STATE"] = stateTxt;
        pl.SetCustomProperties(initialStateProps);
    }

    // 게임 시작 가능여부를 확인할 함수
    bool StateCheck(Image[] teamListImg, Text[] teamState)
    {
        isStart = true;
        Debug.Log("팀인원 : " + (teamListImg.Length - 3).ToString());
        if (teamListImg.Length > 6 || teamListImg.Length < 3)  // 팀의 인원을 초과한 경우
        {
            isStart = false;
            Debug.Log("팀인원초과");
            StartCoroutine(ExcessivePeaple());
        }
        if (isStart)
        {
            // 인원의 준비상태를 찾아서 결과 전당
            for (int i = 0; i < teamState.Length; i++)
            {
                if (teamState[i].text.Contains("중"))
                {
                    isStart = false;
                    Debug.Log("팀 준비 중");
                    StartCoroutine(UnableStart());
                    break;
                }
            }
        }
        return isStart;
    }

    // 게임 시작 버튼 클릭 (방장만)
    public void OnClickStartGame()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        blueTeamList = userInfoListContents[0].GetComponentsInChildren<Image>();
        redTeamList = userInfoListContents[1].GetComponentsInChildren<Image>();
        Text[] blueTeamTxt = userInfoListContents[0].GetComponentsInChildren<Text>();
        Text[] redTeamTxt = userInfoListContents[1].GetComponentsInChildren<Text>();

        // 모든 인원이 준비완료되면 게임 시작
        if (StateCheck(blueTeamList, blueTeamTxt) && StateCheck(redTeamList, redTeamTxt))
        {
            isGameStart = true;
            Debug.Log("게임 시작");
            StartCoroutine(GameStart());
        }
    }

    // 준비가 아직 되지않았을 때 안내문구
    IEnumerator UnableStart()
    {
        unableStart.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        unableStart.gameObject.SetActive(false);
    }

    // 팀 인원 초과되었을 때 안내문구
    IEnumerator ExcessivePeaple()
    {
        excessivePeaple.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        excessivePeaple.gameObject.SetActive(false);
    }

    // 게임 시작을 알리는 문구
    IEnumerator GameStart()
    {
        gameStart.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        gameStart.gameObject.SetActive(false);

        pv.RPC("BattleScene", RpcTarget.All);
        // 게임 씬 입장 추가
    }

    [PunRPC]
    void BattleScene()
    {
        PhotonNetwork.LoadLevel("BattleScene");
    }


    // 방 생성 실패했을 때 에러출력
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "방 생성 실패\n" + message;

        MenuManager.Instance.OpenMenu("error");
    }

    // 게스트 입장인 경우 랜덤 아이디 부여하고
    // 아이디 값 저장
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

    // 방 나가기
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    // 방 입장
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

    // 방 현 인원 초과 시 입장 불가안내 문구
    IEnumerator ImpossJoin()
    {
        impossJoin.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        impossJoin.gameObject.SetActive(false);
    }

    // 랜덤한 방 입장
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    // 방 나가기
    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("lobby");
    }

    // 팀 체인지
    public void OnclickChangeTeam(string color, Player pl)
    {
        blueTeamList = userInfoListContents[0].GetComponentsInChildren<Image>();
        redTeamList = userInfoListContents[1].GetComponentsInChildren<Image>();

        Debug.Log("blueTeamPlayerCount : " + (blueTeamList.Length - 3).ToString());
        Debug.Log("redTeamPlayerCount : " + (redTeamList.Length - 3).ToString());

        // 색에 따라 체인지 가능하다면 팀 체인지
        if (color == "Blue" && ChangePossible(blueTeamList, blueChange))
        {
            TeamChange(0, pl);
        }
        else if (color == "Red" && ChangePossible(redTeamList, redChange))
        {
            TeamChange(1, pl);
        }
    }

    // 팀 변경 가능여부를 리턴해줄 함수
    bool ChangePossible(Image[] list, bool teamChange)
    {
        if (list.Length > 6)
            teamChange = false;
        else
            teamChange = true;

        return teamChange;
    }

    // 팀 변경 함수
    void TeamChange(int num, Player pl)
    {
        Hashtable initialProps = new Hashtable();
        initialProps["NUMBER"] = num;
        pl.SetCustomProperties(initialProps);
        Debug.Log("ToChangeTeam");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null; // 삭제된 룸아이템 임시 저장용 변수

        foreach (var room in roomList)
        {
            if (room.RemovedFromList) // 방이 삭제될 경우
            {
                // 딕셔너리에서 룸 이름으로 검색하여 임시저장 변수에 저장
                rooms.TryGetValue(room.Name, out tempRoom);
                Destroy(tempRoom); // 룸 아이템 프리팹 삭제
                rooms.Remove(room.Name); // 딕셔너리에서 삭제
            }

            else  // 룸 정보 변경
            {
                // Contains >> 존재하면 true
                // rooms라는 딕셔너리에 해당 룸이름이 존재X = 새로 추가
                if (!rooms.ContainsKey(room.Name))
                {
                    GameObject roomPrefab = Instantiate(roomListPrefab, roomListContent);
                    roomPrefab.GetComponent<RoomListItem>().SetUp(room);
                    rooms.Add(room.Name, roomPrefab);
                }
                // rooms라는 딕셔너리에 해당 룸이름이 존재X = 룸 정보 갱신
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
