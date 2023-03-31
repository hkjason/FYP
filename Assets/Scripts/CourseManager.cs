using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Net;

public class CourseManager : MonoBehaviour
{
    private string URL = "https://mongoserver-1-y3258239.deta.app/";
    private HttpClient client;

    [Header("Course")]
    [SerializeField] private GameObject coursePanel;
    [SerializeField] private GameObject courseGO;
    [SerializeField] private GameObject courseGOTeacher;
    [SerializeField] private Transform courseList;


    [Header("CreateCourse")]
    [SerializeField] private Button createCourseBtn;
    [SerializeField] private GameObject createPanel;
    [SerializeField] private TMP_InputField courseName;
    [SerializeField] private Button createPanelCloseBtn;
    [SerializeField] private Button createDimBg;
    [SerializeField] private Button createConfirmBtn;

    [Header("JoinCourse")]
    [SerializeField] private Button joinCourseBtn;
    [SerializeField] private GameObject joinPanel;
    [SerializeField] private TMP_InputField connectionCodeInput;
    [SerializeField] private Button joinPanelCloseBtn;
    [SerializeField] private Button joinDimBg;
    [SerializeField] private Button joinConfirmBtn;

    [Space(5)]
    [SerializeField] private UIManager uIManager;
    [SerializeField] private EXReviewManager eXReviewManager;
    [SerializeField] private EXListManager eXListManager;

    void Awake()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(URL);
        client.DefaultRequestHeaders.Add("x-api-key", "c0pPE1CyrvbW_keBnGfJuxJfKr2HAPB3T3U6zCF2JcR4e");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        createCourseBtn.onClick.AddListener(CreateCourseOnClick);
        createConfirmBtn.onClick.AddListener(CreateConfirmOnClick);
        createDimBg.onClick.AddListener(CreateCloseOnClick);
        createPanelCloseBtn.onClick.AddListener(CreateCloseOnClick);

        joinCourseBtn.onClick.AddListener(JoinCourseOnClick);
        joinConfirmBtn.onClick.AddListener(JoinConfirmOnClick);
        joinDimBg.onClick.AddListener(JoinCloseOnClick);
        joinPanelCloseBtn.onClick.AddListener(JoinCloseOnClick);
    }

    public async void GetCourseList()
    {
        HttpResponseMessage res;
        try
        {
            uIManager.Loading();
            res = await client.GetAsync("course/user/" + UserManager.instance.UID);
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
            if (string.Compare(content, "User does not exist.") == 0)
            {
                uIManager.NotiSetText("User not found", "找不到用戶");
                return;
            }
            else if (string.Compare(content, "User role is not teacher.") == 0)
            {
                uIManager.NotiSetText("User role is not teacher", "用戶不是老師");
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
            CourseDataRoot data = JsonUtility.FromJson<CourseDataRoot>("{\"root\":" + content + "}");
            if (UserManager.instance.ROLE_TYPE == 0)
            {
                createCourseBtn.gameObject.SetActive(true);
                DisplayCourseTeacher(data);
            }
            else 
            {
                joinCourseBtn.gameObject.SetActive(true);
                DisplayCourse(data);
            }
        }
    }

    void DisplayCourseTeacher(CourseDataRoot courseListData)
    {
        while (courseList.childCount > 0)
        {
            DestroyImmediate(courseList.GetChild(0).gameObject);
        }

        for (int i = 0; i < courseListData.root.Length; i++)
        {
            GameObject courseObj = Instantiate(courseGOTeacher, Vector3.zero, Quaternion.identity, courseList);
            TMP_Text[] courseArray = courseObj.GetComponentsInChildren<TMP_Text>();
            courseArray[0].text = courseListData.root[i].courseName;
            courseArray[1].text = courseListData.root[i].students.Length + " students";
            courseArray[2].text = courseListData.root[i].connectionCode;
            Button btn = courseObj.GetComponent<Button>();
            int cID = courseListData.root[i].courseId;
            btn.onClick.AddListener(delegate { CourseItemOnClick(cID); });
        }
    }

    void DisplayCourse(CourseDataRoot courseListData)
    {
        while (courseList.childCount > 0)
        {
            DestroyImmediate(courseList.GetChild(0).gameObject);
        }

        for (int i = 0; i < courseListData.root.Length; i++)
        {
            GameObject courseObj = Instantiate(courseGO, Vector3.zero, Quaternion.identity, courseList);
            TMP_Text[] exArray = courseObj.GetComponentsInChildren<TMP_Text>();
            exArray[0].text = courseListData.root[i].courseName;
            exArray[1].text = "Teacher: " + courseListData.root[i].teacher.username;
            Button btn = courseObj.GetComponent<Button>();
            int cID = courseListData.root[i].courseId;
            btn.onClick.AddListener(delegate { CourseItemOnClick(cID); });
        }
    }

    void CourseItemOnClick(int cID)
    {
        UserManager.instance.COURSEID = cID;
        switch (UserManager.instance.ROLE_TYPE)
        {
            case 0:
                uIManager.AssignPop();
                break;
            case 1:
                uIManager.ExerciseListPop();
                break;
        }
    }

    void CreateCourseOnClick()
    {
        createPanel.SetActive(true);
    }

    async void CreateConfirmOnClick()
    {
        List<string> str = new List<string> { "userId", UserManager.instance.UID, "CourseName", courseName.text };
        var payload = ExtensionFunction.StringEncoder(str);
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        HttpResponseMessage res;
        try
        {
            uIManager.Loading();
            res = await client.PostAsync("course/", c);
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
            if (string.Compare(content, "User does not exist.") == 0)
            {
                uIManager.NotiSetText("User not found", "找不到用戶");
                return;
            }
            else if (string.Compare(content, "User role is not teacher.") == 0)
            {
                uIManager.NotiSetText("User role is not teacher", "用戶不是老師");
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
            uIManager.NotiSetText("Course created successfully", "課程創建成功");
            createPanel.SetActive(false);
            GetCourseList();
        }
    }

    void CreateCloseOnClick()
    {
        createPanel.SetActive(false);
    }

    void JoinCourseOnClick()
    {
        joinPanel.SetActive(true);
    }

    async void JoinConfirmOnClick()
    {
        List<string> str = new List<string> { "userId", UserManager.instance.UID, "connectionCode", connectionCodeInput.text };
        var payload = ExtensionFunction.StringEncoder(str);
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        HttpResponseMessage res = await client.PostAsync("course/register", c);
        try
        {
            uIManager.Loading();
            res = await client.PostAsync("course/register", c);
        }
        catch (HttpRequestException e)
        {
            uIManager.NotiSetText("Connection failure, please check network connection", "連接失敗，請檢查網絡連接");
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
            if (string.Compare(content, "User does not exist.") == 0)
            {
                uIManager.NotiSetText("User not found", "找不到用戶");
                return;
            }
            else if (string.Compare(content, "User role is not a student.") == 0)
            {
                uIManager.NotiSetText("User role is not a student", "用戶不是學生");
                return;
            }
            if (string.Compare(content, "Course not found.") == 0)
            {
                uIManager.NotiSetText("Invalid Code", "無效的代碼");
                return;
            }
            else if (string.Compare(content, "User already registered for this course.") == 0)
            {
                uIManager.NotiSetText("You are already in this course", "你已經在這個課程中");
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
            connectionCodeInput.text = "";
            uIManager.NotiSetText("Join Course Successful", "加入課程成功");
            joinPanel.SetActive(false);
            GetCourseList();
        }
    }

    void JoinCloseOnClick()
    {
        joinPanel.SetActive(false);
    }
}


