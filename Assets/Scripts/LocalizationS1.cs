using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocalizationS1 : MonoBehaviour
{
    [SerializeField] private TMP_FontAsset engFont;
    [SerializeField] private TMP_FontAsset chiFont;

    [Header("TopBar")]
    [SerializeField] private TMP_Text topBarText;

    [Header("Menu")]
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text exerciseBtnText;
    [SerializeField] private TMP_Text assignBtnText;
    [SerializeField] private TMP_Text reviewBtnText;
    [SerializeField] private TMP_Text courseBtnText;
    [SerializeField] private TMP_Text settingsBtnText;
    [SerializeField] private TMP_Text feedbackBtnText;
    [SerializeField] private TMP_Text aboutBtnText;
    [SerializeField] private TMP_Text logoutBtnText;
    [SerializeField] private TMP_Text exitAppBtnText;

    
    //[Header("Exercises")]

    //[Header("Assign")]

    //[Header("Review")]
    
    //[Header("Connection")]

    //[Header("Feedback")]
    
    [Header("About")]
    [SerializeField] private TMP_Text aboutTitle;
    [SerializeField] private TMP_Text aboutText;

    [Header("Logout")]
    [SerializeField] private TMP_Text logoutTitle;
    [SerializeField] private TMP_Text logoutText;
    [SerializeField] private TMP_Text logoutYes;
    [SerializeField] private TMP_Text logoutNo;

    [Header("ExitApp")]
    [SerializeField] private TMP_Text exitAppTitle;
    [SerializeField] private TMP_Text exitAppText;
    [SerializeField] private TMP_Text exitAppYes;
    [SerializeField] private TMP_Text exitAppNo;

    public void LoadText(int langNum)
    {
        switch (langNum)
        {
            case 0:
                usernameText.font = engFont;
                exerciseBtnText.font = engFont;
                assignBtnText.font = engFont;
                reviewBtnText.font = engFont;
                courseBtnText.font = engFont;
                settingsBtnText.font = engFont;
                feedbackBtnText.font = engFont;
                aboutBtnText.font = engFont;
                logoutBtnText.font = engFont;
                exitAppBtnText.font = engFont;

                usernameText.text = "User:";
                exerciseBtnText.text = "Exercises";
                assignBtnText.text = "Assign";
                reviewBtnText.text = "Review";
                courseBtnText.text = "Courses";
                settingsBtnText.text = "Settings";
                feedbackBtnText.text = "Feedback";
                aboutBtnText.text = "About";
                logoutBtnText.text = "Logout";
                exitAppBtnText.text = "Exit App";

                aboutTitle.font = engFont;
                aboutText.font = engFont;

                aboutTitle.text = "About";
                aboutText.text = "A mathematic learning application\nCreated by Yeung Yue Hei\nAs Computer Science Final Year Project(2023)";

                logoutTitle.font = engFont;
                logoutText.font = engFont;
                logoutYes.font = engFont;
                logoutNo.font = engFont;

                logoutTitle.text = "Logout";
                logoutText.text = "Are you sure?";
                logoutYes.text = "Yes";
                logoutNo.text = "No";

                exitAppTitle.font = engFont;
                exitAppText.font = engFont;
                exitAppYes.font = engFont;
                exitAppNo.font = engFont;

                exitAppTitle.text = "Exit App";
                exitAppText.text = "Are you sure?";
                exitAppYes.text = "Yes";
                exitAppNo.text = "No";

                break;
            case 1:
                usernameText.font = chiFont;
                exerciseBtnText.font = chiFont;
                assignBtnText.font = chiFont;
                reviewBtnText.font = chiFont;
                courseBtnText.font = chiFont;
                settingsBtnText.font = chiFont;
                feedbackBtnText.font = chiFont;
                aboutBtnText.font = chiFont;
                logoutBtnText.font = chiFont;
                exitAppBtnText.font = chiFont;

                usernameText.text = "用戶:";
                exerciseBtnText.text = "練習";
                assignBtnText.text = "分派";
                reviewBtnText.text = "檢閱";
                courseBtnText.text = "課程";
                settingsBtnText.text = "設置";
                feedbackBtnText.text = "反饋";
                aboutBtnText.text = "關於";
                logoutBtnText.text = "登出";
                exitAppBtnText.text = "離開";

                aboutTitle.font = chiFont;
                aboutText.font = chiFont;

                aboutTitle.text = "關於";
                aboutText.text = "由楊宇曦設計的數學學習程式\n為2023年電腦科學的畢業習作";

                logoutTitle.font = chiFont;
                logoutText.font = chiFont;
                logoutYes.font = chiFont;
                logoutNo.font = chiFont;

                logoutTitle.text = "登出";
                logoutText.text = "確認登出?";
                logoutYes.text = "是";
                logoutNo.text = "否";

                exitAppTitle.font = chiFont;
                exitAppText.font = chiFont;
                exitAppYes.font = chiFont;
                exitAppNo.font = chiFont;

                exitAppTitle.text = "離開";
                exitAppText.text = "確認離開?";
                exitAppYes.text = "是";
                exitAppNo.text = "否";
                break;
        }

        LoadTopBar(langNum);
    }

    private void LoadTopBar(int langNum)
    {
        switch (topBarText.text)
        {
            case "Exercises":case "練習":
                topBarText.font = langNum == 0 ? engFont : chiFont;
                topBarText.text = langNum == 0 ? "Exercises" : "練習";
                break;
            case "Assign":case "分派":
                topBarText.font = langNum == 0 ? engFont : chiFont;
                topBarText.text = langNum == 0 ? "Assign" : "分派";
                break;
            case "Review":case "檢閱":
                topBarText.font = langNum == 0 ? engFont : chiFont;
                topBarText.text = langNum == 0 ? "Review" : "檢閱";
                break;
            case "Connection":case "連接":
                topBarText.font = langNum == 0 ? engFont : chiFont;
                topBarText.text = langNum == 0 ? "Courses" : "課程";
                break;
            default:
                Debug.Log("ERROR");
                break;
        }
    }
}
