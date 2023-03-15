using DG.Tweening;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private string URL = "http://localhost:3000";
    private HttpClient client;

    [SerializeField] private Transform menu;
    [SerializeField] private Button menuVirtualBackground;
    [SerializeField] private TMP_Text topBarText;
    [SerializeField] private TMP_Text usernameText;

    [Header("Buttons")]
    [SerializeField] private Button menuButton;
    [SerializeField] private Button exerciseButton;
    [SerializeField] private Button assignButton;
    [SerializeField] private Button reviewButton;
    [SerializeField] private Button connectionButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button feedbackButton;
    [SerializeField] private Button aboutButton;
    [SerializeField] private Button logoutButton;
    [SerializeField] private Button exitButton;

    [Header("Exercise")]
    [SerializeField] private EXListManager exListManager;
    [SerializeField] private GameObject exerciseListPanel;

    [Header("Assign")]
    [SerializeField] private GameObject assignPanel;

    [Header("Review")]
    [SerializeField] private EXReviewManager exReviewManager;
    [SerializeField] private GameObject reviewPanel;

    [Header("Connection")]
    [SerializeField] private Connection connection;
    [SerializeField] private GameObject connectionPanel;

    [Header("Feedback")]
    [SerializeField] private GameObject feedbackPanel;
    [SerializeField] private TMP_InputField feedbackText;
    [SerializeField] private TMP_Text feedbackInfo;
    [SerializeField] private Button feedbackSend;
    [SerializeField] private Button feedbackDimed;
    [SerializeField] private Button feedbackExit;

    [Header("About")]
    [SerializeField] private GameObject aboutPanel;
    [SerializeField] private Button aboutDimed;
    [SerializeField] private Button aboutExit;

    [Header("Logout")]
    [SerializeField] private GameObject logoutPanel;
    [SerializeField] private Button logoutDimed;
    [SerializeField] private Button logoutYes;
    [SerializeField] private Button logoutNo;

    [Header("ExitApp")]
    [SerializeField] private GameObject exitPanel;
    [SerializeField] private Button exitDimed;
    [SerializeField] private Button exitYes;
    [SerializeField] private Button exitNo;

    [Header("Noti")]
    [SerializeField] private Transform noti;
    [SerializeField] private TMP_Text notiText;

    private bool menuState = false;

    void Start()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(URL);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        menuButton.onClick.AddListener(MenuOnClick);
        menuVirtualBackground.onClick.AddListener(MenuOnClick);

        exerciseButton.onClick.AddListener(ExerciseListPop);

        assignButton.onClick.AddListener(AssignPop);

        reviewButton.onClick.AddListener(ReviewPop);

        connectionButton.onClick.AddListener(ConnectionPop);

        feedbackButton.onClick.AddListener(FeedbackPop);
        feedbackSend.onClick.AddListener(FeedbackSend);
        feedbackDimed.onClick.AddListener(FeedbackExit);
        feedbackExit.onClick.AddListener(FeedbackExit);

        aboutButton.onClick.AddListener(AboutPop);
        aboutDimed.onClick.AddListener(AboutExit);
        aboutExit.onClick.AddListener(AboutExit);

        logoutButton.onClick.AddListener(LogoutPop);
        logoutDimed.onClick.AddListener(LogoutNo);
        logoutYes.onClick.AddListener(LogoutYes);
        logoutNo.onClick.AddListener(LogoutNo);

        exitButton.onClick.AddListener(ExitPop);
        exitDimed.onClick.AddListener(ExitNo);
        exitYes.onClick.AddListener(ExitYes);
        exitNo.onClick.AddListener(ExitNo);

        usernameText.text = Userdata.instance.USERNAME;

        if (Userdata.instance.ROLE_TYPE == 0)
        {
            assignButton.gameObject.SetActive(true);
            assignPanel.SetActive(true);
            topBarText.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
            topBarText.text = Localization.instance.GetLangNum() == 0 ? "Assign" : "分派";
        }
        else
        {
            exerciseButton.gameObject.SetActive(true);
            exerciseListPanel.SetActive(true);
            topBarText.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
            topBarText.text = Localization.instance.GetLangNum() == 0 ? "Exercises" : "練習";
        }
    }

    void MenuOnClick()
    {
        if (!menuState)
        {
            DOTween.Kill(menu);
            MenuPop();
            menuState = true;
            menuVirtualBackground.gameObject.SetActive(true);
        }
        else
        {
            DOTween.Kill(menu);
            MenuUnpop();
            menuState = false;
            menuVirtualBackground.gameObject.SetActive(false);
        }
    }

    void MenuPop()
    {
        menu.DOMoveX(280, 0.3f);
    }

    void MenuUnpop() 
    {
        menu.DOMoveX(-280, 0.3f);
    }

    void ExerciseListPop()
    {
        DisableFuncPanels();
        MenuOnClick();
        exListManager.GetExList();
        topBarText.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
        topBarText.text = Localization.instance.GetLangNum() == 0 ? "Exercises" : "練習";
    }

    void AssignPop()
    {
        DisableFuncPanels();
        MenuOnClick();
        assignPanel.SetActive(true);
        topBarText.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
        topBarText.text = Localization.instance.GetLangNum() == 0 ? "Assign" : "分派";
    }

    void ReviewPop()
    {
        DisableFuncPanels();
        MenuOnClick();
        exReviewManager.GetReviewList();
        topBarText.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
        topBarText.text = Localization.instance.GetLangNum() == 0 ? "Review" : "檢閱";
    }

    void ConnectionPop()
    {
        DisableFuncPanels();
        MenuOnClick();
        connection.onChange();
        topBarText.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
        topBarText.text = Localization.instance.GetLangNum() == 0 ? "Connection" : "連接";
    }

    void DisableFuncPanels()
    {
        exerciseListPanel.SetActive(false);
        assignPanel.SetActive(false);
        reviewPanel.SetActive(false);
        connectionPanel.SetActive(false);
    }

    void FeedbackPop()
    {
        MenuOnClick();
        feedbackText.text = "";
        feedbackInfo.text = "";
        feedbackPanel.SetActive(true);
    }

    async void FeedbackSend()
    {
        if (feedbackText.text != "")
        {
            var payload = "{\"feedbacktext\": \"" + feedbackText.text + "\"}";
            HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage res;
            try
            {
                res = await client.PostAsync("feedback/sendfeedback", c);
            }
            catch (HttpRequestException e)
            {
                NotiSetText("Connection failure, please check network connection or server", "連接失敗，請檢查網絡連接或伺服器");
                return;
            }
            var content = await res.Content.ReadAsStringAsync();

            if (string.Compare(content, "Feedback Success") == 0)
            {
                feedbackInfo.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
                feedbackInfo.text = Localization.instance.GetLangNum() == 0 ? "Feedback Success" : "反饋成功";
            }
            else if (string.Compare(content, "Feedback Error") == 0)
            {
                feedbackInfo.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
                feedbackInfo.text = Localization.instance.GetLangNum() == 0 ? "Error, please try again later" : "錯誤，請稍後重試";
            }
            feedbackText.text = "";
        }
        else
        {
            feedbackInfo.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
            feedbackInfo.text = Localization.instance.GetLangNum() == 0 ? "Feedback cannot be empty" : "反饋不能為空";
        }
    }

    void FeedbackExit()
    {
        feedbackPanel.SetActive(false);
    }

    void AboutPop()
    {
        MenuOnClick();
        aboutPanel.SetActive(true);
    }

    void AboutExit()
    {
        aboutPanel.SetActive(false);
    }

    void LogoutPop()
    {
        MenuOnClick();
        logoutPanel.SetActive(true);
    }

    void LogoutYes()
    {
        Userdata.instance.UID = "";
        Userdata.instance.USERNAME = "";
        Userdata.instance.TEACHER_UID = "";
        Userdata.instance.ROLE_TYPE = 0;
        SceneManager.LoadScene(0);
    }

    void LogoutNo()
    {
        logoutPanel.SetActive(false);
    }

    void ExitPop()
    {
        MenuOnClick();
        exitPanel.SetActive(true);
    }

    void ExitYes()
    {
        Application.Quit();
    }

    void ExitNo()
    {
        exitPanel.SetActive(false);
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
        noti.DOMoveY(1693, 0.2f).OnComplete(NotiUnpop);
    }

    void NotiUnpop()
    {
        noti.DOMoveY(1920, 0.2f).SetDelay(2);
    }
}
