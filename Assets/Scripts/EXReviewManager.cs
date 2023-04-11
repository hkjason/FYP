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

public class EXReviewManager : MonoBehaviour
{
    private string URL = "https://mongoserver-1-y3258239.deta.app/";
    private HttpClient client;

    [SerializeField] private GameObject reviewObject;
    [SerializeField] private GameObject reviewObjectStu;
    [SerializeField] private Transform reviewListParent;
    [SerializeField] private GameObject reviewListPanel;
    [SerializeField] private UIManager uIManager;
    [SerializeField] private TMP_Dropdown sortSelect;
    [SerializeField] private TMP_Dropdown sortSelectStu;

    private int questionIdx = 0;
    private QuestionDataRoot questionList;

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
    [SerializeField] private Sprite starOn;

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

    [Header("ExItemDetail")]
    [SerializeField] private GameObject exDetailPanel;
    [SerializeField] private TMP_Text exNameText;
    [SerializeField] private Button exDetailCloseBtn;
    [SerializeField] private Button exDetailDimBg;
    [SerializeField] private Button exDetailConfirmBtn;
    [SerializeField] private GameObject exGO;
    [SerializeField] private GameObject exGO2;
    [SerializeField] private Transform exList;

    private ExerciseDataRoot exerciseReviewList = new ExerciseDataRoot();

    [Serializable]
    public class ResultItemRoot
    {
        public ResultItem[] root;
    }

    [Serializable]
    public class ResultItem
    {
        public int userId;
        public string username;
        public RecordData record;
    }

    [Serializable]
    public class RecordData
    {
        public string answer = "";
        public string completionTime = "";
    }

    void Start()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(URL);
        client.DefaultRequestHeaders.Add("x-api-key", "c0pPE1CyrvbW_keBnGfJuxJfKr2HAPB3T3U6zCF2JcR4e");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        nextBtn.onClick.AddListener(NextReview);
        previousBtn.onClick.AddListener(PreviousReview);
        exitExBtn.onClick.AddListener(ExitOnClick);
        stuResBtn.onClick.AddListener(ReviewStudentResult);

        sortSelect.onValueChanged.AddListener(delegate{ SortOnClick(); });
        sortSelectStu.onValueChanged.AddListener(delegate { SortOnClick(); });

