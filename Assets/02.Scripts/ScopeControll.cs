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
        currentState = animator.GetCurrentAnimatorStateInfo(0); // ���� �ִϸ��̼� ����

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(currentAnimationName);
        Debug.Log(scoped);
        if (EquipSniper) // ���� �������̶��
        {
            if (Input.GetMouseButtonDown(1)  && !scoped)  // ������ ���콺 ������ ( ������ ���� �ƴҶ�
            {
                scoped = true;  // scoped = !scoped;
                animator.SetBool("Scoped", scoped); // �ִϸ��̼� ������ �Ķ���� ���ֱ�
            }
            else if(Input.GetMouseButtonDown(1) && scoped) 
            {
                scoped = false;
                animator.SetBool("Scoped", scoped); // �ִϸ��̼� ������ �Ķ���� ���ֱ�
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
        //currentState = animator.GetCurrentAnimatorStateInfo(0); // ���� �ִϸ��̼� ����

        //currentAnimationName = currentState.IsName("Sniper1Scoped") ? "Sniper1Scoped" : "Sniper1scope";


    }


    IEnumerator SniperScope()
    {
        yield return new WaitForSeconds(0.3f);
        Sniper1Scope.SetActive(true); // ���� ���� ui �̹��� ���ֱ�
        sniper1.SetActive(false);  // ���� ������Ʈ�� ���ֱ�
    }
    IEnumerator SniperUnScope()
    {
        
        yield return new WaitForSeconds(0.3f);
        Sniper1Scope.SetActive(false);
        sniper1.SetActive(true);
    }
}

