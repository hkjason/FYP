using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using UnityEngine.UI;
using System.Linq;

public class EXReviewManager : MonoBehaviour
{
    private string URL = "http://localhost:3000";
    private HttpClient client;

    [SerializeField] private GameObject reviewObject;
    [SerializeField] private GameObject reviewObjectStu;
    [SerializeField] private Transform reviewListParent;
    [SerializeField] private GameObject reviewListPanel;
    [SerializeField] private UIManager uIManager;
    [SerializeField] private TMP_Dropdown sortSelect;
    [SerializeField] private TMP_Dropdown sortSelectStu;

    private int questionIdx = 0;
    private ReviewItemRoot questionList;

    [Header("ExDisplay")]
    [SerializeField] private GameObject reviewDisplayParent;
    [SerializeField] private TMP_Text exName;
    [SerializeField] private TMP_Text exQ;
    [SerializeField] private GameObject sQGO;
    [SerializeField] private TMP_Text corrAnsText;
    [SerializeField] private TMP_Text yourAnsText;
    [SerializeField] private Button nextBtn;
    [SerializeField] private Button previousBtn;
    [SerializeField] private Button exitExBtn;
    [SerializeField] private GameObject sqAnsGO;

    [Header("McImages")]
    [SerializeField] private Image mcImageA;
    [SerializeField] private Image mcImageB;
    [SerializeField] private Image mcImageC;
    [SerializeField] private Image mcImageD;
    [SerializeField] private TMP_Text imgTextA;
    [SerializeField] private TMP_Text imgTextB;
    [SerializeField] private TMP_Text imgTextC;
    [SerializeField] private TMP_Text imgTextD;

    [Space(5)]
    [SerializeField] private Sprite greenBtnImg;
    [SerializeField] private Sprite redBtnImg;
    [SerializeField] private Sprite blueBtnImg;

    [Header("Teacher Review")]
    [SerializeField] private Button stuResBtn;
    [SerializeField] private Transform listParent;
    [SerializeField] private GameObject listPanel;
    [SerializeField] private GameObject stuObject;

    private ReviewListDataRoot reviewListData = new ReviewListDataRoot();
    private TeacherReviewListDataRoot teacherReviewListData = new TeacherReviewListDataRoot();

    [Serializable]
    public class ReviewListDataRoot
    {
        public ReviewListData[] root;
    }

    [Serializable]
    public class ReviewListData
    {
        public string EXERCISE_ID;
        public string EXERCISE_NAME;
        public string COMPLETION_TIME;
    }

    [Serializable]
    public class ReviewItemRoot
    {
        public ReviewItem[] root;
    }
    [Serializable]
    public class ReviewItem
    {
        public string QUESTION_ID;
        public string QUESTION;
        public string QUESTION_TYPE;
        public string CORRECT_ANSWER;
        public string ANSWER_B;
        public string ANSWER_C;
        public string ANSWER_D;
        public string ANSWER;
    }
    [Serializable]
    public class TeacherReviewListDataRoot
    {
        public TeacherReviewListData[] root;
    }

    [Serializable]
    public class TeacherReviewListData
    {
        public string EXERCISE_ID;
        public string EXERCISE_NAME;
        public string DUEDATE;
    }

    [Serializable]
    public class ResultItemRoot
    {
        public ResultItem[] root;
    }

    [Serializable]
    public class ResultItem
    {
        public string STUDENT;
        public string ANSWER;
        public string COMPLETION_TIME;
    }

    void Start()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(URL);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        nextBtn.onClick.AddListener(NextReview);
        previousBtn.onClick.AddListener(PreviousReview);
        exitExBtn.onClick.AddListener(ExitOnClick);

        sortSelect.onValueChanged.AddListener(delegate{ SortOnClick(); });
        sortSelectStu.onValueChanged.AddListener(delegate { SortOnClick(); });

