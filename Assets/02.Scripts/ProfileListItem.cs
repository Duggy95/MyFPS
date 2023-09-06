using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileListItem : MonoBehaviour
{
    public string userId;

    public void SetUserId(string nickName)
    {
        userId = nickName;
    }
}
