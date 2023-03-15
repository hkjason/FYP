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

public class LoginManager : MonoBehaviour
{
    private string URL = "http://localhost:3000";
    private HttpClient client;

    [SerializeField] private Button loginButton;
    [SerializeField] private Button regButton;
    [SerializeField] private TMP_InputField loginAccount;
    [SerializeField] private TMP_InputField loginPassword;
    [SerializeField] private TMP_InputField regEmail;
    [SerializeField] private TMP_InputField regAccount;
    [SerializeField] private TMP_InputField regPassword;
    [SerializeField] private TMP_InputField regConfirmPassword;

    [SerializeField] private TMP_Text loginInfo;
    [SerializeField] private TMP_Text regInfo;

    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject regPanel;

    [SerializeField] private Button noAccButton;
    [SerializeField] private Button loginNowButton;

    [Serializable]
    public class UserDataRoot
    {
        public UserData[] root;
    }

    [Serializable]
    public class UserData
    {
        public int USER_ID;
        public string USERNAME;
        public int TEACHER_UID;
        public int TYPE;
    }

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

    async void LoginOnClick()
    {
        if (loginAccount.text == "")
        {
            loginInfo.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
            loginInfo.text = Localization.instance.GetLangNum() == 0 ? "Please enter a username" : "請輸入帳戶名稱";
            return;
        }
        if (loginPassword.text == "")
        {
            loginInfo.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
            loginInfo.text = Localization.instance.GetLangNum() == 0 ? "Please enter a password" : "請輸入密碼";
            return;
        }

        List<string> str = new List<string> { "AccountName", loginAccount.text, "Password", loginPassword.text };
        var payload = ExtensionFunction.StringEncoder(str);
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        HttpResponseMessage res;
        try
        {
            res = await client.PostAsync("login/trylogin", c);
        }
        catch (HttpRequestException e)
        {
            loginInfo.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
            loginInfo.text = Localization.instance.GetLangNum() == 0 ? "Connection failure, please check network connection or server" : "連接失敗，請檢查網絡連接或伺服器";
            return;
        }
        var content = await res.Content.ReadAsStringAsync();

        if (string.Compare(content, "Incorrect password") == 0)
        {
            loginInfo.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
            loginInfo.text = Localization.instance.GetLangNum() == 0 ? "Incorrect password" : "密碼錯誤";
            loginPassword.text = "";
        }
        else if (string.Compare(content, "User not found") == 0)
        {
            loginInfo.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
            loginInfo.text = Localization.instance.GetLangNum() == 0 ? "User not found" : "找不到用戶";
            loginAccount.text = "";
            loginPassword.text = "";
        }
        else
        {
            //LOGIN SUCCESSFUL
            var data = JsonUtility.FromJson<UserDataRoot>("{\"root\":" + content + "}");

            Userdata.instance.UID = data.root[0].USER_ID.ToString();
            Userdata.instance.USERNAME = data.root[0].USERNAME;
            Userdata.instance.TEACHER_UID = data.root[0].TEACHER_UID.ToString();
            Userdata.instance.ROLE_TYPE = data.root[0].TYPE;

            loginInfo.text = "";
            loginAccount.text = "";
            loginPassword.text = "";
            SceneManager.LoadScene(1);
        }
    }

    async void RegOnClick()
    {
        //check empty
        if (regEmail.text == "")
        {
            regInfo.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
            regInfo.text = Localization.instance.GetLangNum() == 0 ? "Please enter email" : "請輸入電子郵件";
            return;
        }

        if (regAccount.text == "")
        {
            regInfo.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
            regInfo.text = Localization.instance.GetLangNum() == 0 ? "Please enter username" : "請輸入帳戶名稱";
            return;
        }

        if (regPassword.text == "")
        {
            regInfo.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
            regInfo.text = Localization.instance.GetLangNum() == 0 ? "Please enter password" : "請輸入密碼";
            return;
        }

        if (regConfirmPassword.text == "")
        {
            regInfo.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
            regInfo.text = Localization.instance.GetLangNum() == 0 ? "Please enter confirm password" : "請輸入確認密碼";
            return;
        }

        if (!ValidEmail(regEmail.text))
        {
            regEmail.text = "";
            regInfo.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
            regInfo.text = Localization.instance.GetLangNum() == 0 ? "Invalid Email" : "無效的電子郵件";
            regInfo.text = "Invalid Email";
            return;
        }

        if (string.Compare(regPassword.text, regConfirmPassword.text) != 0)
        {
            regPassword.text = "";
            regConfirmPassword.text = "";
            regInfo.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
            regInfo.text = Localization.instance.GetLangNum() == 0 ? "Password not same" : "密碼不一致";
            return;
        }

        List<string> str = new List<string> { "AccountName", regAccount.text, "PassWord", regPassword.text };
        var payload = ExtensionFunction.StringEncoder(str);
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        HttpResponseMessage res;
        try
        {
            res = await client.PostAsync("login/tryregister", c);
        }
        catch (HttpRequestException e)
        {
            regInfo.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
            regInfo.text = Localization.instance.GetLangNum() == 0 ? "Connection failure, please check network connection or server" : "連接失敗，請檢查網絡連接或伺服器";
            return;
        }
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
            regInfo.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
            regInfo.text = Localization.instance.GetLangNum() == 0 ? "Username exists" : "用戶名已存在";
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
        regInfo.text = "";
        regAccount.text = "";
        regPassword.text = "";
        regConfirmPassword.text = "";
        regPanel.SetActive(false);
        loginPanel.SetActive(true);
    }
}
