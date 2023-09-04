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

    public Material HeadArmor; // 흰색 방어구들
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
        if (Time.time > 10) // 00 : 9 이런 식 말고 00 : 09 이런 식으로 보여주려 함.
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
                // 캐릭터가 총들고 있는거 바뀌어야함.
                if (i == 0 && int.Parse(MoneyText.text) >= 100)
                {
                    EquipGun.transform.localScale = new Vector3(1, 1, 1);
                    EquipGun.transform.localRotation = Quaternion.Euler(95, -381, -376); // 회전값 맞춰주기
                    GunMesh.sharedMesh = Equipgun1; // 총 모양(메쉬값) 1로 설정
                                                    //   gunShader.mainTexture = GunShader1;

                    GunMesh.material = GunShader1;  // 메터리얼도 총 종류에 맞는 메터리얼로 정해줌.

                    MoneyText.text = (int.Parse(MoneyText.text) - 100).ToString();
                    break;
                }
                else if (i == 1 && int.Parse(MoneyText.text) >= 200)
                {
                    EquipGun.transform.localScale = new Vector3(1, 1, 1);
                    EquipGun.transform.localRotation = Quaternion.Euler(-7, -359, -352);
                    GunMesh.sharedMesh = Equipgun2;  // 총 모양(메쉬값) 2로 설정

                    GunMesh.material = GunShader2;

                    MoneyText.text = (int.Parse(MoneyText.text) - 200).ToString();

                    break;

                }
                else if (i == 2 && int.Parse(MoneyText.text) >= 300)
                {
                    EquipGun.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    EquipGun.transform.localRotation = Quaternion.Euler(172.263f, -178.481f, -180.989f);
                    GunMesh.sharedMesh = Equipgun3;  // 총 모양(메쉬값) 3로 설정

                    GunMesh.material = GunShader3;

                    MoneyText.text = (int.Parse(MoneyText.text) - 300).ToString();

                    break;
                }
                else if (i == 3 && int.Parse(MoneyText.text) >= 50) // 수류탄
                {
                    grenade.SetActive(true);
                    // 수류탄 수 증가 시켜줘야함 게임매니저로 관리하면 좋을거 같음.
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

                MoneyText.text = (int.Parse(MoneyText.text) - (i + 1) * 100).ToString(); // 원래 가지고 있는 돈에서 차감함.
                                                                                         // 산 총의 칼라는 회색으로 하고 다시 못 누르게 해야함.
            }
            else if (int.Parse(MoneyText.text) < (i + 1) * 100)
            {
                // 돈이 부족해 살 수 없습니다 UI 띄우기
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
