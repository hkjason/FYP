using DG.Tweening;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private string URL = "https://mongoserver-1-y3258239.deta.app/";
    private HttpClient client;

    [SerializeField] private Transform menu;
    [SerializeField] private Button menuVirtualBackground;
    [SerializeField] private TMP_Text topBarText;
    [SerializeField] private TMP_Text usernameText;

    [Header("Buttons")]
    [SerializeField] private Button menuButton;
    [SerializeField] private Button courseButton;
    [SerializeField] private Button exerciseButton;
    [SerializeField] private Button assignButton;
    [SerializeField] private Button reviewButton;
    [SerializeField] private Button quizButton;
    [SerializeField] private Button addStudentButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button feedbackButton;
    [SerializeField] private Button aboutButton;
    [SerializeField] private Button logoutButton;
    [SerializeField] private Button exitButton;

    [Header("Course")]
    [SerializeField] private CourseManager courseManager;
    [SerializeField] private GameObject coursePanel;

    [Header("Exercise")]
    [SerializeField] private EXListManager exListManager;
    [SerializeField] private GameObject exerciseListPanel;

    [Header("Assign")]
    [SerializeField] private GameObject assignPanel;
    [SerializeField] private EXAssignManager exAssignManager;

    [Header("Review")]
    [SerializeField] private EXReviewManager exReviewManager;

    [Header("Quiz")]
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private RealTimeQuiz realTimeQuiz;

    [Header("AddStudent")]
    [SerializeField] private GameObject addStudentPanel;
    [SerializeField] private Button addStudentDimed;
    [SerializeField] private Button addStudentQuit;

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

    [SerializeField] private GameObject loadingGO;

    private bool menuState = false;


    [SerializeField] private Transform notiOri;
    [SerializeField] private Transform notiTo;

    void Start()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(URL);
        client.DefaultRequestHeaders.Add("x-api-key", "c0pPE1CyrvbW_keBnGfJuxJfKr2HAPB3T3U6zCF2JcR4e");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        menuButton.onClick.AddListener(MenuOnClick);
        menuVirtualBackground.onClick.AddListener(CloseMenu);

        courseButton.onClick.AddListener(CoursePop);

        exerciseButton.onClick.AddListener(ExerciseListPop);

        assignButton.onClick.AddListener(AssignPop);

        reviewButton.onClick.AddListener(ReviewPop);

        quizButton.onClick.AddListener(QuizPop);

        addStudentButton.onClick.AddListener(AddStudentPop);
        addStudentDimed.onClick.AddListener(AddStudentExit);
        addStudentQuit.onClick.AddListener(AddStudentExit);

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

        usernameText.text = UserManager.instance.USERNAME;

        courseManager.GetCourseList();
        topBarText.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
        topBarText.text = Localization.instance.GetLangNum() == 0 ? "Courses" : "課程";
        
        if (UserManager.instance.ROLE_TYPE == 0)
        {
            assignButton.gameObject.SetActive(true);
            addStudentButton.gameObject.SetActive(true);
        }
        else
        {
            exerciseButton.gameObject.SetActive(true);
            addStudentButton.gameObject.SetActive(false);
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
            CloseMenu();
        }
    }

    void CloseMenu()
    {
        DOTween.Kill(menu);
        MenuUnpop();
        menuState = false;
        menuVirtualBackground.gameObject.SetActive(false);
    }

    void MenuPop()
    {
        menu.DOMoveX(280, 0.3f);
    }

    void MenuUnpop()
    {
        menu.DOMoveX(-280, 0.3f);
    }

    void CoursePop()
    {
        DisableFuncPanels();
        CloseMenu();
        courseManager.GetCourseList();
        coursePanel.SetActive(true); 
        topBarText.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
        topBarText.text = Localization.instance.GetLangNum() == 0 ? "Courses" : "課程";
    }

    public void ExerciseListPop()
    {
        CloseMenu();
        if (UserManager.instance.COURSEID <= 0)
        {
            NotiSetText("Please select course first", "請先選擇課程");
            return;
        }
        DisableFuncPanels();
        exListManager.GetExList();
        topBarText.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
        topBarText.text = Localization.instance.GetLangNum() == 0 ? "Exercises" : "練習";
    }

    public void AssignPop()
    {
        CloseMenu();
        if (UserManager.instance.COURSEID <= 0)
        {
            NotiSetText("Please select course first", "請先選擇課程");
            return;
        }
        DisableFuncPanels();
        assignPanel.SetActive(true);
        topBarText.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
        topBarText.text = Localization.instance.GetLangNum() == 0 ? "Assign" : "分派";
    }

    void ReviewPop()
    {
        CloseMenu();
        if (UserManager.instance.COURSEID <= 0)
        {
            NotiSetText("Please select course first", "請先選擇課程");
            return;
        }
        DisableFuncPanels();
        exReviewManager.StartReview();
        topBarText.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
        topBarText.text = Localization.instance.GetLangNum() == 0 ? "Review" : "檢閱";
    }

    void QuizPop()
    {
        CloseMenu();
        if (UserManager.instance.COURSEID <= 0)
        {
            NotiSetText("Please select course first", "請先選擇課程");
            return;
        }
        DisableFuncPanels();
        realTimeQuiz.StartQuiz();
        topBarText.font = Localization.instance.GetLangNum() == 0 ? Localization.instance.engFont : Localization.instance.chiFont;
        topBarText.text = Localization.instance.GetLangNum() == 0 ? "Quiz" : "測驗";
    }


    void AddStudentPop()
    {
        CloseMenu();
        if (UserManager.instance.COURSEID <= 0)
        {
            NotiSetText("Please select course first", "請先選擇課程");
            return;
        }
        addStudentPanel.SetActive(true);
    }

    void AddStudentExit()
    {
        addStudentPanel.SetActive(false);
    }

    void DisableFuncPanels()
    {
        exListManager.ExitEx();
        exAssignManager.ExitAssign();
        exReviewManager.ExitReview();
        realTimeQuiz.ExitQuiz();
        coursePanel.SetActive(false);
    }

    void FeedbackPop()
    {
        CloseMenu();
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
                res = await client.PostAsync("feedback/", c);
            }
            catch (HttpRequestException e)
            {
                NotiSetText("Connection failure, please check network connection or server", "連接失敗，請檢查網絡連接或伺服器");
                return;
            }
            var content = await res.Content.ReadAsStringAsync();
            if (res.StatusCode.Equals(HttpStatusCode.InternalServerError))
            {
                NotiSetText("Server Error, please try again later", "服務器錯誤，請稍後再試");
                return;
            }
            else if (res.StatusCode.Equals(HttpStatusCode.BadRequest))
            {
                if (string.Compare(content, "Feedback Error") == 0)
                {
                    NotiSetText("Error, please try again later", "錯誤，請稍後重試");
                }
            }
            else if (res.StatusCode.Equals(HttpStatusCode.OK))
            {
                NotiSetText("Feedback Success", "反饋成功");
                feedbackText.text = "";
                FeedbackExit();
                return;
            }
        }
        else
        {
            NotiSetText("Feedback cannot be empty", "反饋不能為空");
        }
    }

    public void Loading()
    {
        loadingGO.SetActive(true);
    }

    public void DoneLoading()
    {
        loadingGO.SetActive(false);
    }

    void FeedbackExit()
    {
        feedbackPanel.SetActive(false);
    }

    void AboutPop()
    {
        CloseMenu();
        aboutPanel.SetActive(true);
    }

    void AboutExit()
    {
        aboutPanel.SetActive(false);
    }

    void LogoutPop()
    {
        CloseMenu();
        logoutPanel.SetActive(true);
    }

    void LogoutYes()
    {
        UserManager.instance.ResetUserData();
        SceneManager.LoadScene(0);
    }

    void LogoutNo()
    {
        logoutPanel.SetActive(false);
    }

    void ExitPop()
    {
        CloseMenu();
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
        noti.DOMoveY(notiTo.position.y, 0.2f).OnComplete(NotiUnpop);
    }

    void NotiUnpop()
    {
        noti.DOMoveY(notiOri.position.y, 0.2f).SetDelay(2);
    }
}
