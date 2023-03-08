using DG.Tweening;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TMPro;
using UnityEngine;
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
    [SerializeField] private Button exitButton;

    [Header("Exercise")]
    [SerializeField] private GameObject exerciseListPanel;

    [Header("Assign")]
    [SerializeField] private GameObject assignPanel;

    [Header("Review")]
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

    [Header("ExitApp")]
    [SerializeField] private GameObject exitPanel;
    [SerializeField] private Button exitDimed;
    [SerializeField] private Button exitYes;
    [SerializeField] private Button exitNo;

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

        exitButton.onClick.AddListener(ExitPop);
        exitDimed.onClick.AddListener(ExitNo);
        exitYes.onClick.AddListener(ExitYes);
        exitNo.onClick.AddListener(ExitNo);

        usernameText.text = "User:\n" + Userdata.instance.USERNAME;

        if (Userdata.instance.ROLE_TYPE == 0)
        {
            assignButton.gameObject.SetActive(true);
            assignPanel.SetActive(true);
            topBarText.text = "Assign";
        }
        else
        {
            exerciseButton.gameObject.SetActive(true);
            exerciseListPanel.SetActive(true);
            topBarText.text = "Exercises";
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
        exerciseListPanel.SetActive(true);
        topBarText.text = "Exercises";
    }

    void AssignPop()
    {
        DisableFuncPanels();
        MenuOnClick();
        assignPanel.SetActive(true);
        topBarText.text = "Assign";
    }

    void ReviewPop()
    {
        DisableFuncPanels();
        MenuOnClick();
        reviewPanel.SetActive(true);
        topBarText.text = "Review";
    }

    void ConnectionPop()
    {
        DisableFuncPanels();
        MenuOnClick();
        connection.onChange();
        topBarText.text = "Connection";
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
            var res = await client.PostAsync("feedback/sendfeedback", c);
            var content = await res.Content.ReadAsStringAsync();

            if (string.Compare(content, "Feedback Success") == 0)
            {
                feedbackInfo.text = "Feedback Success";
            }
            else if (string.Compare(content, "Feedback Error") == 0)
            {
                feedbackInfo.text = "Error, please try again later";
            }

            feedbackText.text = "";
        }
        else
        {
            feedbackInfo.text = "Feedback cannot be empty";
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
}
