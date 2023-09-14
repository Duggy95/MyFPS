//using Autodesk.Fbx;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;

//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ReloadAnim : MonoBehaviour
{
    Animator animator;
    RigBuilder playerrigbuilder;

    public TwoBoneIKConstraint RightHand;
    public TwoBoneIKConstraint LeftHand;

    public Transform[] MainLeftHandtarget = new Transform[6];
    public Transform[] MainRightHandtarget = new Transform[6];

    public Transform PistolLeftHandTarget;
    public Transform pistolRightHandTarget;

    public Transform KnifeLeftHandTarget;
    public Transform KnifeRightHandTarget;

    public Transform GrenadeLeftHandTarget;

    public GameObject MainWeapon;
    public GameObject Pistol;
    public GameObject Knife;
    public GameObject grenade;

    public GameObject Grip;
    public GameObject PistolGrip;

    public GameObject Spine2;


    public RuntimeAnimatorController mainanim;
    public RuntimeAnimatorController pistolanim;
    public RuntimeAnimatorController knifeanim;
    public RuntimeAnimatorController grenadeanim;

    bool pistolreload = false;
    bool mainreload = false;

    float timer = 0f;
    float lastShootMain;

    bool shoot = true;

    public GameObject camera1;

    Vector3 currentposition;

    //Vector3 targetposition;
    Vector3 originPos; // ���� ���� ����ġ ������
    Vector3 recoilBack;
    Vector3 pistolPos; // ���� ���� ����ġ ������

    Vector3 cameraposition;
    Quaternion camerarotation;

    
    Quaternion originrot; // ���� ���� �����̼�

    public GameObject target;

    Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        playerrigbuilder = GetComponent<RigBuilder>(); 
        animator = GetComponent<Animator>();


        currentposition = Grip.transform.localPosition; // ���� ����ִ� ����
        originPos = Grip.transform.localPosition; // ������ ��ġ, �ݵ� �����Ҷ� �̿�.

        Debug.Log(originPos);

        originrot = PistolGrip.transform.localRotation; // ������ �ݵ� �����Ҷ� �̿�.

        recoilBack = new Vector3(Grip.transform.localPosition.x, Grip.transform.localPosition.y, Grip.transform.localPosition.z - 0.1f); // �̰͵� �ݵ� ������ ���� ����.


        pistolPos = PistolGrip.transform.localPosition; // ���ѹݵ�

        cameraposition = camera1.transform.position; // ī�޶� �����ǰ� �����̼�
        camerarotation = camera1.transform.rotation;

        //Debug.Log("cameraposition : " + cameraposition);
        Debug.Log("camerarotation : " + camerarotation);


        ///////////////////////////////////// �� ó�� ������ �������� �ؾ���.
        RightHand.weight = 1.0f; // ������ ���� ���� ����,  weight 0~1���� ������ 1�� ����������� ik�� ��ǥ Ÿ�ٿ� ���������. 
        LeftHand.weight = 1.0f; // ���� ���� ���� ����, 

        LeftHand.data.target = MainLeftHandtarget[0]; // �޼��� �� ���� ���� ���� �����̷� ����
        RightHand.data.target = MainRightHandtarget[0]; // �������� �� ���� ���� ������ �����̷� ����

        playerrigbuilder.enabled = false; // ���׺��� �ʱ�ȭ, �̰� �ؾ� �޼հ� �������� ��ġ�� �ʱ�ȭ�ȴ�.
        playerrigbuilder.enabled = true;
        ///////////////////////////////////////


        camera = GameObject.Find("Camera1").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
       // targetposition = target.transform.position;
        cameraposition = camera1.transform.localPosition;
        camerarotation = camera1.transform.localRotation;

        //camera.transform.LookAt(targetposition);

        //Debug.Log(shoot);
            

        if (Input.GetKeyDown(KeyCode.R) && MainWeapon.activeSelf) // r ������ ���ο��� �����ϸ�
        {
            mainreloadAnimation(); // ����
        }
        else if (Input.GetKeyDown(KeyCode.R) && Pistol.activeSelf) // r ������ ���� �����ϸ�
        {
            pistolreloadAnimation(); // ���� ����
        }

     


        if (Input.GetKeyDown(KeyCode.Alpha1))  //&& !MainWeapon.activeSelf) 1�� ������
        {
            mainWeaponChange(); // ���ι���� �ٲ۴�.

            camera1.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) //&& !Pistol.activeSelf) // 2�� ������
        {
            pistolChange(); // �������� �ٲ۴�.
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // 3�� ������
        {         
            knifeChange(); // Į�� �ٲ۴�.                    
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) // 4�� ������
        {
            grenadeChange();  //����ź���� �ٲ۴�.
        }




        if (pistolreload && LeftHand.weight <= 0.7f) // ���� ���������̰� ������ ���� �޼��� ��ġ���� 0.7���ϸ�
        {
            LeftHand.weight += Time.fixedDeltaTime / 10;  // weight���� 0�� �������ؼ� ���Ѱ� ���� �������� ����� �ش�. �̰� ���ָ� ���Ѹ� ��� �����۰Ÿ�.

            Debug.Log(LeftHand.weight);
        }
        else if (mainreload && LeftHand.weight <= 1.0f) // �̹��� ���ι��� ���������̰� ���� �����.
        {
            LeftHand.weight += Time.fixedDeltaTime / 10;

            Debug.Log(LeftHand.weight);

        }

        //  timer += Time.deltaTime;
        //  Debug.Log(timer);


        if (MainWeapon.activeSelf) // ���ι��� ����ְ�
        {


            if (Input.GetMouseButton(0) && shoot) // ���콺 ���ʹ�ư Ŭ��(���� ����) + shoot���� Ʈ���
            {
                
                    Shot(); // ���ι���� ���.
                
                

            }
        }
        else if(Pistol.activeSelf) // ���� ����ְ�
        {
            if(Input.GetMouseButtonDown(0) && shoot) // ���콺 ���ʹ�ư Ŭ��(�� ��) + shoot���� Ʈ���
            {
                StartCoroutine(Pistolaa()); // ������ ���.

            }
        }
        


        if (Knife.activeSelf) // Į ����ְ�
        {
            if (Input.GetMouseButtonDown(0)) //���콺 ���� ��ư �� �� Ŭ��
            {
                animator.SetTrigger("Knife"); // Į���Ѵ�.


            }
        }
        else if (grenade.activeSelf) // ����ź ����ְ�
        {
            if (Input.GetMouseButton(0)) // ���콺 ���� ��ư Ŭ��
            {
                animator.SetTrigger("ReadyGrenade"); // ����ź ���� �غ�

            }
            else if (Input.GetMouseButtonUp(0)) // ���콺 ����
            {
                animator.SetTrigger("ThrowGrenade"); // ����ź ������.

            }
        }

    }

    private void Shot()
    {
        
        StartCoroutine(aa());

    }

    IEnumerator mainAttack()
    {
        Vector3 currentposition = Grip.transform.localPosition;



        currentposition.z += -0.1f;
        Grip.transform.localPosition = currentposition;


        yield return null;
        //  yield return new WaitForSeconds(0.05f);


        currentposition.z = 0f;
        Grip.transform.localPosition = currentposition;

    }

    IEnumerator aa() // �ݵ� ���� 
    {
        yield return null;

        //Debug.Log("191");
        //Vector3 currentposition = Grip.transform.localPosition;

        
        //Debug.Log("currentPos : " + currentposition);
        //Debug.Log("originPos : " + originPos);
        //recoilBack = new Vector3(currentposition.x + 1f, currentposition.y, currentposition.z - 1f);


        //while (currentposition.z <= 0f && currentposition.z > -0.1f) // �ѿ� �ݵ� �ֱ� , ���� ����1
        //{
        //    currentposition = Vector3.Lerp(currentposition, recoilBack, 10f);
        //    Grip.transform.localPosition = currentposition;

        //    Debug.Log("200");
        //    Debug.Log(currentposition.z);

        //    shoot = false;

        //    yield return null;
        //}

        //while (currentposition != originPos) // �� ���ڸ��� ������
        //{
        //    currentposition = Vector3.Lerp(currentposition, originPos, 0.3f);
        //    Grip.transform.localPosition = currentposition;

        //    yield return null;
        //    Debug.Log("208");
        //}

        ///////////////////////////////////////////////////////////////////////////////////////

        //while (currentposition.z <= 0.036f && currentposition.z > -0.064f) // �ѿ� �ݵ� �ֱ� , ���� ����2
        //{
        //    currentposition = Vector3.Lerp(currentposition, recoilBack, 10f);
        //    Grip.transform.localPosition = currentposition;

        //    Debug.Log("200");
        //    Debug.Log(currentposition.z);

        //    shoot = false;

        //    yield return null;
        //}

        //while (currentposition != originPos) // �� ���ڸ��� ������
        //{
        //    currentposition = Vector3.Lerp(currentposition, originPos, 0.2f);
        //    Grip.transform.localPosition = currentposition;

        //    yield return null;
        //    Debug.Log("208");
        //}

        ////////////////////////////////////////////////////////////////////////////////////////


        //while (currentposition.z <= 0.273f && currentposition.z >= 0.173) // �ѿ� �ݵ� �ֱ� , ���� ����3
        //{
        //    currentposition = Vector3.Lerp(currentposition, recoilBack, 10f);
        //    Grip.transform.localPosition = currentposition;

        //    Debug.Log("200");
        //    Debug.Log(currentposition.z);

        //    shoot = false;

        //    Debug.Log("364 : " + currentposition);

        //    yield return null;
        //}

        //while (!shoot) // �� ���ڸ��� ������
        //{
        //    currentposition = Vector3.Lerp(currentposition, originPos, 0.02f);
        //    Grip.transform.localPosition = currentposition;

        //    yield return null;
        //    Debug.Log("208");
        //}


        //if (currentposition == originPos)
        //{
        //    shoot = true;

        //}

        //Debug.Log("��");
    }

    IEnumerator Pistolaa() // ���� �ݵ�����
    {
        yield return null;


       // Debug.Log("191");

       // Vector3 currentposition = PistolGrip.transform.localPosition;
       // Vector3 currentposition1 = new Vector3(currentposition.x, pistolPos.y + 0.2f, currentposition.z);

       // Quaternion currentrotation = PistolGrip.transform.localRotation;
       



    



       // Quaternion RecoilRotation = Quaternion.AngleAxis(30f, Vector3.left);
       // Quaternion targetRotation = originrot * RecoilRotation;

       // float elapsedTime = 0f;
       // while(elapsedTime < 0.1f)
       // {
       //     currentposition = Vector3.Lerp(currentposition, currentposition1, elapsedTime / 0.1f);
       //     currentrotation = Quaternion.Lerp(PistolGrip.transform.localRotation, targetRotation, elapsedTime / 0.1f);

       //     PistolGrip.transform.localRotation = currentrotation;
       //     elapsedTime += Time.deltaTime;


       //   //  shoot = false;
       //     yield return null;
       // }

       // //recoilBack = new Vector3(currentposition.x + 1f, currentposition.y, currentposition.z - 1f);

       // while(currentrotation != originrot)
       // {
       //     currentrotation = Quaternion.Slerp(PistolGrip.transform.localRotation, originrot, 0.1f);
       //     PistolGrip.transform.localRotation = currentrotation;

       //   //  shoot = true;
       //     Debug.Log("291");
       // }
       //// PistolGrip.transform.localRotation = originrot;
       // PistolGrip.transform.localPosition = pistolPos;

       // Debug.Log("������");
    }



    void mainreloadAnimation()
    {

        mainreload = true;
        playerrigbuilder.enabled = false; // ���� ������ ��� ���༭ �޼��� �����Ӱ� ���ƴٴϵ��� ����.

        LeftHand.weight = 0f; // ���� ���� �����Ҷ��� �޼��� ��ġ�� ����� ���������ϹǷ� 0�� ��.

        // ���� �ִϸ��̼� ����
        StartCoroutine(startReloadAnim());
    }

    public void resetKnife()
    {
        animator.ResetTrigger("Knife"); // Į�� �ѹ� �ֵθ��� ������ ����Ʈ���Ÿ� ���ش�. �ִϸ��̼� �̺�Ʈ�� ��ϵ�.
    }

    public void resetGrenade()
    {
        animator.ResetTrigger("ReadyGrenade"); // ����ź�� �� �� ������ ���� ������ ���ش�. �ִϸ��̼� �̺�Ʈ�� ��ϵ�.
        animator.ResetTrigger("ThrowGrenade");
    }


    void pistolreloadAnimation() // ���� ���� ����
    {

        pistolreload = true;
        LeftHand.weight = 0f;


        StartCoroutine(startReloadAnim());

    }


    void mainWeaponChange() // ���� ����� ��ü
    {
        //Pistol.SetActive(false);
        //Knife.SetActive(false);
        //grenade.SetActive(false);

        //recoilBack = new Vector3(Grip.transform.localPosition.x, Grip.transform.localPosition.y, Grip.transform.localPosition.z - 0.1f);

      //  Debug.Log("416 : " + targetposition);

        animator.runtimeAnimatorController = mainanim; // ��Ʈ�ѷ��� ���ι��� �����ִ°ŷ� �ٲ��ش�.

       // Debug.Log("420 : " + targetposition);

        camera1.transform.localPosition = cameraposition;
        camera1.transform.localRotation = camerarotation;

       // Debug.Log("425 : " + targetposition);


      //  MainWeapon.SetActive(true);
      //if(GameManager.instance.EquipGunsprite.name == "Gun1")
      //  {
      //      LeftHand.data.target = MainLeftHandtarget[0];
      //      RightHand.data.target = MainRightHandtarget[0];
      //  }
      //else if(GameManager.instance.EquipGunsprite.name == "Gun2")
      //  {
      //      LeftHand.data.target = MainLeftHandtarget[1];
      //      RightHand.data.target = MainRightHandtarget[1];
      //  }
      //else if(GameManager.instance.EquipGunsprite.name == "Gun3")
      //  {
      //      LeftHand.data.target = MainLeftHandtarget[2];
      //      RightHand.data.target = MainRightHandtarget[2];
      //  }
      //  else if (GameManager.instance.EquipGunsprite.name == "Gun4")
      //  {
      //      LeftHand.data.target = MainLeftHandtarget[3];
      //      RightHand.data.target = MainRightHandtarget[3];
      //  }
      //  else if (GameManager.instance.EquipGunsprite.name == "Gun5")
      //  {
      //      LeftHand.data.target = MainLeftHandtarget[4];
      //      RightHand.data.target = MainRightHandtarget[4];
      //  }
      //  else if (GameManager.instance.EquipGunsprite.name == "Gun6")
      //  {
      //      LeftHand.data.target = MainLeftHandtarget[5];
      //      RightHand.data.target = MainRightHandtarget[5];
      //  }

        //Debug.Log(GameManager.instance.EquipGunsprite);

        RightHand.weight = 1.0f; // ���� ������ �����̵鿡 �����հ� �޼� ��ġ
        LeftHand.weight = 1.0f;

        LeftHand.data.target = MainLeftHandtarget[0]; // �޼��� ������ ��ġ
        RightHand.data.target = MainRightHandtarget[0]; // �������� ������ ��ġ

        playerrigbuilder.enabled = false; // ���׺��� �ʱ�ȭ
        playerrigbuilder.enabled = true;

      //  Debug.Log("440 : " + targetposition);
    }

    void pistolChange()
    {

      //  Debug.Log("444 : " + targetposition);

        //MainWeapon.SetActive(false);
        //Knife.SetActive(false);
        //grenade.SetActive(false);


        animator.runtimeAnimatorController = pistolanim; // �ִϸ����͸� ���� ��� �ִ� ������� �ٲ��ش�.

      //  Debug.Log("453 : " + targetposition);

        camera1.transform.localPosition = cameraposition;
        camera1.transform.localRotation = camerarotation;

      //  Debug.Log("458 : " + targetposition);

        //Pistol.SetActive(true);

        RightHand.weight = 1.0f; // �������� 1.0 �޼��� ���� ������ 0.7�� �༭ �ڿ������� ���̰� ���ֱ�.
        LeftHand.weight = 0.7f;

        LeftHand.data.target = PistolLeftHandTarget; // �޼��� ������ ��ġ
        RightHand.data.target = pistolRightHandTarget; // �������� ������ ��ġ 

        playerrigbuilder.enabled = false; // ���׺��� �ʱ�ȭ
        playerrigbuilder.enabled = true;

      //  Debug.Log("473 : " + targetposition);
    }



    void knifeChange() // Į�� ��ü
    {
        //MainWeapon.SetActive(false);
        //Pistol.SetActive(false);
        //grenade.SetActive(false);

        animator.runtimeAnimatorController = knifeanim; // Į ��� �ִ� �ִϸ��̼����� ��ü
        camera1.transform.localPosition = cameraposition;
        camera1.transform.localRotation = camerarotation;
        //Knife.SetActive(true);

        RightHand.weight = 0f; 
        LeftHand.weight = 1f;

        LeftHand.data.target = null;
        RightHand.data.target = KnifeRightHandTarget; // �������� ������ ��ġ�� ���� ���̴�.

        playerrigbuilder.enabled = false; // ���׺��� �ʱ�ȭ
        playerrigbuilder.enabled = true;
    }


    void grenadeChange() // ����ź���� ��ü
    {
        //MainWeapon.SetActive(false);
        //Pistol.SetActive(false);
        //Knife.SetActive(false);

        animator.runtimeAnimatorController = grenadeanim; // ����ź ����ִ� �ִϸ��̼����� ��ü���ش�.
        camera1.transform.localPosition = cameraposition;
        camera1.transform.localRotation = camerarotation;
       //grenade.SetActive(true);

        LeftHand.weight = 1f;
        RightHand.weight = 0f;

        LeftHand.data.target = GrenadeLeftHandTarget; // �޼��� ������ ��ġ
        RightHand.data.target = null;

        playerrigbuilder.enabled = false; // ���׺��� �ʱ�ȭ
        playerrigbuilder.enabled = true;
    }



    IEnumerator startReloadAnim()
    {






        animator.SetBool("Reload1", true); // ���� �Ķ���� �����ֱ�


        yield return new WaitForSeconds(3f);

        animator.SetBool("Reload1", false); 

        Debug.Log("�����Ϸ�");

        if (MainWeapon.activeSelf) // �����Ϸ� �Ŀ��� �����·� �ǵ����ش�.
        {
            LeftHand.data.target = MainLeftHandtarget[0];
            LeftHand.weight = 1.0f;
            mainreload = false;
        }
        else
        {

            LeftHand.weight = 0.7f;
            pistolreload = false;
        }


        playerrigbuilder.enabled = false;
        playerrigbuilder.enabled = true;
    }



}
