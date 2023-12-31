using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;

public class BattleManager : MonoBehaviourPun
{
    private static BattleManager m_instance;

    public Transform[] blueTeamSpawnPos;  // 블루팀 스폰 포인트 지점
    public Transform[] redTeamSpawnPos;  // 레드팀 스폰 포인트 지점
    public GameObject blueContent;  // 블루팀 프로필 띄우는 스크롤뷰
    public GameObject redContent;  // 레드팀 프로필 띄우는 스크롤뷰
    public GameObject profileItem;  // 프로필
    public GameObject marketUI;  // 상점 UI
    public GameObject battleUI;  // 배틀 UI
    public GameObject[] walls;  // 처음 게임이 시작되기 전 통로를 막아줄 벽 오브젝트
    public GameObject battleReadyTxt;
    public GameObject battleStartTxt;
    public Canvas canvas;  // 현재 캔버스
    public Text BattleTimeTxt;  // 배틀 시간 텍스트
    public Text blueTeamScoreTxt;
    public Text redTeamScoreTxt;

    [HideInInspector]
    public int redTeamScore;  // 레드팀 점수
    [HideInInspector]
    public int blueTeamScore;  // 블루팀 점수
    [HideInInspector]
    public string teamName;   // 블루인지 레드인지
    [HideInInspector]
    public GameObject myPlayer;  // 내가 조작하는 플레이어
    [HideInInspector]
    public Camera myCamera;  // 현재 내 플레이어의 카메라
    PhotonView pv;
    Vector3 spawnDis;   // 스폰될 곳과의 거리
    List<Image> profiles = new List<Image>();
    Image myProfile;
    float battleTime = 300f;  // 배틀 시간
    bool isGameStart;  // 게임 시작 여부
    bool isGameOver;  // 게임 종료 여부
    int victoryCount = 3; // 3승해야 게임 끝

