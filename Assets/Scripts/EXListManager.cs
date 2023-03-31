using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using UnityEngine.UI;
using System.Linq;
using System.Net;

public class EXListManager : MonoBehaviour
{
    private string URL = "https://mongoserver-1-y3258239.deta.app/";
    private HttpClient client;

    [SerializeField] private TMP_Dropdown sortSelect;

    [SerializeField] private Transform exListParent;
    [SerializeField] private GameObject exObject;
    [SerializeField] private Button submitButton;
    [SerializeField] private GameObject exListPanel;

    [Header("McButtons")]
    [SerializeField] private Button btnA;
    [SerializeField] private Button btnB;
    [SerializeField] private Button btnC;
    [SerializeField] private Button btnD;

    [Header("ExDisplay")]
    [SerializeField] private GameObject exDisplayParent;
    [SerializeField] private TMP_Text exName;
    [SerializeField] private TMP_Text exQ;
    [SerializeField] private TMP_Text btnTextA;
    [SerializeField] private TMP_Text btnTextB;
    [SerializeField] private TMP_Text btnTextC;
    [SerializeField] private TMP_Text btnTextD;
    [SerializeField] private TMP_InputField answerInput;
    [SerializeField] private Sprite starOn;

    [Space(5)]
    [SerializeField] private Button exitExBtn;

    [Space(5)]
    [SerializeField] private Sprite whiteBtnImg;
    [SerializeField] private Sprite blueBtnImg;

    [Header("ExerciseItems")]
    private ListItemRoot questionList;
    private int questionIdx = 0;
    private int currentMcSelection = -1;

    [SerializeField] private Button nextBtn;
    [SerializeField] private UIManager uIManager;

    public List<QuestionRecord> questionRecordList = new List<QuestionRecord>();
    private ListDataRoot exListData = new ListDataRoot();
    public class QuestionRecord
    {
        public string questionId = "";
        public string answer = "";
    }

    [Serializable]
    public class ListDataRoot
    {
        public ListData[] root;
    }
       
    [Serializable]
    public class ListData
    {
        public string EXERCISE_ID;
        public string EXERCISE_NAME;
        public string DUEDATE;
        public int DIFFICULTY;
    }
    [Serializable]
    public class ListItemRoot
    { 
        public ListItem[] root;
    }

    [Serializable]
    public class ListItem
    {
        public string QUESTION_ID;
        public string E_ID;
        public string QUESTION;
        public string QUESTION_TYPE;
        public string ANSWER;
        public string ANSWER_A;
        public string ANSWER_B;
        public string ANSWER_C;
        public string ANSWER_D;
    }

    void Start()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(URL);
        client.DefaultRequestHeaders.Add("x-api-key", "c0pPE1CyrvbW_keBnGfJuxJfKr2HAPB3T3U6zCF2JcR4e");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        nextBtn.onClick.AddListener(NextQuestion);
        submitButton.onClick.AddListener(SubmitAnswer);
        exitExBtn.onClick.AddListener(ExitOnClick);

        sortSelect.onValueChanged.AddListener(delegate { DisplayEx(); });

