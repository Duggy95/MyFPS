using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance; // �̱����� �Ҵ�� static ����. �� ������ GameManager Ŭ���� �ȿ����� ���ٰ���. ���⼭�� �� instance�ż��忡�� ���� ��.

    public string CurrentUserNickName;
    public bool loginSuccess = false;

    public Sprite EquipGunsprite;
    public bool HasGrenade = false; // ����ź �������� ����� üũ
    public bool HasArmor = false; // �Ƹ� �������� ����� üũ

    // �̱��� ���ٿ� ������Ƽ
    public static GameManager instance        //GmaeManagerŸ���� ���� instance�� �Ʒ� �ڵ��� M_instance�� ��ȯ�ϴ� ����ƽ �ż��� ����.
    {
        get
        {
            // ���� �̱��� ������ ���� ������Ʈ�� �Ҵ���� �ʾҴٸ�
            if (m_instance == null)
            {
                // ������ GameManager ������Ʈ�� ã�� �Ҵ�
                m_instance = FindObjectOfType<GameManager>();
            }
            // �̱��� ������Ʈ�� ��ȯ
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
    }

    private void Update()
    {
        Debug.Log(CurrentUserNickName);
    }
}
