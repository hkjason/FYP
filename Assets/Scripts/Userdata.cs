using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Userdata : MonoBehaviour
{
    public static Userdata instance { get; private set; }

    public string UID;
    public string USERNAME;
    public string TEACHER_UID;
    public int ROLE_TYPE;

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
}
