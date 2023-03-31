using System;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using DG.Tweening;
using System.Net;

public class LoginManager : MonoBehaviour
{
    private string URL = "https://mongoserver-1-y3258239.deta.app/";
    //private string URL = "http://localhost:3000";
    private HttpClient client;

    [SerializeField] private Button loginButton;
    [SerializeField] private Button regButton;
    [SerializeField] private TMP_InputField loginAccount;
    [SerializeField] private TMP_InputField loginPassword;
    [SerializeField] private TMP_Dropdown regDropdown;
    [SerializeField] private TMP_InputField regEmail;
    [SerializeField] private TMP_InputField regAccount;
    [SerializeField] private TMP_InputField regPassword;
    [SerializeField] private TMP_InputField regConfirmPassword;

    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject regPanel;

    [SerializeField] private Button noAccButton;
    [SerializeField] private Button loginNowButton;

    [SerializeField] private GameObject loadingGO;

    [Header("Noti")]
    [SerializeField] private Transform noti;
    [SerializeField] private TMP_Text notiText;

    [Serializable]
    public class UserData
    {
        public int userId;
        public string username;
        public int role;
    }

    void Start()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(URL);
        client.DefaultRequestHeaders.Add("x-api-key", "c0pPE1CyrvbW_keBnGfJuxJfKr2HAPB3T3U6zCF2JcR4e");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        loginButton.onClick.AddListener(LoginOnClick);
        regButton.onClick.AddListener(RegOnClick);
        noAccButton.onClick.AddListener(NoAccOnClick);
        loginNowButton.onClick.AddListener(LoginNowOnClick);
    }

    async void LoginOnClick()
    {
        if (loginAccount.text == "")
        {
            NotiSetText("Please enter a username", "�п�J�b��W��");
            return;
        }
        if (loginPassword.text == "")
        {
            NotiSetText("Please enter a password", "�п�J�K�X");
            return;
        }

        List<string> str = new List<string> { "AccountName", loginAccount.text, "Password", loginPassword.text };
        var payload = ExtensionFunction.StringEncoder(str);
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        HttpResponseMessage res;
        try
        {
            Loading();
            res = await client.PostAsync("login/login", c);
        }
        catch (HttpRequestException e)
        {
            NotiSetText("Connection failure, please check network connection or server", "�s�����ѡA���ˬd�����s���Φ��A��");
            return;
        }
        finally
        {
            DoneLoading();
        }
        var content = await res.Content.ReadAsStringAsync();
        if (res.StatusCode.Equals(HttpStatusCode.InternalServerError))
        {
            NotiSetText("Server Error, please try again later", "�A�Ⱦ����~�A�еy��A��");
            return;
        }
        else if (res.StatusCode.Equals(HttpStatusCode.BadRequest))
        {
            if (string.Compare(content, "User not found.") == 0)
            {
                NotiSetText("User not found", "�䤣��Τ�");
                return;
            }
            else if (string.Compare(content, "Incorrect password.") == 0)
            {
                NotiSetText("Incorrect password", "�K�X���~");
                return;
            }
            else
            {
                NotiSetText("Server Error, please try again later", "�A�Ⱦ����~�A�еy��A��");
                return;
            }
        }
        else if (res.StatusCode.Equals(HttpStatusCode.OK))
        {
            var data = JsonUtility.FromJson<UserData>(content);
            UserManager.instance.UID = data.userId.ToString();
            UserManager.instance.USERNAME = data.username;
            UserManager.instance.ROLE_TYPE = data.role;
            SceneManager.LoadScene(1);
        }
    }

    async void RegOnClick()
    {
        //check empty
        if (regEmail.text == "")
        {
            NotiSetText("Please enter email", "�п�J�q�l�l��");
            return;
        }

        if (regAccount.text == "")
        {
            NotiSetText("Please enter username", "�п�J�b��W��");
            return;
        }

        if (regPassword.text == "")
        {
            NotiSetText("Please enter password", "�п�J�K�X");
            return;
        }

        if (regConfirmPassword.text == "")
        {
            NotiSetText("Please enter confirm password", "�п�J�T�{�K�X");
            return;
        }

        if (!ValidEmail(regEmail.text))
        {
            NotiSetText("Invalid Email", "�L�Ī��q�l�l��");
            return;
        }

        if (string.Compare(regPassword.text, regConfirmPassword.text) != 0)
        {
            NotiSetText("Password not same", "�K�X���@�P");
            return;
        }

        List<string> str = new List<string> { "AccountName", regAccount.text, "Password", regPassword.text, "Email", regEmail.text, "Role", regDropdown.value.ToString()};
        var payload = ExtensionFunction.StringEncoder(str);
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        HttpResponseMessage res;
        try
        {
            Loading();
            res = await client.PostAsync("login/", c);
        }
        catch (HttpRequestException e)
        {
            NotiSetText("Connection failure, please check network connection or server", "�s�����ѡA���ˬd�����s���Φ��A��");
            return;
        }
        finally
        {
            DoneLoading();
        }
        var content = await res.Content.ReadAsStringAsync();
        if (res.StatusCode.Equals(HttpStatusCode.InternalServerError))
        {
            NotiSetText("Server Error, please try again later", "�A�Ⱦ����~�A�еy��A��");
            return;
        }
        else if (res.StatusCode.Equals(HttpStatusCode.BadRequest))
        {
            if (string.Compare(content, "username exists.") == 0)
            {
                NotiSetText("Username exists", "�Τ�W�w�s�b");
                return;
            }
            else if (string.Compare(content, "email exists.") == 0)
            {
                NotiSetText("Email exists", "�q�l�l��w�s�b");
                return;
            }
            else
            {
                NotiSetText("Server Error, please try again later", "�A�Ⱦ����~�A�еy��A��");
                return;
            }
        }
        else if (res.StatusCode.Equals(HttpStatusCode.OK))
        {
            regAccount.text = "";
            regPassword.text = "";
            regConfirmPassword.text = "";
            regEmail.text = "";
            NotiSetText("Registration success", "���U���\");
            regPanel.SetActive(false);
            loginPanel.SetActive(true);
        }
    }

    bool ValidEmail(string email) 
    { 
        string regex = @"^[^@\s]+@[^@\s]+\.(com|org|net|edu|gov)$";
        string regex1 = @"^[^@\s]+@[^@\s]+\.(com|org|net|edu|gov)+\.[^@\s]$";

        return (Regex.IsMatch(email, regex, RegexOptions.IgnoreCase) || Regex.IsMatch(email, regex1, RegexOptions.IgnoreCase));
    }

    void NoAccOnClick() 
    {
        loginAccount.text = "";
        loginPassword.text = "";
        loginPanel.SetActive(false);
        regPanel.SetActive(true);
    }

    void LoginNowOnClick() 
    {
        regEmail.text = "";
        regAccount.text = "";
        regPassword.text = "";
        regConfirmPassword.text = "";
        regPanel.SetActive(false);
        loginPanel.SetActive(true);
    }

    void Loading()
    {
        loadingGO.SetActive(true);
    }

    void DoneLoading()
    {
        loadingGO.SetActive(false);
    }

    public void NotiSetText(string str1, string str2)
    {
        notiText.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
        notiText.text = Localization.instance.GetLangNum() == 0 ? str1 : str2;
        NotiPop();
    }

    void NotiPop()
    {
        DOTween.Kill(noti);
        noti.DOMoveY(1835, 0.2f).OnComplete(NotiUnpop);
    }

    void NotiUnpop()
    {
        noti.DOMoveY(2015, 0.2f).SetDelay(2);
    }
}
