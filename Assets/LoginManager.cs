using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

public class LoginManager : MonoBehaviour
{
    private String URL = "http://localhost:3000";
    private HttpClient client;

    [Serializable]
    private class LoginInfo
    {
        public String userName;
        public String passWord;
    }


    // Start is called before the first frame update
    void Start()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(URL);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        Debug.Log("start");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space");

            //var myContent = JsonUtility.ToJson(data);

            var payload = "{\"CustomerId\": 5,\"CustomerName\": \"Pepsi\"}";

            HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");

            client.PostAsync("login/posttry", c);


            Debug.Log("sent");
        }
    }
}
