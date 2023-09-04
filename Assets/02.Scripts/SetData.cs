using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SetData : MonoBehaviour
{

    string _Account_Reg = "http://192.168.106.21:80/FPSInsertUser.php"; // 회원가입
    string _Account_Get = "http://192.168.106.21:80/FPSGetUser.php";  // 로그인

    public InputField Sign_ID_Value;
    public InputField Sign_PW_Value;
    public InputField NickName;
    public InputField LoginID;
    public InputField LoginPW;

    public GameObject _CheckStatus;
    public GameObject SignupMenu;
    public GameObject LoginMenu;
    public GameObject Entrance;

    Text _CheckStatusTxt;

    private void Awake()
    {
        _CheckStatusTxt = _CheckStatus.GetComponentInChildren<Text>();
        _CheckStatus.gameObject.SetActive(false);
    }

    public void SendAccount() // 회원가입
    {
        // StartCoroutine(SetAccout(_ID_Value.text, _PW_Value.text, _NAME_Value.text, _AGE_Value.text));
        StartCoroutine(SetAccout(Sign_ID_Value.text, Sign_PW_Value.text, NickName.text));
    }

    //IEnumerator SetAccout(string _id, string _pw, string _name, string _age)
    IEnumerator SetAccout(string _id, string _pw, string _nick)
    {
        WWWForm _PostData = new WWWForm();

        _PostData.AddField("ID_Value", _id.Trim());
        _PostData.AddField("PW_Value", _pw.Trim());
        _PostData.AddField("NickName", _nick.Trim());
        //  _PostData.AddField("NAME_Value", _name.Trim());
        //  _PostData.AddField("AGE_Value", _age.Trim());

        using (UnityWebRequest _sendData = UnityWebRequest.Post(_Account_Reg, _PostData))
        {
            yield return _sendData.SendWebRequest();

            if (_sendData.isNetworkError || _sendData.isHttpError)
            {
                _CheckStatus.gameObject.SetActive(true);
                _CheckStatusTxt.text = _sendData.error;
            }
            else
            {
                if (_sendData.downloadHandler.text.Equals("you Login Success"))
                {
                    StartCoroutine(LoginResult("Complete"));
                }
                else
                {
                    _CheckStatus.gameObject.SetActive(true);
                    _CheckStatusTxt.text = _sendData.downloadHandler.text;
                    SignupMenu.SetActive(false);
                    LoginMenu.SetActive(true);

                    if (_sendData.downloadHandler.text == "1")
                    {
                        print("이미 존재하는 회원");
                    }
                }
            }
            _sendData.Dispose();
        }
    }

    public void GetAccount() // 로그인
    {
        StartCoroutine(GetAccout(LoginID.text, LoginPW.text));
    }

    IEnumerator GetAccout(string _id, string _pw)
    {
        WWWForm _PostData = new WWWForm();
        _PostData.AddField("ID_Value", _id.Trim());
        _PostData.AddField("PW_Value", _pw.Trim());

        using (UnityWebRequest _sendData = UnityWebRequest.Post(_Account_Get, _PostData))
        {
            yield return _sendData.SendWebRequest();

            if (_sendData.isNetworkError || _sendData.isHttpError)
            {
                _CheckStatusTxt.text = _sendData.error;
            }
            else
            {
                if (_sendData.downloadHandler.text.Equals("99"))
                {
                    StartCoroutine(LoginResult("Login Fail"));
                }
                else
                {
                    StartCoroutine(LoginResult("로그인 성공!"));

                    GameManager.instance.CurrentUserNickName = _sendData.downloadHandler.text;
                    GameManager.instance.loginSuccess = true;
                    Launcher.Instance.Login(_sendData.downloadHandler.text);

                    LoginMenu.SetActive(false);
                    Entrance.SetActive(true);
                }
            }
            _sendData.Dispose();
        }
    }

    IEnumerator LoginResult(string result)
    {
        _CheckStatus.gameObject.SetActive(true);
        _CheckStatusTxt.text = result;

        yield return new WaitForSeconds(2f);

        _CheckStatus.gameObject.SetActive(false);
    }
}