        btnA.onClick.AddListener(delegate { McOnClick(0); });
        btnB.onClick.AddListener(delegate { McOnClick(1); });
        btnC.onClick.AddListener(delegate { McOnClick(2); });
        btnD.onClick.AddListener(delegate { McOnClick(3); });
    }

    public async void GetExList()
    {
        HttpResponseMessage res;
        try
        {
            uIManager.Loading();
            res = await client.GetAsync("exercise/pending/" + UserManager.instance.COURSEID + "/" + UserManager.instance.UID);
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
            if (string.Compare(content, "Course id does not exist.") == 0)
            {
                uIManager.NotiSetText("Course id does not exist", "課程編號不存在");
                return;
            }
            else if (string.Compare(content, "User id does not exist.") == 0)
            {
                uIManager.NotiSetText("User id does not exist", "用戶編號不存在");
                return;
            }
            else if (string.Compare(content, "User is not a student.") == 0)
            {
                uIManager.NotiSetText("User is not a student", "用戶不是學生");
                return;
            }
            else if (string.Compare(content, "User didn't register for the Course.") == 0)
            {
                uIManager.NotiSetText("User didn't register for the Course", "用戶沒有註冊課程");
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
            exListPanel.SetActive(true);
            Debug.Log("exlist 171: " + content);
            exListData = JsonUtility.FromJson<ListDataRoot>("{\"root\":" + content + "}");
            DisplayEx();
        }

    }

    void DisplayEx()
    {
        switch (sortSelect.value)
        {
            case 0:
                exListData.root = exListData.root.OrderBy(x => x.DUEDATE).ToArray();
                break;
            case 1:
                exListData.root = exListData.root.OrderBy(x => x.EXERCISE_NAME).ToArray();
                break;
            case 2:
                exListData.root = exListData.root.OrderBy(x => x.DIFFICULTY).ToArray();
                break;
            case 3:
                exListData.root = exListData.root.OrderByDescending(x => x.DUEDATE).ToArray();
                break;
            case 4:
                exListData.root = exListData.root.OrderByDescending(x => x.EXERCISE_NAME).ToArray();
                break;
            case 5:
                exListData.root = exListData.root.OrderByDescending(x => x.DIFFICULTY).ToArray();
                break;
        }
        while (exListParent.childCount > 0)
        {
            DestroyImmediate(exListParent.GetChild(0).gameObject);
        }

        for (int i = 0; i < exListData.root.Length; i++)
        {
            GameObject exObj = Instantiate(exObject, Vector3.zero, Quaternion.identity, exListParent);
            TMP_Text[] exArray = exObj.GetComponentsInChildren<TMP_Text>();
            exArray[0].text = exListData.root[i].EXERCISE_NAME;
            exArray[1].text = exListData.root[i].DUEDATE.Substring(8, 2) + "/" + exListData.root[i].DUEDATE.Substring(5, 2);
            Image[] imgArray = exObj.GetComponentsInChildren<Image>();
            if (exListData.root[i].DIFFICULTY == 2)
            {
                imgArray[2].sprite = starOn;
            }
            else if (exListData.root[i].DIFFICULTY == 3)
            {
                imgArray[2].sprite = starOn;
                imgArray[3].sprite = starOn;
            }
            Button btn = exObj.GetComponent<Button>();
            string eID = exListData.root[i].EXERCISE_ID;
            btn.onClick.AddListener(delegate { ExItemOnClick(eID); });
        }
    }

    async void ExItemOnClick(string eID)
    {
        questionIdx = 0;
        questionRecordList = new List<QuestionRecord>();

        HttpResponseMessage res;
        try
        {
            uIManager.Loading();
            res = await client.GetAsync("question/getQuestionsByExerciseId/" + eID);
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
            if (string.Compare(content, "Exercise id does not exist.") == 0)
            {
                uIManager.NotiSetText("Exercise id does not exist", "練習編號不存在");
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
            //OK
        }

        questionList = JsonUtility.FromJson<ListItemRoot>("{\"root\":" + content + "}");
        QuestionDisplay();
    }

    void QuestionDisplay() 
    {
        if (questionIdx > questionList.root.Length - 1)
        {
            return;
        }

        if (questionIdx == questionList.root.Length - 1) 
        {
            nextBtn.gameObject.SetActive(false);
            submitButton.gameObject.SetActive(true);
        }
        else
        {
            nextBtn.gameObject.SetActive(true);
            submitButton.gameObject.SetActive(false);
        }

        exListPanel.SetActive(false);
        //reset mc buttons
        btnA.GetComponent<Image>().sprite = blueBtnImg;
        btnB.GetComponent<Image>().sprite = blueBtnImg;
        btnC.GetComponent<Image>().sprite = blueBtnImg;
        btnD.GetComponent<Image>().sprite = blueBtnImg;
        currentMcSelection = -1;

        exName.text = "Question: " + (questionIdx + 1);

        exQ.text = questionList.root[questionIdx].QUESTION;

        switch (questionList.root[questionIdx].QUESTION_TYPE)
        {
            case "0":
                answerInput.gameObject.SetActive(false);

                List<string> suffleList = new List<string> { questionList.root[questionIdx].ANSWER_A, questionList.root[questionIdx].ANSWER_B, questionList.root[questionIdx].ANSWER_C, questionList.root[questionIdx].ANSWER_D };

                for (int i = 0; i < suffleList.Count; i++)
                {
                    string temp = suffleList[i];
                    int randomIndex = UnityEngine.Random.Range(i, suffleList.Count);
                    suffleList[i] = suffleList[randomIndex];
                    suffleList[randomIndex] = temp;
                }

                btnTextA.text = suffleList[0];
                btnTextB.text = suffleList[1];
                btnTextC.text = suffleList[2];
                btnTextD.text = suffleList[3];

                btnA.gameObject.SetActive(true);
                btnB.gameObject.SetActive(true);
                btnC.gameObject.SetActive(true);
                btnD.gameObject.SetActive(true);
                break;
            case "1":
                answerInput.gameObject.SetActive(true);
                btnA.gameObject.SetActive(false);
                btnB.gameObject.SetActive(false);
                btnC.gameObject.SetActive(false);
                btnD.gameObject.SetActive(false);
                break;
            default:
                Debug.Log("Value error");
                break;
        }

        exDisplayParent.SetActive(true);
    }

    void McOnClick(int idx) 
    {
        btnA.GetComponent<Image>().sprite = blueBtnImg;
        btnB.GetComponent<Image>().sprite = blueBtnImg;
        btnC.GetComponent<Image>().sprite = blueBtnImg;
        btnD.GetComponent<Image>().sprite = blueBtnImg;

        if (currentMcSelection == idx)
        {
            currentMcSelection = -1;
        }
        else
        {
            currentMcSelection = idx;
            switch (idx) 
            {
                case 0:
                    btnA.GetComponent<Image>().sprite = whiteBtnImg;
                    break;
                case 1:
                    btnB.GetComponent<Image>().sprite = whiteBtnImg;
                    break;
                case 2:
                    btnC.GetComponent<Image>().sprite = whiteBtnImg;
                    break;
                case 3:
                    btnD.GetComponent<Image>().sprite = whiteBtnImg;
                    break;
            }
        }
    }

    void NextQuestion()
    {
        if (!SaveQuestionData())
            return;
        questionIdx++;
        QuestionDisplay();
    }

    bool SaveQuestionData()
    {
        switch (questionList.root[questionIdx].QUESTION_TYPE)
        {
            case "0":
                if (currentMcSelection == -1)
                {
                    uIManager.NotiSetText("Please select an option", "請選擇一個選項");
                    return false;
                }
                break;
            case "1":
                if (string.Compare(answerInput.text, "") == 0)
                {
                    uIManager.NotiSetText("Please enter answer", "請輸入答案");
                    return false;
                }
                break;
        }

        QuestionRecord newRecord = new QuestionRecord();
        newRecord.questionId = questionList.root[questionIdx].QUESTION_ID;
        switch (questionList.root[questionIdx].QUESTION_TYPE)
        {
            case "0":
                if (currentMcSelection == 0) newRecord.answer = btnTextA.text;
                if (currentMcSelection == 1) newRecord.answer = btnTextB.text;
                if (currentMcSelection == 2) newRecord.answer = btnTextC.text;
                if (currentMcSelection == 3) newRecord.answer = btnTextD.text;
                break;
            case "1":
                newRecord.answer = answerInput.text;
                break;
        }
        questionRecordList.Add(newRecord);
        return true;
    }

    void ExitOnClick()
    {
        exListPanel.SetActive(true);
        exDisplayParent.SetActive(false);
    }

    public void ExitEx()
    {
        exListPanel.SetActive(false);
        exDisplayParent.SetActive(false);
    }

    async void SubmitAnswer()
    {
        if (!SaveQuestionData())
        {
            return;
        }

        string dataList = "{\"root\":[";

        for (int listIdx = 0; listIdx < questionRecordList.Count; listIdx++)
        {
            if (listIdx != 0)
            {
                dataList = dataList + ",";
            }
            List<string> str = new List<string>();
            str = new List<string> { "questionId", questionRecordList[listIdx].questionId, "answer" , questionRecordList[listIdx].answer };
            var dataStr = StringEncoder(str);
            dataList = dataList + dataStr;
        }

        dataList = dataList + "], \"userId\": \"" + UserManager.instance.UID + "\"}";

        Debug.Log(dataList);

        HttpContent c = new StringContent(dataList, Encoding.UTF8, "application/json");
        HttpResponseMessage res;
        try
        {
            uIManager.Loading();
            res = await client.PostAsync("question/", c);
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
            if (string.Compare(content, "UserId does not exist.") == 0)
            {
                uIManager.NotiSetText("User id does not exist", "用戶編號不存在");
                return;
            }
            else if (string.Compare(content, "User is not a student.") == 0)
            {
                uIManager.NotiSetText("User is not a student", "用戶不是學生");
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
            //OK
        }

        uIManager.NotiSetText("Submit Successful", "提交成功");
        ExitOnClick();
        //GetExList();
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
