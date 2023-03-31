using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net;

public class EXAssignManager : MonoBehaviour
{
    private string URL = "https://mongoserver-1-y3258239.deta.app/";
    private HttpClient client;

    [Header("New")]
    private int courseId;

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
    private int questionIdx = 0;
    [SerializeField] private GameObject assignGO;
    [Space(5)]
    [SerializeField] private TMP_Text questionNameText;
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
    [SerializeField] private Button backBtn;

    [SerializeField] private UIManager uIManager;

    [Header("Difficulty")]
    [SerializeField] private Button diff1;
    [SerializeField] private Button diff2;
    [SerializeField] private Button diff3;

    [SerializeField] private Sprite starOn;
    [SerializeField] private Sprite starOff;


    //ExData
    private string exNameData ="";
    private string dueDateData ="";
    private string scheduleData ="";
    private int diffData = 1;
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
        client.DefaultRequestHeaders.Add("x-api-key", "c0pPE1CyrvbW_keBnGfJuxJfKr2HAPB3T3U6zCF2JcR4e");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        confirmBtn.onClick.AddListener(OnConfirm);
        assignButton.onClick.AddListener(OnAssign);
        toggleBtn.onClick.AddListener(OnToggle);
        saveNNext.onClick.AddListener(delegate { OnSave(); } );
        backBtn.onClick.AddListener(AssignBackOnClick);

        questionType.onValueChanged.AddListener(delegate { ChangeType(questionType); });
        dueDate.onValueChanged.AddListener(delegate { OnDateChanged(dueDate); });
        scheduleDate.onValueChanged.AddListener(delegate { OnDateChanged(scheduleDate); });

