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
    Vector3 originPos; // 메인 무기 원위치 포지션
    Vector3 recoilBack;
    Vector3 pistolPos; // 권총 무기 원위치 포지션

    Vector3 cameraposition;
    Quaternion camerarotation;

    
    Quaternion originrot; // 권총 원래 로테이션

    public GameObject target;

    Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        playerrigbuilder = GetComponent<RigBuilder>(); 
        animator = GetComponent<Animator>();


        currentposition = Grip.transform.localPosition; // 현재 잡고있는 무기
        originPos = Grip.transform.localPosition; // 원래의 위치, 반동 구현할때 이용.

        Debug.Log(originPos);

        originrot = PistolGrip.transform.localRotation; // 권총의 반동 구현할때 이용.

        recoilBack = new Vector3(Grip.transform.localPosition.x, Grip.transform.localPosition.y, Grip.transform.localPosition.z - 0.1f); // 이것도 반동 구현에 관련 변수.


        pistolPos = PistolGrip.transform.localPosition; // 권총반동

        cameraposition = camera1.transform.position; // 카메라 포지션과 로테이션
        camerarotation = camera1.transform.rotation;

        //Debug.Log("cameraposition : " + cameraposition);
        Debug.Log("camerarotation : " + camerarotation);


        ///////////////////////////////////// 맨 처음 시작은 주총으로 해야함.
        RightHand.weight = 1.0f; // 오른쪽 손의 리그 빌더,  weight 0~1까지 있으며 1에 가까워질수록 ik가 목표 타겟에 가까워진다. 
        LeftHand.weight = 1.0f; // 왼쪽 손의 리그 빌더, 

        LeftHand.data.target = MainLeftHandtarget[0]; // 왼손이 갈 곳을 총의 왼쪽 손잡이로 고정
        RightHand.data.target = MainRightHandtarget[0]; // 오른손이 갈 곳을 총의 오른쪽 손잡이로 고정

        playerrigbuilder.enabled = false; // 리그빌더 초기화, 이걸 해야 왼손과 오른손의 위치가 초기화된다.
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
            

        if (Input.GetKeyDown(KeyCode.R) && MainWeapon.activeSelf) // r 누르고 메인웨폰 존재하면
        {
            mainreloadAnimation(); // 장전
        }
        else if (Input.GetKeyDown(KeyCode.R) && Pistol.activeSelf) // r 누르고 권총 존재하면
        {
            pistolreloadAnimation(); // 권총 장전
        }

     


        if (Input.GetKeyDown(KeyCode.Alpha1))  //&& !MainWeapon.activeSelf) 1번 누르면
        {
            mainWeaponChange(); // 메인무기로 바꾼다.

            camera1.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) //&& !Pistol.activeSelf) // 2번 누르면
        {
            pistolChange(); // 권총으로 바꾼다.
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // 3번 누르면
        {         
            knifeChange(); // 칼로 바꾼다.                    
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) // 4번 누르면
        {
            grenadeChange();  //수류탄으로 바꾼다.
        }




        if (pistolreload && LeftHand.weight <= 0.7f) // 권총 장전상태이고 권총을 향한 왼손의 위치값이 0.7이하면
        {
            LeftHand.weight += Time.fixedDeltaTime / 10;  // weight값을 0과 가까이해서 권총과 손이 떨어지는 모션을 준다. 이거 안주면 권총만 계속 만지작거림.

            Debug.Log(LeftHand.weight);
        }
        else if (mainreload && LeftHand.weight <= 1.0f) // 이번엔 메인무기 장전상태이고 위와 비슷함.
        {
            LeftHand.weight += Time.fixedDeltaTime / 10;

            Debug.Log(LeftHand.weight);

        }

        //  timer += Time.deltaTime;
        //  Debug.Log(timer);


        if (MainWeapon.activeSelf) // 메인무기 살아있고
        {


            if (Input.GetMouseButton(0) && shoot) // 마우스 왼쪽버튼 클릭(연사 가능) + shoot조건 트루면
            {
                
                    Shot(); // 메인무기로 쏜다.
                
                

            }
        }
        else if(Pistol.activeSelf) // 권총 살아있고
        {
            if(Input.GetMouseButtonDown(0) && shoot) // 마우스 왼쪽버튼 클릭(한 번) + shoot조건 트루면
            {
                StartCoroutine(Pistolaa()); // 권총을 쏜다.

            }
        }
        


        if (Knife.activeSelf) // 칼 살아있고
        {
            if (Input.GetMouseButtonDown(0)) //마우스 왼쪽 버튼 한 번 클릭
            {
                animator.SetTrigger("Knife"); // 칼질한다.


            }
        }
        else if (grenade.activeSelf) // 수류탄 살아있고
        {
            if (Input.GetMouseButton(0)) // 마우스 왼쪽 버튼 클릭
            {
                animator.SetTrigger("ReadyGrenade"); // 수류탄 던질 준비

            }
            else if (Input.GetMouseButtonUp(0)) // 마우스 떼면
            {
                animator.SetTrigger("ThrowGrenade"); // 수류탄 던진다.

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

    IEnumerator aa() // 반동 관련 
    {
        yield return null;

        //Debug.Log("191");
        //Vector3 currentposition = Grip.transform.localPosition;

        
        //Debug.Log("currentPos : " + currentposition);
        //Debug.Log("originPos : " + originPos);
        //recoilBack = new Vector3(currentposition.x + 1f, currentposition.y, currentposition.z - 1f);


        //while (currentposition.z <= 0f && currentposition.z > -0.1f) // 총에 반동 주기 , 메인 무기1
        //{
        //    currentposition = Vector3.Lerp(currentposition, recoilBack, 10f);
        //    Grip.transform.localPosition = currentposition;

        //    Debug.Log("200");
        //    Debug.Log(currentposition.z);

        //    shoot = false;

        //    yield return null;
        //}

        //while (currentposition != originPos) // 총 제자리로 돌리기
        //{
        //    currentposition = Vector3.Lerp(currentposition, originPos, 0.3f);
        //    Grip.transform.localPosition = currentposition;

        //    yield return null;
        //    Debug.Log("208");
        //}

        ///////////////////////////////////////////////////////////////////////////////////////

        //while (currentposition.z <= 0.036f && currentposition.z > -0.064f) // 총에 반동 주기 , 메인 무기2
        //{
        //    currentposition = Vector3.Lerp(currentposition, recoilBack, 10f);
        //    Grip.transform.localPosition = currentposition;

        //    Debug.Log("200");
        //    Debug.Log(currentposition.z);

        //    shoot = false;

        //    yield return null;
        //}

        //while (currentposition != originPos) // 총 제자리로 돌리기
        //{
        //    currentposition = Vector3.Lerp(currentposition, originPos, 0.2f);
        //    Grip.transform.localPosition = currentposition;

        //    yield return null;
        //    Debug.Log("208");
        //}

        ////////////////////////////////////////////////////////////////////////////////////////


        //while (currentposition.z <= 0.273f && currentposition.z >= 0.173) // 총에 반동 주기 , 메인 무기3
        //{
        //    currentposition = Vector3.Lerp(currentposition, recoilBack, 10f);
        //    Grip.transform.localPosition = currentposition;

        //    Debug.Log("200");
        //    Debug.Log(currentposition.z);

        //    shoot = false;

        //    Debug.Log("364 : " + currentposition);

        //    yield return null;
        //}

        //while (!shoot) // 총 제자리로 돌리기
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

        //Debug.Log("탕");
    }

    IEnumerator Pistolaa() // 권총 반동관련
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

       // Debug.Log("권총탕");
    }



    void mainreloadAnimation()
    {

        mainreload = true;
        playerrigbuilder.enabled = false; // 리그 빌더는 잠깐 꺼줘서 왼손이 자유롭게 돌아다니도록 해줌.

        LeftHand.weight = 0f; // 메인 무기 장전할때는 왼손의 위치가 무기와 떨어져야하므로 0을 줌.

        // 장전 애니메이션 시작
        StartCoroutine(startReloadAnim());
    }

    public void resetKnife()
    {
        animator.ResetTrigger("Knife"); // 칼을 한번 휘두르고 나서는 리셋트리거를 해준다. 애니메이션 이벤트에 등록됨.
    }

    public void resetGrenade()
    {
        animator.ResetTrigger("ReadyGrenade"); // 수류탄을 한 번 던지고 나선 리셋을 해준다. 애니메이션 이벤트에 등록됨.
        animator.ResetTrigger("ThrowGrenade");
    }


    void pistolreloadAnimation() // 권총 장전 관련
    {

        pistolreload = true;
        LeftHand.weight = 0f;


        StartCoroutine(startReloadAnim());

    }


    void mainWeaponChange() // 메인 무기로 교체
    {
        //Pistol.SetActive(false);
        //Knife.SetActive(false);
        //grenade.SetActive(false);

        //recoilBack = new Vector3(Grip.transform.localPosition.x, Grip.transform.localPosition.y, Grip.transform.localPosition.z - 0.1f);

      //  Debug.Log("416 : " + targetposition);

        animator.runtimeAnimatorController = mainanim; // 컨트롤러는 메인무기 끼고있는거로 바꿔준다.

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

        RightHand.weight = 1.0f; // 메인 무기의 손잡이들에 오른손과 왼손 위치
        LeftHand.weight = 1.0f;

        LeftHand.data.target = MainLeftHandtarget[0]; // 왼손이 가야할 위치
        RightHand.data.target = MainRightHandtarget[0]; // 오른손이 가야할 위치

        playerrigbuilder.enabled = false; // 리그빌더 초기화
        playerrigbuilder.enabled = true;

      //  Debug.Log("440 : " + targetposition);
    }

    void pistolChange()
    {

      //  Debug.Log("444 : " + targetposition);

        //MainWeapon.SetActive(false);
        //Knife.SetActive(false);
        //grenade.SetActive(false);


        animator.runtimeAnimatorController = pistolanim; // 애니메이터를 권총 들고 있는 모션으로 바꿔준다.

      //  Debug.Log("453 : " + targetposition);

        camera1.transform.localPosition = cameraposition;
        camera1.transform.localRotation = camerarotation;

      //  Debug.Log("458 : " + targetposition);

        //Pistol.SetActive(true);

        RightHand.weight = 1.0f; // 오른손은 1.0 왼손은 조금 떨어진 0.7로 줘서 자연스럽게 보이게 해주기.
        LeftHand.weight = 0.7f;

        LeftHand.data.target = PistolLeftHandTarget; // 왼손이 가야할 위치
        RightHand.data.target = pistolRightHandTarget; // 오른손이 가야할 위치 

        playerrigbuilder.enabled = false; // 리그빌더 초기화
        playerrigbuilder.enabled = true;

      //  Debug.Log("473 : " + targetposition);
    }



    void knifeChange() // 칼로 교체
    {
        //MainWeapon.SetActive(false);
        //Pistol.SetActive(false);
        //grenade.SetActive(false);

        animator.runtimeAnimatorController = knifeanim; // 칼 들고 있는 애니메이션으로 교체
        camera1.transform.localPosition = cameraposition;
        camera1.transform.localRotation = camerarotation;
        //Knife.SetActive(true);

        RightHand.weight = 0f; 
        LeftHand.weight = 1f;

        LeftHand.data.target = null;
        RightHand.data.target = KnifeRightHandTarget; // 오른손이 가야할 위치는 도끼 쪽이다.

        playerrigbuilder.enabled = false; // 리그빌더 초기화
        playerrigbuilder.enabled = true;
    }


    void grenadeChange() // 수류탄으로 교체
    {
        //MainWeapon.SetActive(false);
        //Pistol.SetActive(false);
        //Knife.SetActive(false);

        animator.runtimeAnimatorController = grenadeanim; // 수류탄 들고있는 애니메이션으로 교체해준다.
        camera1.transform.localPosition = cameraposition;
        camera1.transform.localRotation = camerarotation;
       //grenade.SetActive(true);

        LeftHand.weight = 1f;
        RightHand.weight = 0f;

        LeftHand.data.target = GrenadeLeftHandTarget; // 왼손이 가야할 위치
        RightHand.data.target = null;

        playerrigbuilder.enabled = false; // 리그빌더 초기화
        playerrigbuilder.enabled = true;
    }



    IEnumerator startReloadAnim()
    {






        animator.SetBool("Reload1", true); // 장전 파라미터 던져주기


        yield return new WaitForSeconds(3f);

        animator.SetBool("Reload1", false); 

        Debug.Log("장전완료");

        if (MainWeapon.activeSelf) // 장전완료 후에는 원상태로 되돌려준다.
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
