using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScopeControll : MonoBehaviour
{
    Animator animator;

    public GameObject sniper1;
    public GameObject sniper2;

    bool scoped = false;

    public GameObject Sniper1Scope;
    public GameObject Sniper2Scope;

    AnimatorStateInfo currentState;
    string currentAnimationName;

    bool EquipSniper = true;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        currentState = animator.GetCurrentAnimatorStateInfo(0); // 현재 애니메이션 상태

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(currentAnimationName);
        Debug.Log(scoped);
        if (EquipSniper) // 스나 장착중이라면
        {
            if (Input.GetMouseButtonDown(1)  && !scoped)  // 오른쪽 마우스 누르면 ( 스코프 상태 아닐때
            {
                scoped = true;  // scoped = !scoped;
                animator.SetBool("Scoped", scoped); // 애니메이션 스코프 파라미터 켜주기
            }
            else if(Input.GetMouseButtonDown(1) && scoped) 
            {
                scoped = false;
                animator.SetBool("Scoped", scoped); // 애니메이션 스코프 파라미터 꺼주기
            }
            
            if(scoped)
            {
                StartCoroutine(SniperScope());
            }
            else
            {
                StartCoroutine(SniperUnScope());
            }
            

        }
        //currentState = animator.GetCurrentAnimatorStateInfo(0); // 현재 애니메이션 상태

        //currentAnimationName = currentState.IsName("Sniper1Scoped") ? "Sniper1Scoped" : "Sniper1scope";


    }


    IEnumerator SniperScope()
    {
        yield return new WaitForSeconds(0.3f);
        Sniper1Scope.SetActive(true); // 스나 조준 ui 이미지 켜주기
        sniper1.SetActive(false);  // 스나 오브젝트는 꺼주기
    }
    IEnumerator SniperUnScope()
    {
        
        yield return new WaitForSeconds(0.3f);
        Sniper1Scope.SetActive(false);
        sniper1.SetActive(true);
    }
}