        diff1.onClick.AddListener(delegate { DifficultyChange(1); });
        diff2.onClick.AddListener(delegate { DifficultyChange(2); });
        diff3.onClick.AddListener(delegate { DifficultyChange(3); });
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
        if (!OnSave())
        {
            return;
        }

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
                    str = new List<string> { "question", questionDataList[listIdx].question, "questionType", questionDataList[listIdx].questionType, "answer", questionDataList[listIdx].answer, "answerA", "", "answerB", "", "answerC", "", "answerD", ""};
                    break;
                default:
                    Debug.Log("Value error");
                    break;
            }
            var dataStr = StringEncoder(str);
            dataList = dataList + dataStr;
        }
        dataList = dataList + "], \"courseId\": \"" + courseId + "\", \"userId\": \""+ UserManager.instance.UID + "\", \"exName\": \"" + exNameData + "\", \"dueDate\": \"" + ChangeToDate(dueDateData) + "\", \"scheduleDate\": \"" + ChangeToDate(scheduleData) + "\", \"difficulty\": " + diffData + "}";

        HttpContent c = new StringContent(dataList, Encoding.UTF8, "application/json");
        HttpResponseMessage res;
        try
        {
            uIManager.Loading();
            res = await client.PostAsync("exercise/", c);
        }
        catch (HttpRequestException e)
        {
            uIManager.NotiSetText("Connection failure, please check network connection or server", "連接失敗，請檢查網絡連接或伺服器");
            return;
        }
        finally
        {
            uIManager.DoneLoading();
        }
        var content = await res.Content.ReadAsStringAsync();
        if (res.StatusCode.Equals(HttpStatusCode.InternalServerError))
        {
            uIManager.NotiSetText("Server Error, please try again later", "服務器錯誤，請稍後再試");
            return;
        }
        else if (res.StatusCode.Equals(HttpStatusCode.BadRequest))
        {
            if (string.Compare(content, "Course does not exist.") == 0)
            {
                uIManager.NotiSetText("Course does not exist", "課程不存在");
                return;
            }
            else if (string.Compare(content, "User is not the teacher.") == 0)
            {
                uIManager.NotiSetText("User is not the teacher", "用戶不是課程的老師");
                return;
            }
            else
            {
                uIManager.NotiSetText("Server Error, please try again later", "服務器錯誤，請稍後再試");
                return;
            }
        }
        else if (res.StatusCode.Equals(HttpStatusCode.OK))
        {
            uIManager.NotiSetText("Assign Successful", "分派成功");
            AssignBackOnClick();
        }
    }

    string ChangeToDate(string dateStr)
    {
        if (string.Compare(dateStr, "") == 0)
        {
            return "";
        }

        string dateVal = "";
        string day = dateStr.Substring(0, 2);
        string month = dateStr.Substring(3, 2);
        string year = dateStr.Substring(6, 4);
        string time = dateStr.Substring(11, 8);

        dateVal = dateVal + year + "-" + month + "-" + day + " " + time;
        return dateVal;
    }

    bool OnSave()
    {
        if (string.Compare(questionInput.text, "") == 0)
        {
            uIManager.NotiSetText("Question cannot be empty", "問題不能為空");
            return false;
        }
        if (questionType.value == 0)
        {
            if (string.Compare(aInput.text, "") == 0)
            {
                uIManager.NotiSetText("Option cannot be empty", "選項不能為空");
                return false;
            }
            if (string.Compare(bInput.text, "") == 0)
            {
                uIManager.NotiSetText("Option cannot be empty", "選項不能為空");
                return false;
            }
            if (string.Compare(cInput.text, "") == 0)
            {
                uIManager.NotiSetText("Option cannot be empty", "選項不能為空");
                return false;
            }
            if (string.Compare(dInput.text, "") == 0)
            {
                uIManager.NotiSetText("Option cannot be empty", "選項不能為空");
                return false;
            }
        }
        if (questionType.value == 1)
        {
            if (string.Compare(answerInput.text, "") == 0)
            {
                uIManager.NotiSetText("Answer cannot be empty", "答案不能為空");
                return false;
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
                return false;
        }

        questionDataList.Add(questionData);
        questionIdx++;

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
                return false;
        }
        questionNameText.text = "Question: " + (questionIdx + 1);
        return true;
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
            uIManager.NotiSetText("Exercise Name cannot be empty", "練習名稱不能為空");
            return;
        }
        if (string.Compare(dueDate.text, "") == 0)
        {
            uIManager.NotiSetText("Duedate cannot be empty", "截止日期不能為空");
            return;
        }
        if (toggleOn.activeSelf)
        {
            if (string.Compare(scheduleDate.text, "") == 0)
            {
                uIManager.NotiSetText("Schedule date cannot be empty", "發布日期不能為空");
                return;
            }
            if (DateValidate(scheduleDate.text + " 00:00:00"))
            {
                scheduleData = scheduleDate.text + " 00:00:00";
            }
            else
            {
                uIManager.NotiSetText("Invalid schedule date", "無效的發布日期");
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
            uIManager.NotiSetText("Invalid due date", "無效的截止日期");
            return;
        }
        DateTime dueDateTime = DateTime.Parse(dueDate.text + " 23:59:59");
        if (DateTime.Compare(dueDateTime, DateTime.Now) < 0)
        {
            uIManager.NotiSetText("Due date cannot be earlier than today", "截止日期不能早於今天");
            return;
        }
        if (toggleOn.activeSelf)
        {
            DateTime scheduleDateTime = DateTime.Parse(scheduleDate.text + " 00:00:00");
            if (DateTime.Compare(scheduleDateTime, DateTime.Now) < 0)
            {
                uIManager.NotiSetText("Schedule date cannot be earlier than today", "發布日期不能早於今天");
                return;
            }
            if (DateTime.Compare(dueDateTime, scheduleDateTime) < 0)
            {
                uIManager.NotiSetText("Due date cannot be earlier than schedule date", "截止日期不能早於發布日期");
                return;
            }
        }
        questionDataList = new List<QuestionData>();
        questionIdx = 0;
        exNameData = exerciseName.text;

        ResetPreAssignValue();

        questionNameText.text = "Question: " + (questionIdx + 1);
        preAssignGO.SetActive(false);
        assignGO.SetActive(true);
    }

    void DifficultyChange(int level)
    {
        diffData = level;
        switch (level)
        {
            case 1:
                diff2.GetComponent<Image>().sprite = starOff;
                diff3.GetComponent<Image>().sprite = starOff;
                break;
            case 2:
                diff2.GetComponent<Image>().sprite = starOn;
                diff3.GetComponent<Image>().sprite = starOff;
                break;
            case 3:
                diff2.GetComponent<Image>().sprite = starOn;
                diff3.GetComponent<Image>().sprite = starOn;
                break;
        }
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

    public void ExitAssign()
    {
        preAssignGO.SetActive(false);
        assignGO.SetActive(false);
        DifficultyChange(1);
    }

    void AssignBackOnClick()
    {
        preAssignGO.SetActive(true);
        assignGO.SetActive(false);
        DifficultyChange(1);
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
