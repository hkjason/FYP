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

    [Header("Buttons")]
    [SerializeField] private Button menuButton;
    [SerializeField] private Button exerciseButton;
    [SerializeField] private Button reviewButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button feedbackButton;
    [SerializeField] private Button aboutButton;
    [SerializeField] private Button exitButton;

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
