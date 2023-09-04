using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName;
    public bool open;

    // 화면 열기
    public void Open()
    {
        open = true;
        gameObject.SetActive(open);
    }

    // 화면 닫기
    public void Close()
    {
        open = false;
        gameObject.SetActive(open);
    }
}
