using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

public class EXListManager : MonoBehaviour
{
    private string URL = "http://localhost:3000";
    private HttpClient client;

    [SerializeField]
    private TMP_Dropdown sortSelect;

    [SerializeField]
    private Transform exListParent;
    [SerializeField]
    private GameObject exObject;

    [Serializable]
    public class ListDataRoot
    {
        public ListData[] root;
    }
       
    [Serializable]
    public class ListData
    {
        public string QUESTION_NAME;
        public string DUEDATE;
    }

    void Start()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(URL);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        sortSelect.onValueChanged.AddListener(delegate
        {
            ChangeSort(sortSelect);
        });

        GetExList();
    }

    async void GetExList()
    {
        ////GET EX LIST from Server

        //var payload = "{\"userID\": " + LoginManager.UID + "}";
        var payload = "{\"userID\": 41}";
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        var res = await client.PostAsync("exercise/getexlist", c);
        var content = await res.Content.ReadAsStringAsync();
        //var test = JsonUtility.FromJson<ListDataRoot>("{\"root\":" + content + "}");
        print(content);
        var data = JsonUtility.FromJson<ListDataRoot>("{\"root\":" + content + "}");
        DisplayEx(data);
    }

    void DisplayEx(ListDataRoot data)
    {
        for(int i = 0; i< data.root.Length; i++) 
        {
            GameObject exObj = Instantiate(exObject, Vector3.zero, Quaternion.identity, exListParent);
            TMP_Text[] exArray = exObj.GetComponentsInChildren<TMP_Text>();
            exArray[0].text = data.root[i].QUESTION_NAME;
            exArray[1].text = data.root[i].DUEDATE;
        }
    }

    void ChangeSort(TMP_Dropdown change)
    {
        switch (change.value)
        {
            case 0:

                break;
            case 1:

                break;
            default:
                Debug.Log("Value error");
                break;
        }
    }
}
