using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using UnityEngine.UI;

public class EXReviewManager : MonoBehaviour
{
    private string URL = "http://localhost:3000";
    private HttpClient client;

    [SerializeField] private GameObject reviewObject;


    [Serializable]
    public class ReviewListDataRoot
    {
        public ReviewListData[] root;
    }

    [Serializable]
    public class ReviewListData
    {
        public string RECORD_EID;
        public string ANSWER;
        public string RECORD_ANSWER;
        public string COMPLETION_TIME;
    }

    void Start()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(URL);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


    }

    async void GetReviewList()
    {
        //var payload = "{\"uID\": " + Userdata.instance.UID + "}";
        var payload = "{\"uID\": 41}";
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        var res = await client.PostAsync("exercise/getreviewlist", c);
        var content = await res.Content.ReadAsStringAsync();
        var data = JsonUtility.FromJson<ReviewListDataRoot>("{\"root\":" + content + "}");
        DisplayList(data);
    }

    void DisplayList(ReviewListDataRoot data)
    { 
    
    }
}