        exDetailCloseBtn.onClick.AddListener(ExDetailCloseOnClick);
        exDetailDimBg.onClick.AddListener(ExDetailCloseOnClick);
    }

    public void StartReview()
    {
        if (UserManager.instance.ROLE_TYPE == 0)
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
        HttpResponseMessage res;
        try
        {
            uIManager.Loading();
            res = await client.GetAsync("exercise/completed/" + UserManager.instance.COURSEID + "/" + UserManager.instance.UID);
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
            Debug.Log("Review 166:" + content);
            sortSelectStu.gameObject.SetActive(true);
            reviewListPanel.SetActive(true);
            exerciseReviewList = JsonUtility.FromJson<ExerciseDataRoot>("{\"root\":" + content + "}");
            DisplayList();
        }
    }
    
    void DisplayList()
    {
        switch (sortSelectStu.value)
        {
            case 0:
                exerciseReviewList.root = exerciseReviewList.root.OrderBy(x => x.completionTime).ToArray();
                break;
            case 1:
                exerciseReviewList.root = exerciseReviewList.root.OrderBy(x => x.exerciseName).ToArray();
                break;
            case 2:
                exerciseReviewList.root = exerciseReviewList.root.OrderBy(x => x.difficulty).ToArray();
                break;
            case 3:
                exerciseReviewList.root = exerciseReviewList.root.OrderByDescending(x => x.completionTime).ToArray();
                break;
            case 4:
                exerciseReviewList.root = exerciseReviewList.root.OrderByDescending(x => x.exerciseName).ToArray();
                break;
            case 5:
                exerciseReviewList.root = exerciseReviewList.root.OrderByDescending(x => x.difficulty).ToArray();
                break;
        }

        while (reviewListParent.childCount > 0)
        {
            DestroyImmediate(reviewListParent.GetChild(0).gameObject);
        }

        for (int i = 0; i < exerciseReviewList.root.Length; i++)
        {
            GameObject exObj = Instantiate(reviewObjectStu, Vector3.zero, Quaternion.identity, reviewListParent);
            TMP_Text[] exArray = exObj.GetComponentsInChildren<TMP_Text>();
            exArray[0].text = exerciseReviewList.root[i].exerciseName;
            exArray[1].text = exerciseReviewList.root[i].completionTime.Substring(8, 2) + "/" + exerciseReviewList.root[i].completionTime.Substring(5, 2);

            if (exerciseReviewList.root[i].releaseDate == "" || exerciseReviewList.root[i].releaseDate == null)
            {
                exArray[2].text = "Answers available";
            }
            else
            {
                DateTime dateTime = DateTime.Parse(exerciseReviewList.root[i].releaseDate);
                int res = DateTime.Compare(dateTime, DateTime.Now);
                if (res < 0)
                {

                    exArray[2].text = "Answers release on: " + exerciseReviewList.root[i].releaseDate.Substring(8, 2) + "/" + exerciseReviewList.root[i].releaseDate.Substring(5, 2);
                }
            }
            exArray[3].text = "Score: " + exerciseReviewList.root[i].correctAnswers + "/" + exerciseReviewList.root[i].questions;

            Image[] imgArray = exObj.GetComponentsInChildren<Image>();
            if (exerciseReviewList.root[i].difficulty == 2)
            {
                imgArray[2].sprite = starOn;
            }
            else if (exerciseReviewList.root[i].difficulty == 3)
            {
                imgArray[2].sprite = starOn;
                imgArray[3].sprite = starOn;
            }
            Button btn = exObj.GetComponent<Button>();
            string eID = exerciseReviewList.root[i].exerciseId.ToString();
            btn.onClick.AddListener(delegate { ExItemOnClick(eID); });
        }
    }

    async void ExItemOnClick(string eID)
    {
        HttpResponseMessage res;
        try
        {
            uIManager.Loading();
            res = await client.GetAsync("record/exerciseId/" + eID + "/" + UserManager.instance.UID);
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
            else if (string.Compare(content, "User is not a student.") == 0)
            {
                uIManager.NotiSetText("User is not a student", "用戶不是學生");
                return;
            }
            else if (string.Compare(content, "User isn't registered to the Course.") == 0)
            {
                uIManager.NotiSetText("User isn't registered to the Course", "用戶沒有註冊該課程");
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
            questionIdx = 0;
            questionList = JsonUtility.FromJson<QuestionDataRoot>("{\"root\":" + content + "}");
            QuestionDisplay();
        }
    }

    void QuestionDisplay()
    {
        if (UserManager.instance.ROLE_TYPE == 0)
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

        Debug.Log("333 review qidx" + questionIdx);
        Debug.Log("334 review root length:" + questionList.root.Length);
        exQ.text = questionList.root[questionIdx].question;

        switch (questionList.root[questionIdx].questionType)
        {
            case 0:
                sQGO.SetActive(false);
                imgTextA.text = questionList.root[questionIdx].correctAnswer;
                imgTextB.text = questionList.root[questionIdx].answerB;
                imgTextC.text = questionList.root[questionIdx].answerC;
                imgTextD.text = questionList.root[questionIdx].answerD;

                mcImageA.gameObject.SetActive(true);
                mcImageB.gameObject.SetActive(true);
                mcImageC.gameObject.SetActive(true);
                mcImageD.gameObject.SetActive(true);

                if (string.Compare(questionList.root[questionIdx].answer, questionList.root[questionIdx].answerB) == 0) 
                {
                    mcImageB.sprite = redBtnImg;
                }
                if (string.Compare(questionList.root[questionIdx].answer, questionList.root[questionIdx].answerC) == 0)
                {
                    mcImageC.sprite = redBtnImg;
                }
                if (string.Compare(questionList.root[questionIdx].answer, questionList.root[questionIdx].answerD) == 0)
                {
                    mcImageD.sprite = redBtnImg;
                }

                break;
            case 1:
                sQGO.SetActive(true);
                mcImageA.gameObject.SetActive(false);
                mcImageB.gameObject.SetActive(false);
                mcImageC.gameObject.SetActive(false);
                mcImageD.gameObject.SetActive(false);
                corrAnsText.text = "Correct Answer: " + questionList.root[questionIdx].correctAnswer;
                if (UserManager.instance.ROLE_TYPE == 0)
                {
                    sqAnsGO.SetActive(false);
                }
                else
                {
                    sqAnsGO.SetActive(true);
                    yourAnsText.text = "Your Answer: " + questionList.root[questionIdx].answer;
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
        HttpResponseMessage res;
        try
        {
            uIManager.Loading();
            res = await client.GetAsync("record/getRecordsByQuestionId/" + questionList.root[questionIdx].questionId);
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
            if (string.Compare(content, "Question doesn't exist.") == 0)
            {
                uIManager.NotiSetText("Question doesn't exist", "問題不存在");
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
            if (listPanel.activeSelf)
            {
                listPanel.SetActive(false);
                return;
            }
            Debug.Log("review395: " + content);
            var data = JsonUtility.FromJson<ResultItemRoot>("{\"root\":" + content + "}");
            DisplayStudentResult(data);
        }
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
            exArray[0].text = "Student: " + data.root[i].username;
            if (data.root[i].record.answer != "" && data.root[i].record.completionTime != "")
            {
                exArray[1].text = "Answer: " + data.root[i].record.answer;
                exArray[2].text = data.root[i].record.completionTime.Substring(8, 2) + "/" + data.root[i].record.completionTime.Substring(5, 2);
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
        HttpResponseMessage res;
        try
        {
            uIManager.Loading();
            res = await client.GetAsync("exercise/" + UserManager.instance.COURSEID + "/" + UserManager.instance.UID);
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
            sortSelect.gameObject.SetActive(true);
            reviewListPanel.SetActive(true);
            exerciseReviewList = JsonUtility.FromJson<ExerciseDataRoot>("{\"root\":" + content + "}");
            DisplayTeacherList();
        }
    }

    void DisplayTeacherList()
    {
        switch (sortSelect.value)
        {
            case 0:
                exerciseReviewList.root = exerciseReviewList.root.OrderBy(x => x.dueDate).ToArray();
                break;
            case 1:
                exerciseReviewList.root = exerciseReviewList.root.OrderBy(x => x.exerciseName).ToArray();
                break;
            case 2:
                exerciseReviewList.root = exerciseReviewList.root.OrderBy(x => x.difficulty).ToArray();
                break;
            case 3:
                exerciseReviewList.root = exerciseReviewList.root.OrderByDescending(x => x.dueDate).ToArray();
                break;
            case 4:
                exerciseReviewList.root = exerciseReviewList.root.OrderByDescending(x => x.exerciseName).ToArray();
                break;
            case 5:
                exerciseReviewList.root = exerciseReviewList.root.OrderByDescending(x => x.difficulty).ToArray();
                break;
        }

        while (reviewListParent.childCount > 0)
        {
            DestroyImmediate(reviewListParent.GetChild(0).gameObject);
        }

        for (int i = 0; i < exerciseReviewList.root.Length; i++)
        {
            GameObject exObj = Instantiate(reviewObject, Vector3.zero, Quaternion.identity, reviewListParent);
            TMP_Text[] exArray = exObj.GetComponentsInChildren<TMP_Text>();
            exArray[0].text = exerciseReviewList.root[i].exerciseName;
            exArray[1].text = exerciseReviewList.root[i].dueDate.Substring(8, 2) + "/" + exerciseReviewList.root[i].dueDate.Substring(5, 2);

            if (exerciseReviewList.root[i].releaseDate == null || exerciseReviewList.root[i].releaseDate == "")
            {
                exArray[2].text = "";
            }
            else
            {
                exArray[2].text = "Answer release on: " + exerciseReviewList.root[i].releaseDate.Substring(8, 2) + "/" + exerciseReviewList.root[i].releaseDate.Substring(5, 2);
            }
            Button btn = exObj.GetComponent<Button>();
            Image[] imgArray = exObj.GetComponentsInChildren<Image>();
            if (exerciseReviewList.root[i].difficulty == 2)
            {
                imgArray[2].sprite = starOn;
            }
            else if (exerciseReviewList.root[i].difficulty == 3)
            {
                imgArray[2].sprite = starOn;
                imgArray[3].sprite = starOn;
            }
            int idx = i;
            btn.onClick.AddListener(delegate { DisplayExItemDetails(idx); });
        }
    }

    void DisplayExItemDetails(int idx)
    {
        exDetailPanel.SetActive(true);
        exNameText.text = exerciseReviewList.root[idx].exerciseName;

        while (exList.childCount > 0)
        {
            DestroyImmediate(exList.GetChild(0).gameObject);
        }

        for (int i = 0; i < exerciseReviewList.root[idx].studentRecords.Length; i++)
        {
            GameObject studentObj = Instantiate(exGO, Vector3.zero, Quaternion.identity, exList);
            TMP_Text[] exArray = studentObj.GetComponentsInChildren<TMP_Text>();
            exArray[0].text = exerciseReviewList.root[idx].studentRecords[i].username;
            exArray[1].text = "Score: " + exerciseReviewList.root[idx].studentRecords[i].correctAnswers + "/" + exerciseReviewList.root[idx].studentRecords[i].questions;
            //Ref void 434 DisplayStudentResult(ResultItemRoot data)
            exArray[2].text = "Submitted on: " + exerciseReviewList.root[idx].studentRecords[i].completionTime.Substring(8, 2) + "/" + exerciseReviewList.root[idx].studentRecords[i].completionTime.Substring(5, 2);
        }
        for (int i = 0; i < exerciseReviewList.root[idx].studentsNotAnswered.Length; i++)
        {
            GameObject studentObj = Instantiate(exGO2, Vector3.zero, Quaternion.identity, exList);
            TMP_Text[] exArray = studentObj.GetComponentsInChildren<TMP_Text>();
            exArray[0].text = exerciseReviewList.root[idx].studentsNotAnswered[i].username;
            exArray[1].text = "Not Submitted";
        }


        string eID = exerciseReviewList.root[idx].exerciseId.ToString();
        exDetailConfirmBtn.onClick.AddListener(delegate { TeacherExItemOnClick(eID); });
    }

    void ExDetailCloseOnClick()
    {
        exDetailConfirmBtn.onClick.RemoveAllListeners();
        exDetailPanel.SetActive(false);
    }

    async void TeacherExItemOnClick(string eID)
    {
        ExDetailCloseOnClick();
        HttpResponseMessage res;
        try
        {
            uIManager.Loading();
            res = await client.GetAsync("question/getQuestionDetailsByExerciseId/" + eID);
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
            questionIdx = 0;
            Debug.Log("658 re: " + content);
            questionList = JsonUtility.FromJson<QuestionDataRoot>("{\"root\":" + content + "}");
            QuestionDisplay();
        }

    }

    void SortOnClick()
    {
        if (UserManager.instance.ROLE_TYPE == 0)
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
