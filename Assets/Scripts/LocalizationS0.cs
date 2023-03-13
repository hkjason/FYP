using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationS0 : MonoBehaviour
{
    [Header("LocaleButtons")]
    [SerializeField] private Button localeButton;
    [SerializeField] private TMP_Text localeFrontText;
    [SerializeField] private TMP_Text localeBackText;


    [SerializeField] private TMP_Text welcomeText;

    [SerializeField] private TMP_FontAsset engFont;
    [SerializeField] private TMP_FontAsset chiFont;

    [Header("LoginPanel")]
    [SerializeField] private TMP_Text loginTitleText;
    [SerializeField] private TMP_Text loginUsernameText;
    [SerializeField] private TMP_Text loginPasswordText;
    [SerializeField] private TMP_Text loginButtonText;
    [SerializeField] private TMP_Text signUpNowButtonText;

    [Header("SignUpPanel")]
    [SerializeField] private TMP_Text signUpTitleText;
    [SerializeField] private TMP_Text emailText;
    [SerializeField] private TMP_Text signUsernameText;
    [SerializeField] private TMP_Text signPasswordText;
    [SerializeField] private TMP_Text signConfirmPasswordText;
    [SerializeField] private TMP_Text signButtonText;
    [SerializeField] private TMP_Text loginNowButtonText;

    private void Start()
    {
        localeButton.onClick.AddListener(ChangeOnClick);
    }

    public void LoadText(int langNum)
    {
        switch (langNum)
        {
            case 0:
                localeFrontText.font = engFont;
                localeFrontText.text = "EN";
                localeBackText.font = chiFont;
                localeBackText.text = "��";


                welcomeText.font = engFont;
                welcomeText.text = "Welcome";

                loginTitleText.font = engFont;
                loginUsernameText.font = engFont;
                loginPasswordText.font = engFont;
                loginButtonText.font = engFont;
                signUpNowButtonText.font = engFont;

                loginTitleText.text = "Login";
                loginUsernameText.text = "Username";
                loginPasswordText.text = "Password";
                loginButtonText.text = "Login";
                signUpNowButtonText.text = "SIGN UP";

                signUpTitleText.font = engFont;
                emailText.font = engFont;
                signUsernameText.font = engFont;
                signPasswordText.font = engFont;
                signConfirmPasswordText.font = engFont;
                signButtonText.font = engFont;
                loginNowButtonText.font = engFont;

                signUpTitleText.text = "Sign Up";
                emailText.text = "Email";
                signUsernameText.text = "Username";
                signPasswordText.text = "Password";
                signConfirmPasswordText.text = "Confirm Password";
                signButtonText.text = "Create Account";
                loginNowButtonText.text = "LOGIN";
                break;
            case 1:
                localeFrontText.font = chiFont;
                localeFrontText.text = "��";
                localeBackText.font = engFont;
                localeBackText.text = "EN";

                welcomeText.font = chiFont;
                welcomeText.text = "�w��";

                loginTitleText.font = chiFont;
                loginUsernameText.font = chiFont;
                loginPasswordText.font = chiFont;
                loginButtonText.font = chiFont;
                signUpNowButtonText.font = chiFont;

                loginTitleText.text = "�n�J";
                loginUsernameText.text = "�b��W��";
                loginPasswordText.text = "�K�X";
                loginButtonText.text = "�n�J";
                signUpNowButtonText.text = "���U";

                signUpTitleText.font = chiFont;
                emailText.font = chiFont;
                signUsernameText.font = chiFont;
                signPasswordText.font = chiFont;
                signConfirmPasswordText.font = chiFont;
                signButtonText.font = chiFont;
                loginNowButtonText.font = chiFont;

                signUpTitleText.text = "���U�b��";
                emailText.text = "�q�l�l��";
                signUsernameText.text = "�b��W��";
                signPasswordText.text = "�K�X";
                signConfirmPasswordText.text = "�T�{�K�X";
                signButtonText.text = "�Ыؽ��";
                loginNowButtonText.text = "�n�J";
                break;
            default:
                Debug.LogError("lang num error");
                break;
        }
    }

    void ChangeOnClick()
    {
        Localization.instance.ChangeLocale();
    }
}
