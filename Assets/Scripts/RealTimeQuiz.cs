using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RealTimeQuiz : MonoBehaviour
{
    private string URL = "https://mongoserver-1-y3258239.deta.app/";
    private HttpClient client;

    [SerializeField] private GameObject quizPanel;


    //Students
    [SerializeField] private GameObject listPanel;
    [SerializeField] private Transform listParent;
    [SerializeField] private GameObject stuObject;

    //Teacher
    //[SerializeField] private Button startQuizBtn;

    //BeforeStart
    [SerializeField] private GameObject assignPanel;
    [SerializeField] private TMP_Dropdown durationDropdown;
    [SerializeField] private TMP_Dropdown ansDropdown;
    [SerializeField] private TMP_InputField question;
    [SerializeField] private TMP_InputField optionA;
    [SerializeField] private TMP_InputField optionB;
    [SerializeField] private TMP_InputField optionC;
    [SerializeField] private TMP_InputField optionD;
    [SerializeField] private Button createRoomBtn;

    [Space(5)]
    [SerializeField] private Sprite whiteBtnImg;
    [SerializeField] private Sprite blueBtnImg;

    [SerializeField] private UIManager uIManager;

    private int currentMcSelection = -1;
    private bool timerStarted = false;

    //DisplayQ
    [SerializeField] private GameObject waitingForStartPanel;
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private TMP_Text questionName;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Button btnA;
    [SerializeField] private Button btnB;
    [SerializeField] private Button btnC;
    [SerializeField] private Button btnD;
    [SerializeField] private TMP_Text btnAText;
    [SerializeField] private TMP_Text btnBText;
    [SerializeField] private TMP_Text btnCText;
    [SerializeField] private TMP_Text btnDText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Button submitBtn;

    [SerializeField] private Button studentResultBtn;
    //[SerializeField] private 

    private bool roomChecking = false;
    private bool questionChecking = false;
    private bool questionFirstDisplay = false;

    //Polling

    //Pending for submission
    [SerializeField] private GameObject pendingPanel;
    [SerializeField] private GameObject pendingGO;
    [SerializeField] private Transform pendingList;

    [SerializeField] private Button waitingCloseBtn;

    [SerializeField] private GameObject waitingPanel1;

    [SerializeField] private GameObject studentResultsPanel;
    [SerializeField] private GameObject studentResultsGO;
    [SerializeField] private Transform studentResultsList;

    //CreateSuccess
    public GameObject createSuccessPanel;
    public Button createSuccessBtn;

    public Sprite greenBtnImg;
    public Sprite redBtnImg;

    private PollResultRoot pollData;

    [Serializable]
    public class PollCreateRoot
    {
        public PollCreate[] root;
    }

    [Serializable]
    public class PollCreate
    {
        public int pollingId;
    }

    [Serializable]
    public class StudentPollsRoot
    {
        public StudentPolls[] root;
    }

    [Serializable]
    public class StudentPolls
    {
        public int pollingId;
        public string expiryTime;
    }

    [Serializable]
    public class PollResultRoot
    {
        public PollResult[] root;
    }

    [Serializable]
    public class PollResult
    {
        public int pollingId;
        public string expiryTime;
        public QuestionData question;
    }

    // Start is called before the first frame update
    void Start()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(URL);
        client.DefaultRequestHeaders.Add("x-api-key", "c0pPE1CyrvbW_keBnGfJuxJfKr2HAPB3T3U6zCF2JcR4e");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        btnA.onClick.AddListener(delegate { McOnClick(0); });
        btnB.onClick.AddListener(delegate { McOnClick(1); });
        btnC.onClick.AddListener(delegate { McOnClick(2); });
        btnD.onClick.AddListener(delegate { McOnClick(3); });

        //startQuizBtn.onClick.AddListener(StartQuiz);

        waitingCloseBtn.onClick.AddListener(WaitingClose);
        createRoomBtn.onClick.AddListener(CreateRoom);
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

    void StartCreate()
    {
        assignPanel.SetActive(true);
    }

    public void StartQuiz()
    {
        if (UserManager.instance.ROLE_TYPE == 0)
        {
            StartCreate();
        }
        else
        {
            GetRoomList();
        }
    }

    public void ExitQuiz()
    {
        assignPanel.SetActive(false);
        createSuccessPanel.SetActive(false);
        listPanel.SetActive(false);
        pendingPanel.SetActive(false);
        questionPanel.SetActive(false);
        waitingForStartPanel.SetActive(false);
        quizPanel.SetActive(false);
        waitingPanel1.SetActive(false);
        roomChecking = false;
        questionChecking = false;
        questionFirstDisplay = false;
}

    async void GetRoomList()
    {
        listPanel.SetActive(true);
        HttpResponseMessage res;
        try
        {
            res = await client.GetAsync("polling/getPending/" + UserManager.instance.UID + "/");
        }
        catch (HttpRequestException e)
        {
            uIManager.NotiSetText("Connection failure, please check network connection or server", "連接失敗，請檢查網絡連接或伺服器");
            return;
        }
        var content = await res.Content.ReadAsStringAsync();
        Debug.Log("qz163: " + content);
        if (res.StatusCode.Equals(HttpStatusCode.OK))
        {
            var data = JsonUtility.FromJson<StudentPollsRoot>("{\"root\":" + content + "}");
            Debug.Log("datalength:" + data.root.Length);
            while (listParent.childCount > 0)
            {
                DestroyImmediate(listParent.GetChild(0).gameObject);
            }
            for (int i = 0; i < data.root.Length; i++)
            {
                GameObject exObj = Instantiate(stuObject, Vector3.zero, Quaternion.identity, listParent);
                TMP_Text[] exArray = exObj.GetComponentsInChildren<TMP_Text>();
                exArray[0].text = "Quiz: " + data.root[i].pollingId;

                Button btn = exObj.GetComponent<Button>();
                int pId = data.root[i].pollingId;
                btn.onClick.AddListener(delegate { PollItemOnClick(pId); });
            }
            return;
        }
    }

    void PollItemOnClick(int pId)
    {
        questionChecking = true;
        questionFirstDisplay = true;
        listPanel.SetActive(false);
        StartCoroutine(LoopLoading(pId));
    }

    IEnumerator LoopLoading(int pId)
    {
        while (questionChecking)
        {
            CheckQuestionFunc(pId);
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator LoopLoadingTeacher(int pId)
    {
        while (questionChecking)
        {
            Debug.Log("261 looploadingteacher");
            CheckQuestionFuncTeacher(pId);
            yield return new WaitForSeconds(1);
        }
    }

    async void CheckQuestionFunc(int pId)
    {
        HttpResponseMessage res;
        try
        {
            res = await client.GetAsync("polling/" + pId + "/");
        }
        catch (HttpRequestException e)
        {
            uIManager.NotiSetText("Connection failure, please check network connection or server", "連接失敗，請檢查網絡連接或伺服器");
            return;
        }
        var content = await res.Content.ReadAsStringAsync();
        Debug.Log("qz163: " + content);
        var data = JsonUtility.FromJson<PollResultRoot>("{\"root\": [" + content + "]}");
        if (res.StatusCode.Equals(HttpStatusCode.OK))
        {
            Debug.Log("dataroot0: " + data.root[0]);
            if (data.root[0].expiryTime == null)
            {
                waitingForStartPanel.SetActive(true);
            }
            else
            {
                DateTime expTime = DateTime.Parse(data.root[0].expiryTime);
                Debug.Log("expTime:" + expTime);
                Debug.Log("dTnow: " + DateTime.Now);

                int firstTime = int.Parse(expTime.ToString().Substring(16, 2));
                int secondTime = int.Parse(DateTime.Now.ToString().Substring(16, 2));

                if (firstTime < secondTime)
                {
                    firstTime += 60;
                }

                CountDown(firstTime - secondTime);

                waitingPanel1.SetActive(false);
                waitingForStartPanel.SetActive(false);
                int compareRes = DateTime.Compare(expTime, DateTime.Now);
                if (compareRes > 0)
                {
                    if (questionFirstDisplay)
                    {
                        submitBtn.enabled = true;
                        btnA.enabled = true;
                        btnB.enabled = true;
                        btnC.enabled = true;
                        btnD.enabled = true;

                        questionPanel.SetActive(true);
                        questionName.text = "Quiz: " + pId;
                        questionText.text = data.root[0].question.question;
                        btnAText.text = data.root[0].question.answerA;
                        btnBText.text = data.root[0].question.answerB;
                        btnCText.text = data.root[0].question.answerC;
                        btnDText.text = data.root[0].question.answerD;

                        submitBtn.onClick.RemoveAllListeners();
                        submitBtn.onClick.AddListener(delegate { SubmitMc(data.root[0].question.questionId); });
                        questionFirstDisplay = false;
                    }
                }
                else
                {
                    waitingPanel1.SetActive(false);
                    questionChecking = false;
                    btnA.enabled = false;
                    btnB.enabled = false;
                    btnC.enabled = false;
                    btnD.enabled = false;

                    if (string.Compare(data.root[0].question.answer, data.root[0].question.answerA) == 0)
                    {
                        btnA.GetComponent<Image>().sprite = greenBtnImg;
                    }
                    else if (string.Compare(data.root[0].question.answer, data.root[0].question.answerB) == 0)
                    {
                        btnB.GetComponent<Image>().sprite = greenBtnImg;
                    }
                    else if (string.Compare(data.root[0].question.answer, data.root[0].question.answerC) == 0)
                    {
                        btnC.GetComponent<Image>().sprite = greenBtnImg;
                    }
                    else if (string.Compare(data.root[0].question.answer, data.root[0].question.answerD) == 0)
                    {
                        btnD.GetComponent<Image>().sprite = greenBtnImg;
                    }
                    double countA = 0;
                    double countB = 0;
                    double countC = 0;
                    double countD = 0;

                    for (int i = 0; i < data.root[0].question.records.Length; i++)
                    {
                        if (data.root[0].question.records[i].answerBy.userId == int.Parse(UserManager.instance.UID))
                        {
                            if (string.Compare(data.root[0].question.records[i].answer, data.root[0].question.answerA) == 0)
                            {
                                countA++;
                            }
                            else if (string.Compare(data.root[0].question.records[i].answer, data.root[0].question.answerB) == 0)
                            {
                                countB++;
                            }
                            else if (string.Compare(data.root[0].question.records[i].answer, data.root[0].question.answerC) == 0)
                            {
                                countC++;
                            }
                            else if (string.Compare(data.root[0].question.records[i].answer, data.root[0].question.answerD) == 0)
                            {
                                countD++;
                            }

                            if (data.root[0].question.records[i].isCorrect)
                            {
                                break;
                            }
                            if (string.Compare(data.root[0].question.records[i].answer, data.root[0].question.answerA) == 0)
                            {
                                btnA.GetComponent<Image>().sprite = redBtnImg;
                            }
                            else if (string.Compare(data.root[0].question.records[i].answer, data.root[0].question.answerB) == 0)
                            {
                                btnB.GetComponent<Image>().sprite = redBtnImg;
                            }
                            else if (string.Compare(data.root[0].question.records[i].answer, data.root[0].question.answerC) == 0)
                            {
                                btnC.GetComponent<Image>().sprite = redBtnImg;
                            }
                            else if (string.Compare(data.root[0].question.records[i].answer, data.root[0].question.answerD) == 0)
                            {
                                btnD.GetComponent<Image>().sprite = redBtnImg;
                            }
                        }


                    }
                    btnAText.text = data.root[0].question.answerA + " (" + Math.Round((countA / data.root[0].question.records.Length), 2) * 100 + "%)";
                    btnBText.text = data.root[0].question.answerB + " (" + Math.Round((countB / data.root[0].question.records.Length), 2) * 100 + "%)";
                    btnCText.text = data.root[0].question.answerC + " (" + Math.Round((countC / data.root[0].question.records.Length), 2) * 100 + "%)";
                    btnDText.text = data.root[0].question.answerD + " (" + Math.Round((countD / data.root[0].question.records.Length), 2) * 100 + "%)";
                }
            }
        }
    }

    async void SubmitMc(int qId)
    {
        string dataList = "{\"root\":[";

        List<string> str = new List<string>();
        string ansStr = "";
        switch (currentMcSelection)
        {
            case 0:
                ansStr = btnAText.text;
                break;
            case 1:
                ansStr = btnBText.text;
                break;
            case 2:
                ansStr = btnCText.text;
                break;
            case 3:
                ansStr = btnDText.text;
                break;
        }

        str = new List<string> { "questionId", qId.ToString(), "answer", ansStr };
        var dataStr = ExtensionFunction.StringEncoder(str);
        dataList = dataList + dataStr;

        dataList = dataList + "], \"userId\": \"" + UserManager.instance.UID + "\"}";

        HttpContent c = new StringContent(dataList, Encoding.UTF8, "application/json");
        HttpResponseMessage res;
        try
        {
            uIManager.Loading();
            res = await client.PostAsync("record/", c);
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
        Debug.Log("qz163: " + content);
        if (res.StatusCode.Equals(HttpStatusCode.OK))
        {
            submitBtn.enabled = false;
            btnA.enabled = false;
            btnB.enabled = false;
            btnC.enabled = false;
            btnD.enabled = false;

            waitingPanel1.SetActive(true);
            return;
        }
    }

    void WaitingClose()
    {
        questionChecking = false;
        listPanel.SetActive(true);
        waitingPanel1.SetActive(false);
    }

    async void CheckQuestionFuncTeacher(int pId)
    {
        HttpResponseMessage res;
        try
        {
            res = await client.GetAsync("polling/" + pId + "/");
        }
        catch (HttpRequestException e)
        {
            uIManager.NotiSetText("Connection failure, please check network connection or server", "連接失敗，請檢查網絡連接或伺服器");
            return;
        }
        var content = await res.Content.ReadAsStringAsync();
        Debug.Log("qz163: " + content);
        pollData = JsonUtility.FromJson<PollResultRoot>("{\"root\": [" + content + "]}");
        if (res.StatusCode.Equals(HttpStatusCode.OK))
        {
            DateTime expTime = DateTime.Parse(pollData.root[0].expiryTime);
            int compareRes = DateTime.Compare(expTime, DateTime.Now);
            if (compareRes > 0)
            {
                pendingPanel.SetActive(true);

                while (pendingList.childCount > 0)
                {
                    DestroyImmediate(pendingList.GetChild(0).gameObject);
                }

                for (int i = 0; i < pollData.root[0].question.records.Length; i++)
                {
                    GameObject exObj = Instantiate(pendingGO, Vector3.zero, Quaternion.identity, pendingList);
                    TMP_Text[] exArray = exObj.GetComponentsInChildren<TMP_Text>();
                    exArray[0].text = pollData.root[0].question.records[i].answerBy.username + " submitted.";
                }
            }
            else
            {
                studentResultBtn.onClick.RemoveAllListeners();
                studentResultBtn.onClick.AddListener(DisplayStudentResults);
                studentResultBtn.gameObject.SetActive(true);
                timerText.gameObject.SetActive(false);
                submitBtn.gameObject.SetActive(false);
                pendingPanel.SetActive(false);
                questionChecking = false;
                btnA.enabled = false;
                btnB.enabled = false;
                btnC.enabled = false;
                btnD.enabled = false;
                questionPanel.SetActive(true);
                questionText.text = pollData.root[0].question.question;
                questionName.text = "Quiz: " + pId;

                if (string.Compare(pollData.root[0].question.answer, pollData.root[0].question.answerA) == 0)
                {
                    btnA.GetComponent<Image>().sprite = greenBtnImg;
                }
                else if (string.Compare(pollData.root[0].question.answer, pollData.root[0].question.answerB) == 0)
                {
                    btnB.GetComponent<Image>().sprite = greenBtnImg;
                }
                else if (string.Compare(pollData.root[0].question.answer, pollData.root[0].question.answerC) == 0)
                {
                    btnC.GetComponent<Image>().sprite = greenBtnImg;
                }
                else if (string.Compare(pollData.root[0].question.answer, pollData.root[0].question.answerD) == 0)
                {
                    btnD.GetComponent<Image>().sprite = greenBtnImg;
                }

                double countA = 0;
                double countB = 0;
                double countC = 0;
                double countD = 0;

                for (int i = 0; i < pollData.root[0].question.records.Length; i++)
                {
                    if (string.Compare(pollData.root[0].question.records[i].answer, pollData.root[0].question.answerA) == 0)
                    {
                        countA++;
                    }
                    else if (string.Compare(pollData.root[0].question.records[i].answer, pollData.root[0].question.answerB) == 0)
                    {
                        countB++;
                    }
                    else if (string.Compare(pollData.root[0].question.records[i].answer, pollData.root[0].question.answerC) == 0)
                    {
                        countC++;
                    }
                    else if (string.Compare(pollData.root[0].question.records[i].answer, pollData.root[0].question.answerD) == 0)
                    {
                        countD++;
                    }

                }
                btnAText.text = pollData.root[0].question.answerA + " (" + Math.Round((countA / pollData.root[0].question.records.Length), 2) * 100 + "%)";
                btnBText.text = pollData.root[0].question.answerB + " (" + Math.Round((countB / pollData.root[0].question.records.Length), 2) * 100 + "%)";
                btnCText.text = pollData.root[0].question.answerC + " (" + Math.Round((countC / pollData.root[0].question.records.Length), 2) * 100 + "%)";
                btnDText.text = pollData.root[0].question.answerD + " (" + Math.Round((countD / pollData.root[0].question.records.Length), 2) * 100 + "%)";
            }
        }
    }

    void DisplayStudentResults()
    {
        if (studentResultsPanel.activeSelf)
        {
            studentResultsPanel.SetActive(false);
        }
        else
        {
            studentResultsPanel.SetActive(true);
            while (studentResultsList.childCount > 0)
            {
                DestroyImmediate(studentResultsList.GetChild(0).gameObject);
            }

            for (int i = 0; i < pollData.root[0].question.records.Length; i++)
            {
                GameObject studentObj = Instantiate(studentResultsGO, Vector3.zero, Quaternion.identity, studentResultsList);
                TMP_Text[] exArray = studentObj.GetComponentsInChildren<TMP_Text>();
                exArray[0].text = pollData.root[0].question.records[i].answerBy.username;
                exArray[1].text = "Answer: " + pollData.root[0].question.records[i].answer;
            }
        }
    }

    async void CreateRoom()
    {
        string dataList = "{\"questions\":";
        List<string> str = new List<string>();
        string answerStr = "";
        switch (ansDropdown.value)
        {
            case 0:
                answerStr = optionA.text;
                break;
            case 1:
                answerStr = optionB.text;
                break;
            case 2:
                answerStr = optionC.text;
                break;
            case 3:
                answerStr = optionD.text;
                break;
        }

        str = new List<string> { "question", question.text, "questionType", "0", "answer", answerStr, "answerA", optionA.text, "answerB", optionB.text, "answerC", optionC.text, "answerD", optionD.text };
        var dataStr = ExtensionFunction.StringEncoder(str);
        dataList = dataList + dataStr;
        dataList = dataList + ", \"courseId\": \"" + UserManager.instance.COURSEID + "\", \"userId\": \"" + UserManager.instance.UID + "\"";
        Debug.Log("pl:" + dataList);
        switch (durationDropdown.value)
        {
            case 0:
                dataList = dataList + ", \"expirySeconds\": 30}";
                break;
            case 1:
                dataList = dataList + ", \"expirySeconds\": 60}";
                break;
            case 2:
                dataList = dataList + ", \"expirySeconds\": 120}";
                break;
        }

        HttpContent c = new StringContent(dataList, Encoding.UTF8, "application/json");
        HttpResponseMessage res;
        try
        {
            uIManager.Loading();
            res = await client.PostAsync("polling/", c);
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
            uIManager.NotiSetText("Server Error, please try again later", "服務器錯誤，請稍後再試");
            return;
        }
        else if (res.StatusCode.Equals(HttpStatusCode.OK))
        {
            uIManager.NotiSetText("Room Created", "房間創建成功");
            Debug.Log("qz 166: " + content);

            PollCreateRoot pollData = JsonUtility.FromJson<PollCreateRoot>("{\"root\":[" + content + "]}");
            int rId = pollData.root[0].pollingId;
            Debug.Log("rID: " + rId);
            createSuccessBtn.onClick.RemoveAllListeners();
            createSuccessPanel.SetActive(true);
            createSuccessBtn.onClick.AddListener(delegate { StartTheQuiz(rId); });
        }
    }


    async void StartTheQuiz(int rId)
    {
        createSuccessPanel.SetActive(false);
        assignPanel.SetActive(false);
        Debug.Log("Start the quiz call");
        string dataList = "{\"pollingId\":" + rId + ", \"userId\":" + UserManager.instance.UID + "}";
        Debug.Log("dataList:" + dataList);
        HttpContent c = new StringContent(dataList, Encoding.UTF8, "application/json");
        HttpResponseMessage res;
        try
        {
            uIManager.Loading();
            res = await client.PostAsync("polling/", c);
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
        if (res.StatusCode.Equals(HttpStatusCode.OK))
        {
            Debug.Log("254:" + content);
            questionChecking = true;
            uIManager.NotiSetText("Quiz Started", "測驗開始");
            StartCoroutine(LoopLoadingTeacher(rId));
            return;
        }
    }

    void CountDown(int time)
    {
        //string minStr = timeSpan.Substring(3, 2);
        string minStr = "00";
        string secStr = time.ToString();

        if (time > 30)
        {
            secStr = "00";
        }
        if (time < 10)
        {
            secStr = "0" + time.ToString();
        }

        timerText.text = minStr + ":" + secStr;

    }
}
