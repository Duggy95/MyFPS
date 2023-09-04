using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyGun : MonoBehaviour
{
    public GameObject BuyCheckUI;
    public GameObject YoucantBuy;
    public GameObject grenade;
    public Text remainningtime;
    public Text MoneyText;

    int time;

    public GameObject gun1;
    public GameObject gun2;
    public GameObject gun3;
    bool[] gunsbuy = new bool[5];

    public Transform EquipGun;
    public SkinnedMeshRenderer GunMesh;

    public Mesh Equipgun1;
    public Mesh Equipgun2;
    public Mesh Equipgun3;

    public Material GunShader1;
    public Material GunShader2;
    public Material GunShader3;

    public SkinnedMeshRenderer Head;
    public SkinnedMeshRenderer Torso;
    public SkinnedMeshRenderer Leg;

    public Material HeadArmor; // ��� ����
    public Material TorsoArmor;
    public Material LegArmor;

    // Start is called before the first frame update
    void Start()
    {
        remainningtime.text = "Buy Time Remaining : 00 : 20";

        for (int i = 0; i < 5; i++)
        {
            gunsbuy[i] = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        remainningtime.text = "Buy Time Remaining : 00 : " + ((int)20 - (int)Time.time).ToString();
        if (Time.time > 10) // 00 : 9 �̷� �� ���� 00 : 09 �̷� ������ �����ַ� ��.
        {
            remainningtime.text = "Buy Time Remaining : 00 : 0" + ((int)20 - (int)Time.time).ToString();
        }
        if (Time.time >= 20)
        {
            this.gameObject.SetActive(false);
            gun1.SetActive(false);
            gun2.SetActive(false);
            gun3.SetActive(false);
            remainningtime.text = "";
        }
    }


    public void BuyGunClick()
    {
        BuyCheckUI.SetActive(true);

        Debug.Log(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);

        for (int i = 1; i <= 5; i++)
        {
            if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name == "Gun" + i.ToString())
            {
                gunsbuy[i - 1] = true;
            }
        }
    }

    public void BuyGunYes()
    {
        BuyCheckUI.SetActive(false);

        Debug.Log(int.Parse(MoneyText.text));

        for (int i = 0; i < 5; i++)
        {
            if (gunsbuy[i])
            {
                // ĳ���Ͱ� �ѵ�� �ִ°� �ٲ�����.
                if (i == 0 && int.Parse(MoneyText.text) >= 100)
                {
                    EquipGun.transform.localScale = new Vector3(1, 1, 1);
                    EquipGun.transform.localRotation = Quaternion.Euler(95, -381, -376); // ȸ���� �����ֱ�
                    GunMesh.sharedMesh = Equipgun1; // �� ���(�޽���) 1�� ����
                                                    //   gunShader.mainTexture = GunShader1;

                    GunMesh.material = GunShader1;  // ���͸��� �� ������ �´� ���͸���� ������.

                    MoneyText.text = (int.Parse(MoneyText.text) - 100).ToString();
                    break;
                }
                else if (i == 1 && int.Parse(MoneyText.text) >= 200)
                {
                    EquipGun.transform.localScale = new Vector3(1, 1, 1);
                    EquipGun.transform.localRotation = Quaternion.Euler(-7, -359, -352);
                    GunMesh.sharedMesh = Equipgun2;  // �� ���(�޽���) 2�� ����

                    GunMesh.material = GunShader2;

                    MoneyText.text = (int.Parse(MoneyText.text) - 200).ToString();

                    break;

                }
                else if (i == 2 && int.Parse(MoneyText.text) >= 300)
                {
                    EquipGun.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    EquipGun.transform.localRotation = Quaternion.Euler(172.263f, -178.481f, -180.989f);
                    GunMesh.sharedMesh = Equipgun3;  // �� ���(�޽���) 3�� ����

                    GunMesh.material = GunShader3;

                    MoneyText.text = (int.Parse(MoneyText.text) - 300).ToString();

                    break;
                }
                else if (i == 3 && int.Parse(MoneyText.text) >= 50) // ����ź
                {
                    grenade.SetActive(true);
                    // ����ź �� ���� ��������� ���ӸŴ����� �����ϸ� ������ ����.
                    MoneyText.text = (int.Parse(MoneyText.text) - 50).ToString();

                    break;
                }
                else if (i == 4 && int.Parse(MoneyText.text) >= 100)
                {
                    Head.material = HeadArmor;
                    Torso.material = TorsoArmor;
                    Leg.material = LegArmor;

                    MoneyText.text = (int.Parse(MoneyText.text) - 100).ToString();

                    break;
                }

                MoneyText.text = (int.Parse(MoneyText.text) - (i + 1) * 100).ToString(); // ���� ������ �ִ� ������ ������.
                                                                                         // �� ���� Į��� ȸ������ �ϰ� �ٽ� �� ������ �ؾ���.
            }
            else if (int.Parse(MoneyText.text) < (i + 1) * 100)
            {
                // ���� ������ �� �� �����ϴ� UI ����
                YoucantBuy.SetActive(true);
            }
        }

        for (int i = 0; i < 5; i++)
        {
            gunsbuy[i] = false;
        }
    }
    public void BuyGunNo()
    {
        BuyCheckUI.SetActive(false);
    }

    public void youcantbuythisCheckYes()
    {
        YoucantBuy.SetActive(false);
    }
}