        stuResBtn.onClick.AddListener(ReviewStudentResult);
    }

    public void StartReview()
    {
        if (Userdata.instance.ROLE_TYPE == 0)
        {
            sortSelect.gameObject.SetActive(true);
            GetTeacherReviewList();
        }
        else
        {
            sortSelectStu.gameObject.SetActive(true);
            GetReviewList();
        }
    }

    async void GetReviewList()
    {
        reviewListPanel.SetActive(true);

        var payload = "{\"userID\": " + Userdata.instance.UID + "}";
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        HttpResponseMessage res;
        try
        {
            uIManager.Loading();
            res = await client.PostAsync("exercise/getreviewlist", c);
            uIManager.DoneLoading();
        }
        catch (HttpRequestException e)
        {
            uIManager.DoneLoading();
            uIManager.NotiSetText("Connection failure, please check network connection or server", "連接失敗，請檢查網絡連接或伺服器");
            return;
        }
        var content = await res.Content.ReadAsStringAsync();
        reviewListData = JsonUtility.FromJson<ReviewListDataRoot>("{\"root\":" + content + "}");
        DisplayList();
    }

    void DisplayList()
    {
        switch (sortSelectStu.value)
        {
            case 0:
                reviewListData.root = reviewListData.root.OrderBy(x => x.COMPLETION_TIME).ToArray();
                break;
            case 1:
                reviewListData.root = reviewListData.root.OrderBy(x => x.EXERCISE_NAME).ToArray();
                break;
            case 2:
                reviewListData.root = reviewListData.root.OrderByDescending(x => x.COMPLETION_TIME).ToArray();
                break;
            case 3:
                reviewListData.root = reviewListData.root.OrderByDescending(x => x.EXERCISE_NAME).ToArray();
                break;
        }

        while (reviewListParent.childCount > 0)
        {
            DestroyImmediate(reviewListParent.GetChild(0).gameObject);
        }

        for (int i = 0; i < reviewListData.root.Length; i++)
        {
            GameObject exObj = Instantiate(reviewObjectStu, Vector3.zero, Quaternion.identity, reviewListParent);
            TMP_Text[] exArray = exObj.GetComponentsInChildren<TMP_Text>();
            exArray[0].text = reviewListData.root[i].EXERCISE_NAME;
            exArray[1].text = reviewListData.root[i].COMPLETION_TIME.Substring(8, 2) + "/" + reviewListData.root[i].COMPLETION_TIME.Substring(5, 2);
            Button btn = exObj.GetComponent<Button>();
            string eID = reviewListData.root[i].EXERCISE_ID;
            btn.onClick.AddListener(delegate { ExItemOnClick(eID); });
        }
    }

    async void ExItemOnClick(string eID)
    {
        questionIdx = 0;
        var payload = "{\"uID\": " + Userdata.instance.UID + ", \"eID\": " + eID + "}";
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        HttpResponseMessage res;
        try
        {
            uIManager.Loading();
            res = await client.PostAsync("exercise/getreviewdetail", c);
            uIManager.DoneLoading();
        }
        catch (HttpRequestException e)
        {
            uIManager.DoneLoading();
            uIManager.NotiSetText("Connection failure, please check network connection or server", "連接失敗，請檢查網絡連接或伺服器");
            return;
        }
        var content = await res.Content.ReadAsStringAsync();
        questionList = JsonUtility.FromJson<ReviewItemRoot>("{\"root\":" + content + "}");
        QuestionDisplay();
    }

    void QuestionDisplay()
    {
        if (Userdata.instance.ROLE_TYPE == 0)
        {
            stuResBtn.gameObject.SetActive(true);
        }
        else
        {
            stuResBtn.gameObject.SetActive(false);
        }

        reviewListPanel.SetActive(false);

        previousBtn.gameObject.SetActive(!(questionIdx == 0));
        nextBtn.gameObject.SetActive(!(questionIdx == questionList.root.Length - 1));

        //reset mc buttons
        mcImageA.sprite = greenBtnImg;
        mcImageB.sprite = blueBtnImg;
        mcImageC.sprite = blueBtnImg;
        mcImageD.sprite = blueBtnImg;

        exName.text = "Question: " + (questionIdx + 1);

        exQ.text = questionList.root[questionIdx].QUESTION;

        switch (questionList.root[questionIdx].QUESTION_TYPE)
        {
            case "0":
                sQGO.SetActive(false);
                imgTextA.text = questionList.root[questionIdx].CORRECT_ANSWER;
                imgTextB.text = questionList.root[questionIdx].ANSWER_B;
                imgTextC.text = questionList.root[questionIdx].ANSWER_C;
                imgTextD.text = questionList.root[questionIdx].ANSWER_D;

                mcImageA.gameObject.SetActive(true);
                mcImageB.gameObject.SetActive(true);
                mcImageC.gameObject.SetActive(true);
                mcImageD.gameObject.SetActive(true);

                if (string.Compare(questionList.root[questionIdx].ANSWER, questionList.root[questionIdx].ANSWER_B) == 0) 
                {
                    mcImageB.sprite = redBtnImg;
                }
                if (string.Compare(questionList.root[questionIdx].ANSWER, questionList.root[questionIdx].ANSWER_C) == 0)
                {
                    mcImageC.sprite = redBtnImg;
                }
                if (string.Compare(questionList.root[questionIdx].ANSWER, questionList.root[questionIdx].ANSWER_D) == 0)
                {
                    mcImageD.sprite = redBtnImg;
                }

                break;
            case "1":
                sQGO.SetActive(true);
                mcImageA.gameObject.SetActive(false);
                mcImageB.gameObject.SetActive(false);
                mcImageC.gameObject.SetActive(false);
                mcImageD.gameObject.SetActive(false);
                corrAnsText.text = "Correct Answer: " + questionList.root[questionIdx].CORRECT_ANSWER;
                if (Userdata.instance.ROLE_TYPE == 0)
                {
                    sqAnsGO.SetActive(false);
                }
                else
                {
                    sqAnsGO.SetActive(true);
                    yourAnsText.text = "Your Answer: " + questionList.root[questionIdx].ANSWER;
                }

                break;
            default:
                Debug.Log("Value error");
                break;
        }

        reviewDisplayParent.SetActive(true);
    }

    async void ReviewStudentResult()
    {
        if (listPanel.activeSelf)
        {
            listPanel.SetActive(false);
            return;
        }

        var payload = "{\"qID\": " + questionList.root[questionIdx].QUESTION_ID + ", \"uID\": "+ Userdata.instance.UID +"}";
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        HttpResponseMessage res;
        try
        {
            uIManager.Loading();
            res = await client.PostAsync("exercise/getstudentperformance", c);
            uIManager.DoneLoading();
        }
        catch (HttpRequestException e)
        {
            uIManager.DoneLoading();
            uIManager.NotiSetText("Connection failure, please check network connection or server", "連接失敗，請檢查網絡連接或伺服器");
            return;
        }
        var content = await res.Content.ReadAsStringAsync();
        Debug.Log("Content: " + content);
        var data = JsonUtility.FromJson<ResultItemRoot>("{\"root\":" + content + "}");
        DisplayStudentResult(data);
    }

    void DisplayStudentResult(ResultItemRoot data)
    {
        listPanel.SetActive(true);
        while (listParent.childCount > 0)
        {
            DestroyImmediate(listParent.GetChild(0).gameObject);
        }
        for (int i = 0; i < data.root.Length; i++)
        {
            GameObject exObj = Instantiate(stuObject, Vector3.zero, Quaternion.identity, listParent);
            TMP_Text[] exArray = exObj.GetComponentsInChildren<TMP_Text>();
            exArray[0].text = "Student: " + data.root[i].STUDENT;
            if (data.root[i].ANSWER != "" && data.root[i].COMPLETION_TIME != "")
            {
                exArray[1].text = "Answer: " + data.root[i].ANSWER;
                exArray[2].text = data.root[i].COMPLETION_TIME.Substring(8, 2) + "/" + data.root[i].COMPLETION_TIME.Substring(5, 2);
            }
            else
            {
                exArray[1].text = "Not submitted";
                exArray[2].text = "None";
            }
        }
    }

    async void GetTeacherReviewList()
    {
        reviewListPanel.SetActive(true);

        var payload = "{\"uID\": " + Userdata.instance.UID + "}";
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        HttpResponseMessage res;
        try
        {
            uIManager.Loading();
            res = await client.PostAsync("exercise/getteacherreviewlist", c);
            uIManager.DoneLoading();
        }
        catch (HttpRequestException e)
        {
            uIManager.DoneLoading();
            uIManager.NotiSetText("Connection failure, please check network connection or server", "連接失敗，請檢查網絡連接或伺服器");
            return;
        }
        var content = await res.Content.ReadAsStringAsync();
        teacherReviewListData = JsonUtility.FromJson<TeacherReviewListDataRoot>("{\"root\":" + content + "}");
        DisplayTeacherList();
    }

    void DisplayTeacherList()
    {
        switch (sortSelect.value)
        {
            case 0:
                teacherReviewListData.root = teacherReviewListData.root.OrderBy(x => x.DUEDATE).ToArray();
                break;
            case 1:
                teacherReviewListData.root = teacherReviewListData.root.OrderBy(x => x.EXERCISE_NAME).ToArray();
                break;
            case 2:
                teacherReviewListData.root = teacherReviewListData.root.OrderByDescending(x => x.DUEDATE).ToArray();
                break;
            case 3:
                teacherReviewListData.root = teacherReviewListData.root.OrderByDescending(x => x.EXERCISE_NAME).ToArray();
                break;
        }

        while (reviewListParent.childCount > 0)
        {
            DestroyImmediate(reviewListParent.GetChild(0).gameObject);
        }

        for (int i = 0; i < teacherReviewListData.root.Length; i++)
        {
            GameObject exObj = Instantiate(reviewObject, Vector3.zero, Quaternion.identity, reviewListParent);
            TMP_Text[] exArray = exObj.GetComponentsInChildren<TMP_Text>();
            exArray[0].text = teacherReviewListData.root[i].EXERCISE_NAME;
            exArray[1].text = teacherReviewListData.root[i].DUEDATE.Substring(8, 2) + "/" + teacherReviewListData.root[i].DUEDATE.Substring(5, 2);
            Button btn = exObj.GetComponent<Button>();
            string eID = teacherReviewListData.root[i].EXERCISE_ID;
            btn.onClick.AddListener(delegate { TeacherExItemOnClick(eID); });
        }
    }

    async void TeacherExItemOnClick(string eID)
    {
        questionIdx = 0;
        var payload = "{\"eID\": " + eID + "}";
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        HttpResponseMessage res;
        try
        {
            uIManager.Loading();
            res = await client.PostAsync("exercise/getteacherreviewdetail", c);
            uIManager.DoneLoading();
        }
        catch (HttpRequestException e)
        {
            uIManager.DoneLoading();
            uIManager.NotiSetText("Connection failure, please check network connection or server", "連接失敗，請檢查網絡連接或伺服器");
            return;
        }
        var content = await res.Content.ReadAsStringAsync();
        questionList = JsonUtility.FromJson<ReviewItemRoot>("{\"root\":" + content + "}");
        QuestionDisplay();
    }

    void SortOnClick()
    {
        if (Userdata.instance.ROLE_TYPE == 0)
        {
            DisplayTeacherList();
        }
        else
        {
            DisplayList();
        }
    }
    void ExitOnClick()
    {
        listPanel.SetActive(false);
        reviewDisplayParent.SetActive(false);
        reviewListPanel.SetActive(true);
    }

    void NextReview()
    {
        questionIdx++;
        QuestionDisplay();
    }

    void PreviousReview()
    {
        questionIdx--;
        QuestionDisplay();
    }

    public void ExitReview()
    {
        listPanel.SetActive(false);
        reviewDisplayParent.SetActive(false);
        reviewListPanel.SetActive(false);
    }
}
