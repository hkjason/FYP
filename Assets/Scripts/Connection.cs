using DG.Tweening;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Connection : MonoBehaviour
{
    private string URL = "http://localhost:3000";
    private HttpClient client;

    [SerializeField] private GameObject connectionPanel;
    [SerializeField] private GameObject connectedGO;
    [SerializeField] private GameObject notConnectedGO;
    [SerializeField] private GameObject teacherGO;
    [SerializeField] private Transform connectionNoti;

    [SerializeField] private TMP_Text titleText;

    [SerializeField] private TMP_Text connectionNotiText;

    [SerializeField] private TMP_Text connectionCode;
    [SerializeField] private TMP_InputField connectionCodeInput;
    [SerializeField] private Button addButton;

    [SerializeField] private TMP_Text connectedText;

    void Start()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(URL);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        addButton.onClick.AddListener(AddOnClick);
    }

    public void onChange()
    {
        UnactivateAll();

        connectionPanel.SetActive(true);

        if (Userdata.instance.ROLE_TYPE == 0) //Teacher
        {
            titleText.text = "Get Connected";
            GetCode();
        }
        else 
        {
            if (Userdata.instance.TEACHER_UID == "0")
            {
                titleText.text = "Get Connected";
                notConnectedGO.SetActive(true);
            }
            else 
            {
                titleText.text = "Connected";
                GetConnection();
            }
        }
    }

    async void GetCode()
    {
        var payload = "{\"userID\": " + Userdata.instance.UID  + "}";
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        var res = await client.PostAsync("connection/getcode", c);
        var content = await res.Content.ReadAsStringAsync();
        connectionCode.text = "Please ask students to enter the following code to connect.\n\n" + content;
        teacherGO.SetActive(true);
    }


    async void AddOnClick()
    {
        var payload = "{\"userID\": " + Userdata.instance.UID + ", \"code\": \"" + connectionCodeInput.text + "\"}";
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        var res = await client.PostAsync("connection/addconnection", c);
        var content = await res.Content.ReadAsStringAsync();
        if (string.Compare(content, "CODE ERROR") == 0)
        {
            connectionNotiText.text = "Code Error, Please try again.";
            NotiPop();
        }
        else
        {
            Userdata.instance.TEACHER_UID = content;
            connectionNotiText.text = "Connection Success!";
            NotiPop();
            onChange();
        }
    }

    async void GetConnection()
    {
        //DisplayLoading
        var payload = "{\"TUID\": " + Userdata.instance.TEACHER_UID + "}";
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        var res = await client.PostAsync("connection/getconnection", c);
        var content = await res.Content.ReadAsStringAsync();
        connectedText.text = "You are connected with teacher\n" + content;
        connectedGO.SetActive(true);
    }

    void NotiPop()
    {
        DOTween.Kill(connectionNoti);
        connectionNoti.DOMoveY(1693, 0.3f).OnComplete(NotiUnpop);
    }

    void NotiUnpop()
    {
        connectionNoti.DOMoveY(1920, 0.3f).SetDelay(2);
    }

    void UnactivateAll()
    {
        connectedGO.SetActive(false);
        notConnectedGO.SetActive(false);
        teacherGO.SetActive(false);
    }
}