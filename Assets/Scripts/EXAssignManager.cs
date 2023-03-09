using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Text.RegularExpressions;

public class EXAssignManager : MonoBehaviour
{
    private string URL = "http://localhost:3000";
    private HttpClient client;

    [SerializeField] private GameObject assignGO;

    [SerializeField] private Button assignButton;
    [SerializeField] private TMP_Dropdown questionType;
    [SerializeField] private TMP_InputField questionName;
    [SerializeField] private TMP_InputField questionInput;

    [Space(5)]
    [SerializeField] private TMP_Text answerInputText;
    [SerializeField] private TMP_InputField answerInput;

    [Space(5)]
    [SerializeField] private TMP_Text currAnsText;
    [SerializeField] private TMP_Text otherAnsText;
    [SerializeField] private TMP_InputField aInput;
    [SerializeField] private TMP_InputField bInput;
    [SerializeField] private TMP_InputField cInput;
    [SerializeField] private TMP_InputField dInput;
    [SerializeField] private TMP_InputField dueDate;
    [SerializeField] private TMP_InputField scheduleDate;

    [Header("Toggle")]
    [SerializeField] private Button toggleBtn;
    [SerializeField] private GameObject toggleOn;
    [SerializeField] private GameObject toggleOff;

    void Start()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(URL);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        Button btn;
        btn = assignButton.GetComponent<Button>();
        btn.onClick.AddListener(OnAssign);

        questionType.onValueChanged.AddListener(delegate {
            ChangeType(questionType);
        });

        dueDate.onValueChanged.AddListener(delegate { OnDateChanged(); });
        toggleBtn.onClick.AddListener(OnToggle);
    }

    private void Update()
    {
        if (assignGO.activeSelf)
        {
            dueDate.caretPosition = dueDate.text.Length;
        }
    }

    async void OnAssign()
    {
        string scheduleReleaseDate = "";
        if (toggleOn.activeSelf)
        {
            scheduleReleaseDate = scheduleDate.text;
        }

        List<string> str = new List<string>();
        switch (questionType.value)
        {
            case 0:
                str = new List<string> { "userID", Userdata.instance.UID, "questionName", questionName.text,"question", questionInput.text, "questionType", "0", "answerA", aInput.text, "answerB", bInput.text, "answerC", cInput.text, "answerD", dInput.text, "answer", aInput.text, "duedate", dueDate.text, "scheduledate", scheduleReleaseDate};
                break;
            case 1:
                str = new List<string> { "userID", Userdata.instance.UID, "questionName", questionName.text,"question", questionInput.text, "questionType", "1", "answer", answerInput.text, "duedate", dueDate.text, "scheduledate", scheduleReleaseDate};
                break;
            default:
                Debug.Log("Value error");
                break;
        }

        var payload = StringEncoder(str);
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        var res = await client.PostAsync("exercise/assignex", c);
        var content = await res.Content.ReadAsStringAsync();

        if (string.Compare(content, "assign successful") == 0)
        {
            switch (questionType.value)
            {
                case 0:
                    ResetMCValue();
                    break;
                case 1:
                    ResetSQValue();
                    break;
                default:
                    Debug.Log("Value error");
                    break;
            }
        }
        else if (string.Compare(content, "question exists") == 0)
        {

        }
        else
        {
            Debug.Log("ASSIGN ERROR");
            return;
        }
    }

    void ChangeType(TMP_Dropdown change) 
    {
        switch (change.value)
        {
            case 0:
                answerInput.gameObject.SetActive(false);
                answerInputText.enabled = false;

                currAnsText.enabled = true;
                otherAnsText.enabled = true;

                aInput.gameObject.SetActive(true);
                bInput.gameObject.SetActive(true);
                cInput.gameObject.SetActive(true);
                dInput.gameObject.SetActive(true);
                break;
            case 1:
                answerInput.gameObject.SetActive(true);
                answerInputText.enabled = true;

                currAnsText.enabled = false;
                otherAnsText.enabled = false;

                aInput.gameObject.SetActive(false);
                bInput.gameObject.SetActive(false);
                cInput.gameObject.SetActive(false);
                dInput.gameObject.SetActive(false);
                break;
            default:
                Debug.Log("Value error");
                break;
        }
    }

    void OnDateChanged()
    {
        if (string.IsNullOrEmpty(dueDate.text))
        {
            dueDate.text = string.Empty;
        }
        else
        {
            string input = dueDate.text;
            string MatchPattern = @"^((\d{2}/){0,2}(\d{1,2})?)$";
            string ReplacementPattern = "$1/$3";
            string ToReplacePattern = @"((\.?\d{2})+)(\d)";

            input = Regex.Replace(input, ToReplacePattern, ReplacementPattern);
            Match result = Regex.Match(input, MatchPattern);
            if (result.Success)
            {
                dueDate.SetTextWithoutNotify(input);
            }

            dueDate.caretPosition = dueDate.text.Length;
        }
    }

    bool DateValidate(string date)
    {
        if (date.Length != 10) 
        {
            return false;
        }

        string[] strArray = date.Split('/');

        int day = int.Parse(strArray[0]);
        int month = int.Parse(strArray[1]);
        int year = int.Parse(strArray[2]);

        string str = "28/02/2023 23:59:59";
        DateTime outDate;
        Debug.Log(DateTime.TryParse(str, out outDate));

        return false;
    }

    void OnToggle()
    {
        toggleOn.SetActive(!toggleOn.activeSelf);
        toggleOff.SetActive(!toggleOff.activeSelf);
        scheduleDate.gameObject.SetActive(!scheduleDate.gameObject.activeSelf);
    }

    void ResetMCValue() 
    {
        questionInput.text = "";
        aInput.text = "";
        bInput.text = "";
        cInput.text = "";
        dInput.text = "";
    }

    void ResetSQValue() 
    {
        questionInput.text = "";
        answerInput.text = "";
    }

    string StringEncoder(List<string> list)
    {
        string str = "";
        str += "{";
        for (int i = 0; i < list.Count - 1;)
        {
            str += "\"" + list[i++] + "\": ";
            str += "\"" + list[i++] + "\"";
            if (i < list.Count - 1)
                str += ", ";
        }
        str += "}";
        return str;
    }
}
