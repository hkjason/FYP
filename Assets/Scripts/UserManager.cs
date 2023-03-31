using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    public static UserManager instance { get; private set; }

    public string UID;
    public string USERNAME;
    public int ROLE_TYPE;
    public int COURSEID;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void ResetUserData()
    {
        UID = "";
        USERNAME = "";
        ROLE_TYPE = 0;
        COURSEID = -1;
    }
}
