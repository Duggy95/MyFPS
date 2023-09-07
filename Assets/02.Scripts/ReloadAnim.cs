using Autodesk.Fbx;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ReloadAnim : MonoBehaviour
{
    Animator animator;
    RigBuilder playerrigbuilder;

    public TwoBoneIKConstraint RightHand;
    public TwoBoneIKConstraint LeftHand;

    public Transform MainLeftHandtarget;
    public Transform MainRightHandtarget;

    public Transform PistolLeftHandTarget;
    public Transform pistolRightHandTarget;

    public Transform KnifeLeftHandTarget;
    public Transform KnifeRightHandTarget;

    public Transform GrenadeRightHandTarget;

    public GameObject MainWeapon;
    public GameObject Pistol;
    public GameObject Knife;
    public GameObject grenade;

    public GameObject Grip;
    public GameObject PistolGrip;

    public AnimatorController mainanim;
    public AnimatorController pistolanim;
    public AnimatorController knifeanim;
    public AnimatorController grenadeanim;

    bool pistolreload = false;
    bool mainreload = false;

    float timer = 0f;
    float lastShootMain;

    bool shoot = true;


    Vector3 currentposition;
    Vector3 originPos; // 메인 무기 원위치 포지션
    Vector3 recoilBack;
    Vector3 pistolPos; // 권총 무기 원위치 포지션

    
    Quaternion originrot; // 권총 원래 로테이션

    // Start is called before the first frame update
    void Start()
    {
        playerrigbuilder = GetComponent<RigBuilder>();
        animator = GetComponent<Animator>();


        currentposition = Grip.transform.localPosition;
        originPos = Grip.transform.localPosition;
        originrot = PistolGrip.transform.localRotation;

        recoilBack = new Vector3(Grip.transform.localPosition.x, Grip.transform.localPosition.y, Grip.transform.localPosition.z - 0.1f);


        pistolPos = PistolGrip.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && MainWeapon.activeSelf)
        {
            mainreloadAnimation();
        }
        else if (Input.GetKeyDown(KeyCode.R) && Pistol.activeSelf)
        {
            pistolreloadAnimation();
        }




        if (Input.GetKeyDown(KeyCode.Alpha1))  //&& !MainWeapon.activeSelf)
        {
            mainWeaponChange();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) //&& !Pistol.activeSelf)
        {
            pistolChange();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            knifeChange();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            grenadeChange();
        }




        if (pistolreload && LeftHand.weight <= 0.7f)
        {
            LeftHand.weight += Time.fixedDeltaTime / 10;

            Debug.Log(LeftHand.weight);
        }
        else if (mainreload && LeftHand.weight <= 1.0f)
        {
            LeftHand.weight += Time.fixedDeltaTime / 10;

            Debug.Log(LeftHand.weight);

        }

        //  timer += Time.deltaTime;
        //  Debug.Log(timer);


        if (MainWeapon.activeSelf)
        {


            if (Input.GetMouseButton(0) && shoot)
            {
                
                    Shot();
                
                

            }
        }
        else if(Pistol.activeSelf)
        {
            if(Input.GetMouseButtonDown(0) && shoot)
            {
                StartCoroutine(Pistolaa());

            }
        }
        


        if (Knife.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("Knife");


            }
        }
        else if (grenade.activeSelf)
        {
            if (Input.GetMouseButton(0))
            {
                animator.SetTrigger("ReadyGrenade");

            }
            else if (Input.GetMouseButtonUp(0))
            {
                animator.SetTrigger("ThrowGrenade");

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

    IEnumerator aa()
    {

        Debug.Log("191");
        Vector3 currentposition = Grip.transform.localPosition;
        
        //recoilBack = new Vector3(currentposition.x + 1f, currentposition.y, currentposition.z - 1f);


        while (currentposition.z <= 0f && currentposition.z > -0.1f) // 총에 반동 주기
        {
            currentposition = Vector3.Lerp(currentposition, recoilBack, 10f);
            Grip.transform.localPosition = currentposition;

            Debug.Log("200");
            Debug.Log(currentposition.z);

            shoot = false;

            yield return null;
        }

        while (currentposition != originPos) // 총 제자리로 돌리기
        {
            currentposition = Vector3.Lerp(currentposition, originPos, 0.3f);
            Grip.transform.localPosition = currentposition;

            yield return null;
            Debug.Log("208");
        }

        if (currentposition == originPos)
        {
            shoot = true;

        }

        //yield return null;
     
        //while (currentposition != originPos)
        //{
        //    currentposition = Vector3.Lerp(currentposition, originPos, 0.1f);
        //    yield return null;
        //}
    }

    IEnumerator Pistolaa()
    {
     


        Debug.Log("191");

        Vector3 currentposition = PistolGrip.transform.localPosition;
        Vector3 currentposition1 = new Vector3(currentposition.x, pistolPos.y + 0.2f, currentposition.z);

        Quaternion currentrotation = PistolGrip.transform.localRotation;
       



    



        Quaternion RecoilRotation = Quaternion.AngleAxis(30f, Vector3.left);
        Quaternion targetRotation = originrot * RecoilRotation;

        float elapsedTime = 0f;
        while(elapsedTime < 0.1f)
        {
            currentposition = Vector3.Lerp(currentposition, currentposition1, elapsedTime / 0.1f);
            currentrotation = Quaternion.Lerp(PistolGrip.transform.localRotation, targetRotation, elapsedTime / 0.1f);

            PistolGrip.transform.localRotation = currentrotation;
            elapsedTime += Time.deltaTime;


          //  shoot = false;
            yield return null;
        }

        //recoilBack = new Vector3(currentposition.x + 1f, currentposition.y, currentposition.z - 1f);

        while(currentrotation != originrot)
        {
            currentrotation = Quaternion.Slerp(PistolGrip.transform.localRotation, originrot, 0.1f);
            PistolGrip.transform.localRotation = currentrotation;

          //  shoot = true;
            Debug.Log("291");
        }
       // PistolGrip.transform.localRotation = originrot;
        PistolGrip.transform.localPosition = pistolPos;
    }



    void mainreloadAnimation()
    {

        mainreload = true;
        playerrigbuilder.enabled = false;

        LeftHand.weight = 0f;

        // 장전 애니메이션 시작
        StartCoroutine(startReloadAnim());
    }

    public void resetKnife()
    {
        animator.ResetTrigger("Knife");
    }

    public void resetGrenade()
    {
        animator.ResetTrigger("ReadyGrenade");
        animator.ResetTrigger("ThrowGrenade");
    }


    void pistolreloadAnimation()
    {

        pistolreload = true;
        LeftHand.weight = 0f;


        StartCoroutine(startReloadAnim());

    }


    void mainWeaponChange()
    {
        Pistol.SetActive(false);
        Knife.SetActive(false);
        grenade.SetActive(false);


        animator.runtimeAnimatorController = mainanim; // 컨트롤러는 메인무기 끼고있는거로 바꿔준다.
        MainWeapon.SetActive(true);


        RightHand.weight = 1.0f;
        LeftHand.weight = 1.0f;

        LeftHand.data.target = MainLeftHandtarget;
        RightHand.data.target = MainRightHandtarget;

        playerrigbuilder.enabled = false; // 리그빌더 초기화
        playerrigbuilder.enabled = true;
    }

    void pistolChange()
    {
        MainWeapon.SetActive(false);
        Knife.SetActive(false);
        grenade.SetActive(false);

        animator.runtimeAnimatorController = pistolanim;
        Pistol.SetActive(true);

        RightHand.weight = 1.0f;
        LeftHand.weight = 0.7f;

        LeftHand.data.target = PistolLeftHandTarget;
        RightHand.data.target = pistolRightHandTarget;

        playerrigbuilder.enabled = false; // 리그빌더 초기화
        playerrigbuilder.enabled = true;


    }



    void knifeChange()
    {
        MainWeapon.SetActive(false);
        Pistol.SetActive(false);
        grenade.SetActive(false);

        animator.runtimeAnimatorController = knifeanim;
        Knife.SetActive(true);

        RightHand.weight = 0f;
        LeftHand.weight = 1f;

        LeftHand.data.target = null;
        RightHand.data.target = KnifeRightHandTarget;

        playerrigbuilder.enabled = false; // 리그빌더 초기화
        playerrigbuilder.enabled = true;
    }


    void grenadeChange()
    {
        MainWeapon.SetActive(false);
        Pistol.SetActive(false);
        Knife.SetActive(false);

        animator.runtimeAnimatorController = grenadeanim;
        grenade.SetActive(true);

        LeftHand.weight = 1f;
        RightHand.weight = 0f;

        LeftHand.data.target = null;
        RightHand.data.target = GrenadeRightHandTarget;

        playerrigbuilder.enabled = false; // 리그빌더 초기화
        playerrigbuilder.enabled = true;
    }



    IEnumerator startReloadAnim()
    {






        animator.SetBool("Reload1", true);


        yield return new WaitForSeconds(3f);

        animator.SetBool("Reload1", false);

        Debug.Log("장전완료");

        if (MainWeapon.activeSelf)
        {
            LeftHand.data.target = MainLeftHandtarget;
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
