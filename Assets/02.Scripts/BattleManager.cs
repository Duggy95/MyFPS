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

    public Transform[] blueTeamSpawnPos;  // ����� ���� ����Ʈ ����
    public Transform[] redTeamSpawnPos;  // ������ ���� ����Ʈ ����
    public GameObject blueContent;  // ����� ������ ���� ��ũ�Ѻ�
    public GameObject redContent;  // ������ ������ ���� ��ũ�Ѻ�
    public GameObject profileItem;  // ������
    public GameObject marketUI;  // ���� UI
    public GameObject battleUI;  // ��Ʋ UI
    public GameObject[] walls;  // ó�� ������ ���۵Ǳ� �� ��θ� ������ �� ������Ʈ
    public GameObject battleReadyTxt;
    public GameObject battleStartTxt;
    public Canvas canvas;  // ���� ĵ����
    public Text BattleTimeTxt;  // ��Ʋ �ð� �ؽ�Ʈ
    public Text blueTeamScoreTxt;
    public Text redTeamScoreTxt;

    [HideInInspector]
    public int redTeamScore;  // ������ ����
    [HideInInspector]
    public int blueTeamScore;  // ����� ����
    [HideInInspector]
    public string teamName;   // ������� ��������
    [HideInInspector]
    public GameObject myPlayer;  // ���� �����ϴ� �÷��̾�
    [HideInInspector]
    public Camera myCamera;  // ���� �� �÷��̾��� ī�޶�
    PhotonView pv;
    Vector3 spawnDis;   // ������ ������ �Ÿ�
    List<Image> profiles = new List<Image>();
    Image myProfile;
    float battleTime = 300f;  // ��Ʋ �ð�
    bool isGameStart;  // ���� ���� ����
    bool isGameOver;  // ���� ���� ����
    int victoryCount = 3; // 3���ؾ� ���� ��

    // �̱��� ���ٿ� ������Ƽ
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
        // ���� �̱��� ������Ʈ�� �� �ٸ� GameManager ������Ʈ�� �ִٸ�
        if (instance != this)   //�ּҰ����� ���ϴ°�
        {
            // �ڽ��� �ı�
            Destroy(gameObject);
        }

        pv = GetComponent<PhotonView>();

        // ���� ���� �÷��̾ ����� �� ������ �޾ƿ� ����
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
                    // ������̸� ����� ������ ����
                    profiles.Add(Instantiate(profileItem, blueContent.transform).GetComponent<Image>());
                }
                else if (num == 1)
                {
                    // �������̸� ������ ������ ����
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
        spawnDis = new Vector3(0, 2, 0);  // ������ ������ �Ÿ�

        if (teamName == "Blue")
        {
            // ������̸� ����� ���� ������ �� �ϳ��� ���� ����
            int spawnNum = Random.Range(0, blueTeamSpawnPos.Length);
            myPlayer = PhotonNetwork.Instantiate("Player", blueTeamSpawnPos[spawnNum].position + spawnDis, Quaternion.identity);
            WeaponManager.instance.PlayerFind();
            myCamera = myPlayer.GetComponentInChildren<Camera>();
        }
        else if (teamName == "Red")
        {
            // �������̸� ������ ���� ������ �� �ϳ��� ���� ����
            int spawnNum = Random.Range(0, redTeamSpawnPos.Length);
            myPlayer = PhotonNetwork.Instantiate("Player", redTeamSpawnPos[spawnNum].position + spawnDis, Quaternion.identity);
            WeaponManager.instance.PlayerFind();
            myCamera = myPlayer.GetComponentInChildren<Camera>();
        }
    }

    void OpenMarket()
    {
        // ���� ����
        // ���� ���ѽð� ����
        marketUI.gameObject.SetActive(true);
        battleUI.gameObject.SetActive(false);
    }

    IEnumerator CheckGameStart()
    {
        // ���� ���۵� ������ �ݺ�
        while (!isGameStart)
        {
            // ���� �÷��̾� ����ŭ �ݺ��ϸ鼭 ��� ������ ���� �غ�Ǿ����� Ȯ��
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
                                    // ������ UI Ȱ��ȭ �ڵ� �ۼ�
                                    // �غ�� ������ �������� ������� �ؽ�Ʈ�� ���� ������
                                    profiles[i].GetComponent<Image>().color = Color.white;
                                    profiles[i].GetComponentInChildren<Text>().color = Color.black;
                                    break;
                                }
                            }
                        }
                    }
                }
                // �غ� �� �Ǿ��ٸ� ��Ʋ ����
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
        // �÷��̾� ������ �غ�Ǿ����� ����
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
        battleReadyTxt.gameObject.SetActive(true);   // ��Ʋ �غ�

        yield return new WaitForSeconds(2f);

        battleReadyTxt.gameObject.SetActive(false);  // 2�� �� ��Ʋ ����
        battleStartTxt.gameObject.SetActive(true);

        // ���� ���� �Լ� ȣ��
        if (PhotonNetwork.IsMasterClient)
            pv.RPC("BattleStart", RpcTarget.All);

        yield return new WaitForSeconds(2f);

        battleStartTxt.gameObject.SetActive(false);  
    }

    [PunRPC]
    void BattleStart()
    {
        // ��Ʋ �ð� ����
        StartCoroutine(BattleTimeCountDown());
        StartCoroutine(BattleTimeUIUpdate());

        // ��� �� ���ֱ�
        foreach (GameObject wall in walls)
        {
            wall.SetActive(false);
        }
    }

    IEnumerator BattleTimeCountDown()
    {
        while (!isGameOver)
        {
            battleTime -= Time.deltaTime;  // �ð� ī��Ʈ �ٿ�
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

            BattleTimeTxt.text = min + ":" + sec;  // ��Ʋ �ð� UI ������Ʈ

            if(sec < 10)
                BattleTimeTxt.text = min + ":0" + sec;  // ��Ʋ �ð� UI ������Ʈ

            yield return null;
        }
    }
}
