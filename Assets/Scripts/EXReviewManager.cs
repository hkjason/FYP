using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using UnityEngine.UI;

public class EXReviewManager : MonoBehaviour
{
    private string URL = "http://localhost:3000";
    private HttpClient client;

    [SerializeField] private GameObject reviewObject;
    [SerializeField] private Transform reviewListParent;
    [SerializeField] private GameObject reviewListPanel;
    [SerializeField] private UIManager uIManager;

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

        stuResBtn.onClick.AddListener(ReviewStudentResult);
    }

    public void StartReview()
    {
        if (Userdata.instance.ROLE_TYPE == 0)
        {
            GetTeacherReviewList();
        }
        else
        {
            GetReviewList();
        }
    }

    async void GetReviewList()
    {
        while (reviewListParent.childCount > 0)
        {
            DestroyImmediate(reviewListParent.GetChild(0).gameObject);
        }

        reviewListPanel.SetActive(true);

        var payload = "{\"userID\": " + Userdata.instance.UID + ", \"teacherID\": " + Userdata.instance.TEACHER_UID + "}";
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        HttpResponseMessage res;
        try
        {
            res = await client.PostAsync("exercise/getreviewlist", c);
        }
        catch (HttpRequestException e)
        {
            uIManager.NotiSetText("Connection failure, please check network connection or server", "連接失敗，請檢查網絡連接或伺服器");
            return;
        }
        var content = await res.Content.ReadAsStringAsync();
        Debug.Log("Content: "+ content);
        var data = JsonUtility.FromJson<ReviewListDataRoot>("{\"root\":" + content + "}");
        DisplayList(data);
    }

    void DisplayList(ReviewListDataRoot data)
    {
        for (int i = 0; i < data.root.Length; i++)
        {
            GameObject exObj = Instantiate(reviewObject, Vector3.zero, Quaternion.identity, reviewListParent);
            TMP_Text[] exArray = exObj.GetComponentsInChildren<TMP_Text>();
            exArray[0].text = data.root[i].EXERCISE_NAME;
            exArray[1].text = data.root[i].COMPLETION_TIME.Substring(8, 2) + "/" + data.root[i].COMPLETION_TIME.Substring(5, 2);
            Button btn = exObj.GetComponent<Button>();
            string eID = data.root[i].EXERCISE_ID;
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
            res = await client.PostAsync("exercise/getreviewdetail", c);
        }
        catch (HttpRequestException e)
        {
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

        reviewListParent.gameObject.SetActive(false);

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

        var payload = "{\"qID\": " + questionList.root[questionIdx].QUESTION_ID + "}";
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        HttpResponseMessage res;
        try
        {
            res = await client.PostAsync("exercise/getstudentperformance", c);
        }
        catch (HttpRequestException e)
        {
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
            exArray[1].text = "Answer: " + data.root[i].ANSWER;
            exArray[2].text = data.root[i].COMPLETION_TIME.Substring(8, 2) + "/" + data.root[i].COMPLETION_TIME.Substring(5, 2);
        }
    }

    async void GetTeacherReviewList()
    {
        while (reviewListParent.childCount > 0)
        {
            DestroyImmediate(reviewListParent.GetChild(0).gameObject);
        }

        reviewListPanel.SetActive(true);

        var payload = "{\"uID\": " + Userdata.instance.UID + "}";
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        HttpResponseMessage res;
        try
        {
            res = await client.PostAsync("exercise/getteacherreviewlist", c);
        }
        catch (HttpRequestException e)
        {
            uIManager.NotiSetText("Connection failure, please check network connection or server", "連接失敗，請檢查網絡連接或伺服器");
            return;
        }
        var content = await res.Content.ReadAsStringAsync();
        Debug.Log("Content: " + content);
        var data = JsonUtility.FromJson<TeacherReviewListDataRoot>("{\"root\":" + content + "}");
        DisplayTeacherList(data);
    }

    void DisplayTeacherList(TeacherReviewListDataRoot data)
    {
        for (int i = 0; i < data.root.Length; i++)
        {
            GameObject exObj = Instantiate(reviewObject, Vector3.zero, Quaternion.identity, reviewListParent);
            TMP_Text[] exArray = exObj.GetComponentsInChildren<TMP_Text>();
            exArray[0].text = data.root[i].EXERCISE_NAME;
            exArray[1].text = data.root[i].DUEDATE.Substring(8, 2) + "/" + data.root[i].DUEDATE.Substring(5, 2);
            Button btn = exObj.GetComponent<Button>();
            string eID = data.root[i].EXERCISE_ID;
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
            res = await client.PostAsync("exercise/getteacherreviewdetail", c);
        }
        catch (HttpRequestException e)
        {
            uIManager.NotiSetText("Connection failure, please check network connection or server", "連接失敗，請檢查網絡連接或伺服器");
            return;
        }
        var content = await res.Content.ReadAsStringAsync();
        questionList = JsonUtility.FromJson<ReviewItemRoot>("{\"root\":" + content + "}");
        QuestionDisplay();
    }


    void ExitOnClick()
    {
        listPanel.SetActive(false);
        reviewListParent.gameObject.SetActive(true);
        reviewDisplayParent.SetActive(false);
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
}