    // 싱글톤 접근용 프로퍼티
    public static BattleManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<BattleManager>();
            }
            return m_instance;
        }
    }

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)   //주소값으로 비교하는것
        {
            // 자신을 파괴
            Destroy(gameObject);
        }

        pv = GetComponent<PhotonView>();

        // 포톤 로컬 플레이어에 저장된 팀 정보를 받아와 저장
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("NUMBER", out object numberValue))
        {
            int num = (int)numberValue;

            if (num == 0)
            {
                teamName = "Blue";
            }
            else
            {
                teamName = "Red";
            }
        }
        // pv.RPC("MakeProfile", RpcTarget.All);
        MakeProfile();
        OpenMarket();
        StartCoroutine(CheckGameStart());
    }

    //[PunRPC]
    void MakeProfile()
    {
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].CustomProperties.TryGetValue("NUMBER", out object numberValue))
            {
                int num = (int)numberValue;

                if (num == 0)
                {
                    // 블루팀이면 블루팀 프로필 생성
                    profiles.Add(Instantiate(profileItem, blueContent.transform).GetComponent<Image>());
                }
                else if (num == 1)
                {
                    // 레드팀이면 레드팀 프로필 생성
                    profiles.Add(Instantiate(profileItem, redContent.transform).GetComponent<Image>());
                }

                if (players[i] == PhotonNetwork.LocalPlayer)
                {
                    myProfile = profiles[i];
                }
            }

            if (players[i].CustomProperties.TryGetValue("NICKNAME", out object nameValue))
            {
                string name = (string)nameValue;
                profiles[i].GetComponent<ProfileListItem>().userId = name;
                profiles[i].GetComponentInChildren<Text>().text = name;
                profiles[i].GetComponentInChildren<Text>().color = Color.white;
            }
        }
    }

    void MakePlayer()
    {
        spawnDis = new Vector3(0, 2, 0);  // 생성될 땅과의 거리

        if (teamName == "Blue")
        {
            // 블루팀이면 블루팀 스폰 포지션 중 하나에 랜덤 생성
            int spawnNum = Random.Range(0, blueTeamSpawnPos.Length);
            myPlayer = PhotonNetwork.Instantiate("Player", blueTeamSpawnPos[spawnNum].position + spawnDis, Quaternion.identity);
            myCamera = myPlayer.GetComponentInChildren<Camera>();
        }
        else if (teamName == "Red")
        {
            // 레드팀이면 레드팀 스폰 포지션 중 하나에 랜덤 생성
            int spawnNum = Random.Range(0, redTeamSpawnPos.Length);
            myPlayer = PhotonNetwork.Instantiate("Player", redTeamSpawnPos[spawnNum].position + spawnDis, Quaternion.identity);
            myCamera = myPlayer.GetComponentInChildren<Camera>();
        }
    }

    void OpenMarket()
    {
        // 상점 띄우기
        // 상점 제한시간 시작
        marketUI.gameObject.SetActive(true);
        battleUI.gameObject.SetActive(false);
    }

    IEnumerator CheckGameStart()
    {
        // 게임 시작될 때까지 반복
        while (!isGameStart)
        {
            // 포톤 플레이어 수만큼 반복하면서 모든 유저가 게임 준비되었는지 확인
            Player[] players = PhotonNetwork.PlayerList;
            int num = 0;
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].CustomProperties.TryGetValue("READY", out object readyValue))
                {
                    if ((string)readyValue == "True")
                    {
                        num++;
                        for (int j = 0; j < profiles.Count; j++)
                        {
                            if (players[i].CustomProperties.TryGetValue("NICKNAME", out object nameValue))
                            {
                                if ((string)nameValue == profiles[i].GetComponent<ProfileListItem>().userId)
                                {
                                    // 프로필 UI 활성화 코드 작성
                                    // 준비된 유저의 프로필은 흰색으로 텍스트는 검은 색으로
                                    profiles[i].GetComponent<Image>().color = Color.white;
                                    profiles[i].GetComponentInChildren<Text>().color = Color.black;
                                    break;
                                }
                            }
                        }
                    }
                }
                // 준비가 다 되었다면 배틀 시작
                if (num == players.Length)
                {
                    isGameStart = true;
                    BattleReady();
                    break;
                }
            }
            yield return null;
        }
    }

    public void Ready()
    {
        canvas.worldCamera = myCamera;
        marketUI.gameObject.SetActive(false);
        battleUI.gameObject.SetActive(true);
        // 플레이어 게임할 준비되었는지 저장
        Hashtable ready = new Hashtable();
        ready["READY"] = "True";
        PhotonNetwork.LocalPlayer.SetCustomProperties(ready);

        //myProfile.color = Color.white;
        MakePlayer();
    }

    void BattleReady()
    {
        StartCoroutine(BattleReadyAndStart());
    }

    IEnumerator BattleReadyAndStart()
    {
        battleReadyTxt.gameObject.SetActive(true);   // 배틀 준비

        yield return new WaitForSeconds(2f);

        battleReadyTxt.gameObject.SetActive(false);  // 2초 뒤 배틀 시작
        battleStartTxt.gameObject.SetActive(true);

        // 게임 시작 함수 호출
        if (PhotonNetwork.IsMasterClient)
            pv.RPC("BattleStart", RpcTarget.All);

        yield return new WaitForSeconds(2f);

        battleStartTxt.gameObject.SetActive(false);  
    }

    [PunRPC]
    void BattleStart()
    {
        // 배틀 시간 동작
        StartCoroutine(BattleTimeCountDown());
        StartCoroutine(BattleTimeUIUpdate());

        // 통로 벽 없애기
        foreach (GameObject wall in walls)
        {
            wall.SetActive(false);
        }
    }

    IEnumerator BattleTimeCountDown()
    {
        while (!isGameOver)
        {
            battleTime -= Time.deltaTime;  // 시간 카운트 다운
            yield return null;
        }
    }

    IEnumerator BattleTimeUIUpdate()
    {
        int min = 0;
        int sec = 0;

        while (!isGameOver)
        {
            min = (int)battleTime / 60;
            sec = (int)battleTime % 60;

            BattleTimeTxt.text = min + ":" + sec;  // 배틀 시간 UI 업데이트

            if(sec < 10)
                BattleTimeTxt.text = min + ":0" + sec;  // 배틀 시간 UI 업데이트

            yield return null;
        }
    }
}
