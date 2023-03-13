using System;
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

    [Header("PreAssign")]
    [SerializeField] private GameObject preAssignGO;
    [Space(5)]
    [SerializeField] private TMP_InputField exerciseName;
    [SerializeField] private TMP_InputField dueDate;
    [SerializeField] private TMP_InputField scheduleDate;
    [Space(5)]
    [SerializeField] private Button toggleBtn;
    [SerializeField] private GameObject toggleOn;
    [SerializeField] private GameObject toggleOff;
    [SerializeField] private Button confirmBtn;

    [Header("Assign")]
    [SerializeField] private GameObject assignGO;
    [Space(5)]
    [SerializeField] private TMP_Dropdown questionType;
    [SerializeField] private TMP_InputField questionInput;
    [SerializeField] private TMP_Text currAnsText;
    [SerializeField] private TMP_Text otherAnsText;
    [SerializeField] private TMP_InputField aInput;
    [SerializeField] private TMP_InputField bInput;
    [SerializeField] private TMP_InputField cInput;
    [SerializeField] private TMP_InputField dInput;
    [SerializeField] private TMP_Text answerInputText;
    [SerializeField] private TMP_InputField answerInput;
    [Space(5)]
    [SerializeField] private Button saveNNext;
    [SerializeField] private Button assignButton;

    [Header("Noti")]
    [SerializeField] private Transform noti;
    [SerializeField] private TMP_Text notiText;
    [SerializeField] private UIManager uiManager;

    //ExData
    private string exNameData ="";
    private string dueDateData ="";
    private string scheduleData ="";
    public List<QuestionData> questionDataList = new List<QuestionData>();
    public class QuestionData
    {
        public string question = "";
        public string questionType = "";
        public string answerA = "";
        public string answerB = "";
        public string answerC = "";
        public string answerD = "";
        public string answer = "";
    }

    void Start()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(URL);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        confirmBtn.onClick.AddListener(OnConfirm);
        assignButton.onClick.AddListener(OnAssign);
        saveNNext.onClick.AddListener(OnSave);
        toggleBtn.onClick.AddListener(OnToggle);

        questionType.onValueChanged.AddListener(delegate { ChangeType(questionType); });


        dueDate.onValueChanged.AddListener(delegate { OnDateChanged(dueDate); });
        scheduleDate.onValueChanged.AddListener(delegate { OnDateChanged(scheduleDate); });
    }

    private void Update()
    {
        if (preAssignGO.activeSelf)
        {
            dueDate.caretPosition = dueDate.text.Length;
            if (toggleOn.activeSelf)
            {
                scheduleDate.caretPosition = scheduleDate.text.Length;
            }
        }
    }

    async void OnAssign()
    {
        OnSave();

        string dataList = "{\"root\":[";

        for (int listIdx = 0; listIdx < questionDataList.Count; listIdx++)
        {
            if (listIdx != 0)
            {
                dataList = dataList + ",";
            }
            List<string> str = new List<string>();
            switch (questionDataList[listIdx].questionType)
            {
                case "0":
                    str = new List<string> { "question", questionDataList[listIdx].question, "questionType", questionDataList[listIdx].questionType, "answer", questionDataList[listIdx].answer, "answerA", questionDataList[listIdx].answerA, "answerB", questionDataList[listIdx].answerB, "answerC", questionDataList[listIdx].answerC, "answerD", questionDataList[listIdx].answerD };
                    break;
                case "1":
                    str = new List<string> { "question", questionDataList[listIdx].question, "questionType", questionDataList[listIdx].questionType, "answer", questionDataList[listIdx].answer };
                    break;
                default:
                    Debug.Log("Value error");
                    break;
            }
            var dataStr = StringEncoder(str);
            dataList = dataList + dataStr;
        }
        dataList = dataList + "], \"userID\": \""+ Userdata.instance.UID + "\", \"exName\": \"" + exNameData + "\", \"dueDate\": \"" + dueDateData + "\", \"scheduleDate\": \"" + scheduleData + "\"}";

        Debug.Log(dataList);

        HttpContent c = new StringContent(dataList, Encoding.UTF8, "application/json");
        var res = await client.PostAsync("exercise/assignex", c);
        var content = await res.Content.ReadAsStringAsync();

        if (string.Compare(content, "assign successful") == 0)
        {
            uiManager.NotiSetText("Assign Successful", "分派成功");
        }
        else
        {
            Debug.Log("ASSIGN ERROR");
            return;
        }
    }

    void OnSave()
    {
        if (string.Compare(questionInput.text, "") == 0)
        {
            uiManager.NotiSetText("Question cannot be empty", "問題不能為空");
            return;
        }
        if (questionType.value == 0)
        {
            if (string.Compare(aInput.text, "") == 0)
            {
                uiManager.NotiSetText("Option cannot be empty", "選項不能為空");
                return;
            }
            if (string.Compare(bInput.text, "") == 0)
            {
                uiManager.NotiSetText("Option cannot be empty", "選項不能為空");
                return;
            }
            if (string.Compare(cInput.text, "") == 0)
            {
                uiManager.NotiSetText("Option cannot be empty", "選項不能為空");
                return;
            }
            if (string.Compare(dInput.text, "") == 0)
            {
                uiManager.NotiSetText("Option cannot be empty", "選項不能為空");
                return;
            }
        }
        if (questionType.value == 1)
        {
            if (string.Compare(answerInput.text, "") == 0)
            {
                uiManager.NotiSetText("Answer cannot be empty", "答案不能為空");
                return;
            }
        }

        QuestionData questionData = new QuestionData();

        switch (questionType.value)
        {
            case 0:
                questionData.question = questionInput.text;
                questionData.questionType = "0";
                questionData.answer = aInput.text;
                questionData.answerA = aInput.text;
                questionData.answerB = bInput.text;
                questionData.answerC = cInput.text;
                questionData.answerD = dInput.text;
                break;
            case 1:
                questionData.question = questionInput.text;
                questionData.questionType = "1";
                questionData.answer = answerInput.text;
                break;
            default:
                Debug.Log("Value error");
                break;
        }

        questionDataList.Add(questionData);

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

    void OnDateChanged(TMP_InputField inputField)
    {
        if (string.IsNullOrEmpty(inputField.text))
        {
            inputField.text = string.Empty;
        }
        else
        {
            string input = inputField.text;
            string MatchPattern = @"^((\d{2}/){0,2}(\d{1,2})?)$";
            string ReplacementPattern = "$1/$3";
            string ToReplacePattern = @"((\.?\d{2})+)(\d)";

            input = Regex.Replace(input, ToReplacePattern, ReplacementPattern);
            Match result = Regex.Match(input, MatchPattern);
            if (result.Success)
            {
                inputField.SetTextWithoutNotify(input);
            }

            inputField.caretPosition = inputField.text.Length;
        }
    }

    bool DateValidate(string date)
    {
        if (date.Length != 19) 
        {
            return false;
        }
        DateTime outDate;
        return DateTime.TryParse(date, out outDate);
    }
    void OnConfirm()
    {
        if (string.Compare(exerciseName.text, "") == 0)
        {
            uiManager.NotiSetText("Exercise Name cannot be empty", "練習名稱不能為空");
            return;
        }
        if (string.Compare(dueDate.text, "") == 0)
        {
            uiManager.NotiSetText("Duedate cannot be empty", "截止日期不能為空");
            return;
        }
        if (toggleOn.activeSelf)
        {
            if (string.Compare(scheduleDate.text, "") == 0)
            {
                uiManager.NotiSetText("Schedule date cannot be empty", "發布日期不能為空");
                return;
            }
            if (DateValidate(scheduleDate.text + " 00:00:00"))
            {
                scheduleData = scheduleDate.text + " 00:00:00";
            }
            else
            {
                uiManager.NotiSetText("Invalid date", "無效的日期");
                return;
            }
        }
        else
        {
            scheduleData = "";
        }

        if (DateValidate(dueDate.text + " 23:59:59"))
        {
            dueDateData = dueDate.text + " 23:59:59";
        }
        else
        {
            uiManager.NotiSetText("Invalid date", "無效的日期");
            return;
        }

        exNameData = exerciseName.text;

        ResetPreAssignValue();

        preAssignGO.SetActive(false);
        assignGO.SetActive(true);
    }

    void OnToggle()
    {
        toggleOn.SetActive(!toggleOn.activeSelf);
        toggleOff.SetActive(!toggleOff.activeSelf);
        scheduleDate.gameObject.SetActive(!scheduleDate.gameObject.activeSelf);
    }

    void ResetPreAssignValue()
    {
        exerciseName.text = "";
        dueDate.text = "";
        scheduleDate.text = "";
        toggleOn.SetActive(false);
        toggleOff.SetActive(true);
        scheduleDate.gameObject.SetActive(false);
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
