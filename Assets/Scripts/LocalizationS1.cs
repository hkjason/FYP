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

                usernameText.text = "�Τ�:";
                exerciseBtnText.text = "�m��";
                assignBtnText.text = "����";
                reviewBtnText.text = "�˾\";
                courseBtnText.text = "�ҵ{";
                settingsBtnText.text = "�]�m";
                feedbackBtnText.text = "���X";
                aboutBtnText.text = "����";
                logoutBtnText.text = "�n�X";
                exitAppBtnText.text = "���}";

                aboutTitle.font = chiFont;
                aboutText.font = chiFont;

                aboutTitle.text = "����";
                aboutText.text = "�ѷ��t�f�]�p���ƾǾǲߵ{��\n��2023�~�q����Ǫ����~�ߧ@";

                logoutTitle.font = chiFont;
                logoutText.font = chiFont;
                logoutYes.font = chiFont;
                logoutNo.font = chiFont;

                logoutTitle.text = "�n�X";
                logoutText.text = "�T�{�n�X?";
                logoutYes.text = "�O";
                logoutNo.text = "�_";

                exitAppTitle.font = chiFont;
                exitAppText.font = chiFont;
                exitAppYes.font = chiFont;
                exitAppNo.font = chiFont;

                exitAppTitle.text = "���}";
                exitAppText.text = "�T�{���}?";
                exitAppYes.text = "�O";
                exitAppNo.text = "�_";
                break;
        }

        LoadTopBar(langNum);
    }

    private void LoadTopBar(int langNum)
    {
        switch (topBarText.text)
        {
            case "Exercises":case "�m��":
                topBarText.font = langNum == 0 ? engFont : chiFont;
                topBarText.text = langNum == 0 ? "Exercises" : "�m��";
                break;
            case "Assign":case "����":
                topBarText.font = langNum == 0 ? engFont : chiFont;
                topBarText.text = langNum == 0 ? "Assign" : "����";
                break;
            case "Review":case "�˾\":
                topBarText.font = langNum == 0 ? engFont : chiFont;
                topBarText.text = langNum == 0 ? "Review" : "�˾\";
                break;
            case "Connection":case "�s��":
                topBarText.font = langNum == 0 ? engFont : chiFont;
                topBarText.text = langNum == 0 ? "Courses" : "�ҵ{";
                break;
            default:
                Debug.Log("ERROR");
                break;
        }
    }
}
