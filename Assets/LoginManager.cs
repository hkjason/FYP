using System;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    private string URL = "http://localhost:3000";
    private HttpClient client;

    [SerializeField]
    private Button loginButton;
    [SerializeField]
    private Button regButton;
    [SerializeField]
    private TMP_InputField loginAccount;
    [SerializeField]
    private TMP_InputField loginPassword;
    [SerializeField]
    private TMP_InputField regAccount;
    [SerializeField]
    private TMP_InputField regPassword;
    [SerializeField]
    private TMP_InputField regConfirmPassword;

    [SerializeField]
    private TMP_Text loginInfo;
    [SerializeField]
    private TMP_Text regInfo;

    [SerializeField]
    private GameObject loginPanel;
    [SerializeField]
    private GameObject regPanel;

    [SerializeField]
    private Button noAccButton;
    [SerializeField]
    private Button loginNowButton;

    void Start()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(URL);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        Button btn;
        btn = loginButton.GetComponent<Button>();
        btn.onClick.AddListener(LoginOnClick);
        btn = regButton.GetComponent<Button>();
        btn.onClick.AddListener(RegOnClick);
        btn = noAccButton.GetComponent<Button>();
        btn.onClick.AddListener(noAccOnClick);
        btn = loginNowButton.GetComponent<Button>();
        btn.onClick.AddListener(loginNowOnClick);
    }

    //PAYLOAD DEMO
    //var payload = "{\"CustomerId\": 5,\"CustomerName\": \"Pepsi\"}";
    async void LoginOnClick()
    {
        List<string> str = new List<string> { "AccountName", loginAccount.text, "PassWord", loginPassword.text };
        var payload = StringEncoder(str);
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        var res = await client.PostAsync("login/trylogin", c);
        var content = await res.Content.ReadAsStringAsync();

        if (string.Compare(content, "login successful") == 0)
        {
            loginInfo.text = "";
            loginAccount.text = "";
            loginPassword.text = "";
            SceneManager.LoadScene(1);
        }
        else if (string.Compare(content, "incorrect password") == 0)
        {
            loginInfo.text = "Incorrect password";
            loginPassword.text = "";
        }
        else if (string.Compare(content, "user not found") == 0)
        {
            loginInfo.text = "User not found";
            loginAccount.text = "";
            loginPassword.text = "";
        }
        else
        {
            Debug.Log("LOGIN ERROR");
            return;
        }
    }

    async void RegOnClick()
    {
        if (string.Compare(regPassword.text, regConfirmPassword.text) != 0)
        {
            regPassword.text = "";
            regConfirmPassword.text = "";
            regInfo.text = "Password not same";
            return;
        }

        List<string> str = new List<string> { "AccountName", regAccount.text, "PassWord", regPassword.text };
        var payload = StringEncoder(str);
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        var res = await client.PostAsync("login/tryregister", c);
        var content = await res.Content.ReadAsStringAsync();

        if (string.Compare(content, "register successful") == 0)
        {
            regInfo.text = "";
            regAccount.text = "";
            regPassword.text = "";
            regConfirmPassword.text = "";
            regPanel.SetActive(false);
            loginPanel.SetActive(true);
        }
        else if (string.Compare(content, "username exists") == 0)
        {
            regInfo.text = "Username exists";
            regAccount.text = "";
            regPassword.text = "";
            regConfirmPassword.text = "";
        loginInfo.text = "";
        }
        else
        {
            Debug.Log("REGISTER ERROR");
            return;
        }
    }

    void noAccOnClick() {
        loginAccount.text = "";
        loginPassword.text = "";
        loginPanel.SetActive(false);
        regPanel.SetActive(true);
    }

    void loginNowOnClick() {
        regInfo.text = "";
        regAccount.text = "";
        regPassword.text = "";
        regConfirmPassword.text = "";
        regPanel.SetActive(false);
        loginPanel.SetActive(true);
    }

    string StringEncoder(List<string> list) 
    {
        string str = "";
        str += "{";
        for (int i = 0; i < list.Count - 1;) 
        {
            str += "\"" + list[i++] + "\": ";
            str += "\"" + list[i++] + "\"";
            if (i < list.Count - 1)
                str += ", ";
        }
        str += "}";
        return str;
    }
}
