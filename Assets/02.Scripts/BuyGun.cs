using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BuyGun : MonoBehaviour
{
    public GameObject BuyCheckUI;
    public GameObject YoucantBuy;
    public GameObject grenade;
    public GameObject WeaponRifle;
    public GameObject gun1; // 상점 칸에 있는 총들
    public GameObject gun2;
    public GameObject gun3;
    public GameObject gun4;
    public GameObject gun5;
    public GameObject gun6;
    public GameObject Shotgun;  // 옆에 로봇이 끼고있는 총
    public GameObject Camo;
    public GameObject Sniper;
    public GameObject weaponchoose;
    public GameObject[] Shopguns = new GameObject[6];

    public Sprite[] gunsprites = new Sprite[6];
    public Image[] Greenimages = new Image[8];
    public Image grenadeimage;
    public Image vestImage;
    public Text remainningtime;
    public Text MoneyText;
    public Transform EquipGun;

    public Mesh Equipgun1;
    public Mesh Equipgun2;
    public Mesh Equipgun3;

    public Material GunShader1;
    public Material GunShader2;
    public Material GunShader3;
    public Material HeadArmor; // 흰색 방어구들
    public Material TorsoArmor;
    public Material LegArmor;

    public SkinnedMeshRenderer GunMesh;
    public SkinnedMeshRenderer Head;
    public SkinnedMeshRenderer Torso;
    public SkinnedMeshRenderer Leg;

    Color originColor;
    bool choose = false;
    bool[] guncheck = new bool[8];
    bool[] gunsbuy = new bool[8];
    float maxTime = 20;
    int Totalmoney;

    void Start()
    {
        remainningtime.text = "Buy Time Remaining : 00 : " + maxTime.ToString();

        for (int i = 0; i < 8; i++)
        {
            guncheck[i] = false;
            gunsbuy[i] = false;
        }

        originColor = Greenimages[0].color;
        Totalmoney = int.Parse(MoneyText.text);
    }

    void Update()
    {
        maxTime -= Time.deltaTime;

        if (maxTime < 10) // 00 : 9 이런 식 말고 00 : 09 이런 식으로 보여주려 함.
        {
            remainningtime.text = "Buy Time Remaining : 00 : 0" + (int)maxTime;
        }
        else
        {
            remainningtime.text = "Buy Time Remaining : 00 : " + (int)maxTime;
        }
        if (0 >= maxTime)
        {
            CloseMarket();
        }

        if (choose)
        {
            if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != null &&
                !UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name.Contains("ChooseMainWeapon"))
            {
                weaponchoose.SetActive(false);
                choose = false;
            }
        }

        if(BuyCheckUI.activeSelf)
        {
            Shopguns[4].layer = 0;  ///ui의 레이어 바꾸기
            Transform[] allTransforms = Shopguns[4].GetComponentsInChildren<Transform>(true);

            foreach (Transform childTransform in allTransforms)
            {
                childTransform.gameObject.layer = 0;
            }
        }
        else
        {
            Shopguns[4].layer = 6;  // ui의 레이어 바꾸기
            Transform[] allTransforms = Shopguns[4].GetComponentsInChildren<Transform>(true);

            foreach (Transform childTransform in allTransforms)
            {
                childTransform.gameObject.layer = 6;
            }
        }
    }

    public void CloseMarket()
    {
        BattleManager.instance.Ready();
    }

    public void BuyGunClick()
    {
        if (!BuyCheckUI.activeSelf)
        {
            BuyCheckUI.SetActive(true);
        }

        Debug.Log(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);

        for (int i = 1; i <= 8; i++)
        {
            if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name == "Gun" + i.ToString())
            {
                Debug.Log(i);

                guncheck[i - 1] = true;
                gunsbuy[i - 1] = true;
                if (guncheck[0])
                {
                    WeaponRifle.SetActive(true);
                    Shotgun.SetActive(false);
                    Camo.SetActive(false);
                    Sniper.SetActive(false);

                    EquipGun.transform.localScale = new Vector3(1, 1, 1);
                    EquipGun.transform.localRotation = Quaternion.Euler(95, -381, -376); // 회전값 맞춰주기
                    GunMesh.sharedMesh = Equipgun1; // 총 모양(메쉬값) 1로 설정
                    GunMesh.material = GunShader1;  // 메터리얼도 총 종류에 맞는 메터리얼로 정해줌.
                    guncheck[0] = false;
                    break;
                }
            }
            else if (guncheck[1])
            {
                WeaponRifle.SetActive(true);
                Shotgun.SetActive(false);
                Camo.SetActive(false);
                Sniper.SetActive(false);

                EquipGun.transform.localScale = new Vector3(1, 1, 1);
                EquipGun.transform.localRotation = Quaternion.Euler(-7, -359, -352);
                GunMesh.sharedMesh = Equipgun2;  // 총 모양(메쉬값) 2로 설정
                GunMesh.material = GunShader2;
                guncheck[1] = false;
                break;
            }
            else if (guncheck[2])
            {
                WeaponRifle.SetActive(true);
                Shotgun.SetActive(false);
                Camo.SetActive(false);
                Sniper.SetActive(false);

                EquipGun.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                EquipGun.transform.localRotation = Quaternion.Euler(172.263f, -178.481f, -180.989f);
                GunMesh.sharedMesh = Equipgun3;  // 총 모양(메쉬값) 3로 설정
                GunMesh.material = GunShader3;
                guncheck[2] = false;
                break;
            }
            else if (guncheck[3])
            {
                WeaponRifle.SetActive(false);
                Camo.SetActive(false);
                Sniper.SetActive(false);
                Shotgun.SetActive(true);
                guncheck[3] = false;
            }
            else if (guncheck[4])
            {
                WeaponRifle.SetActive(false);
                Shotgun.SetActive(false);
                Sniper.SetActive(false);
                Camo.SetActive(true);
                guncheck[4] = false;
            }
            else if (guncheck[5])
            {
                WeaponRifle.SetActive(false);
                Shotgun.SetActive(false);
                Camo.SetActive(false);
                Sniper.SetActive(true);
                guncheck[5] = false;
            }
            else if (guncheck[6])
            {
                guncheck[6] = false;
            }
            else if (guncheck[7])
            {
                guncheck[7] = false;
            }
        }
    }

    public void BuyGunYes()
    {
        BuyCheckUI.SetActive(false);

        Debug.Log(int.Parse(MoneyText.text));

        for (int i = 0; i < 8; i++)
        {
            if (gunsbuy[i])
            {
                // 캐릭터가 총들고 있는거 바뀌어야함.
                if (i == 0 && Totalmoney >= 100) // 건1
                {
                    for (int j = 0; j < 6; j++)  // 이 부분은 원래 보유중이었던 상품을 원래대로 되돌리는 역할을 해준다. 총들만 관련있음.
                    {
                        GameObject textobj = this.gameObject.transform.GetChild(j).GetChild(3).gameObject; // 보유중 텍스트

                        if (textobj.activeSelf) // 보유중 글자가 있는지 없는지로 판단
                        {
                            if (j == 0 ^ j == 1 ^ j == 2)
                            {
                                Totalmoney += 100;
                            }
                            else
                            {
                                Totalmoney += 200;
                            }

                            textobj.SetActive(false);

                            RotateGun rotategun1 = Shopguns[j].GetComponent<RotateGun>();
                            rotategun1.enabled = true;                       

                            Button buttons = Greenimages[j].GetComponent<Button>();
                            buttons.enabled = true;

                            Image image3 = Greenimages[j].GetComponent<Image>();

                            Color b = image3.color;

                            b.a = 0.4f;

                            image3.color = b;

                            image3.color = originColor;
                            break;
                        }

                    }




                    EquipGun.transform.localScale = new Vector3(1, 1, 1);
                    EquipGun.transform.localRotation = Quaternion.Euler(95, -381, -376); // 회전값 맞춰주기
                    GunMesh.sharedMesh = Equipgun1; // 총 모양(메쉬값) 1로 설정
                                                    //   gunShader.mainTexture = GunShader1;
                    GunMesh.material = GunShader1;  // 메터리얼도 총 종류에 맞는 메터리얼로 정해줌.

                    Color a = Greenimages[i].color;  // 초록색 이미지의 색깔 투명도를 0으로 해줄거다.

                    a.a = 0;
                    Greenimages[i].color = a;

                    GameObject ownText = Greenimages[i].transform.GetChild(3).gameObject; // 보유 중 텍스트 띄워주기
                    ownText.SetActive(true);

                    RotateGun rotategun = gun1.GetComponent<RotateGun>();  // 총 회전하는 스크립트도 꺼주기

                    gun1.transform.localRotation = Quaternion.Euler(0, -90, 0);

                    rotategun.enabled = false;

                    Greenimages[i].GetComponent<Button>().enabled = false; // 보유중이면 다시 못사게 버튼 누르지 못하게 해주기

                    Totalmoney -= 100;
                    MoneyText.text = Totalmoney.ToString();

                    Image aaa = weaponchoose.GetComponent<Image>();
                    aaa.sprite = gunsprites[0];

                    GameManager.instance.EquipGunsprite = gunsprites[0];

                    break;
                }
                else if (i == 1 && Totalmoney >= 100) //건2
                {
                    for (int j = 0; j < 6; j++)  // 이 부분은 원래 보유중이었던 상품을 원래대로 되돌리는 역할을 해준다. 총들만 관련있음.
                    {
                        GameObject textobj = this.gameObject.transform.GetChild(j).GetChild(3).gameObject;

                        if (textobj.activeSelf)
                        {
                            if (j == 0 ^ j == 1 ^ j == 2)
                            {
                                Totalmoney += 100;
                            }
                            else
                            {
                                Totalmoney += 200;
                            }


                            textobj.SetActive(false);

                            RotateGun rotategun1 = Shopguns[j].GetComponent<RotateGun>(); 
                            rotategun1.enabled = true;

                            Button buttons = Greenimages[j].GetComponent<Button>();
                            buttons.enabled = true;

                            Image image3 = Greenimages[j].GetComponent<Image>();

                            Color b = image3.color;

                            b.a = 0.4f;

                            image3.color = b;

                            image3.color = originColor;


                            break;
                        }

                    }




                    EquipGun.transform.localScale = new Vector3(1, 1, 1);
                    EquipGun.transform.localRotation = Quaternion.Euler(-7, -359, -352);
                    GunMesh.sharedMesh = Equipgun2;  // 총 모양(메쉬값) 2로 설정

                    GunMesh.material = GunShader2;

                    Color a = Greenimages[i].color;

                    a.a = 0;
                    Greenimages[i].color = a;

                        GameObject ownText = Greenimages[i].transform.GetChild(3).gameObject;
                    ownText.SetActive(true);

                    RotateGun rotategun = gun2.GetComponent<RotateGun>();

                    gun2.transform.localRotation = Quaternion.Euler(0, -90, 0);

                    rotategun.enabled = false;

                    //Greenimages[i].GetComponent<Button>().enabled = false; // 보유중이면 다시 못사게 버튼 누르지 못하게 해주기

                    //Image image1 = weaponchoose.transform.GetChild(i).GetComponent<Image>();
                    //Color color1 = image1.color;

                    //color1 = new Color(255, 255, 255, 255);
                    //image1.color = color1;

                    Totalmoney -= 100;
                    MoneyText.text = Totalmoney.ToString();

                    Image aaa = weaponchoose.GetComponent<Image>();
                    aaa.sprite = gunsprites[1];

                    GameManager.instance.EquipGunsprite = gunsprites[1];

                    break;

                }
                else if (i == 2 && Totalmoney >= 100) // 건3
                {
                    for (int j = 0; j < 6; j++)  // 이 부분은 원래 보유중이었던 상품을 원래대로 되돌리는 역할을 해준다. 총들만 관련있음.
                    {
                        GameObject textobj = this.gameObject.transform.GetChild(j).GetChild(3).gameObject;

                        if (textobj.activeSelf)
                        {
                            if (j == 0 ^ j == 1 ^ j == 2)
                            {
                                Totalmoney += 100;
                            }
                            else
                            {
                                Totalmoney += 200;
                            }

                            textobj.SetActive(false);

                            RotateGun rotategun1 = Shopguns[j].GetComponent<RotateGun>();
                            rotategun1.enabled = true;

                            Button buttons = Greenimages[j].GetComponent<Button>();
                            buttons.enabled = true;

                            Image image3 = Greenimages[j].GetComponent<Image>();

                            Color b = image3.color;

                            b.a = 0.4f;

                            image3.color = b;

                            image3.color = originColor;

                            Image aaaa = weaponchoose.GetComponent<Image>();
                            aaaa.sprite = gunsprites[2];

                            

                            break;
                        }

                    }

                    EquipGun.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    EquipGun.transform.localRotation = Quaternion.Euler(172.263f, -178.481f, -180.989f);
                    GunMesh.sharedMesh = Equipgun3;  // 총 모양(메쉬값) 3로 설정

                    GunMesh.material = GunShader3;

                    Color a = Greenimages[i].color;

                    a.a = 0;
                    Greenimages[i].color = a;

                    GameObject ownText = Greenimages[i].transform.GetChild(3).gameObject;
                    ownText.SetActive(true);

                    RotateGun rotategun = gun3.GetComponent<RotateGun>();

                    gun3.transform.localRotation = Quaternion.Euler(-86, -90, 0);

                    rotategun.enabled = false;

                    Greenimages[i].GetComponent<Button>().enabled = false; // 보유중이면 다시 못사게 버튼 누르지 못하게 해주기

                    Totalmoney -= 100;
                    MoneyText.text = Totalmoney.ToString();
                    Image aaa = weaponchoose.GetComponent<Image>();
                    aaa.sprite = gunsprites[2];

                    GameManager.instance.EquipGunsprite = gunsprites[2];

                    break;
                }
                else if (i == 3 && Totalmoney >= 200) // 샷건
                {
                    for (int j = 0; j < 6; j++)  // 이 부분은 원래 보유중이었던 상품을 원래대로 되돌리는 역할을 해준다. 총들만 관련있음.
                    {
                        GameObject textobj = this.gameObject.transform.GetChild(j).GetChild(3).gameObject;

                        if (textobj.activeSelf)
                        {
                            if (j == 0 ^ j == 1 ^ j == 2 )
                            {
                                Totalmoney += 100;
                            }
                            else
                            {
                                Totalmoney += 200;
                            }
                            


                            textobj.SetActive(false);

                            RotateGun rotategun1 = Shopguns[j].GetComponent<RotateGun>();
                            rotategun1.enabled = true;

                            Button buttons = Greenimages[j].GetComponent<Button>();
                            buttons.enabled = true;

                            Image image3 = Greenimages[j].GetComponent<Image>();

                            Color b = image3.color;

                            b.a = 0.4f;

                            image3.color = b;

                            image3.color = originColor;
                            break;
                        }

                    }


                    Color a = Greenimages[i].color;

                    a.a = 0;
                    Greenimages[i].color = a;

                    GameObject ownText = Greenimages[i].transform.GetChild(3).gameObject;
                    ownText.SetActive(true);

                    RotateGun rotategun = gun4.GetComponent<RotateGun>();

                    gun4.transform.localRotation = Quaternion.Euler(0, 180, 0);

                    rotategun.enabled = false;

                    Greenimages[i].GetComponent<Button>().enabled = false; // 보유중이면 다시 못사게 버튼 누르지 못하게 해주기

                    Totalmoney -= 200;
                    MoneyText.text = Totalmoney.ToString();
                    Image aaa = weaponchoose.GetComponent<Image>();
                    aaa.sprite = gunsprites[3];

                    GameManager.instance.EquipGunsprite = gunsprites[3];

                    break;
                }
                else if (i == 4 && Totalmoney >= 200) // camo
                {
                    for (int j = 0; j < 6; j++)  // 이 부분은 원래 보유중이었던 상품을 원래대로 되돌리는 역할을 해준다. 총들만 관련있음.
                    {
                        GameObject textobj = this.gameObject.transform.GetChild(j).GetChild(3).gameObject;

                        if (textobj.activeSelf)
                        {
                            if (j == 0 ^ j == 1 ^ j == 2)
                            {
                                Totalmoney += 100;
                            }
                            else
                            {
                                Totalmoney += 200;
                            }

                            textobj.SetActive(false);

                            RotateGun rotategun1 = Shopguns[j].GetComponent<RotateGun>();
                            rotategun1.enabled = true;

                            Button buttons = Greenimages[j].GetComponent<Button>();
                            buttons.enabled = true;

                            Image image3 = Greenimages[j].GetComponent<Image>();

                            Color b = image3.color;

                            b.a = 0.4f;

                            image3.color = b;

                            image3.color = originColor;
                            break;
                        }

                    }

                    Color a = Greenimages[i].color;

                    a.a = 0;
                    Greenimages[i].color = a;

                    GameObject ownText = Greenimages[i].transform.GetChild(3).gameObject;
                    ownText.SetActive(true);

                    RotateGun rotategun = gun5.GetComponent<RotateGun>();

                    gun5.transform.localRotation = Quaternion.Euler(0, -90, 0);

                    rotategun.enabled = false;

                    Greenimages[i].GetComponent<Button>().enabled = false; // 보유중이면 다시 못사게 버튼 누르지 못하게 해주기

                    Totalmoney -= 200;
                    MoneyText.text = Totalmoney.ToString();
                    Image aaa = weaponchoose.GetComponent<Image>();
                    aaa.sprite = gunsprites[4];

                    GameManager.instance.EquipGunsprite = gunsprites[4];

                    break;
                }
                else if (i == 5 && Totalmoney >= 200) // 고급 스나
                {
                    for (int j = 0; j < 6; j++)  // 이 부분은 원래 보유중이었던 상품을 원래대로 되돌리는 역할을 해준다. 총들만 관련있음.
                    {
                        GameObject textobj = this.gameObject.transform.GetChild(j).GetChild(3).gameObject;

                        if (textobj.activeSelf)
                        {
                            if (j == 0 ^ j == 1 ^ j == 2)
                            {
                                Totalmoney += 100;
                            }
                            else
                            {
                                Totalmoney += 200;
                            }

                            textobj.SetActive(false);

                            RotateGun rotategun1 = Shopguns[j].GetComponent<RotateGun>();
                            rotategun1.enabled = true;

                            Button buttons = Greenimages[j].GetComponent<Button>();
                            buttons.enabled = true;

                            Image image3 = Greenimages[j].GetComponent<Image>();

                            Color b = image3.color;

                            b.a = 0.4f;

                            image3.color = b;

                            image3.color = originColor;
                            Image aaaa = weaponchoose.GetComponent<Image>();
                            aaaa.sprite = gunsprites[0];
                            break;
                        }

                    }
                    Color a = Greenimages[i].color;

                    a.a = 0;
                    Greenimages[i].color = a;
                    
                    GameObject ownText = Greenimages[i].transform.GetChild(3).gameObject;
                    ownText.SetActive(true);

                    RotateGun rotategun = gun6.GetComponent<RotateGun>();

                    gun6.transform.localRotation = Quaternion.Euler(0, 180, 0);

                    rotategun.enabled = false;

                    Greenimages[i].GetComponent<Button>().enabled = false; // 보유중이면 다시 못사게 버튼 누르지 못하게 해주기


                    Totalmoney -= 200;
                    MoneyText.text = Totalmoney.ToString();

                    Image aaa = weaponchoose.GetComponent<Image>();
                    aaa.sprite = gunsprites[5];

                    GameManager.instance.EquipGunsprite = gunsprites[5];

                    break;
                }
                else if (i == 6 && Totalmoney >= 50) // 수류탄
                {
                    grenade.SetActive(true);
                    // 수류탄 수 증가 시켜줘야함 게임매니저로 관리하면 좋을거 같음.

                    Color a = Greenimages[i].color;

                    a.a = 0;
                    Greenimages[i].color = a;

                    GameObject ownText = Greenimages[i].transform.GetChild(3).gameObject;
                    ownText.SetActive(true);

                    Greenimages[i].GetComponent<Button>().enabled = false; // 보유중이면 다시 못사게 버튼 누르지 못하게 해주기

                    Totalmoney -= 50;
                    MoneyText.text = Totalmoney.ToString();

                    Color b = grenadeimage.color;

                    b = new Color(255, 255, 255, 255);
                    grenadeimage.color = b;

                    GameManager.instance.HasGrenade = true;

                    break;
                }
                else if (i == 7 && Totalmoney >= 100) // 아머
                {
                    Head.material = HeadArmor;
                    Torso.material = TorsoArmor;
                    Leg.material = LegArmor;

                    Color a = Greenimages[i].color;

                    a.a = 0;
                    Greenimages[i].color = a;

                    GameObject ownText = Greenimages[i].transform.GetChild(4).gameObject;
                    ownText.SetActive(true);

                    Greenimages[i].GetComponent<Button>().enabled = false; // 보유중이면 다시 못사게 버튼 누르지 못하게 해주기

                    Totalmoney -= 100;
                    MoneyText.text = Totalmoney.ToString();

                    Color b = vestImage.color;

                    b = new Color(255, 255, 255, 255);
                    vestImage.color = b;

                    GameManager.instance.HasArmor = true;

                    break;
                }
                
            }
            else if (int.Parse(MoneyText.text) < (i + 1) * 100)
            {
                // 돈이 부족해 살 수 없습니다 UI 띄우기
                YoucantBuy.SetActive(true);
            }
        }

        for (int i = 0; i < 8; i++)
        {
            gunsbuy[i] = false;
        }
    }

    public void BuyGunNo()
    {
        BuyCheckUI.SetActive(false);

        for (int i = 0; i < 8; i++)
        {
            gunsbuy[i] = false;
        }
    }

    public void youcantbuythisCheckYes()
    {
        YoucantBuy.SetActive(false);
    }

    public void MainWeaponChoose()
    {
        Debug.Log("click");

        GameObject weaponchoose = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.GetChild(0).gameObject;
        weaponchoose.SetActive(true);

        RectTransform recttransform = weaponchoose.GetComponent<RectTransform>();

        Debug.Log(recttransform.sizeDelta.y);

        Vector3 a = recttransform.localScale;

        while (recttransform.localScale.y <= 1)
        {
            a.y += 0.0001f;
            recttransform.localScale = a;
        }
    }

    //public void Choosing()
    //{
    //    choose = true;

    //    Debug.Log(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject);

    //    for (int i = 1; i <= 6; i++)
    //    {
    //        if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name == "ChooseMainWeapon" + i.ToString())
    //        {
    //            if (i == 1 && Greenimages[i - 1].transform.GetChild(3).gameObject.activeSelf)
    //            {
    //                WeaponRifle.SetActive(true);
    //                Shotgun.SetActive(false);
    //                Camo.SetActive(false);
    //                Sniper.SetActive(false);

    //                EquipGun.transform.localScale = new Vector3(1, 1, 1);
    //                EquipGun.transform.localRotation = Quaternion.Euler(95, -381, -376); // 회전값 맞춰주기
    //                GunMesh.sharedMesh = Equipgun1; // 총 모양(메쉬값) 1로 설정
    //                                                //   gunShader.mainTexture = GunShader1;
    //                GunMesh.material = GunShader1;  // 메터리얼도 총 종류에 맞는 메터리얼로 정해줌.

    //                Image image1 = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>(); // 고른 무기 이미지를 이것과 동일하게 한다.

    //                Image image2 = weaponchoose.transform.parent.GetComponent<Image>();
    //                image2.sprite = image1.sprite;

    //                weaponchoose.SetActive(false);
    //            }
    //            else if (i == 2 && Greenimages[i - 1].transform.GetChild(3).gameObject.activeSelf)
    //            {
    //                WeaponRifle.SetActive(true);
    //                Shotgun.SetActive(false);
    //                Camo.SetActive(false);
    //                Sniper.SetActive(false);

    //                EquipGun.transform.localScale = new Vector3(1, 1, 1);
    //                EquipGun.transform.localRotation = Quaternion.Euler(-7, -359, -352);
    //                GunMesh.sharedMesh = Equipgun2;  // 총 모양(메쉬값) 2로 설정

    //                GunMesh.material = GunShader2;

    //                Image image1 = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>(); // 고른 무기 이미지를 이것과 동일하게 한다.

    //                Image image2 = weaponchoose.transform.parent.GetComponent<Image>();
    //                image2.sprite = image1.sprite;

    //                weaponchoose.SetActive(false);
    //            }
    //            else if (i == 3 && Greenimages[i - 1].transform.GetChild(3).gameObject.activeSelf)
    //            {
    //                WeaponRifle.SetActive(true);
    //                Shotgun.SetActive(false);
    //                Camo.SetActive(false);
    //                Sniper.SetActive(false);

    //                EquipGun.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    //                EquipGun.transform.localRotation = Quaternion.Euler(172.263f, -178.481f, -180.989f);
    //                GunMesh.sharedMesh = Equipgun3;  // 총 모양(메쉬값) 3로 설정
    //                GunMesh.material = GunShader3;

    //                Image image1 = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>(); // 고른 무기 이미지를 이것과 동일하게 한다.
    //                Image image2 = weaponchoose.transform.parent.GetComponent<Image>();
    //                image2.sprite = image1.sprite;

    //                weaponchoose.SetActive(false);
    //            }
    //            else if (i == 4 && Greenimages[i - 1].transform.GetChild(3).gameObject.activeSelf)
    //            {
    //                WeaponRifle.SetActive(false);
    //                Camo.SetActive(false);
    //                Sniper.SetActive(false);

    //                Shotgun.SetActive(true);

    //                Image image1 = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>(); // 고른 무기 이미지를 이것과 동일하게 한다.
    //                Image image2 = weaponchoose.transform.parent.GetComponent<Image>();
    //                image2.sprite = image1.sprite;

    //                weaponchoose.SetActive(false);
    //            }
    //            else if (i == 5 && Greenimages[i - 1].transform.GetChild(3).gameObject.activeSelf)
    //            {
    //                WeaponRifle.SetActive(false);
    //                Shotgun.SetActive(false);
    //                Sniper.SetActive(false);
    //                Camo.SetActive(true);

    //                Image image1 = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>(); // 고른 무기 이미지를 이것과 동일하게 한다.
    //                Image image2 = weaponchoose.transform.parent.GetComponent<Image>();
    //                image2.sprite = image1.sprite;

    //                weaponchoose.SetActive(false);
    //            }
    //            else if (i == 6 && Greenimages[i - 1].transform.GetChild(3).gameObject.activeSelf)
    //            {
    //                WeaponRifle.SetActive(false);
    //                Shotgun.SetActive(false);
    //                Camo.SetActive(false);
    //                Sniper.SetActive(true);

    //                Image image1 = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>(); // 고른 무기 이미지를 이것과 동일하게 한다.
    //                Image image2 = weaponchoose.transform.parent.GetComponent<Image>();
    //                image2.sprite = image1.sprite;

    //                weaponchoose.SetActive(false);
    //            }
    //        }
    //    }
    //}
}