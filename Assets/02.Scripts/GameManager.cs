using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance; // 싱글톤이 할당될 static 변수. 이 변수는 GameManager 클래스 안에서만 접근가능. 여기서는 위 instance매서드에서 접근 중.

    public string CurrentUserNickName;
    public bool loginSuccess = false;

    // 싱글톤 접근용 프로퍼티
    public static GameManager instance        //GmaeManager타입의 변수 instance는 아래 코드대로 M_instance를 반환하는 스태틱 매서드 역할.
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<GameManager>();
            }
            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)   //주소값으로 비교하는것
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        Debug.Log(CurrentUserNickName);
    }
}
