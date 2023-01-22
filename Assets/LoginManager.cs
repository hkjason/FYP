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

    [SerializeField] private Button loginButton;
    [SerializeField] private Button regButton;
    [SerializeField] private TMP_InputField loginAccount;
    [SerializeField] private TMP_InputField loginPassword;
    [SerializeField] private TMP_InputField regAccount;
    [SerializeField] private TMP_InputField regPassword;
    [SerializeField] private TMP_InputField regConfirmPassword;

    [SerializeField] private TMP_Text loginInfo;
    [SerializeField] private TMP_Text regInfo;

    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject regPanel;

    [SerializeField] private Button noAccButton;
    [SerializeField] private Button loginNowButton;

    public static string UID;

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
        btn.onClick.AddListener(NoAccOnClick);
        btn = loginNowButton.GetComponent<Button>();
        btn.onClick.AddListener(LoginNowOnClick);
    }

    //PAYLOAD DEMO
    //var payload = "{\"CustomerId\": 5,\"CustomerName\": \"Pepsi\"}";
    async void LoginOnClick()
    {
        if (loginAccount.text == "")
        {
            loginInfo.text = "Please enter a username";
            loginPassword.text = "";
            return;
        }
        if (loginPassword.text == "")
        {
            loginInfo.text = "Please enter a password";
            return;
        }

        List<string> str = new List<string> { "AccountName", loginAccount.text, "Password", loginPassword.text };
        var payload = ExtensionFunction.StringEncoder(str);
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        var res = await client.PostAsync("login/trylogin", c);
        var content = await res.Content.ReadAsStringAsync();

        if (string.Compare(content, "incorrect password") == 0)
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
            //LOGIN SUCCESSFUL
            UID = content;
            Debug.Log(UID);
            loginInfo.text = "";
            loginAccount.text = "";
            loginPassword.text = "";
            SceneManager.LoadScene(1);
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
        var payload = ExtensionFunction.StringEncoder(str);
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

    void NoAccOnClick() {
        loginAccount.text = "";
        loginPassword.text = "";
        loginPanel.SetActive(false);
        regPanel.SetActive(true);
    }

    void LoginNowOnClick() {
        regInfo.text = "";
        regAccount.text = "";
        regPassword.text = "";
        regConfirmPassword.text = "";
        regPanel.SetActive(false);
        loginPanel.SetActive(true);
    }
}
