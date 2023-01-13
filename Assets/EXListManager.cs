using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

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

    void Start()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(URL);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        sortSelect.onValueChanged.AddListener(delegate {
            ChangeSort(sortSelect);
        });

        GetExList();
    }

    async void GetExList()
    {
        var payload = "{\"userID\": " + LoginManager.UID + "}";
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        var res = await client.PostAsync("exercise/getexlist", c);
        var content = await res.Content.ReadAsStringAsync();
        Debug.Log("getexlist: " + content);
    }

    void DisplayEx()
    {
        GameObject exObj = Instantiate(exObject, Vector3.zero, Quaternion.identity, exListParent);
        TMP_Text[] exArray = exObj.GetComponentsInChildren<TMP_Text>();
        exArray[0].text = "ABC";
        exArray[1].text = "TEST";
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
