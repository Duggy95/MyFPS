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
    public Canvas canvas;  // ���� ĵ����
    public Text BattleTimeTxt;  // ��Ʋ �ð� �ؽ�Ʈ

    [HideInInspector]
    public string teamName;   // ������� ��������
    [HideInInspector]
    public GameObject myPlayer;  // ���� �����ϴ� �÷��̾�

    Camera myCamera;  // ���� �� �÷��̾��� ī�޶�
    PhotonView pv;
    Vector3 spawnDis;   // ������ ������ �Ÿ�
    List<Image> profiles = new List<Image>();
    Image myProfile;
    float battleTime = 180f;  // ��Ʋ �ð�
    float shopTime = 20f;  // ���� �̿� �ð�
    bool isGameStart;  // ���� ���� ����

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
        /*if (teamName == "Blue")
        {
            // ������̸� ����� ������ ����
            // PhotonNetwork.Instantiate("PlayerImgPrefab", blueContent.transform.position, Quaternion.identity);
            myProfile = Instantiate(profileItem, blueContent.transform).GetComponent<Image>();
        }
        else if (teamName == "Red")
        {
            // �������̸� ������ ������ ����
            //PhotonNetwork.Instantiate("PlayerImgPrefab", redContent.transform.position, Quaternion.identity);
            myProfile = Instantiate(profileItem, redContent.transform).GetComponent<Image>();
        }*/

        Player[] players = PhotonNetwork.PlayerList;
        for(int i = 0; i < players.Length; i++)
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
            myCamera = myPlayer.GetComponentInChildren<Camera>();
        }
        else if (teamName == "Red")
        {
            // �������̸� ������ ���� ������ �� �ϳ��� ���� ����
            int spawnNum = Random.Range(0, redTeamSpawnPos.Length);
            myPlayer = PhotonNetwork.Instantiate("Player", redTeamSpawnPos[spawnNum].position + spawnDis, Quaternion.identity);
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
                        for(int j = 0; j < profiles.Count; j++)
                        {
                            if(players[i].CustomProperties.TryGetValue("NICKNAME", out object nameValue))
                            {
                                if((string)nameValue == profiles[i].GetComponent<ProfileListItem>().userId)
                                {
                                    profiles[i].GetComponent<Image>().color = Color.white; 
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

                    if (PhotonNetwork.IsMasterClient)
                    {
                        // ���� ���� �Լ� ȣ��
                        pv.RPC("BattleStart", RpcTarget.All);
                    }
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

        // ������ UI Ȱ��ȭ �ڵ� �ۼ� - ����ȭ �����̳� ���� ���� ���� ���� Ȱ�� ��Ȱ��
        myProfile.color = Color.white;
        MakePlayer();
    }

    [PunRPC]
    void BattleStart()
    {
        // ��Ʋ �ð� ����
    }

}
